// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using WalkingTec.Mvvm.Core.Auth;
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
        public async Task<IActionResult> Login([FromForm] string account, [FromForm] string password, [FromForm] string tenant=null, [FromForm] bool rememberLogin = false, [FromForm] bool withMenu = true)
        {
            object rv = null;
            if (string.IsNullOrEmpty(tenant) == true)
            {
                rv = await DC.Set<FrameworkUser>().Where(x => x.ITCode.ToLower() == account.ToLower() && (x.Password == Utils.GetMD5String(password) || x.Password == password) && x.IsValid).Select(x => x.GetID()).SingleOrDefaultAsync();
            }
            else
            {
                rv = await DC.Set<FrameworkUser>().IgnoreQueryFilters().Where(x => x.ITCode.ToLower() == account.ToLower() && (x.Password == Utils.GetMD5String(password) || x.Password == password) && x.IsValid && x.TenantCode == tenant).Select(x => x.GetID()).SingleOrDefaultAsync();
            }
            if (rv == null)
            {
                return BadRequest(Localizer["Sys.LoginFailed"].Value);
            }
            LoginUserInfo user = new LoginUserInfo
            {
                UserId = rv.ToString()
            };

            await user.LoadBasicInfoAsync(Wtm);

            //其他属性可以通过user.Attributes["aaa"] = "bbb"方式赋值

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
            await Wtm.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
            if (withMenu == false)
            {
                return Ok(user);
            }
            else
            {
                List<SimpleMenuApi> ms = new List<SimpleMenuApi>();
                LoginUserInfo forapi = new LoginUserInfo();
                forapi.UserId = user.UserId;
                forapi.ITCode = user.ITCode;
                forapi.Name = user.Name;
                forapi.Roles = user.Roles;
                forapi.Groups = user.Groups;
                forapi.PhotoId = user.PhotoId;
                var roleIDs = Wtm.LoginUserInfo.Roles.Select(x => x.RoleCode).ToList();
                var data = DC.Set<FrameworkMenu>().Where(x => string.IsNullOrEmpty(x.MethodName)).ToList();
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


        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> LoginJwt(SimpleLogin loginInfo)
        {

            var userIdStr = HttpContext.User.Claims.Where(x => x.Type == AuthConstants.JwtClaimTypes.Subject).Select(x => x.Value).FirstOrDefault() ?? "";
            var tenant = HttpContext.User.Claims.Where(x => x.Type == AuthConstants.JwtClaimTypes.TenantCode).Select(x => x.Value).FirstOrDefault();

            object rv = null;
            if(loginInfo.IsReload == true && string.IsNullOrEmpty(userIdStr) == false)
            {
                rv = await DC.Set<FrameworkUser>().IgnoreQueryFilters().Where(x => x.ITCode.ToLower() == userIdStr.ToLower() && x.TenantCode == tenant && x.IsValid).Select(x => x.GetID()).SingleOrDefaultAsync();
            }
            else
            {
                if(loginInfo.Tenant == null)
                {
                    rv = await DC.Set<FrameworkUser>().Where(x => x.ITCode.ToLower() == loginInfo.Account.ToLower() && x.Password == Utils.GetMD5String(loginInfo.Password) && x.IsValid).Select(x => x.GetID()).SingleOrDefaultAsync();
                }
                else
                {
                    rv = await DC.Set<FrameworkUser>().IgnoreQueryFilters().Where(x => x.ITCode.ToLower() == loginInfo.Account.ToLower() && x.Password == Utils.GetMD5String(loginInfo.Password) && x.TenantCode == loginInfo.Tenant && x.IsValid).Select(x =>x.GetID()).SingleOrDefaultAsync();
                }
            }

            if (rv == null)
            {
                ModelState.AddModelError(" ", Localizer["Sys.LoginFailed"]);
                return BadRequest(ModelState.GetErrorJson());
            }
            LoginUserInfo user = new LoginUserInfo
            {
                UserId = rv.ToString()
            };
            await user.LoadBasicInfoAsync(Wtm);

            //其他属性可以通过user.Attributes["aaa"] = "bbb"方式赋值

            Wtm.LoginUserInfo = user;
            if (loginInfo.IsReload == false)
            {
                var authService = HttpContext.RequestServices.GetService(typeof(ITokenService)) as ITokenService;

                var token = await authService.IssueTokenAsync(Wtm.LoginUserInfo);
                return Content(JsonSerializer.Serialize(token), "application/json");
            }
            else
            {
                return Ok(user);
            }
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public IActionResult SetTenant(string tenant)
        {
            bool rv = Wtm.SetCurrentTenant(tenant);
            return Ok(rv);
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public IActionResult Reg(SimpleReg regInfo)
        {
            var exist = DC.Set<FrameworkUser>().Any(x => x.ITCode.ToLower() == regInfo.ITCode.ToLower());

            if (exist == true)
            {
                ModelState.AddModelError("ITCode", Localizer["Login.ItcodeDuplicate"]);
                return BadRequest(ModelState.GetErrorJson());
            }

            var hasuserrole = DC.Set<FrameworkRole>().Where(x => x.RoleCode == "002").FirstOrDefault();
            FrameworkUser user = new FrameworkUser
            {
                ITCode = regInfo.ITCode,
                Name = regInfo.Name,
                Password = Utils.GetMD5String(regInfo.Password),
                IsValid = true,
                PhotoId = regInfo.PhotoId,
            };
            if (hasuserrole != null)
            {
                var userrole = new FrameworkUserRole
                {
                    UserCode = user.ITCode,
                    RoleCode = "002"
                };
                DC.Set<FrameworkUserRole>().Add(userrole);
            }
            DC.Set<FrameworkUser>().Add(user);
            DC.SaveChanges();
            return Ok();
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
        [Public]
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
                var data = DC.Set<FrameworkMenu>().Where(x => string.IsNullOrEmpty(x.MethodName)).ToList();
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
            Ok();
        }

    }

    public class SimpleLogin
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string Tenant { get; set; }
        public bool IsReload { get; set; } = false;
    }
    public class SimpleReg
    {
        [Display(Name = "_Admin.Account")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string ITCode { get; set; }

        [Display(Name = "_Admin.Name")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string Name { get; set; }

        [Display(Name = "Login.Password")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string Password { get; set; }

        [Display(Name = "_Admin.Photo")]
        public Guid? PhotoId { get; set; }
    }

}
