using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using WalkingTec.Mvvm.Core;
using System.Collections.Generic;
using System.Reflection;
using WalkingTec.Mvvm.Core.Implement;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using WalkingTec.Mvvm.Core.Support.Json;
using System.Text.Json;
using WalkingTec.Mvvm.Core.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using WalkingTec.Mvvm.Core.Extensions;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Mvc.Filters
{
    public class FrameworkFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync (ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var ctrl = context.Controller as IBaseController;
            if (ctrl == null)
            {
                await next ();
                _OnActionExecuted (context);
                return;
            }
            context.SetWtmContext();
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

                    if (ctrl is BaseApiController apictrl)
                    {
                        //apictrl.TryValidateModel(model);
                        if (context.HttpContext.Items.ContainsKey("DONOTUSE_REQUESTBODY"))
                        {
                            string body = context.HttpContext.Items["DONOTUSE_REQUESTBODY"].ToString();
                            var joption = new JsonSerializerOptions();
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
                        await template.DoReInit();
                        var errorlist = tvm.ErrorListVM;
                        errorlist.CopyContext(model);
                    }
                    if (model is IBasePagedListVM<TopBasePoco, ISearcher> lvm)
                    {
                        var searcher = lvm.Searcher;
                        searcher.CopyContext(lvm);
                        if (ctrl is BaseController)
                        {
                            await searcher.DoInit();
                        }
                    }
                    await model.Validate();
                    //如果是子表外键验证错误，例如Entity.Majors[0].SchoolId为空这种错误，则忽略。因为框架会在添加修改的时候自动给外键赋值
                    var toremove = ctrl.ModelState.Select(x => x.Key).Where(x => Regex.IsMatch(x, ".*?\\[.*?\\]\\..*?id", RegexOptions.IgnoreCase));
                    foreach (var r in toremove)
                    {
                        ctrl.ModelState.Remove(r);
                    }
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
                    if (ctrl is BaseController)
                    {
                        var reinit = model.GetType().GetTypeInfo().GetCustomAttributes(typeof(ReInitAttribute), false).Cast<ReInitAttribute>().SingleOrDefault();
                        if (ctrl.ModelState.IsValid)
                        {
                            if (reinit != null && (reinit.ReInitMode == ReInitModes.SUCCESSONLY || reinit.ReInitMode == ReInitModes.ALWAYS))
                            {
                                await model.DoReInit();
                            }
                        }
                        else
                        {
                            if (reinit == null || (reinit.ReInitMode == ReInitModes.FAILEDONLY || reinit.ReInitMode == ReInitModes.ALWAYS))
                            {
                                await model.DoReInit();
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

            await next ();
            _OnActionExecuted (context);
        }

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

        public void _OnActionExecuted(ActionExecutingContext context)
        {
            var ctrl = context.Controller as BaseController;
            if (ctrl == null)
            {
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
                        context.HttpContext.Response.Headers.Add("X-wtm-PageTitle", Convert.ToBase64String(Encoding.UTF8.GetBytes(pagetitle)));
                    }
                    context.HttpContext.Response.Cookies.Append("divid", model.ViewDivId);
                }
            }
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
        }

        public override async Task OnResultExecutionAsync (ResultExecutingContext context, ResultExecutionDelegate next)
        {
            Exception _ex = null;
            try {
                await next ();
            } catch (Exception ex) {
                _ex = ex;
            }
            var ctrl = context.Controller as IBaseController;
            if (ctrl == null)
            {
                return;
            }
            var ctrlActDesc = context.ActionDescriptor as ControllerActionDescriptor;
            var nolog = ctrlActDesc.MethodInfo.IsDefined(typeof(NoLogAttribute), false) || ctrlActDesc.ControllerTypeInfo.IsDefined(typeof(NoLogAttribute), false);

            BaseVM model = null;
            if (context.Result is ViewResult vr)
            {
                model = vr.Model as BaseVM;
            }
            if (context.Result is PartialViewResult pvr)
            {
                model = pvr.Model as BaseVM;
                await context.HttpContext.Response.WriteAsync($"<script>try{{ff.ResizeChart('{model?.ViewDivId}')}}catch{{}}</script>");
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

                log.LogType = _ex == null ? ActionLogTypesEnum.Normal : ActionLogTypesEnum.Exception;
                log.ActionTime = DateTime.Now;
                log.ITCode = (await ctrl.Wtm?.GetLoginUserInfo ())?.ITCode ?? string.Empty;
                // 给日志的多语言属性赋值
                log.ModuleName = ctrlDes?.GetDescription(ctrl) ?? ctrlActDesc.ControllerName;
                log.ActionName = actDes?.GetDescription(ctrl) ?? ctrlActDesc.ActionName + (postDes == null ? string.Empty : "[P]");
                log.ActionUrl = context.HttpContext.Request.Path;
                log.IP = context.HttpContext.GetRemoteIpAddress();
                log.Remark = _ex?.ToString() ?? string.Empty;
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
            if (_ex != null)
            {
                if (ctrl.Wtm.ConfigInfo.IsQuickDebug == true)
                {
                    await context.HttpContext.Response.WriteAsync(_ex.ToString());
                }
                else
                {
                    await context.HttpContext.Response.WriteAsync(MvcProgram._localizer["Sys.PageError"]);
                }
            }
        }

    }
}
