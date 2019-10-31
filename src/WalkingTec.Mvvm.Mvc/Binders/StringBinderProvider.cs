using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using WalkingTec.Mvvm.Core;

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

            if (context.Metadata.ModelType == typeof(DateRange))
                return new BinderTypeModelBinder(typeof(DateRangeBinder));

            if (context.Metadata.ModelType == typeof(string))
            {
                return new BinderTypeModelBinder(typeof(StringIgnoreLTGTBinder));
            }

            return null;
        }
    }
}
