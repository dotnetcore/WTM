
using System;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.ViewModels.HomeVMs;
using WalkingTec.Mvvm.Mvc;
using Microsoft.Extensions.Logging;
using WalkingTec.Mvvm.Demo.Models;

namespace WalkingTec.Mvvm.Demo.Controllers
{
    [AllRights]
    public class LoginController : BaseController
    {
        [Public]
        [ActionDescription("Login")]
        public IActionResult Login()
        {
            LoginVM vm = CreateVM<LoginVM>();
            vm.Redirect = HttpContext.Request.Query["Redirect"];
            if (ConfigInfo.IsQuickDebug == true)
            {
                vm.ITCode = "admin";
                vm.Password = "000000";
            }
            return View(vm);
        }

        [Public]
        [HttpPost]
        public async Task<ActionResult> Login(LoginVM vm)
        {
            if (ConfigInfo.IsQuickDebug == false)
            {
                var verifyCode = HttpContext.Session.Get<string>("verify_code");
                if (string.IsNullOrEmpty(verifyCode) || verifyCode.ToLower() != vm.VerifyCode.ToLower())
                {
                    vm.MSD.AddModelError("", Localizer["Login.ValidationFail"]);
                    return View(vm);
                }
            }

            var user = vm.DoLogin();
            if (user == null)
            {
                return View(vm);
            }
            else
            {
                LoginUserInfo = user;
                string url = string.Empty;
                if (!string.IsNullOrEmpty(vm.Redirect))
                {
                    url = vm.Redirect;
                }
                else
                {
                    url = "/";
                }

                AuthenticationProperties properties = null;
                if (vm.RememberLogin)
                {
                    properties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30))
                    };
                }

                var principal = user.CreatePrincipal();
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

                return Redirect(HttpUtility.UrlDecode(url));
            }
        }

        [Public]
        public IActionResult Reg()
        {
            var vm = CreateVM<RegVM>();
            return PartialView(vm);
        }

        [Public]
        [HttpPost]
        public IActionResult Reg(RegVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                var rv = vm.DoReg();
                if (rv == true)
                {
                    return FFResult().CloseDialog().Message(Localizer["Reg.Success"]);
                }
                else
                {
                    return PartialView(vm);
                }
            }
        }


        [AllRights]
        [ActionDescription("Logout")]
        public async Task Logout()
        {
            await LoginUserInfo.RemoveUserCache(LoginUserInfo.Id.ToString());
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Response.Redirect("/");
        }

        [AllRights]
        [ActionDescription("ChangePassword")]
        public ActionResult ChangePassword()
        {
            var vm = CreateVM<ChangePasswordVM>();
            vm.ITCode = LoginUserInfo.ITCode;
            return PartialView(vm);
        }

        [AllRights]
        [HttpPost]
        [ActionDescription("ChangePassword")]
        public ActionResult ChangePassword(ChangePasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                vm.DoChange();
                return FFResult().CloseDialog().Alert(Localizer["ChangePasswordSuccess"]);
            }
        }

    }

}
