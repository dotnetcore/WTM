// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.Json;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;
using WalkingTec.Mvvm.Mvc.Auth;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ApiController]
    [Route("api/_[controller]")]
    [ActionDescription("_Admin.LoginApi")]
    [AllRights]
    public class AccountController : BaseApiController
    {
        private readonly ILogger _logger;
        private readonly ITokenService _authService;
        public AccountController(
            ILogger<AccountController> logger,
            ITokenService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromForm] string account, [FromForm] string password, [FromForm] bool rememberLogin = false)
        {

            var rv = await DC.Set<FrameworkUser>().Where(x => x.ITCode.ToLower() == account.ToLower() && x.Password == Utils.GetMD5String(password) && x.IsValid).Select(x => new { itcode = x.ITCode, id = x.GetID() }).SingleOrDefaultAsync();

            if (rv == null)
            {
                return BadRequest(Localizer["Sys.LoginFailed"].Value);
            }
            LoginUserInfo user = new LoginUserInfo
            {
                ITCode = rv.itcode,
                UserId = rv.id.ToString()
            };

            await user.LoadBasicInfoAsync(Wtm);
            Wtm.LoginUserInfo = user;

            AuthenticationProperties properties = null;
            if (rememberLogin)
            {
                properties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30))
                };
            }

            var principal = Wtm.LoginUserInfo.CreatePrincipal();
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
            List<SimpleMenuApi> ms = new List<SimpleMenuApi>();
            LoginUserInfo forapi = new LoginUserInfo();
            forapi.UserId = user.UserId;
            forapi.ITCode = user.ITCode;
            forapi.Name = user.Name;
            forapi.Roles = user.Roles;
            forapi.Groups = user.Groups;
            forapi.PhotoId = user.PhotoId;
            var roleIDs = Wtm.LoginUserInfo.Roles.Select(x => x.RoleCode).ToList();
            var data = DC.Set<FrameworkMenu>().Where(x => x.MethodName == null).ToList();
            var topdata = data.Where(x => x.ParentId == null && x.ShowOnMenu).ToList().FlatTree(x => x.DisplayOrder).Where(x => (x.IsInside == false || x.FolderOnly == true || string.IsNullOrEmpty(x.MethodName)) && x.ShowOnMenu).ToList();
            var allowed = DC.Set<FunctionPrivilege>()
                            .AsNoTracking()
                            .Where(x => x.RoleCode != null && roleIDs.Contains(x.RoleCode))
                            .Select(x => new { x.MenuItem.ID, x.MenuItem.Url })
                            .ToList();

            var allowedids = allowed.Select(x => x.ID).ToList();
            foreach (var item in topdata)
            {
                if (allowedids.Contains(item.ID))
                {
                    ms.Add(new SimpleMenuApi
                    {
                        Id = item.ID.ToString().ToLower(),
                        ParentId = item.ParentId?.ToString()?.ToLower(),
                        Text = item.PageName,
                        Url = item.Url,
                        Icon = item.Icon
                    });
                }
            }

            LocalizeMenu(ms);

            List<string> urls = new List<string>();
            urls.AddRange(allowed.Select(x => x.Url).Distinct());
            urls.AddRange(GlobaInfo.AllModule.Where(x => x.IsApi == true).SelectMany(x => x.Actions).Where(x => (x.IgnorePrivillege == true || x.Module.IgnorePrivillege == true) && x.Url != null).Select(x => x.Url));
            forapi.Attributes = new Dictionary<string, object>();
            forapi.Attributes.Add("Menus", ms);
            forapi.Attributes.Add("Actions", urls);

            return Ok(forapi);
        }


        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> LoginJwt(SimpleLogin loginInfo)
        {

            var rv = await DC.Set<FrameworkUser>().Where(x => x.ITCode.ToLower() == loginInfo.Account.ToLower() && x.Password == Utils.GetMD5String(loginInfo.Password) && x.IsValid).Select(x => new { itcode = x.ITCode, id = x.GetID() }).SingleOrDefaultAsync();

            if (rv == null)
            {
                ModelState.AddModelError(" ", Localizer["Sys.LoginFailed"]);
                return BadRequest(ModelState.GetErrorJson());
            }
            LoginUserInfo user = new LoginUserInfo
            {
                ITCode = rv.itcode,
                UserId = rv.id.ToString()
            };
            await user.LoadBasicInfoAsync(Wtm);
            Wtm.LoginUserInfo = user;

            var authService = HttpContext.RequestServices.GetService(typeof(ITokenService)) as ITokenService;

            var token = await authService.IssueTokenAsync(Wtm.LoginUserInfo);
            return Content(JsonSerializer.Serialize(token), "application/json");
        }


        private void LocalizeMenu(List<SimpleMenuApi> menus)
        {
            if (menus == null)
            {
                return;
            }
            foreach (var menu in menus)
            {
                if (menu.Text?.StartsWith("MenuKey.") == true)
                {
                    menu.Text = Localizer[menu.Text];
                }
            }
        }

        [HttpPost("[action]")]
        [AllRights]
        [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var rv = await _authService.RefreshTokenAsync(refreshToken);
            if (rv == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(rv);
            }
        }

        [AllRights]
        [HttpGet("[action]")]
        public IActionResult CheckUserInfo()
        {
            if (Wtm.LoginUserInfo == null)
            {
                return BadRequest();
            }
            else
            {
                var forapi = new LoginUserInfo();
                forapi.UserId = Wtm.LoginUserInfo.UserId;
                forapi.ITCode = Wtm.LoginUserInfo.ITCode;
                forapi.Name = Wtm.LoginUserInfo.Name;
                forapi.Roles = Wtm.LoginUserInfo.Roles;
                forapi.Groups = Wtm.LoginUserInfo.Groups;
                forapi.PhotoId = Wtm.LoginUserInfo.PhotoId;

                var ms = new List<SimpleMenuApi>();
                var roleIDs = Wtm.LoginUserInfo.Roles.Select(x => x.RoleCode).ToList();
                var data = DC.Set<FrameworkMenu>().Where(x => x.MethodName == null).ToList();
                var topdata = data.Where(x => x.ParentId == null && x.ShowOnMenu).ToList().FlatTree(x => x.DisplayOrder).Where(x => (x.IsInside == false || x.FolderOnly == true || string.IsNullOrEmpty(x.MethodName)) && x.ShowOnMenu).ToList();
                var allowed = DC.Set<FunctionPrivilege>()
                                .AsNoTracking()
                                .Where(x => x.RoleCode != null && roleIDs.Contains(x.RoleCode))
                                .Select(x => new { x.MenuItem.ID, x.MenuItem.Url })
                                .ToList();

                var allowedids = allowed.Select(x => x.ID).ToList();
                foreach (var item in topdata)
                {
                    if (allowedids.Contains(item.ID))
                    {
                        ms.Add(new SimpleMenuApi
                        {
                            Id = item.ID.ToString().ToLower(),
                            ParentId = item.ParentId?.ToString()?.ToLower(),
                            Text = item.PageName,
                            Url = item.Url,
                            Icon = item.Icon
                        });
                    }
                }

                LocalizeMenu(ms);

                List<string> urls = new List<string>();
                urls.AddRange(allowed.Select(x => x.Url).Distinct());
                urls.AddRange(GlobaInfo.AllModule.Where(x => x.IsApi == true).SelectMany(x => x.Actions).Where(x => (x.IgnorePrivillege == true || x.Module.IgnorePrivillege == true) && x.Url != null).Select(x => x.Url));
                forapi.Attributes = new Dictionary<string, object>();
                forapi.Attributes.Add("Menus", ms);
                forapi.Attributes.Add("Actions", urls);
                return Ok(forapi);
            }
        }


        [AllRights]
        [HttpPost("[action]")]
        public IActionResult ChangePassword(ChangePasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                vm.DoChange();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.GetErrorJson());
                }
                else
                {
                    return Ok();
                }
            }

        }

        [AllRights]
        [HttpGet("[action]")]
        public async Task Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Response.Redirect("/");
        }

    }

    public class SimpleLogin
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }
}
