using System.Buffers;
using System.Linq;
using Elsa.Server.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace WalkingTec.Mvvm.Mvc.Binders
{
    /// <summary>
    /// Newtonsoft.Json 格式化特性，同时实现 Action 过滤器、控制器模型约定和 Action 模型约定。
    /// 作用：通过模型约定将请求体的模型绑定器替换为 NewtonsoftJsonBodyModelBinder，
    /// 并在 Action 执行后将响应输出的格式化器由 System.Text.Json 切换为 Newtonsoft.Json，
    /// 从而保证请求与响应统一使用 Newtonsoft 的序列化行为。
    /// </summary>
    public class NewtonsoftJsonFormatterAttribute : ActionFilterAttribute, IControllerModelConvention, IActionModelConvention
    {
        /// <summary>
        /// 控制器模型约定的应用入口，对本控制器下的所有 Action 逐个应用本约定。
        /// </summary>
        /// <param name="controller">当前控制器的模型描述对象</param>
        public void Apply(ControllerModel controller)
        {
            foreach (var action in controller.Actions)
            {
                Apply(action);
            }
        }

        /// <summary>
        /// Action 模型约定的应用入口，将所有从请求体绑定的参数的模型绑定器替换为 NewtonsoftJsonBodyModelBinder。
        /// </summary>
        /// <param name="action">当前 Action 的模型描述对象</param>
        public void Apply(ActionModel action)
        {
            // Set the model binder to NewtonsoftJsonBodyModelBinder for parameters that are bound to the request body.
            var parameters = action.Parameters.Where(p => p.BindingInfo?.BindingSource == BindingSource.Body);
            foreach (var p in parameters)
            {
                p.BindingInfo.BinderType = typeof(NewtonsoftJsonBodyModelBinder);
            }
        }

        /// <summary>
        /// Action 执行完毕后触发，将返回结果（ObjectResult 或 JsonResult）的输出格式化器替换为 Newtonsoft.Json。
        /// </summary>
        /// <param name="context">Action 执行后的过滤器上下文</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var jsonOptions = context.HttpContext.RequestServices.GetService<IOptions<MvcNewtonsoftJsonOptions>>();
            var jsonSettings = jsonOptions.Value.SerializerSettings;
            // 若控制器来自 Elsa.Server.Api，则改用 Elsa 提供的专用序列化配置（工作流定义与普通端点使用不同配置）
            if (context.Controller.GetType().FullName.StartsWith("Elsa.Server.Api"))
            {
                if (context.Controller.GetType().FullName.StartsWith("Elsa.Server.Api.Endpoints.WorkflowDefinitions"))
                {
                    jsonSettings = SerializationHelper.GetSettingsForWorkflowDefinition();
                }
                else
                {
                    jsonSettings = SerializationHelper.GetSettingsForEndpoint();
                }
            }
            // 移除默认的 System.Text.Json 输出格式化器，替换为 Newtonsoft.Json 输出格式化器
            if (context.Result is ObjectResult objectResult)
            {

                objectResult.Formatters.RemoveType<SystemTextJsonOutputFormatter>();
                objectResult.Formatters.Add(new NewtonsoftJsonOutputFormatter(
                    jsonSettings,
                    context.HttpContext.RequestServices.GetRequiredService<ArrayPool<char>>(),
                    context.HttpContext.RequestServices.GetRequiredService<IOptions<MvcOptions>>().Value,
                    context.HttpContext.RequestServices.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>().Value));
            }
            // JsonResult 无法直接替换格式化器，因此将其值包装为 ObjectResult 后再替换，并改写返回结果
            else if(context.Result is JsonResult jr)
            {
                ObjectResult obj = new ObjectResult(jr.Value);

                obj.Formatters.RemoveType<SystemTextJsonOutputFormatter>();
                obj.Formatters.Add(new NewtonsoftJsonOutputFormatter(
                    jsonSettings,
                    context.HttpContext.RequestServices.GetRequiredService<ArrayPool<char>>(),
                    context.HttpContext.RequestServices.GetRequiredService<IOptions<MvcOptions>>().Value,
                    context.HttpContext.RequestServices.GetRequiredService<IOptions<MvcNewtonsoftJsonOptions>>().Value));
                context.Result = obj;
            }
            else
            {
                base.OnActionExecuted(context);
            }
        }
    }

    /// <summary>
    /// 使用 Newtonsoft.Json 输入格式化器解析请求体的模型绑定器。
    /// 构造时自行创建 NewtonsoftJsonInputFormatter，不依赖全局注册的输入格式化器列表。
    /// </summary>
    public class NewtonsoftJsonBodyModelBinder : BodyModelBinder
    {
        /// <summary>
        /// 初始化 NewtonsoftJsonBodyModelBinder，内部生成仅包含 Newtonsoft.Json 输入格式化器的数组并传给基类。
        /// </summary>
        /// <param name="loggerFactory">日志工厂，用于创建输入格式化器所需的日志记录器</param>
        /// <param name="charPool">字符缓冲池，供 JSON 反序列化时复用缓冲区</param>
        /// <param name="readerFactory">请求体读取流的工厂</param>
        /// <param name="objectPoolProvider">对象池提供者，供 JSON 反序列化复用对象</param>
        /// <param name="mvcOptions">MVC 全局配置选项</param>
        /// <param name="jsonOptions">Newtonsoft.Json 的 MVC 配置选项</param>
        public NewtonsoftJsonBodyModelBinder(
            ILoggerFactory loggerFactory,
            ArrayPool<char> charPool,
            IHttpRequestStreamReaderFactory readerFactory,
            ObjectPoolProvider objectPoolProvider,
            IOptions<MvcOptions> mvcOptions,
            IOptions<MvcNewtonsoftJsonOptions> jsonOptions)
            : base(GetInputFormatters(loggerFactory, charPool, objectPoolProvider, mvcOptions, jsonOptions), readerFactory)
        {
        }

        // 构造仅包含 NewtonsoftJsonInputFormatter 的输入格式化器数组
        private static IInputFormatter[] GetInputFormatters(
            ILoggerFactory loggerFactory,
            ArrayPool<char> charPool,
            ObjectPoolProvider objectPoolProvider,
            IOptions<MvcOptions> mvcOptions,
            IOptions<MvcNewtonsoftJsonOptions> jsonOptions)
        {
            var jsonOptionsValue = jsonOptions.Value;
            return new IInputFormatter[]
            {
            new NewtonsoftJsonInputFormatter(
                loggerFactory.CreateLogger<NewtonsoftJsonBodyModelBinder>(),
                jsonOptionsValue.SerializerSettings,
                charPool,
                objectPoolProvider,
                mvcOptions.Value,
                jsonOptionsValue)
            };
        }
    }
}
