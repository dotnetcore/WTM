using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Microsoft.Extensions.DependencyInjection;
using WalkingTec.Mvvm.BlazorDemo.Shared.Shared;

namespace WalkingTec.Mvvm.BlazorDemo.Shared
{
    public static class ServiceExtension
    {
        public static void AddWtmBlazor(this IServiceCollection self)
        {
            self.AddScoped<GlobalItems>();
            self.AddScoped<WtmBlazorContext>();
            self.Configure<BootstrapBlazorOptions>(options =>
            {
                options.ToastPlacement = Placement.TopEnd;
                options.ToastDelay = 3000;
            });
        }
    }
}
