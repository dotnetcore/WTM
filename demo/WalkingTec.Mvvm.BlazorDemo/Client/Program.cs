using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace WalkingTec.Mvvm.BlazorDemo.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddBootstrapBlazor();

            builder.Services.AddSingleton<IStringLocalizerFactory, StringLocalizerFactoryFoo>();
            await builder.Build().RunAsync();
        }

        internal class StringLocalizerFactoryFoo : IStringLocalizerFactory
        {
            private readonly ILoggerFactory _loggerFactory;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="loggerFactory"></param>
            public StringLocalizerFactoryFoo(ILoggerFactory loggerFactory)
            {
                _loggerFactory = loggerFactory;
            }

            /// <summary>
            /// 通过资源类型创建 IStringLocalizer 方法
            /// </summary>
            /// <param name="resourceSource"></param>
            /// <returns></returns>
            public IStringLocalizer Create(Type resourceSource)
            {
                return CreateStringLocalizer();
            }

            /// <summary>
            /// 通过 baseName 与 location 创建 IStringLocalizer 方法
            /// </summary>
            /// <param name="baseName"></param>
            /// <param name="location"></param>
            /// <returns></returns>
            public IStringLocalizer Create(string baseName, string location)
            {
                return CreateStringLocalizer();
            }

            /// <summary>
            /// 创建 IStringLocalizer 实例方法
            /// </summary>
            /// <returns></returns>
            protected virtual IStringLocalizer CreateStringLocalizer()
            {
                var logger = _loggerFactory.CreateLogger<WalkingTec.Mvvm.BlazorDemo.Shared.Program>();

                return new ResourceManagerStringLocalizerFactory(
                                       Options.Create(
                                           new LocalizationOptions
                                           {
                                               ResourcesPath = "Resources"
                                           })
                                           , new Microsoft.Extensions.Logging.LoggerFactory()
                                       )
                                       .Create(typeof(WalkingTec.Mvvm.BlazorDemo.Shared.Program));
            }
        }

    }
}
