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
using WalkingTec.Mvvm.BlazorDemo.Shared;
using BootstrapBlazor.Localization.Json;
using WalkingTec.Mvvm.Core;
using Microsoft.JSInterop;
using System.Globalization;

namespace WalkingTec.Mvvm.BlazorDemo.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<Shared.App>("app");
            builder.Services.AddLocalization(option => option.ResourcesPath = "Resources");
            builder.Services.AddBootstrapBlazor(null, options => { options.ResourceManagerStringLocalizerType = typeof(Shared.Program); });
            builder.Services.AddWtmBlazor(builder.Configuration.Get<Configs>());
            var host = builder.Build();
            var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
            var result = await jsInterop.InvokeAsync<string>("localStorageFuncs.get", "wtmculture");
            if (result != null)
            {
                var culture = new CultureInfo(result);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }
            await host.RunAsync();
        }

    }
}
