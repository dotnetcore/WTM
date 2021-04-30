using System;
using System.Collections.Generic;
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
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.FileHandlers;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Mvc;
using WtmBlazorUtils;

namespace WalkingTec.Mvvm.BlazorDemo.Server
{
    public class Startup
    {
        public IConfiguration ConfigRoot { get; }

        public Startup(IWebHostEnvironment env, IConfiguration config)
        {
            ConfigRoot = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var config = ConfigRoot.Get<Configs>();
            services.AddDistributedMemoryCache();
            services.AddWtmSession(3600, ConfigRoot);
            services.AddWtmCrossDomain(ConfigRoot);
            services.AddWtmAuthentication(ConfigRoot);
            services.AddWtmHttpClient(ConfigRoot);
            services.AddWtmMultiLanguages(ConfigRoot,op => op.LocalizationType = typeof(Shared.Program));
            services.AddWtmSwagger();

            services.AddMvc(options =>
            {
                options.UseWtmMvcOptions();
            })
            .AddJsonOptions(options =>
            {
                options.UseWtmJsonOptions();
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            .ConfigureApiBehaviorOptions(options =>
            {
                options.UseWtmApiOptions();
            })
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
           .AddWtmDataAnnotationsLocalization(typeof(Shared.Program));
            if (config.BlazorMode == BlazorModeEnum.Server)
            {
                services.AddServerSideBlazor();
                services.AddBootstrapBlazor(null, options =>
                {
                    options.ResourceManagerStringLocalizerType = typeof(Shared.Program);
                });
                services.AddWtmBlazor(config);
            }
            services.AddWtmContext(ConfigRoot, (options) =>
            {
                options.DataPrivileges = DataPrivilegeSettings();
                options.CsSelector = CSSelector;
                options.FileSubDirSelector = SubDirSelector;
                options.ReloadUserFunc = ReloadUser;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            IconFontsHelper.GenerateIconFont("wwwroot/font", "wwwroot/font-awesome");
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

            app.UseStaticFiles();
            app.UseWtmStaticFiles();
            app.UseRouting();
            app.UseWtmMultiLanguages();
            app.UseWtmCrossDomain();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseWtmSwagger();
            app.UseWtm();

            if (configs.BlazorMode == BlazorModeEnum.Server)
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapBlazorHub();
                    endpoints.MapRazorPages();
                    endpoints.MapControllerRoute(
                       name: "areaRoute",
                       pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapFallbackToPage("/_Host");
                });
            }
            else
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapRazorPages();
                    endpoints.MapControllerRoute(
                       name: "areaRoute",
                       pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapFallbackToFile("index.html");
                });
            }
            app.UseBlazorFrameworkFiles();
            app.UseWtmContext(true);
        }

        /// <summary>
        /// Wtm will call this function to dynamiclly set connection string
        /// 框架会调用这个函数来动态设定每次访问需要链接的数据库
        /// </summary>
        /// <param name="context">ActionContext</param>
        /// <returns>Connection string key name</returns>
        public string CSSelector(ActionExecutingContext context)
        {
            //To override the default logic of choosing connection string,
            //change this function to return different connection string key
            //根据context返回不同的连接字符串的名称
            return null;
        }

        /// <summary>
        /// Set data privileges that system supports
        /// 设置系统支持的数据权限
        /// </summary>
        /// <returns>data privileges list</returns>
        public List<IDataPrivilege> DataPrivilegeSettings()
        {
            List<IDataPrivilege> pris = new List<IDataPrivilege>();
            //Add data privilege to specific type
            //指定哪些模型需要数据权限
            //pris.Add(new DataPrivilegeInfo<City>("城市权限", m => m.Name));

            //pris.Add(new DataPrivilegeInfo<FrameworkRole>("角色权限", m => m.RoleName));
            //pris.Add(new DataPrivilegeInfo<City>("城市权限", m => m.Name));
            //pris.Add(new DataPrivilegeInfo<School>("学校权限", m => m.SchoolName));

            return pris;
        }

        /// <summary>
        /// Set sub directory of uploaded files
        /// 动态设置上传文件的子目录
        /// </summary>
        /// <param name="fh">IWtmFileHandler</param>
        /// <returns>subdir name</returns>
        public string SubDirSelector(IWtmFileHandler fh)
        {
            if (fh is WtmLocalFileHandler)
            {
                return DateTime.Now.ToString("yyyy-MM-dd");
            }
            return null;
        }

        /// <summary>
        /// Custom Reload user process when cache is not available
        /// 设置自定义的方法重新读取用户信息，这个方法会在用户缓存失效的时候调用
        /// </summary>
        /// <param name="context"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public LoginUserInfo ReloadUser(WTMContext context, string account)
        {
            return null;
        }

    }
}
