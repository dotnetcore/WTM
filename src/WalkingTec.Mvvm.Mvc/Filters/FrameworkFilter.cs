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
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

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
            var validpostonly = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(ValidateFormItemOnlyAttribute), false).Cast<ValidateFormItemOnlyAttribute>().FirstOrDefault();
            var crossDomain = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(CrossDomainAttribute), false).FirstOrDefault() as CrossDomainAttribute;
            if (crossDomain == null) {
                crossDomain = ctrlActDesc.ControllerTypeInfo.GetCustomAttributes(typeof(CrossDomainAttribute), false).FirstOrDefault() as CrossDomainAttribute;
            }
            if (crossDomain != null)
            {
                if (crossDomain.AllowOrigin == "*")
                {
                    context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", context.HttpContext.Request.Headers["Origin"]);
                }
                else
                {
                    context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", crossDomain.AllowOrigin);
                }
                context.HttpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                context.HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS, DELETE");
                context.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, No-Cache, X-Requested-With, If-Modified-Since, Pragma, Last-Modified, Cache-Control, Expires, Content-Type, X-E4M-With,userId,token");

            }
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
                    model.ControllerName = ctrl.GetType().FullName;
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

                    if (ctrl is BaseApiController apictrl)
                    {
                        apictrl.TryValidateModel(model);
                        apictrl.HttpContext.Request.Body.Position = 0;
                        StreamReader tr = new StreamReader(apictrl.HttpContext.Request.Body);
                        string body = tr.ReadToEnd();
                        var obj = JsonConvert.DeserializeObject(body) as JObject;
                        var fields = GetJsonFields(obj);
                        foreach (var field in fields)
                        {
                            model.FC.Add(field, null);
                        }

                    }



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
                                //if (temp.ErrorMessage.Count > 0)
                                //{
                                self.AddErrorColumn();
                                //}
                            };
                            if (temp.ListVM.Searcher != null)
                            {
                                var searcher = temp.ListVM.Searcher;
                                searcher.CopyContext(model);
                                //searcher.DoReInit();
                            }
                            temp.ListVM.DoInitListVM();
                        }
                        temp.LinkedVM?.DoInit();
                    }
                    if (model is IBaseImport<BaseTemplateVM>)
                    {
                        var template = (model as IBaseImport<BaseTemplateVM>).Template;
                        template.CopyContext(model);
                        template.DoReInit();
                    }
                    model.Validate();
                    //SetReinit
                    var invalid = ctrl.ModelState.Where(x => x.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid).Select(x => x.Key).ToList();
                    if ((ctrl as ControllerBase).Request.Method.ToLower() == "put" || validpostonly != null)
                    {
                        foreach (var v in invalid)
                        {
                            if (model.FC.ContainsKey(v) == false)
                            {
                                ctrl.ModelState.Remove(v);
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

                    //如果是子表外键验证错误，例如Entity.Majors[0].SchoolId为空这种错误，则忽略。因为框架会在添加修改的时候自动给外键赋值
                    var toremove = ctrl.ModelState.Select(x => x.Key).Where(x => Regex.IsMatch(x, ".*?\\[.*?\\]\\..*?id", RegexOptions.IgnoreCase));
                    foreach (var r in toremove)
                    {
                        ctrl.ModelState.Remove(r);
                    }
                }
            }

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var ctrl = context.Controller as BaseController;
            if (ctrl == null)
            {
                base.OnActionExecuted(context);
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
                    string pagetitle = "";
                    var menu = Utils.FindMenu(context.HttpContext.Request.Path);
                    if (menu == null)
                    {
                        var ctrlDes = ctrlActDesc.ControllerTypeInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
                        var actDes = ctrlActDesc.MethodInfo.GetCustomAttributes(typeof(ActionDescriptionAttribute), false).Cast<ActionDescriptionAttribute>().FirstOrDefault();
                        if (actDes != null)
                        {
                            if (ctrlDes != null)
                            {
                                pagetitle = ctrlDes.Description + " - ";
                            }
                            pagetitle += actDes.Description;
                        }
                    }
                    else
                    {
                        if (menu.ParentId != null)
                        {
                            var pmenu = ctrl.GlobaInfo.AllMenus.Where(x => x.ID == menu.ParentId).FirstOrDefault();
                            if (pmenu != null)
                            {
                                pagetitle = pmenu.PageName + " - ";
                            }
                        }
                        pagetitle += menu.PageName;
                    }
                    context.HttpContext.Response.Cookies.Append("pagetitle", pagetitle);
                    context.HttpContext.Response.Cookies.Append("divid", model.ViewDivId);

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
            base.OnActionExecuted(context);
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
                    if(string.IsNullOrEmpty(log.Remark) == false && log.Remark.Length > 1000)
                    {
                        log.Remark = log.Remark.Substring(0, 1000);
                    }
                    var starttime = context.HttpContext.Items["actionstarttime"] as DateTime?;
                    if (starttime != null)
                    {
                        log.Duration = DateTime.Now.Subtract(starttime.Value).TotalSeconds;
                    }
                    try
                    {
                        using (var dc = ctrl.CreateDC(true))
                        {
                            dc.Set<ActionLog>().Add(log);
                            dc.SaveChanges();
                        }
                    }
                    catch { }
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
            base.OnResultExecuted(context);
        }

        private IEnumerable<string> GetJsonFields(JObject j)
        {
            var children = j.Children();
            foreach (var item in children)
            {
                var rv = GetTokenFields(item);
                foreach (var i in rv)
                {
                    yield return i;
                }
            }
            yield break;
        }

        private IEnumerable<string> GetTokenFields(JToken j)
        {
            if (j.Type == JTokenType.Property)
            {
                yield return j.Path;
            }
            var children = j.Children();
            foreach (var item in children)
            {
                var rv = GetTokenFields(item);
                foreach (var i in rv)
                {
                    yield return i;
                }
            }
            yield break;
        }
    }
}
