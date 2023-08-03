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
            CreateWebHostBuilder(args).Build().Run();
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
}
