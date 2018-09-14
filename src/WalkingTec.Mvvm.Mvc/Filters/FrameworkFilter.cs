using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using System.Collections.Generic;
using System.Reflection;
using WalkingTec.Mvvm.Core.Implement;

namespace WalkingTec.Mvvm.Mvc.Filters
{
    public class FrameworkFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var ctrl = context.Controller as IBaseController;
            if (ctrl == null)
            {
                base.OnActionExecuting(context);
                return;
            }


            if (context.HttpContext.Items.ContainsKey("actionstarttime") == false)
            {
                context.HttpContext.Items.Add("actionstarttime", DateTime.Now);
            }
            var ctrlActDesc = context.ActionDescriptor as ControllerActionDescriptor;
            var log = new ActionLog();// 初始化log备用
            var ctrlDes = ctrlActDesc.ControllerTypeInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
            var actDes = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
            var postDes = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>().FirstOrDefault();
            log.ITCode = ctrl.LoginUserInfo?.ITCode ?? string.Empty;
            //给日志的多语言属性赋值
            log.ModuleName = ctrlDes?.Description ?? ctrlActDesc.ControllerName;
            log.ActionName = actDes?.Description ?? ctrlActDesc.ActionName + (postDes == null ? string.Empty : "[P]");
            log.ActionUrl = context.HttpContext.GetRemoteIpAddress();
            log.IP = context.HttpContext.Connection.RemoteIpAddress.ToString();

            ctrl.Log = log;
            foreach (var item in context.ActionArguments)
            {
                if (item.Value is BaseVM)
                {
                    var model = item.Value as BaseVM;
                    model.Session = new SessionServiceProvider(context.HttpContext.Session);
                    model.DC = ctrl.DC;
                    model.MSD = new ModelStateServiceProvider(ctrl.ModelState);
                    model.FC = new Dictionary<string, object>();
                    model.CreatorAssembly = this.GetType().Assembly.FullName;
                    model.FromFixedCon = ctrlActDesc.MethodInfo.IsDefined(typeof(FixConnectionAttribute), false) || ctrlActDesc.ControllerTypeInfo.IsDefined(typeof(FixConnectionAttribute), false); ;
                    model.CurrentCS = ctrl.CurrentCS;
                    model.Log = ctrl.Log;
                    model.CurrentUrl = ctrl.BaseUrl;
                    model.ConfigInfo = (Configs)context.HttpContext.RequestServices.GetService(typeof(Configs));
                    model.DataContextCI = ((GlobalData)context.HttpContext.RequestServices.GetService(typeof(GlobalData))).DataContextCI;
                    if (ctrl is BaseController c)
                    {
                        model.WindowIds = c.WindowIds;
                        model.UIService = c.UIService;
                    }
                    else
                    {
                        model.WindowIds = "";
                        model.UIService = new DefaultUIService();
                    }
                    try
                    {
                        var f = context.HttpContext.Request.Form;
                        foreach (var key in f.Keys)
                        {
                            if (model.FC.Keys.Contains(key) == false)
                            {
                                model.FC.Add(key, f[key]);
                            }
                        }
                        if (context.HttpContext.Request.QueryString != null)
                        {
                            foreach (var key in context.HttpContext.Request.Query.Keys)
                            {
                                if (model.FC.Keys.Contains(key) == false)
                                {
                                    model.FC.Add(key, context.HttpContext.Request.Query[key]);
                                }
                            }
                        }
                    }
                    catch { }

                    //如果ViewModel T继承自IBaseBatchVM<BaseVM>，则自动为其中的ListVM和EditModel初始化数据
                    if (model is IBaseBatchVM<BaseVM>)
                    {
                        var temp = model as IBaseBatchVM<BaseVM>;
                        if (temp.ListVM != null)
                        {
                            temp.ListVM.CopyContext(model);
                            temp.ListVM.Ids = temp.Ids == null ? new List<Guid>() : temp.Ids.ToList();
                            temp.ListVM.SearcherMode = ListVMSearchModeEnum.Batch;
                            temp.ListVM.NeedPage = false;
                        }
                        if (temp.LinkedVM != null)
                        {
                            temp.LinkedVM.CopyContext(model);
                        }
                        if (temp.ListVM != null)
                        {
                            //绑定ListVM的OnAfterInitList事件，当ListVM的InitList完成时，自动将操作列移除
                            temp.ListVM.OnAfterInitList += (self) =>
                            {
                                self.RemoveActionColumn();
                                self.RemoveAction();
                                if (temp.ErrorMessage.Count > 0)
                                {
                                    self.AddErrorColumn();
                                }
                            };
                            if (temp.ListVM.Searcher != null)
                            {
                                var searcher = temp.ListVM.Searcher;
                                searcher.CopyContext(model);
                                //searcher.DoReInit();
                            }
                        }
                        temp.LinkedVM?.DoInit();
                    }
                    if (model is IBaseImport<BaseTemplateVM>)
                    {
                        var template = (model as IBaseImport<BaseTemplateVM>).Template;
                        template.CopyContext(model);
                        template.DoReInit();
                    }
                    //SetReinit
                    var invalid = ctrl.ModelState.Where(x => x.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid).Select(x => x.Key).ToList();
                    foreach (var v in invalid)
                    {
                        if (model.FC.ContainsKey(v) == false)
                        {
                            ctrl.ModelState.Remove(v);
                        }
                    }
                    var reinit = model.GetType().GetTypeInfo().GetCustomAttributes(typeof(ReInitAttribute), false).Cast<ReInitAttribute>().SingleOrDefault();
                    model.Validate();
                    if (ctrl.ModelState.IsValid)
                    {
                        if (reinit != null && (reinit.ReInitMode == ReInitModes.SUCCESSONLY || reinit.ReInitMode == ReInitModes.ALWAYS))
                        {
                            model.DoReInit();
                        }
                    }
                    else
                    {                        
                        if (reinit == null || (reinit.ReInitMode == ReInitModes.FAILEDONLY || reinit.ReInitMode == ReInitModes.ALWAYS))
                        {
                            model.DoReInit();
                        }
                    }
                }
            }

