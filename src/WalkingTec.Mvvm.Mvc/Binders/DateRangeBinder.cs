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

            // 先按模型名直接取值；若取不到，则回退尝试 模型名[Value] 形式的键（对应前台提交 DateRange 的 Value 属性）
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

            // 尝试将提交的字符串解析为 DateRange 对象，解析成功则以它作为绑定结果，失败则不设置结果
            if(DateRange.TryParse(value,out var dateRange))
                bindingContext.Result = ModelBindingResult.Success(dateRange);
            return Task.CompletedTask;
        }
    }
}
