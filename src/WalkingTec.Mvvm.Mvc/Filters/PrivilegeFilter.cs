using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using WalkingTec.Mvvm.Core;

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
            if (controller.ConfigInfo.IsQuickDebug && controller is BaseApiController)
            {
                base.OnActionExecuting(context);
                return;
            }
            ControllerActionDescriptor ad = context.ActionDescriptor as ControllerActionDescriptor;

            if (controller is BaseController)
            {
                controller.BaseUrl = $"/{ad.ControllerName}/{ad.ActionName}";
                controller.BaseUrl += context.HttpContext.Request.QueryString.ToUriComponent();
                if (context.RouteData.Values["area"] != null)
                {
                    controller.BaseUrl = $"/{context.RouteData.Values["area"]}{controller.BaseUrl}";
                }
            }
            if (controller is BaseApiController)
            {
                var lg = GlobalServices.GetRequiredService<LinkGenerator>();
                var u = lg.GetPathByAction(ad.ActionName, ad.ControllerName, new { area = context.RouteData.Values["area"] });
                if (u == null)
                {
                    u = lg.GetPathByAction(ad.ActionName, ad.ControllerName, new { area = context.RouteData.Values["area"], id = 0 });
                }
                if (u.EndsWith("/0"))
                {
                    u = u.Substring(0, u.Length - 2);
                    u = u + "/{id}";
                }
                controller.BaseUrl = u;

            }

            //如果是QuickDebug模式，或者Action或Controller上有AllRightsAttribute标记都不需要判断权限
            //如果用户登录信息为空，也不需要判断权限，BaseController中会对没有登录的用户做其他处理

            var isPublic = ad.MethodInfo.IsDefined(typeof(PublicAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(PublicAttribute), false);
            var isAllRights = ad.MethodInfo.IsDefined(typeof(AllRightsAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(AllRightsAttribute), false);
            var isDebug = ad.MethodInfo.IsDefined(typeof(DebugOnlyAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(DebugOnlyAttribute), false);
            if (controller.ConfigInfo.IsFilePublic == true)
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
                        context.Result = c.Content("该地址只能在调试模式下访问");
                    }
                    else if (controller is ControllerBase c2)
                    {
                        context.Result = c2.BadRequest("该地址只能在调试模式下访问");
                    }
                }
                return;
            }

            if (isPublic == true)
            {
                base.OnActionExecuting(context);
                return;
            }

            if (controller.LoginUserInfo == null)
            {
                var publicMenu = controller.GlobaInfo.AllMenus
                               .Where(x => x.Url != null
                                   && x.Url.ToLower() == "/" + ad.ControllerName.ToLower() + "/" + ad.ActionName
                                   && x.IsPublic == true)
                               .FirstOrDefault();
                if (publicMenu == null)
                {
                    if (controller is BaseController c)
                    {
                        context.Result = new ContentResult { Content = $"<script>window.location.href = '/Login/Login?rd={HttpUtility.UrlEncode(controller.BaseUrl)}'</script>", ContentType = "text/html" };
                    }
                    else if (controller is ControllerBase c2)
                    {
                        context.Result = c2.Forbid();
                    }
                    return;
                }
            }
            else
            {
                if (isAllRights == false)
                {
                    bool canAccess = controller.LoginUserInfo.IsAccessable(controller.BaseUrl);
                    if (canAccess == false && controller.ConfigInfo.IsQuickDebug == false)
                    {
                        if (controller is BaseController c)
                        {
                            throw new Exception("您没有访问该页面的权限");
                        }
                        else if (controller is ControllerBase c2)
                        {
                            context.Result = c2.Unauthorized();
                        }
                    }
                }
            }
            base.OnActionExecuting(context);
        }
    }
}
