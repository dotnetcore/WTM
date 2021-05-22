using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using BootstrapBlazor.Components;
using Microsoft.Extensions.DependencyInjection;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.ConfigOptions;
using WalkingTec.Mvvm.Core.Json;

namespace WtmBlazorUtils
{
    public static class ServiceExtension
    {
        public static void AddWtmBlazor(this IServiceCollection self, Configs config, string baseAddress="")
        {
            self.AddScoped<GlobalItems>();
            self.AddSingleton<Configs>(config);
            string url = "";
            Domain domain = null;
            if(config.Domains.TryGetValue("server",out domain) && string.IsNullOrEmpty(domain?.Url) == false){
                url = domain.Url;
            }
            else
            {
                url = baseAddress;
            }
            self.AddHttpClient<ApiClient>(x =>
            {
                x.BaseAddress = new Uri(url);
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
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
            jsonOptions.PropertyNamingPolicy = null;
            jsonOptions.IgnoreNullValues = true;
            jsonOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;
            jsonOptions.AllowTrailingCommas = true;
            jsonOptions.Converters.Add(new DateTimeConverter());
            jsonOptions.Converters.Add(new StringIgnoreLTGTConverter());
            jsonOptions.Converters.Add(new JsonStringEnumConverter());
            jsonOptions.Converters.Add(new DateRangeConverter());
            jsonOptions.Converters.Add(new PocoConverter());
            jsonOptions.Converters.Add(new TypeConverter());
            jsonOptions.Converters.Add(new DynamicDataConverter());
            CoreProgram.DefaultJsonOption = jsonOptions;
            JsonSerializerOptions jsonOptions2 = new JsonSerializerOptions();
            jsonOptions2.PropertyNamingPolicy = null;
            jsonOptions2.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;
            jsonOptions2.AllowTrailingCommas = true;
            jsonOptions2.Converters.Add(new DateTimeConverter());
            jsonOptions2.Converters.Add(new JsonStringEnumConverter());
            jsonOptions2.Converters.Add(new DateRangeConverter());
            jsonOptions2.Converters.Add(new PocoConverter());
            jsonOptions2.Converters.Add(new TypeConverter());
            jsonOptions2.Converters.Add(new DynamicDataConverter());
            CoreProgram.DefaultPostJsonOption = jsonOptions2;

        }
    }
}
