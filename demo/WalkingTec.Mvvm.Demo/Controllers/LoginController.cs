
using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;
using WalkingTec.Mvvm.Demo.ViewModels.HomeVMs;
using System.Collections.Generic;
using System.Linq;

namespace WalkingTec.Mvvm.Demo.Controllers
{
    [AllRights]
    public class LoginController : BaseController
    {
        [Public]
        [ActionDescription("Login")]
        public IActionResult Login()
        {
            LoginVM vm = Wtm.CreateVM<LoginVM>();
            vm.Redirect = HttpContext.Request.Query["ReturnUrl"];
            if (Wtm.ConfigInfo.IsQuickDebug == true)
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
            if (Wtm.ConfigInfo.IsQuickDebug == false)
            {
                var verifyCode = HttpContext.Session.Get<string>("verify_code");
                if (string.IsNullOrEmpty(verifyCode) || verifyCode.ToLower() != vm.VerifyCode.ToLower())
                {
                    vm.MSD.AddModelError("", Localizer["Login.ValidationFail"]);
                    return View(vm);
                }
            }
            var user = Wtm.DoLogin(vm.ITCode, vm.Password, vm.Tenant);
            if (user == null)
            {
                vm.MSD.AddModelError("", Localizer["Sys.LoginFailed"]);
                return View(vm);
            }
            else
            {
                //其他属性可以通过user.Attributes["aaa"] = "bbb"方式赋值
                Wtm.LoginUserInfo = user;
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
            if (ConfigInfo.HasMainHost)
            {
                
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<RegVM>();
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
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (ConfigInfo.HasMainHost == false || Wtm.LoginUserInfo?.CurrentTenant != null)
            {
                HttpContext.Response.Redirect("/");
            }
            else
            {
                Wtm.CallAPI<string>("mainhost", "/api/_account/logout", HttpMethodEnum.GET, new { }, 10).Wait();
                HttpContext.Response.Redirect(ConfigInfo.MainHost);
            }
        }

        [AllRights]
        [ActionDescription("ChangePassword")]
        public ActionResult ChangePassword()
        {
            var vm = Wtm.CreateVM<ChangePasswordVM>();
            vm.ITCode = Wtm.LoginUserInfo.ITCode;
            return PartialView(vm);
        }

        [AllRights]
        [HttpPost]
        [ActionDescription("ChangePassword")]
        public ActionResult ChangePassword(ChangePasswordVM vm)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                var result = Wtm.CallAPI<string>("mainhost", "/api/_account/ChangePassword", HttpMethodEnum.POST, vm, 10).Result;
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return FFResult().CloseDialog().Alert(Localizer["Login.ChangePasswordSuccess"]);
                }
                else if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ModelState.Clear();
                    foreach (var item in result.Errors.Form)
                    {
                        ModelState.AddModelError(item.Key, item.Value);
                    }
                    foreach (var item in result.Errors.Message)
                    {
                        ModelState.AddModelError(" ", item);
                    }
                    return PartialView(vm);
                }
                else
                {
                    return FFResult().CloseDialog().Alert(Localizer["Sys.Error"]);
                }
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return PartialView(vm);
                }
                else
                {
                    vm.DoChange();
                    return FFResult().CloseDialog().Alert(Localizer["Login.ChangePasswordSuccess"]);
                }
            }
        }

    }

}
