using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.FileHandlers;
using WalkingTec.Mvvm.Core.Support.Quartz;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 依赖注入服务注册的扩展类，提供控制台程序使用WTM框架的初始化方法
    /// </summary>
    public static class IServiceExtension
    {
        /// <summary>
        /// 为控制台程序注册WTM框架所需的服务，包括配置、日志、数据库上下文、文件处理和定时任务等
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="jsonFileDir">配置文件所在目录，为null时使用默认目录</param>
        /// <param name="jsonFileName">配置文件名，为null时使用默认文件名</param>
        /// <param name="fileSubDirSelector">附件存储子目录的选择函数</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddWtmContextForConsole(this IServiceCollection services, string jsonFileDir = null, string jsonFileName = null, Func<IWtmFileHandler, string> fileSubDirSelector = null)
        {
            //读取json配置文件并绑定到Configs配置类
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
            //获取全局数据（扫描所有程序集）并注册为单例
            var gd = GetGlobalData();
            services.AddHttpContextAccessor();
            services.AddSingleton(gd);
            WtmFileProvider._subDirFunc = fileSubDirSelector;
            services.TryAddScoped<IDataContext, NullContext>();
            services.AddScoped<WTMContext>();
            services.AddScoped<WtmFileProvider>();
            services.AddHttpClient();
            //为配置的每个外部域名注册独立的HttpClient，设定基础地址和默认请求头
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
            //遍历所有连接串，创建数据库并确保数据库结构已初始化
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
