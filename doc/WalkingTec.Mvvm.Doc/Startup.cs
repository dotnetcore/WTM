using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SpaServices.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Binders;
using WalkingTec.Mvvm.Mvc.Filters;
using WalkingTec.Mvvm.Mvc.Json;
using WalkingTec.Mvvm.TagHelpers.LayUI;

namespace WalkingTec.Mvvm.Doc
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.WTM_SetCurrentDictionary()
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();

            var config = configBuilder.Build();
            Configuration = config;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Configs>(Configuration);
           // services.AddControllersWithViews();
            services.AddDistributedMemoryCache();
            services.AddWtmSession(Configuration, 3600);
            services.AddWtmCrossDomain(Configuration);
            services.AddWtmAuthorization(Configuration);
            services.AddWtmHttpClient(Configuration);
            services.AddWtmSwagger();
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc(options =>
            {
                // ModelBinderProviders
                options.ModelBinderProviders.Insert(0, new StringBinderProvider());

                // Filters
                options.Filters.Add(new AuthorizeFilter());
                options.Filters.Add(new DataContextFilter(CSSelector));
                options.Filters.Add(new PrivilegeFilter());
                options.Filters.Add(new FrameworkFilter());
                options.EnableEndpointRouting = true;
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new WTMContractResolver() { };
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
                options.InvalidModelStateResponseFactory = (a) =>
                {
                    return new BadRequestObjectResult(a.ModelState.GetErrorJson());
                };
            })
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization(options => 
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    if (Core.Program.Buildindll.Any(x => type.FullName.StartsWith(x)))
                    {
                        return factory.Create(typeof(WalkingTec.Mvvm.Core.Program));
                    }
                    else
                    {
                        return factory.Create(typeof(Program));
                    }
                };
            });
            services.AddWtmContext(Configuration, DataPrivilegeSettings());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            IconFontsHelper.GenerateIconFont();
            var configs = app.ApplicationServices.GetRequiredService<IOptions<Configs>>().Value;

            if (configs == null)
            {
                throw new InvalidOperationException("Can not find Configs service, make sure you call AddWtmContext at ConfigService");
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

            app.UseRouting();
            app.UseWtmLanguages();
            app.UseWtmCrossDomain();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();
            app.UseWtmSwagger();
            app.UseWtm();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "areaRoute",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseWtmContext();


        }

        public string CSSelector(ActionExecutingContext context)
        {
            //To override the default logic of choosing connection string,
            //change this function to return different connection string key            
            return null;
        }

        public List<IDataPrivilege> DataPrivilegeSettings()
        {
            List<IDataPrivilege> pris = new List<IDataPrivilege>();
            //Add data privilege to specific type
            //pris.Add(new DataPrivilegeInfo<typea>("aaaPrivilege", m => m.Name));
            //pris.Add(new DataPrivilegeInfo<typeb>("bbbPrivilege", m => m.Name));
            return pris;
        }
    }
}
