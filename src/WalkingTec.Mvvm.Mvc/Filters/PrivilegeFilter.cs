using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth;

namespace WalkingTec.Mvvm.Mvc.Filters
{
    public class PrivilegeFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as IBaseController;
            if (controller == null)
            {
                base.OnActionExecuting(context);
                return;
            }
            if (controller.WtmContext == null)
            {
                controller.WtmContext = context.HttpContext.RequestServices.GetService(typeof(WTMContext)) as WTMContext;
                try
                {
                    controller.WtmContext.MSD = new ModelStateServiceProvider(context.ModelState);
                    controller.WtmContext.Session = new SessionServiceProvider(context.HttpContext.Session);
                }
                catch { }
            }

            if (controller.WtmContext.ConfigInfo.IsQuickDebug && controller is BaseApiController)
            {
                base.OnActionExecuting(context);
                return;
            }
            ControllerActionDescriptor ad = context.ActionDescriptor as ControllerActionDescriptor;

            var lg = GlobalServices.GetRequiredService<LinkGenerator>();

            string u = null;
            if (ad.Parameters.Any(x=>x.Name.ToLower() == "id"))
            {
                u = lg.GetPathByAction(ad.ActionName, ad.ControllerName, new { area = context.RouteData.Values["area"], id = 0 });
            }
            else
            {
                u = lg.GetPathByAction(ad.ActionName, ad.ControllerName, new { area = context.RouteData.Values["area"] });
            }
            if (u != null && u.EndsWith("/0"))
            {
                u = u.Substring(0, u.Length - 2);
                if (controller is BaseApiController)
                {
                    u = u + "/{id}";
                }
            }

            controller.WtmContext.BaseUrl = u + context.HttpContext.Request.QueryString.ToUriComponent(); ;


            //如果是QuickDebug模式，或者Action或Controller上有AllRightsAttribute标记都不需要判断权限
            //如果用户登录信息为空，也不需要判断权限，BaseController中会对没有登录的用户做其他处理

            var isPublic = ad.MethodInfo.IsDefined(typeof(PublicAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(PublicAttribute), false);
            if (!isPublic)
                isPublic = ad.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(AllowAnonymousAttribute), false);

            var isAllRights = ad.MethodInfo.IsDefined(typeof(AllRightsAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(AllRightsAttribute), false);
            var isDebug = ad.MethodInfo.IsDefined(typeof(DebugOnlyAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(DebugOnlyAttribute), false);
            if (controller.WtmContext.ConfigInfo.IsFilePublic == true)
            {
                if (ad.ControllerName == "_Framework" && (ad.MethodInfo.Name == "GetFile" || ad.MethodInfo.Name == "ViewFile"))
                {
                    isPublic = true;
                }
            }
            if (isDebug)
            {
                if (controller.ConfigInfo.IsQuickDebug)
                {
                    base.OnActionExecuting(context);
                }
                else
                {
                    if (controller is BaseController c)
                    {
                        context.Result = c.Content(Program._localizer["DebugOnly"]);
                    }
                    else if (controller is ControllerBase c2)
                    {
                        context.Result = c2.BadRequest(Program._localizer["DebugOnly"]);
                    }
                }
                return;
            }

            if (isPublic == true)
            {
                base.OnActionExecuting(context);
                return;
            }

            if (controller.WtmContext.LoginUserInfo == null)
            {
                context.HttpContext.ChallengeAsync().Wait();
            }
            else
            {
                if (isAllRights == false)
                {
                    bool canAccess = controller.WtmContext.IsAccessable(controller.BaseUrl);
                    if (canAccess == false && controller.ConfigInfo.IsQuickDebug == false)
                    {
                        if (controller is ControllerBase ctrl)
                        {
                            var authenticationSchemes = new List<string>();
                            if (ad.MethodInfo.IsDefined(typeof(AuthorizeAttribute), false))
                            {
                                var authorizeAttr = ad.MethodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;
                                if (authorizeAttr != null)
                                    authenticationSchemes = authorizeAttr.AuthenticationSchemes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            }
                            else if (ad.ControllerTypeInfo.IsDefined(typeof(AuthorizeAttribute), false))
                            {
                                var authorizeAttr = ad.ControllerTypeInfo.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;
                                if (authorizeAttr != null)
                                    authenticationSchemes = authorizeAttr.AuthenticationSchemes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            }

                            if (ctrl.HttpContext.Request.Headers.ContainsKey("Authorization")
                                && authenticationSchemes.Contains(JwtBearerDefaults.AuthenticationScheme))
                            {
                                context.Result = ctrl.Forbid(JwtBearerDefaults.AuthenticationScheme);
                            }
                            else if (ctrl.HttpContext.Request.Cookies.ContainsKey(CookieAuthenticationDefaults.CookiePrefix + AuthConstants.CookieAuthName)
                                && authenticationSchemes.Contains(CookieAuthenticationDefaults.AuthenticationScheme))
                            {
                                throw new Exception(Program._localizer["NoPrivilege"]);
                            }
                            else
                            {
                                throw new Exception(Program._localizer["NoPrivilege"]);
                            }
                        }
                    }
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
