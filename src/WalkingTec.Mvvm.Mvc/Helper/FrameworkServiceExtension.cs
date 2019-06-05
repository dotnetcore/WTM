using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Loader;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SpaServices.StaticFiles;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using Microsoft.Extensions.FileProviders;

using Newtonsoft.Json;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.FDFS;
using WalkingTec.Mvvm.Core.Implement;
using WalkingTec.Mvvm.Mvc.Binders;
using WalkingTec.Mvvm.Mvc.Filters;
using WalkingTec.Mvvm.Mvc.Json;

namespace WalkingTec.Mvvm.Mvc
{
    public static class FrameworkServiceExtension
    {
        public static IServiceCollection AddFrameworkService(this IServiceCollection services,
            Func<ActionExecutingContext, string> CsSector = null,
            List<IDataPrivilege> dataPrivilegeSettings = null,
            WebHostBuilderContext webHostBuilderContext = null
        )
        {
            CurrentDirectoryHelpers.SetCurrentDirectory();
            IConfigurationRoot config = null;

            var configBuilder =
                    new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();

            if (webHostBuilderContext != null)
            {
                IHostingEnvironment env = webHostBuilderContext.HostingEnvironment;
                configBuilder
                    .AddJsonFile(
                        $"appsettings.{env.EnvironmentName}.json",
                        optional: true,
                        reloadOnChange: true
                    );
            }
            config = configBuilder.Build();

            var gd = GetGlobalData();
            services.AddSingleton(gd);
            var con = config.Get<Configs>() ?? new Configs();
            if (dataPrivilegeSettings != null)
            {
                con.DataPrivilegeSettings = dataPrivilegeSettings;
            }
            else
            {
                con.DataPrivilegeSettings = new List<IDataPrivilege>();
            }
            services.AddSingleton(con);
            services.AddResponseCaching();
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = con.CookiePre + ".Session";
                options.IdleTimeout = TimeSpan.FromSeconds(3600);
            });
            SetupDFS(con);


