using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Filters
{
    /// <summary>
    /// 权限过滤器，在 Action 执行前依次完成：构造当前请求的基准 URL、
    /// 判断公开（Public/AllowAnonymous/文件公开）、调试专用（DebugOnly）、登录状态、
    /// 主租户限制（MainTenantOnly）等分支校验，并最终基于菜单配置的 URL 权限决定是否放行。
    /// 校验未通过时按请求类型（layui 搜索、携带 Authorization 头的请求、API、普通页面）返回不同响应。
    /// </summary>
    public class PrivilegeFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Action 执行前触发：初始化 WTM 上下文并触发登录信息加载，生成当前请求的基准 URL，
        /// 再依次进行公开标记、调试标记、登录状态、主租户限制和 URL 权限的校验，未通过则直接改写返回结果。
        /// </summary>
        /// <param name="context">Action 执行前的过滤器上下文</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as IBaseController;
            if (controller == null)
            {
                base.OnActionExecuting(context);
                return;
            }
            context.SetWtmContext();
            // 访问一次 LoginUserInfo，触发框架内部加载当前用户的登录信息与权限数据
            _ =controller.Wtm.LoginUserInfo;
            ControllerActionDescriptor ad = context.ActionDescriptor as ControllerActionDescriptor;

            // 通过 LinkGenerator 生成当前 Action 的标准相对路径，作为 BaseUrl 和后续权限比对的基础
            var lg = context.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();

            string u = null;
            // Action 带 id 参数时先用 0 占位生成 URL，后面再统一移除占位符
            if (ad.Parameters.Any(x=>x.Name.ToLower() == "id"))
            {
                u = lg.GetPathByAction(ad.ActionName, ad.ControllerName, new { area = context.RouteData.Values["area"], id = 0 });
            }
            else
            {
                u = lg.GetPathByAction(ad.ActionName, ad.ControllerName, new { area = context.RouteData.Values["area"] });
            }
            // 移除占位的 /0 或 ?id=0；API 控制器则还原为路由模板形式的 /{id}
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

            // 将最终 URL（含查询串）保存为 BaseUrl，供日志记录和后续权限判断使用
            controller.Wtm.BaseUrl = u + context.HttpContext.Request.QueryString.ToUriComponent();


            //如果是QuickDebug模式，或者Action或Controller上有AllRightsAttribute标记都不需要判断权限
            //如果用户登录信息为空，也不需要判断权限，BaseController中会对没有登录的用户做其他处理

            // 依次读取 Public/AllowAnonymous、AllRights、DebugOnly、MainTenantOnly 等标记，决定后续走哪个校验分支
            var isPublic = ad.MethodInfo.IsDefined(typeof(PublicAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(PublicAttribute), false);
            if (!isPublic)
                isPublic = ad.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(AllowAnonymousAttribute), false);

            var isAllRights = ad.MethodInfo.IsDefined(typeof(AllRightsAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(AllRightsAttribute), false);
            var isDebug = ad.MethodInfo.IsDefined(typeof(DebugOnlyAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(DebugOnlyAttribute), false);
            var isHostOnly = ad.MethodInfo.IsDefined(typeof(MainTenantOnlyAttribute), false) || ad.ControllerTypeInfo.IsDefined(typeof(MainTenantOnlyAttribute), false);
            // 配置了文件公开时，框架内置的文件查看/下载接口视为公开接口，无需登录即可访问
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
            // DebugOnly 标记的接口仅在快速调试模式下可访问，否则返回提示并终止执行
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

            // 特性未标记公开时，再检查系统配置中是否将当前 URL 配置为公开页面
            if(isPublic == false && controller != null && controller.Wtm != null)
            {
                isPublic = controller.Wtm.IsUrlPublic(u);
            }
            if (isPublic == true)
            {
                base.OnActionExecuting(context);
                return;
            }

            // 未登录的处理：layui 搜索请求返回可解析的 JSON；携带 Authorization 头的请求返回 401；
            // API 控制器返回 401 文本；普通页面输出跳转登录页的脚本并携带 ReturnUrl，便于登录后跳回
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
                                // 普通页面：生成跳转登录页的前端脚本；除登出请求外，当前地址作为 ReturnUrl 回传，登录后自动跳回
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
                                context.Result = cr;
                            }
                        }
                    }
                    base.OnActionExecuting(context);
                    return;
                }
            }
            // MainTenantOnly 标记：仅主租户（未绑定租户）可访问，已绑定租户的用户按请求类型返回拒绝
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
                        else if(controller is BaseApiController bac)
                        {
                            context.Result = bac.Forbid();
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

                // 未标记 AllRights 时进行最终的 URL 权限判定；快速调试模式跳过判定直接放行
                if (isAllRights == false)
                {
                    bool canAccess = controller.Wtm.IsAccessable(controller.BaseUrl);
                    if (canAccess == false && controller.ConfigInfo.IsQuickDebug == false)
                    {
                        // 无权限时的响应同样按请求类型区分：layui 格式 JSON、Forbid（403）、API 的 Forbid 或提示文本
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
                            else if (controller is BaseApiController bac)
                            {
                                context.Result = bac.Forbid();
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

        // 从 Action 或 Controller 上的 AuthorizeAttribute 中解析认证方案列表（Action 上的配置优先）
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
