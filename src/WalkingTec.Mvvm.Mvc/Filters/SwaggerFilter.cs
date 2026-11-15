using System.Collections.Generic;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Filters
{
    /// <summary>
    /// Swagger 架构过滤器，用于调整 WTM 特殊类型在 Swagger 文档中的架构生成方式。
    /// </summary>
    public class SwaggerFilter : ISchemaFilter
    {
        /// <summary>
        /// 应用架构过滤规则：下拉选项列表（ComboSelectListItem 的列表类型）不需要生成架构定义。
        /// </summary>
        /// <param name="schema">当前类型对应的 Swagger 架构对象</param>
        /// <param name="context">架构过滤器上下文，可从中获取当前正在处理的类型</param>
        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;
            // 下拉选项列表类型无需在 Swagger 中展开架构定义
            if(type == typeof(List<ComboSelectListItem>))
            {
                schema = null;
            }
        }
    }
}
