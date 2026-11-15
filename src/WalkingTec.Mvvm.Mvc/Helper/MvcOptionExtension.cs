using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Json;
using WalkingTec.Mvvm.Mvc.Binders;
using WalkingTec.Mvvm.Mvc.Filters;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// WTM 框架 MVC 配置扩展类，提供对 MvcOptions、JsonOptions、ApiBehaviorOptions 的统一初始化扩展方法
    /// </summary>
    public static class MvcOptionExtension
    {
        /// <summary>
        /// 为 MvcOptions 应用 WTM 框架默认配置：注册自定义模型绑定器、全局过滤器、
        /// 本地化的模型绑定错误消息，并排除框架内部类型的模型绑定
        /// </summary>
        /// <param name="options">MVC 配置项</param>
        public static void UseWtmMvcOptions(this MvcOptions options)
        {
            // ModelBinderProviders
            options.ModelBinderProviders.Insert(0, new StringBinderProvider());

            // Filters
            options.Filters.Add(new DataContextFilter());
            options.Filters.Add(new PrivilegeFilter());
            options.Filters.Add(new FrameworkFilter());
            // 模型绑定错误消息本地化
            options.ModelBindingMessageProvider.SetValueIsInvalidAccessor((x) => Core.CoreProgram._localizer?["Sys.ValueIsInvalidAccessor", x]);
            options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => Core.CoreProgram._localizer?["Sys.AttemptedValueIsInvalidAccessor", x, y]);
            options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor((x) => Core.CoreProgram._localizer?["Sys.ValueIsInvalidAccessor", x]);
            // 排除框架内部类型的模型绑定，避免这些接口类型被 MVC 误当作可绑定模型
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(IModelStateService)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(IDataContext)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(WTMContext)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(Configs)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(GlobalData)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(IDistributedCache)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(LoginUserInfo)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(ISessionService)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(IModelStateService)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(IUIService)));
            options.ModelMetadataDetailsProviders.Add(new ExcludeBindingMetadataProvider(typeof(IStringLocalizer)));
            options.EnableEndpointRouting = true;

        }

        /// <summary>
        /// 为 JsonOptions 应用 WTM 框架默认的 System.Text.Json 序列化配置，
        /// 并初始化框架全局使用的两份默认 JSON 选项
        /// </summary>
        /// <param name="options">JSON 配置项</param>
        public static void UseWtmJsonOptions(this JsonOptions options)
        {
            // 属性名保持原样输出，序列化时忽略 null 值
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;
            options.JsonSerializerOptions.AllowTrailingCommas = true;
            // 注册可空值类型、可空枚举等转换器，兼容前端提交的空字符串等特殊值
            options.JsonSerializerOptions.Converters.Add(new NullableConverter<int>());
            options.JsonSerializerOptions.Converters.Add(new NullableConverter<long>());
            options.JsonSerializerOptions.Converters.Add(new NullableConverter<decimal>());
            options.JsonSerializerOptions.Converters.Add(new NullableConverter<short>());
            options.JsonSerializerOptions.Converters.Add(new NullableConverter<float>());
            options.JsonSerializerOptions.Converters.Add(new NullableConverter<double>());
            options.JsonSerializerOptions.Converters.Add(new NullableConverter<bool>());
            options.JsonSerializerOptions.Converters.Add(new NullableConverter<DateTime>());
            options.JsonSerializerOptions.Converters.Add(new NullableConverter<Guid>());
            options.JsonSerializerOptions.Converters.Add(new NullableEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new BoolStringConverter());
            options.JsonSerializerOptions.Converters.Add(new DateRangeConverter());
            options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            options.JsonSerializerOptions.Converters.Add(new StringIgnoreLTGTConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new PocoConverter());
            options.JsonSerializerOptions.Converters.Add(new TypeConverter());
            options.JsonSerializerOptions.Converters.Add(new DynamicDataConverter());
            // 将当前配置保存为框架全局默认 JSON 选项
            Core.CoreProgram.DefaultJsonOption = options.JsonSerializerOptions;
            // 构造另一份用于 POST 提交场景的 JSON 选项并保存，其差异在于序列化时不忽略 null 值
            JsonSerializerOptions jsonOptions2 = new JsonSerializerOptions();
            jsonOptions2.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;
            jsonOptions2.AllowTrailingCommas = true;
            jsonOptions2.Converters.Add(new NullableConverter<int>());
            jsonOptions2.Converters.Add(new NullableConverter<long>());
            jsonOptions2.Converters.Add(new NullableConverter<decimal>());
            jsonOptions2.Converters.Add(new NullableConverter<short>());
            jsonOptions2.Converters.Add(new NullableConverter<float>());
            jsonOptions2.Converters.Add(new NullableConverter<double>());
            jsonOptions2.Converters.Add(new NullableConverter<bool>());
            jsonOptions2.Converters.Add(new NullableConverter<DateTime>());
            jsonOptions2.Converters.Add(new NullableConverter<Guid>());
            jsonOptions2.Converters.Add(new NullableEnumConverter());
            jsonOptions2.Converters.Add(new DateTimeConverter());
            jsonOptions2.Converters.Add(new JsonStringEnumConverter());
            jsonOptions2.Converters.Add(new BoolStringConverter());
            jsonOptions2.Converters.Add(new DateRangeConverter());
            jsonOptions2.Converters.Add(new PocoConverter());
            jsonOptions2.Converters.Add(new TypeConverter());
            jsonOptions2.Converters.Add(new DynamicDataConverter());
            CoreProgram.DefaultPostJsonOption = jsonOptions2;
        }
        /// <summary>
        /// 为 ApiBehaviorOptions 应用 WTM 框架默认配置：关闭 MVC 自动模型验证 400 响应，
        /// 改为返回包含验证错误详情的 BadRequestObjectResult
        /// </summary>
        /// <param name="options">API 行为配置项</param>
        public static void UseWtmApiOptions(this ApiBehaviorOptions options)
        {
            options.SuppressModelStateInvalidFilter = true;
            options.InvalidModelStateResponseFactory = (a) =>
            {
                return new BadRequestObjectResult(a.ModelState.GetErrorJson());
            };
        }

    }
}
