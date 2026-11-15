using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WalkingTec.Mvvm.Mvc.Helper
{
    /// <summary>
    /// Minimal apiVersion route constraint for Elsa workflow API endpoints.
    /// Simply matches any value so Elsa's versioned routes work without full API versioning.
    /// </summary>
    public class ApiVersionRouteConstraint : IRouteConstraint
    {
        /// <summary>
        /// 匹配路由约束，直接放行任意版本号
        /// </summary>
        /// <param name="httpContext">当前 HTTP 上下文</param>
        /// <param name="route">当前路由对象</param>
        /// <param name="routeKey">被约束的路由参数键名</param>
        /// <param name="values">路由值字典</param>
        /// <param name="routeDirection">路由匹配方向（处理传入请求或生成 URL）</param>
        /// <returns>始终返回 true，即接受任意版本值</returns>
        public bool Match(HttpContext httpContext, IRouter route, string routeKey,
            RouteValueDictionary values, RouteDirection routeDirection)
        {
            // 不做实际校验，接受任意版本值以兼容带版本号的 Elsa 路由
            return true;
        }
    }
}
