using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.TagHelpers.LayUI;

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
                .ConfigureServices(x =>
                {
                    x.AddFrameworkService();
                    x.AddLayui();
                    x.AddSwaggerGen(c =>
                    {
                        c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                    });
                })
                .Configure(x =>
                {
                    var env = x.ApplicationServices.GetService<IHostingEnvironment>();
                    x.UseDeveloperExceptionPage();
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
