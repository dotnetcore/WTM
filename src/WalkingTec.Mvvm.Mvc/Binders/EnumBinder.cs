using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Threading.Tasks;

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
