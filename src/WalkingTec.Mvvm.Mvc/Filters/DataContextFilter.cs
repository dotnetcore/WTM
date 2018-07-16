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
            string cs = "";
            if (_csfunc != null)
            {
                cs = _csfunc.Invoke(context);
            }
            else
            {
                ControllerActionDescriptor ad = context.ActionDescriptor as ControllerActionDescriptor;
                var fixcontroller = ad.ControllerTypeInfo.GetCustomAttributes(typeof(FixConnectionAttribute), false).Cast<FixConnectionAttribute>().FirstOrDefault();
                var fixaction = ad.MethodInfo.GetCustomAttributes(typeof(FixConnectionAttribute), false).Cast<FixConnectionAttribute>().FirstOrDefault();

                string mode = "Read";
                if (context.HttpContext.Request.Method.ToLower() == "post")
                {
                    mode = "Write";
                }

                if (fixcontroller != null || fixaction != null)
                {
                    cs = fixcontroller?.CsName ?? fixaction?.CsName;
                    var op =  fixcontroller?.Operation ?? fixaction?.Operation;
                    if(op != null)
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
                }
                else
                {
                    cs = context.HttpContext.Request.Query["DONOTUSECSName"];
                }
                cs = Utils.GetCS(cs, mode, controller.ConfigInfo);
            }
            controller.CurrentCS = cs;
            base.OnActionExecuting(context);
        }
    }
}
