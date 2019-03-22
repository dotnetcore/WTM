using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    public static class ModelStateExtension
    {
        public static ErrorObj GetErrorJson(this ModelStateDictionary self)
        {
            var mse = new ErrorObj();
            mse.Entity = new Dictionary<string, string>();
            mse.Message = new List<string>();
            foreach (var item in self)
            {
                if (item.Value.ValidationState == ModelValidationState.Invalid)
                {
                    var name = item.Key;
                    if (name.ToLower().StartsWith("entity."))
                    {
                        name = name.Substring(7);
                        mse.Entity.Add(name, item.Value.Errors.FirstOrDefault()?.ErrorMessage);
                    }
                    else
                    {
                        mse.Message.Add(item.Value.Errors.FirstOrDefault()?.ErrorMessage);
                    }
                }
            }
            return mse;
        }

    }
}
