using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Vue3Demo
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
