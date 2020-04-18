using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Filters
{
    public class DataContextFilter : ActionFilterAttribute
    {
        private Func<ActionExecutingContext, string> _csfunc;

        public DataContextFilter()
        {

        }

        public DataContextFilter(Func<ActionExecutingContext, string> CsSelector)
        {
            this._csfunc = CsSelector;
        }

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

            string cs = "";
            DBTypeEnum? dbtype = null;
            ControllerActionDescriptor ad = context.ActionDescriptor as ControllerActionDescriptor;
            var fixcontroller = ad.ControllerTypeInfo.GetCustomAttributes(typeof(FixConnectionAttribute), false).Cast<FixConnectionAttribute>().FirstOrDefault();
            var fixaction = ad.MethodInfo.GetCustomAttributes(typeof(FixConnectionAttribute), false).Cast<FixConnectionAttribute>().FirstOrDefault();
            var ispost = ad.MethodInfo.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>().FirstOrDefault();
            string mode = "Read";

            if (fixcontroller != null || fixaction != null)
            {
                cs = fixcontroller?.CsName ?? fixaction?.CsName;
                var op = fixcontroller?.Operation ?? fixaction?.Operation;
                if (op != null)
                {
                    switch (op.Value)
                    {
                        case DBOperationEnum.Read:
                            mode = "Read";
                            break;
                        case DBOperationEnum.Write:
                            mode = "Write";
                            break;
                        default:
                            break;
                    }
                }
                dbtype = fixcontroller?.DbType ?? fixaction?.DbType;
                cs = Utils.GetCS(cs, mode, controller.WtmContext.ConfigInfo);
            }
            else
            {
                if (_csfunc != null)
                {
                    cs = _csfunc.Invoke(context);
                }
                else
                {
                    if(ispost != null)
                    {
                        mode = "Write";
                    }
                    cs = context.HttpContext.Request.Query["DONOTUSECSName"];
                    cs = Utils.GetCS(cs, mode, controller.WtmContext.ConfigInfo);
                }
            }

            controller.WtmContext.CurrentCS = cs;
            controller.WtmContext.CurrentDbType = dbtype;
            base.OnActionExecuting(context);
        }
    }
}
