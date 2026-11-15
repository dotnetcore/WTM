using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WalkingTec.Mvvm.Mvc.Binders
{
    /// <summary>
    /// EnumBinder
    /// 忽略enum转换空字符串时的错误
    /// </summary>
    public class EnumBinder : IModelBinder
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

            var valueProviderResult =
                bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;
            // 值为空字符串时直接返回且不设置绑定结果，避免枚举解析空字符串时产生模型绑定错误
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Success(Enum.Parse(bindingContext.ModelType,value));
            }
            return Task.CompletedTask;
        }
    }
}
