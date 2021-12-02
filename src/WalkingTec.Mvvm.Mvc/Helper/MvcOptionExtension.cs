using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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
    public static class MvcOptionExtension
    {
        public static void UseWtmMvcOptions(this MvcOptions options)
        {
            // ModelBinderProviders
            options.ModelBinderProviders.Insert(0, new StringBinderProvider());

            // Filters
            options.Filters.Add(new DataContextFilter());
            options.Filters.Add(new PrivilegeFilter());
            options.Filters.Add(new FrameworkFilter());
            options.ModelBindingMessageProvider.SetValueIsInvalidAccessor((x) => Core.CoreProgram._localizer?["Sys.ValueIsInvalidAccessor", x]);
            options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => Core.CoreProgram._localizer?["Sys.AttemptedValueIsInvalidAccessor", x, y]);
            options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor((x) => Core.CoreProgram._localizer?["Sys.ValueIsInvalidAccessor", x]);
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

        public static void UseWtmJsonOptions(this JsonOptions options)
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;
            options.JsonSerializerOptions.AllowTrailingCommas = true;
            options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            options.JsonSerializerOptions.Converters.Add(new StringIgnoreLTGTConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new BoolStringConverter());
            options.JsonSerializerOptions.Converters.Add(new DateRangeConverter());
            options.JsonSerializerOptions.Converters.Add(new PocoConverter());
            options.JsonSerializerOptions.Converters.Add(new TypeConverter());
            options.JsonSerializerOptions.Converters.Add(new DynamicDataConverter());
            Core.CoreProgram.DefaultJsonOption = options.JsonSerializerOptions;
            JsonSerializerOptions jsonOptions2 = new JsonSerializerOptions();
            jsonOptions2.PropertyNamingPolicy = null;
            jsonOptions2.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;
            jsonOptions2.AllowTrailingCommas = true;
            jsonOptions2.Converters.Add(new DateTimeConverter());
            jsonOptions2.Converters.Add(new JsonStringEnumConverter());
            jsonOptions2.Converters.Add(new BoolStringConverter());
            jsonOptions2.Converters.Add(new DateRangeConverter());
            jsonOptions2.Converters.Add(new PocoConverter());
            jsonOptions2.Converters.Add(new TypeConverter());
            jsonOptions2.Converters.Add(new DynamicDataConverter());
            CoreProgram.DefaultPostJsonOption = jsonOptions2;
        }
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