            base.OnActionExecuting(context);
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var ctrl = context.Controller as BaseController;
            if (ctrl == null)
            {
                base.OnResultExecuting(context);
                return;
            }
            ctrl.ViewData["DONOTUSE_COOKIEPRE"] = ctrl.ConfigInfo.CookiePre;
            var ctrlActDesc = context.ActionDescriptor as ControllerActionDescriptor;
            if (context.Result is PartialViewResult)
            {
                var model = (context.Result as PartialViewResult).ViewData.Model as BaseVM;
                if (model == null)
                {
                    model = ctrl.CreateVM<BaseVM>();
                    (context.Result as PartialViewResult).ViewData.Model = model;
                }
                // 为所有 PartialView 加上最外层的 Div
                if (model != null)
                {
                    context.HttpContext.Response.OnStarting(() =>
                    {
                        return context.HttpContext.Response.WriteAsync($"<div id='{model.ViewDivId}' class='donotuse_pdiv'>");
                    });
                }
            }
            if (context.Result is ViewResult)
            {
                var model = (context.Result as ViewResult).ViewData.Model as BaseVM;
                if (model == null)
                {
                    model = ctrl.CreateVM<BaseVM>();
                    (context.Result as ViewResult).ViewData.Model = model;
                }
            }
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            var ctrl = context.Controller as IBaseController;
            var ctrlActDesc = context.ActionDescriptor as ControllerActionDescriptor;
            //如果是来自Error，则已经记录过日志，跳过
            if (ctrlActDesc.ControllerName == "_Framework" && ctrlActDesc.ActionName == "Error")
            {
                return;
            }
            if (ctrl.ConfigInfo.EnableLog == true)
            {
                if (ctrl.ConfigInfo.LogExceptionOnly == false || context.Exception != null)
                {
                    var log = new ActionLog();
                    var ctrlDes = ctrlActDesc.ControllerTypeInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
                    var actDes = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
                    var postDes = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>().FirstOrDefault();

                    log.LogType = context.Exception == null ? ActionLogTypesEnum.Normal : ActionLogTypesEnum.Exception;
                    log.ActionTime = DateTime.Now;
                    log.ITCode = ctrl.LoginUserInfo?.ITCode ?? string.Empty;
                    // 给日志的多语言属性赋值
                    log.ModuleName = ctrlDes?.Description ?? ctrlActDesc.ControllerName;
                    log.ActionName = actDes?.Description ?? ctrlActDesc.ActionName + (postDes == null ? string.Empty : "[P]");
                    log.ActionUrl = context.HttpContext.Request.Path;
                    log.IP = context.HttpContext.GetRemoteIpAddress();
                    log.Remark = context.Exception?.ToString() ?? string.Empty;
                    var starttime = context.HttpContext.Items["actionstarttime"] as DateTime?;
                    if (starttime != null)
                    {
                        log.Duration = DateTime.Now.Subtract(starttime.Value).TotalSeconds;
                    }
                    using (var dc = ctrl.CreateDC(true))
                    {
                        dc.Set<ActionLog>().Add(log);
                        dc.SaveChanges();
                    }
                }
            }
            if (context.Exception != null)
            {
                context.ExceptionHandled = true;
                if (ctrl.ConfigInfo.IsQuickDebug == true)
                {
                    context.HttpContext.Response.WriteAsync(context.Exception.ToString());
                }
                else
                {
                    context.HttpContext.Response.WriteAsync("页面发生错误");
                }
            }
            if (context.Result is PartialViewResult pr)
            {
                var basevm = pr.Model as BaseVM;
                var title = basevm?.Log?.ModuleName + "-" + basevm?.Log?.ActionName;
                context.HttpContext.Response.WriteAsync($@"
<script>
    var title = document.title;
    if (title == null || title == '')
    {{
        title = ff.GetCookie(window.location.hash,true);
        if (title == null || title == '')
        {{
            title = '{title}';
        }}
        document.title = title;
        ff.SetCookie(window.location.hash,null,true);
    }}
</script>
");
                context.HttpContext.Response.WriteAsync("</div>");
            }
        }
    }
}
