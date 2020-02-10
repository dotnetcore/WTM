using System.IO;
using System.Reflection;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WalkingTec.Mvvm.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            if (args != null) configurationBuilder.AddCommandLine(args);

            var hostingConfig = configurationBuilder.Build();
            var globalConfig = hostingConfig.Get<GlobalConfig>();

            var ASPNETCORE_URLS = hostingConfig.GetValue<string>("ASPNETCORE_URLS");
            if (!string.IsNullOrEmpty(ASPNETCORE_URLS))
                globalConfig.ApplicationUrl = ASPNETCORE_URLS;

            return WebHost
                    .CreateDefaultBuilder(args)
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .ConfigureAppConfiguration((hostingCtx, config) =>
                    {
                        var env = hostingCtx.HostingEnvironment;

                        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                        if (env.IsDevelopment())
                        {
                            var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                            if (appAssembly != null)
                            {
                                config.AddUserSecrets(appAssembly, optional: true);
                            }
                        }

                        config.AddEnvironmentVariables();
                        if (args != null) config.AddCommandLine(args);
                    })
                    .ConfigureLogging((hostingCtx, logging) =>
                    {
                        logging.AddConfiguration(hostingCtx.Configuration.GetSection("Logging"));
                        logging.AddConsole();
                        logging.AddDebug();
                    })
                    .UseUrls(globalConfig.ApplicationUrl)
                    .UseStartup<Startup>();
        }
    }
}
