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
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.Json;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ApiController]
    [Route("api/_[controller]")]
    [ActionDescription("_Admin.LoginApi")]
    [AllRights]
    public class AccountController : BaseApiController
    {

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromForm] string account, [FromForm] string password, [FromForm] string tenant = null, [FromForm] bool rememberLogin = false)
        {

            var user = await Wtm.DoLogin(account, password, tenant);
            if (user == null)
            {
                return BadRequest(Localizer["Sys.LoginFailed"].Value);
            }

            //其他属性可以通过user.Attributes["aaa"] = "bbb"方式赋值

            Wtm.SetLoginUserInfo (user);

            AuthenticationProperties properties = null;
            if (rememberLogin)
            {
                properties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30))
                };
            }

            var principal = (await Wtm.GetLoginUserInfo ()).CreatePrincipal();
            await Wtm.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
            return await CheckUserInfo();
        }


        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> LoginJwt(SimpleLogin loginInfo)
        {
            var user = await Wtm.DoLogin(loginInfo.Account, loginInfo.Password, loginInfo.Tenant);
            if (user == null)
            {
                ModelState.AddModelError(" ", Localizer["Sys.LoginFailed"]);
                return BadRequest(ModelState.GetErrorJson());
            }

            //其他属性可以通过user.Attributes["aaa"] = "bbb"方式赋值

            Wtm.SetLoginUserInfo (user);
            var authService = HttpContext.RequestServices.GetService(typeof(ITokenService)) as ITokenService;
            var token = await authService.IssueTokenAsync((await Wtm.GetLoginUserInfo ()));
            return Content(JsonSerializer.Serialize(token), "application/json");
        }

        [Public]
        [HttpGet("[action]")]
        public async Task<IActionResult> LoginRemote([FromQuery] string _remotetoken)
        {
            if ((await Wtm?.GetLoginUserInfo ()) != null)
            {
                var principal = (await Wtm.GetLoginUserInfo ()).CreatePrincipal();
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, null);
            }
            return await CheckUserInfo();
        }


        [AllRights]
        [HttpGet("[action]")]
        public async Task<IActionResult> SetTenant([FromQuery] string tenant)
        {
            bool rv = await Wtm.SetCurrentTenant(tenant == "" ? null : tenant);
            return Ok(rv);
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Reg(SimpleReg regInfo)
        {
            var exist = await DC.Set<FrameworkUser>().AnyAsync(x => x.ITCode.ToLower() == regInfo.ITCode.ToLower());

            if (exist == true)
            {
                ModelState.AddModelError("ITCode", Localizer["Login.ItcodeDuplicate"]);
                return BadRequest(ModelState.GetErrorJson());
            }

            var hasuserrole = await DC.Set<FrameworkRole>().Where(x => x.RoleCode == "002").FirstOrDefaultAsync();
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
            await DC.SaveChangesAsync();
            return Ok();
        }


        [HttpPost("[action]")]
        [AllRights]
        [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
        public IActionResult RefreshToken(string refreshToken)
        {
            var rv = Wtm.RefreshToken();
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
        public async Task<IActionResult> CheckUserInfo(bool IsApi = true)
        {
            if ((await Wtm.GetLoginUserInfo ()) == null)
            {
                return BadRequest();
            }
            else
            {
                var forapi = (await Wtm.GetLoginUserInfo ());
                if (IsApi)
                {
                    await forapi.SetAttributesForApi(Wtm);
                }
                forapi.DataPrivileges = null;
                forapi.FunctionPrivileges = null;
                if (forapi.Attributes == null)
                {
                    forapi.Attributes = new Dictionary<string, object>();
                }
                if (forapi.Attributes.ContainsKey("IsMainHost"))
                {
                    forapi.Attributes.Remove("IsMainHost");
                }
                if (ConfigInfo.HasMainHost && string.IsNullOrEmpty((await Wtm.GetLoginUserInfo ()).TenantCode) == true)
                {
                    forapi.Attributes.Add("IsMainHost", true);
                }
                else
                {
                    forapi.Attributes.Add("IsMainHost", false);
                }
                return Ok(forapi);
            }
        }


        [AllRights]
        [HttpPost("[action]")]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM vm)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Request.RedirectCall(Wtm).Result;
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                await vm.DoChange();
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

        [Public]
        [HttpGet("[action]")]
        public async Task<IActionResult> Logout()
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                await Wtm.CallAPI<string>("mainhost", "/api/_account/logout", HttpMethodEnum.GET, new { }, 10);
                return Ok(ConfigInfo.MainHost);
            }
            else
            {
                HttpContext.Session.Clear();
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok("/");
            }
        }

        [HttpGet("GetFrameworkRoles")]
        [ActionDescription("GetRoles")]
        [AllRights]
        public async Task<IActionResult> GetFrameworkRoles()
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return await Request.RedirectCall(Wtm, "/api/_account/GetFrameworkRoles");
            }
            return Ok(await DC.Set<FrameworkRole>().GetSelectListItems(Wtm, x => x.RoleName, x => x.RoleCode));
        }

        [HttpGet("GetFrameworkGroups")]
        [ActionDescription("GetGroups")]
        [AllRights]
        public async Task<IActionResult> GetFrameworkGroups()
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return await Request.RedirectCall(Wtm, "/api/_account/GetFrameworkGroups");
            }
            return Ok(await DC.Set<FrameworkGroup>().GetSelectListItems(Wtm, x => x.GroupName, x => x.GroupCode));
        }

        [HttpGet("GetFrameworkGroupsTree")]
        [ActionDescription("GetGroupsTree")]
        [AllRights]
        public async Task<IActionResult> GetFrameworkGroupsTree()
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Request.RedirectCall(Wtm, "/api/_account/GetFrameworkGroupsTree").Result;
            }
            return Ok(await DC.Set<FrameworkGroup>().GetTreeSelectListItems(Wtm, x => x.GroupName, x => x.GroupCode));
        }


        [HttpGet("GetUserById")]
        [AllRights]
        public async Task<IActionResult> GetUserById (string keywords)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return await Request.RedirectCall(Wtm, "/api/_account/GetUserById");
            }
            var users = await DC.Set<FrameworkUser>().Where(x => x.ITCode.ToLower().StartsWith(keywords.ToLower())).GetSelectListItems(Wtm, x => x.Name + "(" + x.ITCode + ")", x => x.ITCode);
            return Ok(users);
        }

        [HttpGet("GetUserByGroup")]
        [AllRights]
        public async Task<IActionResult> GetUserByGroup (string keywords)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return await Request.RedirectCall(Wtm, "/api/_account/GetUserByGroup");
            }
            var users = await DC.Set<FrameworkUserGroup>().Where(x => x.GroupCode == keywords).Select(x => x.UserCode).ToListAsync();
            return Ok(users);
        }

        [HttpGet("GetUserByRole")]
        [AllRights]
        public async Task<IActionResult> GetUserByRole (string keywords)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return await Request.RedirectCall(Wtm, "/api/_account/GetUserByRole");
            }
            var users = await DC.Set<FrameworkUserRole>().Where(x => x.RoleCode == keywords).Select(x => x.UserCode).ToListAsync();
            return Ok(users);
        }

    }

    public class SimpleLogin
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string Tenant { get; set; }

        public string RemoteToken { get; set; }
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
