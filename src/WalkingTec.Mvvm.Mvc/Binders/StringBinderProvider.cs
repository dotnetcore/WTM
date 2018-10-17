using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;

namespace WalkingTec.Mvvm.Mvc.Binders
{
    /// <summary>
    /// CustomBinderProvider
    /// </summary>
    public class StringBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// GetBinder
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(string))
            {
                return new BinderTypeModelBinder(typeof(StringIgnoreLTGTBinder));
            }

            return null;
        }
    }
}
