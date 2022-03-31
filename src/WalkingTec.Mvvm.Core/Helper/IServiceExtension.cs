using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.FileHandlers;
using WalkingTec.Mvvm.Core.Support.Quartz;

namespace WalkingTec.Mvvm.Core
{
    public static class IServiceExtension
    {
        public static IServiceCollection AddWtmContextForConsole(this IServiceCollection services, string jsonFileDir = null, string jsonFileName = null, Func<IWtmFileHandler, string> fileSubDirSelector = null)
        {
            var configBuilder = new ConfigurationBuilder();
            IConfigurationRoot ConfigRoot = configBuilder.WTMConfig(null,jsonFileDir,jsonFileName).Build();
            var WtmConfigs = ConfigRoot.Get<Configs>();
            services.Configure<Configs>(ConfigRoot);
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConfiguration(ConfigRoot.GetSection("Logging"))
                       .AddConsole()
                       .AddDebug()
                       .AddWTMLogger();
            });
            var gd = GetGlobalData();
            services.AddHttpContextAccessor();
            services.AddSingleton(gd);
            WtmFileProvider._subDirFunc = fileSubDirSelector;
            services.TryAddScoped<IDataContext, NullContext>();
            services.AddScoped<WTMContext>();
            services.AddScoped<WtmFileProvider>();
            services.AddHttpClient();
            if (WtmConfigs.Domains != null)
            {
                foreach (var item in WtmConfigs.Domains)
                {
                    services.AddHttpClient(item.Key, x =>
                    {
                        x.BaseAddress = new Uri(item.Value.Url);
                        x.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                        x.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
                    });
                }
            }
            services.AddDistributedMemoryCache();
            var cs = WtmConfigs.Connections;
            foreach (var item in cs)
            {
                var dc = item.CreateDC();
                dc.Database.EnsureCreated();
            }
            WtmFileProvider.Init(WtmConfigs, gd);
            services.TryAddSingleton<QuartzHostService>();
            return services;
        }

        private static GlobalData GetGlobalData()
        {
            GlobalData gd = new GlobalData();
            gd.AllAssembly = Utils.GetAllAssembly();
            return gd;
        }
    }
}