            var mvc = gd.AllAssembly.Where(x => x.ManifestModule.Name == "WalkingTec.Mvvm.Mvc.dll").FirstOrDefault();
            var admin = gd.AllAssembly.Where(x => x.ManifestModule.Name == "WalkingTec.Mvvm.Mvc.Admin.dll").FirstOrDefault();
            services.AddMvc(options =>
            {
                // ModelBinderProviders
                options.ModelBinderProviders.Insert(0, new StringBinderProvider());

                // Filters
                options.Filters.Add(new DataContextFilter(CsSector));
                options.Filters.Add(new PrivilegeFilter());
                options.Filters.Add(new FrameworkFilter());
            })
            .AddJsonOptions(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                // custom ContractResolver
                options.SerializerSettings.ContractResolver = new WTMContractResolver()
                {
                    //NamingStrategy = new CamelCaseNamingStrategy()
                };
            })
            .ConfigureApplicationPartManager(m =>
            {
                var feature = new ControllerFeature();
                if (mvc != null)
                {
                    m.ApplicationParts.Add(new AssemblyPart(mvc));
                }
                if (admin != null)
                {
                    m.ApplicationParts.Add(new AssemblyPart(admin));
                }
                m.PopulateFeature(feature);
                services.AddSingleton(feature.Controllers.Select(t => t.AsType()).ToArray());
            })
            .AddControllersAsServices()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
                options.InvalidModelStateResponseFactory = (a) =>
                {
                    return new BadRequestObjectResult(a.ModelState.GetErrorJson());
                };
            });


            services.Configure<RazorViewEngineOptions>(options =>
            {
                if (mvc != null)
                {
                    options.FileProviders.Add(
                    new EmbeddedFileProvider(
                        mvc,
                        "WalkingTec.Mvvm.Mvc" // your external assembly's base namespace
                    )
                );
                }
                if (admin != null)
                {
                    options.FileProviders.Add(
                        new EmbeddedFileProvider(
                            admin,
                            "WalkingTec.Mvvm.Mvc.Admin" // your external assembly's base namespace
                        )
                    );
                }
            });

            services.Configure<FormOptions>(y =>
            {
                y.ValueLengthLimit = int.MaxValue;
                y.MultipartBodyLengthLimit = con.FileUploadOptions.UploadLimit;
            });

            services.AddSingleton<IUIService, DefaultUIService>();
            GlobalServices.SetServiceProvider(services.BuildServiceProvider());
            return services;
        }

        public static IApplicationBuilder UseFrameworkService(this IApplicationBuilder app, Action<IRouteBuilder> customRoutes = null)
        {
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
            app.UseResponseCaching();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.Cookies.Append("pagemode", configs.PageMode.ToString());
                    context.Response.Cookies.Append("tabmode", configs.TabMode.ToString());
                }
                await next.Invoke();
                if (context.Response.StatusCode == 404)
                {
                    await context.Response.WriteAsync(string.Empty);
                }
            });

            app.UseExceptionHandler("/_Framework/Error");

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = new PathString("/_js"),
                FileProvider = new EmbeddedFileProvider(
                    typeof(_CodeGenController).GetTypeInfo().Assembly,
                    "WalkingTec.Mvvm.Mvc")
            });
            app.UseSession();
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

            var lg = app.ApplicationServices.GetRequiredService<LinkGenerator>();
            foreach (var m in gd.AllModule)
            {
                if (m.IsApi == true)
                {
                    foreach (var a in m.Actions)
                    {
                        var u = lg.GetPathByAction(a.MethodName, m.ClassName, new { area = m.Area?.AreaName });
                        if (u == null)
                        {
                            u = lg.GetPathByAction(a.MethodName, m.ClassName, new { id = 0, area = m.Area?.AreaName });
                        }
                        if (u!= null && u.EndsWith("/0"))
                        {
                            u = u.Substring(0, u.Length - 2);
                            u = u + "/{id}";
                        }
                        a.Url = u;
                    }
                }
            }


            var test = app.ApplicationServices.GetService<ISpaStaticFileProvider>();
            var cs = configs.ConnectionStrings.Select(x => x.Value);
            foreach (var item in cs)
            {
                var dc = (IDataContext)gd.DataContextCI.Invoke(new object[] { item, configs.DbType });
                dc.DataInit(gd.AllModule, test != null).Wait();
            }
            return app;
        }

        private static GlobalData GetGlobalData()
        {
            var gd = new GlobalData();

            //获取所有程序集
            gd.AllAssembly = Utils.GetAllAssembly();
            var admin = GetRuntimeAssembly("WalkingTec.Mvvm.Mvc.Admin");
            if (admin != null && gd.AllAssembly.Contains(admin) == false)
            {
                gd.AllAssembly.Add(admin);
            }
            var mvc = GetRuntimeAssembly("WalkingTec.Mvvm.Mvc");
            if (mvc != null && gd.AllAssembly.Contains(mvc) == false)
            {
                gd.AllAssembly.Add(mvc);
            }
            gd.DataContextCI = GetDbContextCI(gd.AllAssembly);
            gd.AllModels = GetAllModels(gd.DataContextCI);

            var controllers = GetAllControllers(gd.AllAssembly);

            gd.AllAccessUrls = GetAllAccessUrls(controllers);
            gd.AllModule = GetAllModules(controllers);

            gd.SetMenuGetFunc(() =>
            {
                var menus = new List<FrameworkMenu>();
                var cache = GlobalServices.GetService<IMemoryCache>();
                var menuCacheKey = "FFMenus";
                if (cache.TryGetValue(menuCacheKey, out List<FrameworkMenu> rv) == false)
                {
                    var data = GetAllMenus(gd.AllModule, gd.DataContextCI);
                    cache.Set(menuCacheKey, data);
                    menus = data;
                }
                else
                {
                    menus = rv;
                }

                return menus;
            });
            return gd;
        }

        private static List<FrameworkMenu> GetAllMenus(List<FrameworkModule> allModule, ConstructorInfo constructorInfo)
        {
            var ConfigInfo = GlobalServices.GetService<Configs>();
            var menus = new List<FrameworkMenu>();

            if (ConfigInfo.IsQuickDebug)
            {
                menus = new List<FrameworkMenu>();
                foreach (var model in allModule.Where(x => x.NameSpace != "WalkingTec.Mvvm.Admin.Api"))
                {
                    var modelmenu = new FrameworkMenu
                    {
                        //ID = Guid.NewGuid(),
                        PageName = model.ModuleName
                    };
                    menus.Add(modelmenu);
                    foreach (var action in model.Actions)
                    {
                        var url = string.Empty;
                        if (model.Area == null)
                        {
                            url = $"/{model.ClassName}/{action.MethodName}";
                        }
                        else
                        {
                            url = $"/{model.Area.Prefix}/{model.ClassName}/{action.MethodName}";
                        }
                        menus.Add(new FrameworkMenu
                        {
                            ID = Guid.NewGuid(),
                            ParentId = modelmenu.ID,
                            PageName = action.ActionName,
                            Url = url
                        });
                    }
                }
            }
            else
            {
                try
                {
                    using (var dc = (IDataContext)constructorInfo?.Invoke(new object[] { ConfigInfo.ConnectionStrings.Where(x => x.Key.ToLower() == "default").Select(x => x.Value).FirstOrDefault(), ConfigInfo.DbType }))
                    {
                        menus.AddRange(dc?.Set<FrameworkMenu>()
                                .Include(x => x.Domain)
                                .OrderBy(x => x.DisplayOrder)
                                .ToList());
                    }
                }
                catch { }
            }
            return menus;
        }

        /// <summary>
        /// 获取所有继承 BaseController 控制器
        /// </summary>
        /// <param name="allAssemblies"></param>
        /// <returns></returns>
        private static List<Type> GetAllControllers(List<Assembly> allAssemblies)
        {
            var controllers = new List<Type>();
            foreach (var ass in allAssemblies)
            {
                var types = new List<Type>();
                try
                {
                    types.AddRange(ass.GetExportedTypes());
                }
                catch { }

                controllers.AddRange(types.Where(x => typeof(IBaseController).IsAssignableFrom(x)).ToList());
            }
            return controllers;
        }

        /// <summary>
        /// 获取DbContextCI
        /// </summary>
        /// <returns></returns>
        private static ConstructorInfo GetDbContextCI(List<Assembly> AllAssembly)
        {
            ConstructorInfo ci = null;
            foreach (var ass in AllAssembly)
            {
                try
                {
                    var t = ass.GetExportedTypes().Where(x => typeof(DbContext).IsAssignableFrom(x) && x.Name != "DbContext").ToList();
                    ci = t.Where(x => x.Name != "FrameworkContext").FirstOrDefault()?.GetConstructor(new Type[] { typeof(string), typeof(DBTypeEnum) });
                    if (ci != null)
                    {
                        break;
                    }
                }
                catch { }
            }
            return ci;
        }

        /// <summary>
        /// 获取所有 Model
        /// </summary>
        /// <returns></returns>
        private static List<Type> GetAllModels(ConstructorInfo dbContextCI)
        {
            var models = new List<Type>();

            //获取所有模型
            var pros = dbContextCI?.DeclaringType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            if (pros != null)
            {
                foreach (var pro in pros)
                {
                    if (pro.PropertyType.IsGeneric(typeof(DbSet<>)))
                    {
                        models.Add(pro.PropertyType.GetGenericArguments()[0]);
                    }
                }
            }
            //models.Add(typeof(FrameworkMenu));
            //models.Add(typeof(FrameworkUserBase));
            //models.Add(typeof(FrameworkGroup));
            //models.Add(typeof(FrameworkRole));
            //models.Add(typeof(ActionLog));
            return models;
        }

        /// <summary>
        /// 获取所有模块
        /// </summary>
        /// <param name="controllers"></param>
        /// <returns></returns>
        private static List<FrameworkModule> GetAllModules(List<Type> controllers)
        {
            var modules = new List<FrameworkModule>();

            foreach (var ctrl in controllers)
            {
                var pubattr = ctrl.GetCustomAttributes(typeof(PublicAttribute), false);
                var rightattr = ctrl.GetCustomAttributes(typeof(AllRightsAttribute), false);
                var debugattr = ctrl.GetCustomAttributes(typeof(DebugOnlyAttribute), false);
                var areaattr = ctrl.GetCustomAttributes(typeof(AreaAttribute), false);
                var model = new FrameworkModule
                {
                    ClassName = ctrl.Name.Replace("Controller", string.Empty)
                };
                if (ctrl.Namespace == "WalkingTec.Mvvm.Mvc")
                {
                    continue;
                }
                if (areaattr.Length == 0 && model.ClassName == "Home")
                {
                    continue;
                }
                if (pubattr.Length > 0 || rightattr.Length > 0 || debugattr.Length > 0)
                {
                    model.IgnorePrivillege = true;
                }
                if (typeof(BaseApiController).IsAssignableFrom(ctrl))
                {
                    model.IsApi = true;
                }
                model.NameSpace = ctrl.Namespace;
                //获取controller上标记的ActionDescription属性的值
                var attrs = ctrl.GetCustomAttributes(typeof(ActionDescriptionAttribute), false);
                if (attrs.Length > 0)
                {
                    var ada = attrs[0] as ActionDescriptionAttribute;
                    var nameKey = ada.Description;
                    model.ModuleName = nameKey;
                }
                else
                {
                    model.ModuleName = model.ClassName;
                }
                //获取该controller下所有的方法
                var methods = ctrl.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
                //过滤掉/Login/Login方法和特殊方法
                if (model.ClassName.ToLower() == "login")
                {
                    methods = methods.Where(x => x.IsSpecialName == false && x.Name.ToLower() != "login").ToArray();
                }
                else
                {
                    methods = methods.Where(x => x.IsSpecialName == false).ToArray();
                }
                model.Actions = new List<FrameworkAction>();
                //循环所有方法
                foreach (var method in methods)
                {
                    var pubattr2 = method.GetCustomAttributes(typeof(PublicAttribute), false);
                    var arattr2 = method.GetCustomAttributes(typeof(AllRightsAttribute), false);
                    var debugattr2 = method.GetCustomAttributes(typeof(DebugOnlyAttribute), false);
                    var postAttr = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
                    //如果不是post的方法，则添加到controller的action列表里
                    if (postAttr.Length == 0)
                    {
                        var action = new FrameworkAction
                        {
                            Module = model,
                            MethodName = method.Name
                        };
                        if (pubattr2.Length > 0 || arattr2.Length > 0 || debugattr2.Length > 0)
                        {
                            action.IgnorePrivillege = true;
                        }

                        var attrs2 = method.GetCustomAttributes(typeof(ActionDescriptionAttribute), false);
                        if (attrs2.Length > 0)
                        {
                            var ada = attrs2[0] as ActionDescriptionAttribute;
                            var nameKey = ada.Description;
                            action.ActionName = nameKey;
                        }
                        else
                        {
                            action.ActionName = action.MethodName;
                        }
                        var pars = method.GetParameters();
                        if (pars != null && pars.Length > 0)
                        {
                            action.ParasToRunTest = new List<string>();
                            foreach (var par in pars)
                            {
                                action.ParasToRunTest.Add(par.Name);
                            }
                        }
                        model.Actions.Add(action);
                    }
                }
                //再次循环所有方法
                foreach (var method in methods)
                {
                    var pubattr2 = method.GetCustomAttributes(typeof(PublicAttribute), false);
                    var arattr2 = method.GetCustomAttributes(typeof(AllRightsAttribute), false);
                    var debugattr2 = method.GetCustomAttributes(typeof(DebugOnlyAttribute), false);

                    var postAttr = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
                    //找到post的方法且没有同名的非post的方法，添加到controller的action列表里
                    if (postAttr.Length > 0 && model.Actions.Where(x => x.MethodName.ToLower() == method.Name.ToLower()).FirstOrDefault() == null)
                    {
                        if (method.Name.ToLower().StartsWith("dobatch"))
                        {
                            if (model.Actions.Where(x => "do" + x.MethodName.ToLower() == method.Name.ToLower()).FirstOrDefault() != null)
                            {
                                continue;
                            }
                        }
                        var action = new FrameworkAction
                        {
                            Module = model,
                            MethodName = method.Name
                        };
                        if (pubattr2.Length > 0 || arattr2.Length > 0 || debugattr2.Length > 0)
                        {
                            action.IgnorePrivillege = true;
                        }
                        var attrs2 = method.GetCustomAttributes(typeof(ActionDescriptionAttribute), false);
                        if (attrs2.Length > 0)
                        {
                            var ada = attrs2[0] as ActionDescriptionAttribute;
                            string nameKey = ada.Description;
                            action.ActionName = nameKey;
                        }
                        else
                        {
                            action.ActionName = action.MethodName;
                        }
                        model.Actions.Add(action);
                    }
                }
                if (model.Actions != null && model.Actions.Count() > 0)
                {
                    if (areaattr.Length > 0)
                    {
                        string areaName = (areaattr[0] as AreaAttribute).RouteValue;
                        var existArea = modules.Where(x => x.Area?.AreaName == areaName).Select(x => x.Area).FirstOrDefault();
                        if (existArea == null)
                        {
                            model.Area = new FrameworkArea
                            {
                                AreaName = (areaattr[0] as AreaAttribute).RouteValue,
                                Prefix = (areaattr[0] as AreaAttribute).RouteValue,
                            };
                        }
                        else
                        {
                            model.Area = existArea;
                        }
                    }
                    modules.Add(model);
                }
            }

            return modules;
        }

        /// <summary>
        /// 获取所有无需权限验证的链接
        /// </summary>
        /// <param name="controllers"></param>
        /// <returns></returns>
        private static List<string> GetAllAccessUrls(List<Type> controllers)
        {
            var rv = new List<string>();
            foreach (var ctrl in controllers)
            {
                if (typeof(BaseApiController).IsAssignableFrom(ctrl))
                {
                    continue;
                }
                var area = string.Empty;
                var ControllerName = ctrl.Name.Replace("Controller", string.Empty);
                var includeAll = false;
                //获取controller上标记的ActionDescription属性的值
                var attrs = ctrl.GetCustomAttributes(typeof(AllRightsAttribute), false);
                var attrs2 = ctrl.GetCustomAttributes(typeof(PublicAttribute), false);
                var areaAttr = ctrl.GetCustomAttribute(typeof(AreaAttribute), false);
                if (areaAttr != null)
                {
                    area = (areaAttr as AreaAttribute).RouteValue;
                }
                if (attrs.Length > 0 || attrs2.Length > 0)
                {
                    includeAll = true;
                }

                //获取该controller下所有的方法
                var methods = ctrl.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
                //过滤掉特殊方法
                methods = methods.Where(x => x.IsSpecialName == false).ToArray();
                var ActionName = string.Empty;
                //循环所有方法
                foreach (var method in methods)
                {
                    var postAttr = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
                    //如果不是post的方法，则添加到controller的action列表里
                    if (postAttr.Length == 0)
                    {
                        ActionName = method.Name;
                        var url = ControllerName + "/" + ActionName;
                        if (!string.IsNullOrEmpty(area))
                        {
                            url = area + "/" + url;
                        }
                        if (includeAll == true)
                        {
                            rv.Add(url);
                        }
                        else
                        {
                            var attrs3 = method.GetCustomAttributes(typeof(AllRightsAttribute), false);
                            var attrs4 = method.GetCustomAttributes(typeof(PublicAttribute), false);
                            if (attrs3.Length > 0 || attrs4.Length > 0)
                            {
                                rv.Add(url);
                            }
                        }
                    }
                }
                //再次循环所有方法
                foreach (var method in methods)
                {
                    var postAttr = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
                    //找到post的方法且没有同名的非post的方法，添加到controller的action列表里
                    if (postAttr.Length > 0 && !rv.Contains(ControllerName + "/" + method.Name))
                    {
                        ActionName = method.Name;
                        var url = ControllerName + "/" + ActionName;
                        if (!string.IsNullOrEmpty(area))
                        {
                            url = area + "/" + url;
                        }
                        if (includeAll == true)
                        {
                            rv.Add(url);
                        }
                        else
                        {
                            var attrs5 = method.GetCustomAttributes(typeof(AllRightsAttribute), false);
                            var attrs6 = method.GetCustomAttributes(typeof(PublicAttribute), false);
                            if (attrs5.Length > 0 || attrs6.Length > 0)
                            {
                                rv.Add(url);
                            }
                        }
                    }
                }
            }
            return rv;
        }

        private static Assembly GetRuntimeAssembly(string name)
        {
            var path = Assembly.GetEntryAssembly().Location;
            var library = DependencyContext.Default.RuntimeLibraries.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();
            if (library == null)
            {
                return null;
            }
            var r = new CompositeCompilationAssemblyResolver(new ICompilationAssemblyResolver[]
        {
            new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(path)),
            new ReferenceAssemblyPathResolver(),
            new PackageCompilationAssemblyResolver()
        });

            var wrapper = new CompilationLibrary(
                library.Type,
                library.Name,
                library.Version,
                library.Hash,
                library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                library.Dependencies,
                library.Serviceable);

            var assemblies = new List<string>();
            r.TryResolveAssemblyPaths(wrapper, assemblies);
            if (assemblies.Count > 0)
            {
                return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblies[0]);
            }
            else
            {
                return null;
            }
        }

        private static void SetupDFS(Configs con)
        {
            FDFSConfig.ConnectionTimeout = con.DFSServer.ConnectionTimeout ?? 100;
            FDFSConfig.Connection_LifeTime = con.DFSServer.ConnectionLifeTime ?? 3600;
            FDFSConfig.Storage_MaxConnection = con.DFSServer.StorageMaxConnection ?? 100;
            FDFSConfig.Tracker_MaxConnection = con.DFSServer.TrackerMaxConnection ?? 100;
            List<IPEndPoint> TrackerServers = new List<IPEndPoint>();
            if (con.DFSServer?.Trackers != null)
            {
                foreach (var tracker in con.DFSServer.Trackers)
                {
                    if (string.IsNullOrEmpty(tracker.IP) == false)
                    {
                        var point = new IPEndPoint(IPAddress.Parse(tracker.IP), tracker.Port);
                        TrackerServers.Add(point);
                    }
                }
            }
            FDFSConfig.Trackers = TrackerServers;
        }
    }


    /// <summary>
    /// 解决IIS InProgress下CurrentDirectory获取错误的问题
    /// </summary>
    internal class CurrentDirectoryHelpers

    {

        internal const string AspNetCoreModuleDll = "aspnetcorev2_inprocess.dll";



        [System.Runtime.InteropServices.DllImport("kernel32.dll")]

        private static extern IntPtr GetModuleHandle(string lpModuleName);



        [System.Runtime.InteropServices.DllImport(AspNetCoreModuleDll)]

        private static extern int http_get_application_properties(ref IISConfigurationData iiConfigData);



        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]

        private struct IISConfigurationData

        {

            public IntPtr pNativeApplication;

            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)]

            public string pwzFullApplicationPath;

            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.BStr)]

            public string pwzVirtualApplicationPath;

            public bool fWindowsAuthEnabled;

            public bool fBasicAuthEnabled;

            public bool fAnonymousAuthEnable;

        }



        public static void SetCurrentDirectory()

        {

            try

            {

                // Check if physical path was provided by ANCM

                var sitePhysicalPath = Environment.GetEnvironmentVariable("ASPNETCORE_IIS_PHYSICAL_PATH");

                if (string.IsNullOrEmpty(sitePhysicalPath))

                {

                    // Skip if not running ANCM InProcess

                    if (GetModuleHandle(AspNetCoreModuleDll) == IntPtr.Zero)

                    {

                        return;

                    }



                    IISConfigurationData configurationData = default(IISConfigurationData);

                    if (http_get_application_properties(ref configurationData) != 0)

                    {

                        return;

                    }



                    sitePhysicalPath = configurationData.pwzFullApplicationPath;

                }



                Environment.CurrentDirectory = sitePhysicalPath;

            }

            catch

            {

                // ignore

            }

        }

    }
}
