using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using WalkingTec.Mvvm.Admin.ViewModels;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth;
using WalkingTec.Mvvm.Core.Auth.Attribute;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ApiController]
    [Route("api/_[controller]")]
    [Route("api/_login")]
    [ActionDescription("Login")]
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
        public async Task<IActionResult> Login([FromForm]string userid, [FromForm]string password, [FromForm]bool rememberLogin = false, [FromForm]bool cookie = true)
        {

            var user = await WtmContext.LoadUserFromDB(null, userid, password);

            //如果没有找到则输出错误
            if (user == null)
            {
                return BadRequest("LoadFailed");
            }
            WtmContext.LoginUserInfo = user;

            if (cookie) // cookie auth
            {
                AuthenticationProperties properties = null;
                if (rememberLogin)
                {
                    properties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30))
                    };
                }

                var principal = WtmContext.LoginUserInfo.CreatePrincipal();
                // 在上面注册AddAuthentication时，指定了默认的Scheme，在这里便可以不再指定Scheme。
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
                List<SimpleMenu> ms = new List<SimpleMenu>();
                LoginUserInfo forapi = new LoginUserInfo();
                forapi.Id = user.Id;
                forapi.ITCode = user.ITCode;
                forapi.Name = user.Name;
                forapi.Roles = user.Roles;
                forapi.Groups = user.Groups;
                forapi.PhotoId = user.PhotoId;
                var menus = DC.Set<FunctionPrivilege>()
                    .Where(x => x.UserId == user.Id || (x.RoleId != null && user.Roles.Select(x=>x.ID).Contains(x.RoleId.Value)))
                    .Select(x => x.MenuItem)
                    .Where(x => x.MethodName == null)
                    .OrderBy(x=>x.DisplayOrder)
                    .Select(x => new SimpleMenu
                    {
                        Id = x.ID.ToString().ToLower(),
                        ParentId = x.ParentId.ToString().ToLower(),
                        Text = x.PageName,
                        Url = x.Url,
                        Icon = x.ICon
                    });
                ms.AddRange(menus);

                List<string> urls = new List<string>();
                urls.AddRange(DC.Set<FunctionPrivilege>()
                    .Where(x => x.UserId == user.Id || (x.RoleId != null && user.Roles.Select(x => x.ID).Contains(x.RoleId.Value)))
                    .Select(x => x.MenuItem)
                    .Where(x => x.MethodName != null)
                    .Select(x => x.Url)
                    );
                urls.AddRange(GlobaInfo.AllModule.Where(x => x.IsApi == true).SelectMany(x => x.Actions).Where(x => (x.IgnorePrivillege == true || x.Module.IgnorePrivillege == true) && x.Url != null).Select(x => x.Url));
                forapi.Attributes = new Dictionary<string, object>();
                forapi.Attributes.Add("Menus", menus);
                forapi.Attributes.Add("Actions", urls);

                return Ok(forapi);
            }
            else // jwt auth
            {
                var authService = HttpContext.RequestServices.GetService(typeof(ITokenService)) as ITokenService;

                var token = await authService.IssueTokenAsync(WtmContext.LoginUserInfo);
                return Content(JsonConvert.SerializeObject(token), "application/json");
            }
        }

        [HttpPost("[action]")]
        [AllRights]
        [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
        public async Task<Token> RefreshToken(string refreshToken)
        {
            return await _authService.RefreshTokenAsync(refreshToken);
        }

        [AllRights]
        [HttpGet("[action]")]
        public IActionResult CheckUserInfo()
        {
            if (WtmContext.LoginUserInfo == null)
            {
                return BadRequest();
            }
            else
            {
                var forapi = new LoginUserInfo();
                forapi.Id = WtmContext.LoginUserInfo.Id;
                forapi.ITCode = WtmContext.LoginUserInfo.ITCode;
                forapi.Name = WtmContext.LoginUserInfo.Name;
                forapi.Roles = WtmContext.LoginUserInfo.Roles;
                forapi.Groups = WtmContext.LoginUserInfo.Groups;
                forapi.PhotoId = WtmContext.LoginUserInfo.PhotoId;

                var ms = new List<SimpleMenu>();
                var roleIDs = WtmContext.LoginUserInfo.Roles.Select(x => x.ID).ToList();

                var menus = DC.Set<FunctionPrivilege>()
                                .AsNoTracking()
                                .Where(x => x.UserId == WtmContext.LoginUserInfo.Id || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                                .Select(x => x.MenuItem).Distinct()
                                .Where(x => x.MethodName == null)
                                .OrderBy(x => x.DisplayOrder)
                                .Select(x => new SimpleMenu
                                {
                                    Id = x.ID.ToString().ToLower(),
                                    ParentId = x.ParentId.ToString().ToLower(),
                                    Text = x.PageName,
                                    Url = x.Url,
                                    Icon = x.ICon
                                }).ToList();
                var folders = DC.Set<FrameworkMenu>().AsNoTracking().Where(x => x.FolderOnly == true).OrderBy(x => x.DisplayOrder).Select(x => new SimpleMenu
                {
                    Id = x.ID.ToString().ToLower(),
                    ParentId = x.ParentId.ToString().ToLower(),
                    Text = x.PageName,
                    Url = x.Url,
                    Icon = x.ICon
                }).ToList();
                ms.AddRange(folders);
                foreach (var item in menus)
                {
                    if (folders.Any(x => x.Id == item.Id) == false)
                    {
                        ms.Add(item);
                    }
                }
                List<string> urls = new List<string>();
                urls.AddRange(DC.Set<FunctionPrivilege>()
                    .AsNoTracking()
                    .Where(x => x.UserId == WtmContext.LoginUserInfo.Id || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                    .Select(x => x.MenuItem).Distinct()
                    .Where(x => x.MethodName != null)
                    .Select(x => x.Url).ToList()
                    );
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
        [HttpGet("[action]/{id}")]
        public async Task Logout(Guid? id)
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Response.Redirect("/");
        }

    }

    public class SimpleMenu
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }
    }
}
