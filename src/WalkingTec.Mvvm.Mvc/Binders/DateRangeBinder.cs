using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Binders
{
    /// <summary>
    /// DateRangeBinder
    /// DataRange model binding
    /// </summary>
    public class DateRangeBinder : IModelBinder
    {
        /// <summary>
        /// BindModelAsync
        /// </summary>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            var modelName = bindingContext.ModelName;
            if (string.IsNullOrEmpty(modelName))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                modelName += "[Value]";
                valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            }

            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            if(DateRange.TryParse(value,out var dateRange))
                bindingContext.Result = ModelBindingResult.Success(dateRange);
            return Task.CompletedTask;
        }
    }
}
