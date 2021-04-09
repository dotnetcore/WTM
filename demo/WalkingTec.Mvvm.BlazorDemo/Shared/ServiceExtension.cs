using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Microsoft.Extensions.DependencyInjection;
using WalkingTec.Mvvm.BlazorDemo.Shared.Shared;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.BlazorDemo.Shared
{
    public static class ServiceExtension
    {
        public static void AddWtmBlazor(this IServiceCollection self, Configs config)
        {
            self.AddScoped<GlobalItems>();
            self.AddHttpClient<ApiClient>(x =>
            {
                x.BaseAddress = new Uri(config.Domains["server"].Url);
                x.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                x.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
            });

            foreach (var item in config.Domains)
            {
                if (item.Key?.ToLower() != "server")
                {
                    self.AddHttpClient(item.Key, x =>
                    {
                        x.BaseAddress = new Uri(item.Value.Url);
                    });
                }
            }
            self.AddScoped<WtmBlazorContext>();
            self.Configure<BootstrapBlazorOptions>(options =>
            {
                options.ToastPlacement = Placement.TopEnd;
                options.ToastDelay = 3000;
            });
        }
    }
}
