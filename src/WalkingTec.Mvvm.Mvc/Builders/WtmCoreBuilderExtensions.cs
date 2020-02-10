using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SpaServices.StaticFiles;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace Microsoft.AspNetCore.Builder
{
    public static class WtmCoreBuilderExtensions
    {
        public static IApplicationBuilder UseWtmCore(this IApplicationBuilder app, Action<IRouteBuilder> customRoutes = null)
        {
            IconFontsHelper.GenerateIconFont();
            var configs = app.ApplicationServices.GetRequiredService<Configs>();
            var gd = app.ApplicationServices.GetRequiredService<GlobalData>();

            if (configs == null)
            {
                throw new InvalidOperationException("Can not find Configs service, make sure you call AddFrameworkService at ConfigService");
            }
            if (gd == null)
            {
                throw new InvalidOperationException("Can not find GlobalData service, make sure you call AddFrameworkService at ConfigService");
            }
            if (string.IsNullOrEmpty(configs.Languages) == false)
            {
                List<CultureInfo> supportedCultures = new List<CultureInfo>();
                var lans = configs.Languages.Split(",");
                foreach (var lan in lans)
                {
                    supportedCultures.Add(new CultureInfo(lan));
                }

                app.UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture(supportedCultures[0]),
                    SupportedCultures = supportedCultures,
                    SupportedUICultures = supportedCultures
                });
            }

            app.UseExceptionHandler(configs.ErrorHandler);

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = new PathString("/_js"),
                FileProvider = new EmbeddedFileProvider(
                    typeof(_CodeGenController).GetTypeInfo().Assembly,
                    "WalkingTec.Mvvm.Mvc")
            });
            app.UseAuthentication();

            app.UseResponseCaching();

            bool InitDataBase = false;
            app.Use(async (context, next) =>
            {
                if (InitDataBase == false)
                {
                    var lg = app.ApplicationServices.GetRequiredService<LinkGenerator>();
                    foreach (var m in gd.AllModule)
                    {
                        foreach (var a in m.Actions)
                        {
                            var u = lg.GetPathByAction(a.MethodName, m.ClassName, new { area = m.Area?.AreaName });
                            if (u == null)
                            {
                                u = lg.GetPathByAction(a.MethodName, m.ClassName, new { id = 0, area = m.Area?.AreaName });
                            }
                            if (u != null && u.EndsWith("/0"))
                            {
                                u = u.Substring(0, u.Length - 2);
                                u = u + "/{id}";
                            }
                            a.Url = u;
                        }
                    }

                    var test = app.ApplicationServices.GetService<ISpaStaticFileProvider>();
                    var cs = configs.ConnectionStrings;
                    foreach (var item in cs)
                    {
                        var dc = item.CreateDC();
                        dc.DataInit(gd.AllModule, test != null).Wait();
                    }
                    GlobalServices.SetServiceProvider(app.ApplicationServices);
                    InitDataBase = true;
                }

                if (context.Request.Path == "/")
                {
                    context.Response.Cookies.Append("pagemode", configs.PageMode.ToString());
                    context.Response.Cookies.Append("tabmode", configs.TabMode.ToString());
                }
                try
                {
                    await next.Invoke();
                }
                catch (ConnectionResetException) { }
                if (context.Response.StatusCode == 404)
                {
                    await context.Response.WriteAsync(string.Empty);
                }
            });

            app.UseSession();
            if (configs.CorsOptions.EnableAll == true)
            {
                if (configs.CorsOptions?.Policy?.Count > 0)
                {
                    app.UseCors(configs.CorsOptions.Policy[0].Name);
                }
                else
                {
                    app.UseCors("_donotusedefault");
                }
            }

            if (customRoutes != null)
            {
                app.UseMvc(customRoutes);
            }
            else
            {
                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "areaRoute",
                        template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });
            }

            return app;
        }
    }
}
