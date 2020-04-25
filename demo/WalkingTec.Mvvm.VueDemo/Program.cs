using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.TagHelpers.LayUI;
using Microsoft.Extensions.Logging;

namespace WalkingTec.Mvvm.VueDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                         .ConfigureLogging((hostingContext, logging) =>
                         {
                             logging.ClearProviders();
                             logging.AddConsole();
                             logging.AddDebug();
                             logging.AddWTMLogger();
                         })
               .ConfigureServices(x =>
                {
                    var pris = new List<IDataPrivilege>
                        {
                            new DataPrivilegeInfo<FrameworkRole>("测试角色", y => y.RoleName),
                        };
                    x.AddFrameworkService(dataPrivilegeSettings: pris);
                    x.AddLayui();
                    x.AddSwaggerGen(c =>
                    {
                        c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                    });
                    x.AddSpaStaticFiles(configuration =>
                    {
                        configuration.RootPath = "ClientApp/dist";
                    });

                })
                .Configure(x =>
                {
                    var env = x.ApplicationServices.GetService<IHostingEnvironment>();
                    x.UseDeveloperExceptionPage();
                    x.UseSpaStaticFiles();
                    if (env.IsDevelopment())
                    {
                        x.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                        {
                            HotModuleReplacement = false,
                            ConfigFile = "config/webpack.dev.js",
                            ProjectPath = System.IO.Path.Combine(env.ContentRootPath, "ClientApp/")

                        });
                    }
                    x.UseSwagger();
                    x.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    });
                    x.UseFrameworkService();
                })
                .Build();

    }
}
