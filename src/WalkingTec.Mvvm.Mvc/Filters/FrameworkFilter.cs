using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Json;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Mvc.Filters
{
    /// <summary>
    /// 框架基础过滤器。Action 执行前负责初始化 WTM 上下文、构造日志、为 VM 参数填充上下文与 FC 字典、
    /// 初始化各类子 VM（批量 VM、导入 VM、列表 VM 等）并执行模型验证与重新初始化；
    /// Action 执行后处理视图路径、页面标题与容器 Div；结果执行完成后记录操作日志并兜底处理未捕获异常。
    /// </summary>
    public class FrameworkFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Action 执行前触发：初始化 WTM 上下文与日志对象，为 VM、Searcher 参数设置上下文并填充 FC 字典，
        /// 再按 VM 类型（批量、导入、列表等）完成子 VM 初始化、模型验证及验证错误的清理。
        /// </summary>
        /// <param name="context">Action 执行前的过滤器上下文</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var ctrl = context.Controller as IBaseController;
            if (ctrl == null)
            {
                base.OnActionExecuting(context);
                return;
            }
            context.SetWtmContext();
            // 记录 Action 开始执行的时间，供 OnResultExecuted 计算操作耗时
            if (context.HttpContext.Items.ContainsKey("actionstarttime") == false)
            {
                context.HttpContext.Items.Add("actionstarttime", DateTime.Now);
            }
            var ctrlActDesc = context.ActionDescriptor as ControllerActionDescriptor;
            var log = new SimpleLog();// 初始化log备用
            var ctrlDes = ctrlActDesc.ControllerTypeInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
            var actDes = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
            var postDes = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>().FirstOrDefault();
            var validpostonly = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(ValidateFormItemOnlyAttribute), false).Cast<ValidateFormItemOnlyAttribute>().FirstOrDefault();

            //给日志的多语言属性赋值
            log.ModuleName = ctrlDes?.GetDescription(ctrl) ?? ctrlActDesc.ControllerName;
            log.ActionName = actDes?.GetDescription(ctrl) ?? ctrlActDesc.ActionName + (postDes == null ? string.Empty : "[P]");
            log.ActionUrl = ctrl.BaseUrl;
            log.IP = context.HttpContext.Connection.RemoteIpAddress.ToString();

            ctrl.Wtm.Log = log;
            foreach (var item in context.ActionArguments)
            {
                if (item.Value is BaseVM)
                {
                    var model = item.Value as BaseVM;
                    model.Wtm = ctrl.Wtm;
                    model.FC = new Dictionary<string, object>();
                    model.CreatorAssembly = this.GetType().Assembly.FullName;
                    model.ControllerName = context.HttpContext.Request.Path;

                    // 将表单和 QueryString 中的键值填充到 VM 的 FC 字典，模拟框架原有的 FormCollection 机制
                    try
                    {
                        if (context.HttpContext.Request.HasFormContentType)
                        {
                            var f = context.HttpContext.Request.Form;
                            foreach (var key in f.Keys)
                            {
                                if (model.FC.Keys.Contains(key) == false)
                                {
                                    model.FC.Add(key, f[key]);
                                }
                            }
                        }
                        if (context.HttpContext.Request.QueryString != QueryString.Empty)
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

                    // API 控制器：解析缓存的请求体，把其中出现过的字段名补充到 FC（值为空串），供后续验证判断字段是否提交过
                    if (ctrl is BaseApiController apictrl)
                    {
                        //apictrl.TryValidateModel(model);
                        if (context.HttpContext.Items.ContainsKey("DONOTUSE_REQUESTBODY"))
                        {
                            string body = context.HttpContext.Items["DONOTUSE_REQUESTBODY"].ToString();
                            var joption = new JsonSerializerOptions(Core.CoreProgram.DefaultJsonOption);
                            joption.Converters.Add(new BodyConverter());
                            try
                            {
                                var obj = JsonSerializer.Deserialize<PostedBody>(body, joption);
                                foreach (var field in obj.ProNames)
                                {

                                    model.FC.Add(field, "");
                                }
                            }
                            catch { }
                        }
                    }
                    // 递归初始化所有子 VM 并传递上下文与父子关系
                    SetSubVm(model);

                    //如果ViewModel T继承自IBaseBatchVM<BaseVM>，则自动为其中的ListVM和EditModel初始化数据
                    if (model is IBaseBatchVM<BaseVM> temp)
                    {
                        if (temp.ListVM != null)
                        {
                            temp.ListVM.CopyContext(model);
                            temp.ListVM.Ids = temp.Ids == null ? new List<string>() : temp.Ids.ToList();
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
                                self.AddErrorColumn();
                            };
                            if (temp.ListVM.Searcher != null)
                            {
                                var searcher = temp.ListVM.Searcher;
                                searcher.CopyContext(model);
                            }
                            temp.ListVM.DoInitListVM();
                        }
                        temp.LinkedVM?.DoInit();
                    }
                    if (model is IBaseImport<BaseTemplateVM> tvm)
                    {
                        var template = tvm.Template;
                        template.CopyContext(model);
                        template.DoReInit();
                        var errorlist = tvm.ErrorListVM;
                        errorlist.CopyContext(model);
                    }
                    if (model is IBasePagedListVM<TopBasePoco, ISearcher> lvm)
                    {
                        var searcher = lvm.Searcher;
                        searcher.CopyContext(lvm);
                        if (ctrl is BaseController)
                        {
                            searcher.DoInit();
                        }
                    }
                    model.Validate();
                    //如果是子表外键验证错误，例如Entity.Majors[0].SchoolId为空这种错误，则忽略。因为框架会在添加修改的时候自动给外键赋值
                    var toremove = ctrl.ModelState.Select(x => x.Key).Where(x => Regex.IsMatch(x, ".*?\\[.*?\\]\\..*?id", RegexOptions.IgnoreCase));
                    foreach (var r in toremove)
                    {
                        ctrl.ModelState.Remove(r);
                    }
                    // PUT 请求或标记 ValidateFormItemOnly 时，仅保留前台实际提交过的 Entity 字段的验证错误，忽略未提交字段的错误
                    var invalid = ctrl.ModelState.Where(x => x.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid).Select(x => x.Key).ToList();
                    if ((ctrl as ControllerBase).Request.Method.ToLower() == "put" || validpostonly != null)
                    {
                        foreach (var v in invalid)
                        {
                            if (v?.StartsWith($"{model.GetParentStr()}Entity.") == true)
                            {
                                Regex r = new Regex("(.*?)\\[.*?\\](.*?$)");
                                var m = r.Match(v);
                                var check = v;
                                if (m.Success)
                                {
                                    check = m.Groups[1] + "[0]" + m.Groups[2];
                                }
                                if (model.FC.Keys.Any(x => x.ToLower() == check.ToLower()) == false)
                                {
                                    ctrl.ModelState.Remove(v);
                                }
                            }
                        }
                    }
                    // 根据模型上的 ReInitAttribute 配置，在模型验证成功或失败后决定是否重新初始化 VM
                    if (ctrl is BaseController)
                    {
                        var reinit = model.GetType().GetTypeInfo().GetCustomAttributes(typeof(ReInitAttribute), false).Cast<ReInitAttribute>().SingleOrDefault();
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

                if (item.Value is BaseSearcher se)
                {
                    se.FC = new Dictionary<string, object>();
                    se.Wtm = ctrl.Wtm;
                    se.Validate();
                }
            }

            //忽略绑定List<xxx>由于前台传递空字符串造成的错误
            base.OnActionExecuting(context);
        }

        // 递归查找 VM 中类型为 BaseVM 的属性（排除 ParentVM），为其创建实例、复制上下文并建立父子关系
        private void SetSubVm(BaseVM vm)
        {
            var sub = vm.GetType().GetAllProperties().Where(x => typeof(BaseVM).IsAssignableFrom(x.PropertyType) && x.Name != "ParentVM");
            foreach (var prop in sub)
            {
                var subins = prop.GetValue(vm) as BaseVM;
                bool exist = subins == null ? false : true;
                if (subins == null)
                {
                    subins = prop.PropertyType.GetConstructor(Type.EmptyTypes).Invoke(null) as BaseVM;
                }
                if(subins != null)
                {
                    subins.CopyContext(vm);
                    subins.ParentVM = vm;
                    subins.PropertyNameInParent = prop.Name;
                    if (exist == false)
                    {
                        vm.SetPropertyValue(prop.Name, subins);
                    }
                    SetSubVm(subins);
                }
            }

        }

        /// <summary>
        /// Action 执行后触发（仅处理 MVC 的 BaseController）：设置 Cookie 前缀，解析视图实际使用的路径，
        /// 为 PartialView 和 ViewResult 补充默认 VM、生成页面标题（写入 X-wtm-PageTitle 响应头）并记录容器 Div 的 id。
        /// </summary>
        /// <param name="context">Action 执行后的过滤器上下文</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var ctrl = context.Controller as BaseController;
            if (ctrl == null)
            {
                base.OnActionExecuted(context);
                return;
            }
            ctrl.ViewData["DONOTUSE_COOKIEPRE"] = ctrl.Wtm.ConfigInfo.CookiePre;
            var ctrlActDesc = context.ActionDescriptor as ControllerActionDescriptor;
            //get viewname
            string viewName = "";
            if (context.Result is PartialViewResult pvr)
            {
                viewName = pvr.ViewName ?? "";
                if (viewName?.StartsWith("/") == false)
                {
                    var viewEngine = context.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
                    viewName = viewEngine.FindView(context, string.IsNullOrEmpty(viewName) ? ctrlActDesc.ActionName : viewName, false)?.View?.Path;
                }
            }
            if (context.Result is ViewResult vr)
            {
                viewName = vr.ViewName ?? "";
                if (viewName?.StartsWith("/") == false)
                {
                    var viewEngine = context.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
                    viewName = viewEngine.FindView(context, string.IsNullOrEmpty(viewName) ? ctrlActDesc.ActionName : viewName, false)?.View?.Path;
                }
            }
            // PartialView：缺少模型时创建默认 BaseVM；再根据菜单（优先）或 ActionDescription 特性生成页面标题并写入响应头
            if (context.Result is PartialViewResult)
            {
                var model = (context.Result as PartialViewResult).ViewData?.Model as BaseVM;
                if (model == null && (context.Result as PartialViewResult).Model == null && (context.Result as PartialViewResult).ViewData != null)
                {
                    model = ctrl.Wtm.CreateVM<BaseVM>();
                    model.CurrentView = viewName;
                    (context.Result as PartialViewResult).ViewData.Model = model;
                }
                // 为所有 PartialView 加上最外层的 Div
                if (model != null)
                {
                    model.CurrentView = viewName;
                    string pagetitle = string.Empty;
                    var menu = Utils.FindMenu(context.HttpContext.Request.Path, ctrl.GlobaInfo.AllMenus);
                    if (menu == null)
                    {
                        var ctrlDes = ctrlActDesc.ControllerTypeInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
                        var actDes = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
                        if (actDes != null)
                        {
                            if (ctrlDes != null)
                            {
                                pagetitle = ctrlDes.GetDescription(ctrl) + " - ";
                            }
                            pagetitle += actDes.GetDescription(ctrl);
                        }
                    }
                    else
                    {
                        if (menu.ParentId != null)
                        {
                            var pmenu = ctrl.GlobaInfo.AllMenus.Where(x => x.ID == menu.ParentId).FirstOrDefault();
                            if (pmenu != null)
                            {
                                pmenu.PageName = Core.CoreProgram._localizer?[pmenu.PageName];

                                pagetitle = pmenu.PageName + " - ";
                            }
                        }
                        menu.PageName = Core.CoreProgram._localizer?[menu.PageName];
                        pagetitle += menu.PageName;
                    }
                    if (string.IsNullOrEmpty(pagetitle) == false)
                    {
                        context.HttpContext.Response.Headers.Append("X-wtm-PageTitle", Convert.ToBase64String(Encoding.UTF8.GetBytes(pagetitle)));
                    }
                    context.HttpContext.Response.Cookies.Append("divid", model.ViewDivId);
                }
            }
            // ViewResult：同样补充默认 BaseVM，并将容器 Div 的 id 写入 Cookie
            if (context.Result is ViewResult)
            {
                var model = (context.Result as ViewResult).ViewData?.Model as BaseVM;
                if (model == null && (context.Result as ViewResult).Model == null && (context.Result as ViewResult).ViewData != null)
                {
                    model = ctrl.Wtm.CreateVM<BaseVM>();
                    model.CurrentView = viewName;
                    (context.Result as ViewResult).ViewData.Model = model;
                }
                if (model != null)
                {
                    model.CurrentView = viewName;
                    context.HttpContext.Response.Cookies.Append("divid", model?.ViewDivId);
                }
            }
            base.OnActionExecuted(context);
        }

        /// <summary>
        /// 结果执行完成后触发：为 PartialView 输出图表尺寸调整脚本，根据 NoLogAttribute 决定是否记录操作日志
        /// （含模块名、操作人、耗时、异常信息等），并对未处理的异常做兜底输出。
        /// </summary>
        /// <param name="context">结果执行完成后的过滤器上下文</param>
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            var ctrl = context.Controller as IBaseController;
            if (ctrl == null)
            {
                base.OnResultExecuted(context);
                return;
            }
            var ctrlActDesc = context.ActionDescriptor as ControllerActionDescriptor;
            // 判断 Action 或 Controller 是否标记了 NoLogAttribute，标记则跳过操作日志记录
            var nolog = ctrlActDesc.MethodInfo.IsDefined(typeof(NoLogAttribute), false) || ctrlActDesc.ControllerTypeInfo.IsDefined(typeof(NoLogAttribute), false);

            BaseVM model = null;
            if (context.Result is ViewResult vr)
            {
                model = vr.Model as BaseVM;
            }
            // PartialView 渲染完成后输出脚本，尝试调用前台的 ResizeChart 按新容器尺寸调整图表
            if (context.Result is PartialViewResult pvr)
            {
                model = pvr.Model as BaseVM;
                context.HttpContext.Response.WriteAsync($"<script>try{{ff.ResizeChart('{model?.ViewDivId}')}}catch{{}}</script>");
            }

            //如果是来自Error，则已经记录过日志，跳过
            if (ctrlActDesc.ControllerName == "_Framework" && ctrlActDesc.ActionName == "Error")
            {
                return;
            }
            if (nolog == false)
            {
                var log = new ActionLog();
                var ctrlDes = ctrlActDesc.ControllerTypeInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
                var actDes = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
                var postDes = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>().FirstOrDefault();

                log.LogType = context.Exception == null ? ActionLogTypesEnum.Normal : ActionLogTypesEnum.Exception;
                log.ActionTime = DateTime.Now;
                log.ITCode = ctrl.Wtm?.LoginUserInfo?.ITCode ?? string.Empty;
                // 给日志的多语言属性赋值
                log.ModuleName = ctrlDes?.GetDescription(ctrl) ?? ctrlActDesc.ControllerName;
                log.ActionName = actDes?.GetDescription(ctrl) ?? ctrlActDesc.ActionName + (postDes == null ? string.Empty : "[P]");
                log.ActionUrl = context.HttpContext.Request.Path;
                log.IP = context.HttpContext.GetRemoteIpAddress();
                log.Remark = context.Exception?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(log.Remark) == false && log.Remark.Length > 2000)
                {
                    log.Remark = log.Remark.Substring(0, 2000);
                }
                var starttime = context.HttpContext.Items["actionstarttime"] as DateTime?;
                if (starttime != null)
                {
                    log.Duration = DateTime.Now.Subtract(starttime.Value).TotalSeconds;
                }
                try
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ActionLog>>();
                    if (logger != null)
                    {
                        logger.Log<ActionLog>(LogLevel.Information, new EventId(), log, null, (a, b) =>
                        {
                            return a.GetLogString();
                        });
                    }
                }
                catch { }
            }
            // 兜底处理未捕获的异常：快速调试模式输出完整异常信息，否则输出本地化的友好错误提示
            if (context.Exception != null)
            {
                context.ExceptionHandled = true;
                if (ctrl.Wtm.ConfigInfo.IsQuickDebug == true)
                {
                    context.HttpContext.Response.WriteAsync(context.Exception.ToString());
                }
                else
                {
                    context.HttpContext.Response.WriteAsync(MvcProgram._localizer["Sys.PageError"]);
                }
            }
            base.OnResultExecuted(context);
        }

    }
}
