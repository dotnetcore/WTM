using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using WalkingTec.Mvvm.ReactDemo.Models;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.TagHelpers.LayUI;

namespace WalkingTec.Mvvm.ReactDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(x =>
                {
                    var pris = new List<IDataPrivilege>
                        {
                            new DataPrivilegeInfo<School>("学校", y => y.SchoolName),
                        };
                    x.AddFrameworkService(dataPrivilegeSettings: pris);
                    x.AddLayui();
                    x.AddSwaggerGen(c =>
                    {
                        c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                    });
                    x.AddSpaStaticFiles(configuration =>
                    {
                        configuration.RootPath = "ClientApp/build";
                    });
                })
                .Configure(x =>
                {
                    var env = x.ApplicationServices.GetService<IHostingEnvironment>();
                    x.UseSwagger();
                    x.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    });
                    x.UseSpaStaticFiles();
                    x.UseFrameworkService();
                    x.UseSpa(spa =>
                    {
                        spa.Options.SourcePath = "ClientApp";                        
                        if (env.IsDevelopment())
                        {
                            spa.UseReactDevelopmentServer(npmScript: "start");
                        }
                    });

                })
                .Build();

    }
}
