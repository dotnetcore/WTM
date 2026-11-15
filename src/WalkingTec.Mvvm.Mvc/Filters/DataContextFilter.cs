using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Filters
{
    /// <summary>
    /// 数据上下文连接过滤器，在 Action 执行前确定本次请求使用的数据库连接串和数据库类型。
    /// 优先级：Action 或 Controller 上的 FixConnectionAttribute 特性，其次自定义回调 _csfunc，
    /// 最后按请求方式推断（POST 使用写连接，其他使用读连接），并支持请求参数 DONOTUSECSName 显式指定连接串。
    /// 解析结果写入 WtmContext 的 CurrentCS 和 CurrentDbType，供后续创建 DataContext 时使用。
    /// </summary>
    public class DataContextFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 自定义连接串解析回调；返回空表示未指定，将继续走默认的推断逻辑。
        /// 参数为当前 Action 执行上下文，返回值为连接串名称。
        /// </summary>
        public static Func<ActionExecutingContext, string> _csfunc;

        /// <summary>
        /// 初始化 DataContextFilter 的新实例。
        /// </summary>
        public DataContextFilter()
        {

        }

        /// <summary>
        /// Action 执行前触发，解析当前请求应使用的连接串与数据库类型并写入 WtmContext（CurrentCS/CurrentDbType）。
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
            string cs = "";
            DBTypeEnum? dbtype = null;
            // 读取 Controller 和 Action 上的 FixConnectionAttribute，以及 Action 是否为 POST 请求
            ControllerActionDescriptor ad = context.ActionDescriptor as ControllerActionDescriptor;
            var fixcontroller = ad.ControllerTypeInfo.GetCustomAttributes(typeof(FixConnectionAttribute), false).Cast<FixConnectionAttribute>().FirstOrDefault();
            var fixaction = ad.MethodInfo.GetCustomAttributes(typeof(FixConnectionAttribute), false).Cast<FixConnectionAttribute>().FirstOrDefault();
            var ispost = ad.MethodInfo.GetCustomAttributes(typeof(HttpPostAttribute), false).Cast<HttpPostAttribute>().FirstOrDefault();
            string mode = "Read";

            // 指定了 FixConnectionAttribute 时，按其配置确定读写模式和数据库类型，并解析出实际连接串（Action 上的配置优先于 Controller）
            if (fixcontroller != null || fixaction != null)
            {
                cs = fixaction?.CsName ?? fixcontroller?.CsName;
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
                cs = Utils.GetCS(cs, mode, controller.Wtm.ConfigInfo);
            }
            else
            {
                // 未指定特性时：先尝试自定义回调；回调未返回时，POST 请求默认使用写连接，并支持请求参数 DONOTUSECSName 显式指定连接串
                cs = _csfunc?.Invoke(context);
                if (string.IsNullOrEmpty(cs))
                {
                    if(ispost != null)
                    {
                        mode = "Write";
                    }
                    cs = context.HttpContext.Request.Query["DONOTUSECSName"];
                    cs = Utils.GetCS(cs, mode, controller.Wtm.ConfigInfo);
                }
            }

            // 将解析出的连接串和数据库类型写入当前请求的 WtmContext
            controller.Wtm.CurrentCS = cs;
            controller.Wtm.CurrentDbType = dbtype;
            base.OnActionExecuting(context);
        }
    }
}
