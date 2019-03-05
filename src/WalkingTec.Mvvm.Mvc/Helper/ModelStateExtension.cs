using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WalkingTec.Mvvm.Mvc
{
    public static class ModelStateExtension
    {
        public static string GetErrorJson(this ModelStateDictionary self)
        {
            string rv = "{Entity:{";
            for(int i=0;i<self.ErrorCount;i++)
            {
                var item = self.ElementAt(i);
                var name = item.Key;
                if (name.ToLower().StartsWith("entity."))
                {
                    name = name.Substring(7);
                }
                rv += $@"""{name}"":""{item.Value.Errors.FirstOrDefault()?.ErrorMessage}""";
                if(i <self.ErrorCount - 1)
                {
                    rv += ",";
                }
            }
            rv += "}}";
            return rv;
        }
    }
}
