// WTM默认页面 Wtm buidin page
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Login([FromForm] string account, [FromForm] string password, [FromForm] string tenant = null, [FromForm] bool rememberLogin = false)
        {

            var user = Wtm.DoLogin(account, password, tenant);
            if (user == null)
            {
                return BadRequest(Localizer["Sys.LoginFailed"].Value);
            }

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
            return CheckUserInfo();
        }


        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> LoginJwt(SimpleLogin loginInfo)
        {
            var user = Wtm.DoLogin(loginInfo.Account, loginInfo.Password, loginInfo.Tenant);
            if (user == null)
            {
                ModelState.AddModelError(" ", Localizer["Sys.LoginFailed"]);
                return BadRequest(ModelState.GetErrorJson());
            }

            //其他属性可以通过user.Attributes["aaa"] = "bbb"方式赋值

            Wtm.LoginUserInfo = user;
            var authService = HttpContext.RequestServices.GetService(typeof(ITokenService)) as ITokenService;
            var token = await authService.IssueTokenAsync(Wtm.LoginUserInfo);
            return Content(JsonSerializer.Serialize(token), "application/json");
        }

        [AllRights]
        [HttpGet("[action]")]
        public IActionResult SetTenant([FromQuery] string tenant)
        {
            bool rv = Wtm.SetCurrentTenant(tenant == "" ? null : tenant);
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
        public IActionResult CheckUserInfo(bool IsApi = true)
        {
            if (Wtm.LoginUserInfo == null)
            {
                return BadRequest();
            }
            else
            {
                var forapi = Wtm.LoginUserInfo;
                if (IsApi)
                {
                    forapi.SetAttributesForApi(Wtm);
                }
                forapi.DataPrivileges = null;
                forapi.FunctionPrivileges = null;
                return Ok(forapi);
            }
        }


        [AllRights]
        [HttpPost("[action]")]
        public IActionResult ChangePassword(ChangePasswordVM vm)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Request.RedirectCall(Wtm).Result;
            }
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

        [Public]
        [HttpGet("[action]")]
        public async Task<IActionResult> Logout()
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
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
