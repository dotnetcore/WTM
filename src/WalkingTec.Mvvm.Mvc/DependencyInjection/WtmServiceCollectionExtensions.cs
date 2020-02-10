using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using WalkingTec.Mvvm.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WtmServiceCollectionExtensions
    {
        public static IServiceCollection AddWtm(this IServiceCollection services,
            Func<ActionExecutingContext, string> CsSector = null,
            List<IDataPrivilege> dataPrivilegeSettings = null,
            IHostingEnvironment hostingEnvironment = null
        )
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddWtmCore(CsSector, dataPrivilegeSettings, hostingEnvironment);

            return services;
        }
    }
}
