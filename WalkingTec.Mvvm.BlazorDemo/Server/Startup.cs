using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Json;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Binders;
using WalkingTec.Mvvm.Mvc.Filters;

namespace WalkingTec.Mvvm.BlazorDemo.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
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
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Configs>(Configuration);
            // services.AddControllersWithViews();
            services.AddDistributedMemoryCache();
            services.AddWtmSession(Configuration, 3600);
            services.AddWtmCrossDomain(Configuration);
            services.AddWtmAuthentication(Configuration);
            services.AddWtmHttpClient(Configuration);
            services.AddWtmSwagger();
            services.AddWtmMultiLanguages(Configuration);

            services.AddMvc(options =>
            {
                // ModelBinderProviders
                options.ModelBinderProviders.Insert(0, new StringBinderProvider());

                // Filters
                options.Filters.Add(new DataContextFilter(CSSelector));
                options.Filters.Add(new PrivilegeFilter());
                options.Filters.Add(new FrameworkFilter());
                options.EnableEndpointRouting = true;
            })
            .AddJsonOptions(options => {
                options.JsonSerializerOptions.Converters.Add(new StringIgnoreLTGTConverter());
                options.JsonSerializerOptions.Converters.Add(new DateRangeConverter());
                options.JsonSerializerOptions.Converters.Add(new BodyConverter());
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
            //services.AddScoped<IDataContext>(x => Configuration.Get<Configs>().ConnectionStrings[1].CreateDC());
            services.AddWtmContext(DataPrivilegeSettings());

            //services.AddControllersWithViews();
            //services.AddRazorPages();
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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler(configs.ErrorHandler);
            }

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseWtmMultiLanguages();
            app.UseWtmCrossDomain();
            app.UseAuthentication();

            app.UseSession();
            app.UseWtmSwagger();
            app.UseWtm();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });

            app.UseWtmContext();



            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseWebAssemblyDebugging();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            //app.UseHttpsRedirection();
            //app.UseBlazorFrameworkFiles();
            //app.UseStaticFiles();

            //app.UseRouting();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //    endpoints.MapControllers();
            //    endpoints.MapFallbackToFile("index.html");
            //});
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
