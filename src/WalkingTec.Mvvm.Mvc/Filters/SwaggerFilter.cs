using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Filters
{
    public class SwaggerFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;
            if(type == typeof(List<ComboSelectListItem>))
            {
                schema = null;
            }
            //if (type.IsEnum)
            //{
            //    schema.Extensions.Add(
            //        "x-ms-enum",
            //        new OpenApiObject
            //        {
            //            ["name"] = new OpenApiString(type.Name),
            //            ["modelAsString"] = new OpenApiBoolean(true)
            //        }
            //    );
            //};
        }
    }
}
