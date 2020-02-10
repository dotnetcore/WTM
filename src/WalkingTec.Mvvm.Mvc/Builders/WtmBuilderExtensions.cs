
using System;

using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Builder
{
    public static class WtmBuilderExtensions
    {
        public static IApplicationBuilder UseWtm(this IApplicationBuilder app, Action<IRouteBuilder> customRoutes = null)
        {
            app.UseWtmCore(customRoutes);
            return app;
        }
    }
}
