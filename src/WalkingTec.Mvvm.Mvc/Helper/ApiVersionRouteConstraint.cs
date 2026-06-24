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
        public bool Match(HttpContext httpContext, IRouter route, string routeKey,
            RouteValueDictionary values, RouteDirection routeDirection)
        {
            return true;
        }
    }
}
