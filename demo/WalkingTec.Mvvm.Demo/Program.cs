using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var server = CreateWebHostBuilder(args).Build();
            WTMContext wtm = null;
            using (var scope = server.Services.CreateScope())
            {
                wtm = scope.ServiceProvider.GetRequiredService<WTMContext>();
            }
            server.Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return
                Host.CreateDefaultBuilder(args)
                  .ConfigureAppConfiguration((hostingContext, config) =>
                  {
                      config.AddInMemoryCollection(new Dictionary<string, string> { { "HostRoot", hostingContext.HostingEnvironment.ContentRootPath } });
                  })
                .ConfigureLogging((hostingContext, logging) =>
                 {
                     logging.ClearProviders();
                     logging.AddConsole();
                     logging.AddWTMLogger();
                 })
                .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder.UseStartup<Startup>();
                 });
        }

    }

    public static class ConfigInfoExtension
    {
        public static string Key1(this Configs self)
        {
            return self.AppSettings["Key1"];
        }
    }
}
