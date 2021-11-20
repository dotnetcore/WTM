using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
namespace WalkingTec.Mvvm.Mvc.Helper
{ 
    public class HiddenApiFilter : IDocumentFilter
    {
        /// <summary>
        /// 隐藏swagger接口特性标识
        /// </summary>
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
        public class HideApiAttribute : System.Attribute
        {
        }


        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (ApiDescription description in context.ApiDescriptions)
            {
                if (description.TryGetMethodInfo(out MethodInfo method))
                {
                    if (method.ReflectedType.CustomAttributes.Any(t => t.AttributeType == typeof(HideApiAttribute))
                            || method.CustomAttributes.Any(t => t.AttributeType == typeof(HideApiAttribute)))
                    {
                        string key = "/" + description.RelativePath;
                        if (key.Contains("?"))
                        {
                            int idx = key.IndexOf("?", System.StringComparison.Ordinal);
                            key = key.Substring(0, idx);
                        }
                        swaggerDoc.Paths.Remove(key);
                    }
                }
            }
        }
    }
}
