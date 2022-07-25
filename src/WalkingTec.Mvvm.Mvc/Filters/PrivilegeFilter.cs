using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            context.SetWtmContext();
            _ =controller.Wtm.LoginUserInfo;
            //if (controller.Wtm.ConfigInfo.IsQuickDebug && controller is BaseApiController)
            //{
            //    base.OnActionExecuting(context);
            //    return;
            //}
            ControllerActionDescriptor ad = context.ActionDescriptor as ControllerActionDescriptor;

            var lg = context.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();

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
                u = u[0..^2];
                if (controller is BaseApiController)
                {
                    u = u + "/{id}";
                }
            }
            if (u != null && u.EndsWith("?id=0"))
            {
                u = u[0..^5];
                if (controller is BaseApiController)
                {
                    u = u + "/{id}";
                }
            }

            controller.Wtm.BaseUrl = u + context.HttpContext.Request.QueryString.ToUriComponent();


            //如果是QuickDebug模式，或者Action或Controller上有AllRightsAttribute标记都不需要判断权限
            //如果用户登录信息为空，也不需要判断权限，BaseController中会对没有登录的用户做其他处理

            var isPublic = ad.MethodInfo.IsDefined(typeof(PublicAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(PublicAttribute), false);
            if (!isPublic)
                isPublic = ad.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(AllowAnonymousAttribute), false);

            var isAllRights = ad.MethodInfo.IsDefined(typeof(AllRightsAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(AllRightsAttribute), false);
            var isDebug = ad.MethodInfo.IsDefined(typeof(DebugOnlyAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(DebugOnlyAttribute), false);
            var isHostOnly = ad.MethodInfo.IsDefined(typeof(MainTenantOnlyAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(MainTenantOnlyAttribute), false);
            if (controller.Wtm.ConfigInfo.IsFilePublic == true)
            {
                if (ad.ControllerName == "_Framework" && (ad.MethodInfo.Name == "GetFile" || ad.MethodInfo.Name == "ViewFile"))
                {
                    isPublic = true;
                }
                if(ad.ControllerTypeInfo.FullName == "WalkingTec.Mvvm.Admin.Api.FileApiController" && (ad.MethodInfo.Name == "GetFileName" || ad.MethodInfo.Name == "GetFile" || ad.MethodInfo.Name == "DownloadFile"))
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
                        context.Result = c.Content(MvcProgram._localizer["Sys.DebugOnly"]);
                    }
                    else if (controller is ControllerBase c2)
                    {
                        context.Result = c2.BadRequest(MvcProgram._localizer["Sys.DebugOnly"]);
                    }
                }
                return;
            }

            if(isPublic == false && controller != null && controller.Wtm != null)
            {
                isPublic = controller.Wtm.IsUrlPublic(u);
            }
            if (isPublic == true)
            {
                base.OnActionExecuting(context);
                return;
            }

            if (controller.Wtm.LoginUserInfo == null)
            {
                if (controller is ControllerBase ctrl)
                {
                    //if it's a layui search request,returns a layui format message so that it can parse
                    if (ctrl.Request.Headers.ContainsKey("layuisearch"))
                    {
                        ContentResult cr = new ContentResult()
                        {
                            Content = "{\"Data\":[],\"Count\":0,\"Page\":1,\"PageCount\":0,\"Msg\":\"" + MvcProgram._localizer["Sys.NeedLogin"] + "\",\"Code\":401}",
                            ContentType = "application/json",
                            StatusCode = 200
                        };
                        context.Result = cr;
                    }
                    else
                    {
                        if (ctrl.HttpContext.Request.Headers.ContainsKey("Authorization"))
                        {
                            context.Result = ctrl.Unauthorized(JwtBearerDefaults.AuthenticationScheme);
                        }
                        else
                        {
                            if (controller is BaseApiController)
                            {
                                ContentResult cr = new ContentResult()
                                {
                                    Content = MvcProgram._localizer["Sys.NeedLogin"],
                                    ContentType = "text/html",
                                    StatusCode = 401
                                };
                                context.Result = cr;
                            }
                            else
                            {
                                string lp = controller.Wtm.ConfigInfo.CookieOptions.LoginPath;
                                if (lp.StartsWith("/"))
                                {
                                    lp = "~" + lp;
                                }
                                if (lp.StartsWith("~/"))
                                {
                                    lp = ctrl.Url.Content(lp);
                                }
                                ContentResult cr = new ContentResult()
                                {
                                    Content = $"<script>var redirect='{(u?.ToLower().StartsWith("/login/logout")==true?"":u)}'+ window.location.hash; if(redirect=='/'){{window.location.href='{lp}'; }}  else{{window.location.href='{lp}?ReturnUrl='+encodeURIComponent(redirect);}}</script>",
                                    ContentType = "text/html",                                    
                                    StatusCode = 200
                                };
                                //context.HttpContext.Response.Headers.Add("IsScript", "true");
                                context.Result = cr;
                                //context.Result = ctrl.Redirect(GlobalServices.GetRequiredService<IOptions<CookieOptions>>().Value.LoginPath);
                            }
                        }
                    }
                    base.OnActionExecuting(context);
                    return;
                }
                //context.HttpContext.ChallengeAsync().Wait();
            }
            else if (isHostOnly)
            {
                if(controller.Wtm.LoginUserInfo.CurrentTenant != null)
                {
                    if (controller is ControllerBase ctrl)
                    {
                        if (ctrl.HttpContext.Request.Headers.ContainsKey("Authorization"))
                        {
                            context.Result = ctrl.Forbid(JwtBearerDefaults.AuthenticationScheme);
                        }
                        else
                        {
                            ContentResult cr = new ContentResult()
                            {
                                Content = MvcProgram._localizer["_Admin.TenantNotAllowed"],
                                ContentType = "text/html",
                                StatusCode = 200
                            };
                            context.Result = cr;
                        }
                        base.OnActionExecuting(context);
                        return;
                    }
                }
            }

                if (isAllRights == false)
                {
                    bool canAccess = controller.Wtm.IsAccessable(controller.BaseUrl);
                    if (canAccess == false && controller.ConfigInfo.IsQuickDebug == false)
                    {
                        if (controller is ControllerBase ctrl)
                        {
                            //if it's a layui search request,returns a layui format message so that it can parse
                            if (ctrl.Request.Headers.ContainsKey("layuisearch"))
                            {
                                ContentResult cr = new ContentResult()
                                {
                                    Content = "{\"Data\":[],\"Count\":0,\"Page\":1,\"PageCount\":0,\"Msg\":\""+ MvcProgram._localizer["Sys.NoPrivilege"] + "\",\"Code\":403}",
                                    ContentType = "application/json",
                                    StatusCode = 200
                                };
                                context.Result = cr;
                            }
                            else
                            {
                                if (ctrl.HttpContext.Request.Headers.ContainsKey("Authorization"))
                                {
                                    context.Result = ctrl.Forbid(JwtBearerDefaults.AuthenticationScheme);
                                }
                                else
                                {
                                    ContentResult cr = new ContentResult()
                                    {
                                        Content = MvcProgram._localizer["Sys.NoPrivilege"],
                                        ContentType = "text/html",
                                        StatusCode = 200
                                    };
                                    context.Result = cr;

                                }
                            }
                        }
                    }
                }
            
            base.OnActionExecuting(context);
        }

        private List<string> getAuthTypes(ControllerActionDescriptor ad)
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
            return authenticationSchemes;
        }
    }
}
