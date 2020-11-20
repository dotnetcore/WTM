using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Swagger;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.TagHelpers.LayUI;
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

            return
                WebHost.CreateDefaultBuilder(args)
                        .ConfigureLogging((hostingContext, logging) =>
                        {
                            logging.ClearProviders();
                            logging.AddConsole();
                            logging.AddDebug();
                            logging.AddWTMLogger();
                        })
                        .ConfigureServices((hostingCtx, x) =>
                        {
                        var pris = new List<IDataPrivilege>
                        {
                            new DataPrivilegeInfo<School>("学校", y => y.SchoolName),
                            new DataPrivilegeInfo<Major>("专业", y => y.MajorName),
                             new DataPrivilegeInfo<City>("城市", y => y.Name),
                           //new DataPrivilegeInfo<FrameworkMenu>("菜单", y=>y.PageName)
                        };
                        x.AddFrameworkService(dataPrivilegeSettings: pris, webHostBuilderContext: hostingCtx,CsSector:CSSelector);
                        x.AddLayui();
                        x.AddSwaggerGen(c =>
                        {
                            c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                            c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
                            {
                                Description = "JWT Bearer",
                                Name = "Authorization",
                                In = "header",
                                Type = "apiKey"
                            });
                            c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                            {
                                { "Bearer", new string[] { } }
                            });
                        });
                    })
                    .Configure(x =>
                    {
                        var configs = x.ApplicationServices.GetRequiredService<Configs>();
                        //if (configs.IsQuickDebug == true)
                        //{
                            x.UseSwagger();
                            x.UseSwaggerUI(c =>
                            {
                                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                            });
                        //}
                        x.UseFrameworkService();
                    })
                    .UseUrls(globalConfig.ApplicationUrl);
        }

        public static string CSSelector(ActionExecutingContext context)
        {
            var userinfo = (context.Controller as IBaseController)?.LoginUserInfo;
            if (userinfo == null)
            {
                return "default";
            }
            else
            {
                if (userinfo.ITCode.StartsWith("a"))
                {
                    return "default";
                }
                else
                {
                    return "default";
                }
            }
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
