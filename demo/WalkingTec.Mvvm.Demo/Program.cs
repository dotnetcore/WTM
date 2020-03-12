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
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.TagHelpers.LayUI;

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
                Host.CreateDefaultBuilder(args)
                 .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder.ConfigureServices((hostingCtx, x) =>
                    {
                        var pris = new List<IDataPrivilege>
                        {
                            new DataPrivilegeInfo<School>("学校", y => y.SchoolName),
                            new DataPrivilegeInfo<Major>("专业", y => y.MajorName)
                        };
                        x.AddFrameworkService(dataPrivilegeSettings: pris, webHostBuilderContext: hostingCtx,CsSector:CSSelector);
                        x.AddLayui();
                        x.AddSwaggerGen(c =>
                        {
                            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                            var bearer = new OpenApiSecurityScheme()
                            {
                                Description = "JWT Bearer",
                                Name = "Authorization",
                                In = ParameterLocation.Header,
                                Type = SecuritySchemeType.ApiKey

                            };
                            c.AddSecurityDefinition("Bearer", bearer);
                            var sr = new OpenApiSecurityRequirement();
                            sr.Add(new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            }, new string[] { });
                            c.AddSecurityRequirement(sr);
                            var _xml_file = Path.Combine (AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                            c.IncludeXmlComments (_xml_file);
                        });
                    })
                    .Configure(x =>
                    {
                        var configs = x.ApplicationServices.GetRequiredService<Configs>();
                        if (configs.IsQuickDebug == true)
                        {
                            x.UseSwagger();
                            x.UseSwaggerUI(c =>
                            {
                                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                            });
                        }
                        x.UseFrameworkService();
                    })
                    .UseUrls(globalConfig.ApplicationUrl);

                 });

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
