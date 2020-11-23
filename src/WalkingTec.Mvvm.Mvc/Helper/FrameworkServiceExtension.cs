using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SpaServices.StaticFiles;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth;
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

            var configBuilder = new ConfigurationBuilder();

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")))
            {
                var binLocation = Assembly.GetEntryAssembly()?.Location;
                if (!string.IsNullOrEmpty(binLocation))
                {
                    var binPath = new FileInfo(binLocation).Directory?.FullName;
                    if (File.Exists(Path.Combine(binPath, "appsettings.json")))
                    {
                        Directory.SetCurrentDirectory(binPath);
                        configBuilder.SetBasePath(binPath)
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables();
                    }
                }
            }
            else
            {
                configBuilder.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            }

            if (webHostBuilderContext != null)
            {
                var env = webHostBuilderContext.HostingEnvironment;
                configBuilder
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            }
            var config = configBuilder.Build();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            var gd = GetGlobalData();
            var con = config.Get<Configs>() ?? new Configs();
            //services.Configure<Configs>(config);
            if (dataPrivilegeSettings != null)
            {
                con.DataPrivilegeSettings = dataPrivilegeSettings;
            }
            else
            {
                con.DataPrivilegeSettings = new List<IDataPrivilege>();
            }
            SetDbContextCI(gd.AllAssembly, con);
            gd.AllModels = GetAllModels(con);
            services.AddSingleton(gd);
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

            services.AddCors(options =>
            {
                if (con.CorsOptions?.Policy?.Count > 0)
                {
                    foreach (var item in con.CorsOptions.Policy)
                    {
                        string[] domains = item.Domain?.Split(',');
                        options.AddPolicy(item.Name,
                           builder =>
                           {
                               builder.WithOrigins(domains)
                                                   .AllowAnyHeader()
                                                   .AllowAnyMethod()
                                                   .AllowCredentials();
                           });
                    }
                }
                else
                {
                    options.AddPolicy("_donotusedefault",
                        builder =>
                        {
                            builder.SetIsOriginAllowed((a) => true)
                                              .AllowAnyHeader()
                                              .AllowAnyMethod()
                                              .AllowCredentials();
                        });
                }
            });


            // edit start by @vito
            services.TryAdd(ServiceDescriptor.Transient<IAuthorizationService, WTMAuthorizationService>());
            services.TryAdd(ServiceDescriptor.Transient<IPolicyEvaluator, Core.Auth.PolicyEvaluator>());
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IApplicationModelProvider, Core.Auth.AuthorizationApplicationModelProvider>());
            // edit end

            var mvc = gd.AllAssembly.Where(x => x.ManifestModule.Name == "WalkingTec.Mvvm.Mvc.dll").FirstOrDefault();
            var admin = gd.AllAssembly.Where(x => x.ManifestModule.Name == "WalkingTec.Mvvm.Mvc.Admin.dll").FirstOrDefault();

            //set Core's _Callerlocalizer to use localizer point to the EntryAssembly's Program class
            var programType = Assembly.GetEntryAssembly().GetTypes().Where(x => x.Name == "Program").FirstOrDefault();
            var coredll = gd.AllAssembly.Where(x => x.ManifestModule.Name == "WalkingTec.Mvvm.Core.dll").FirstOrDefault();
            var programLocalizer = new ResourceManagerStringLocalizerFactory(
                                        Options.Create(
                                            new LocalizationOptions
                                            {
                                                ResourcesPath = "Resources"
                                            })
                                            , new Microsoft.Extensions.Logging.LoggerFactory()
                                        )
                                        .Create(programType);
            coredll.GetType("WalkingTec.Mvvm.Core.Program").GetProperty("_Callerlocalizer").SetValue(null, programLocalizer);


            services.AddMvc(options =>
            {
                // ModelBinderProviders
                options.ModelBinderProviders.Insert(0, new StringBinderProvider());

                // Filters
                //options.Filters.Add(new AuthorizeFilter());
                options.Filters.Add(new DataContextFilter(CsSector));
                options.Filters.Add(new PrivilegeFilter());
                options.Filters.Add(new FrameworkFilter());

                options.ModelBindingMessageProvider.SetValueIsInvalidAccessor((x) => Core.Program._localizer["ValueIsInvalidAccessor",x]);
                options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x,y) => Core.Program._localizer["AttemptedValueIsInvalidAccessor",x,y]);
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor((x) => Core.Program._localizer["ValueIsInvalidAccessor", x]);
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
            })
            .AddDataAnnotationsLocalization(options =>
            {
                var coreType = coredll?.GetTypes().Where(x => x.Name == "Program").FirstOrDefault();
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    if (Core.Program.Buildindll.Any(x => type.FullName.StartsWith(x)))
                    {
                        var rv = factory.Create(coreType);
                        return rv;
                    }
                    else
                    {
                        return factory.Create(programType);
                    }
                };
            })
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);


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

            #region CookieWithJwtAuth

            // services.AddSingleton<UserStore>();
            services.AddScoped<ITokenService, TokenService>();

            var jwtOptions = config.GetSection("JwtOptions").Get<JwtOptions>();
            if (jwtOptions == null)
            {
                jwtOptions = new JwtOptions();
            }
            services.Configure<JwtOptions>(config.GetSection("JwtOptions"));

            var cookieOptions = config.GetSection("CookieOptions").Get<Core.Auth.CookieOptions>();
            if (cookieOptions == null)
            {
                cookieOptions = new Core.Auth.CookieOptions();
            }

            services.Configure<Core.Auth.CookieOptions>(config.GetSection("CookieOptions"));

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.Cookie.Name = CookieAuthenticationDefaults.CookiePrefix + AuthConstants.CookieAuthName;
                        options.Cookie.HttpOnly = true;
                        options.Cookie.SameSite = SameSiteMode.Strict;

                        options.ClaimsIssuer = cookieOptions.Issuer;
                        options.SlidingExpiration = cookieOptions.SlidingExpiration;
                        options.ExpireTimeSpan = TimeSpan.FromSeconds(cookieOptions.Expires);
                        // options.SessionStore = new MemoryTicketStore();

                        options.LoginPath = cookieOptions.LoginPath;
                        options.LogoutPath = cookieOptions.LogoutPath;
                        options.ReturnUrlParameter = cookieOptions.ReturnUrlParameter;
                        options.AccessDeniedPath = cookieOptions.AccessDeniedPath;
                    })
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            NameClaimType = AuthConstants.JwtClaimTypes.Name,
                            RoleClaimType = AuthConstants.JwtClaimTypes.Role,

                            ValidateIssuer = true,
                            ValidIssuer = jwtOptions.Issuer,

                            ValidateAudience = true,
                            ValidAudience = jwtOptions.Audience,

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = jwtOptions.SymmetricSecurityKey,

                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.FromSeconds(1)
                        };
                    });
            #endregion

            services.AddHttpClient();
            if(con.Domains != null)
            {
                foreach (var item in con.Domains)
                {
                    services.AddHttpClient(item.Key, x => {
                        x.BaseAddress = new Uri(item.Value.Url);
                        x.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                        x.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
                    }).ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        return new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip, UseProxy = false, UseCookies = false };
                    });
                }
            }

            List<CultureInfo> supportedCultures = new List<CultureInfo>();
            var lans = con.Languages.Split(",");
            foreach (var lan in lans)
            {
                supportedCultures.Add(new CultureInfo(lan));
            }

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(supportedCultures[0]);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            GlobalServices.SetServiceProvider(services.BuildServiceProvider());
            return services;
        }

        public static IApplicationBuilder UseFrameworkService(this IApplicationBuilder app, Action<IRouteBuilder> customRoutes = null)
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
                app.UseRequestLocalization();
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
                        //if (m.IsApi == true)
                        //{
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
                        //}
                    }

                    var test = app.ApplicationServices.GetService<ISpaStaticFileProvider>();
                    var cs = configs.ConnectionStrings;
                    foreach (var item in cs)
                    {
                        try
                        {
                            var dc = item.CreateDC();
                            dc.DataInit(gd.AllModule, test != null).Wait();
                        }
                        catch {
                            int a = 0;
                        }
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
            var core = GetRuntimeAssembly("WalkingTec.Mvvm.Core");
            if (core != null && gd.AllAssembly.Contains(core) == false)
            {
                gd.AllAssembly.Add(core);
            }
            var layui = GetRuntimeAssembly("WalkingTec.Mvvm.TagHelpers.LayUI");
            if (layui != null && gd.AllAssembly.Contains(layui) == false)
            {
                gd.AllAssembly.Add(layui);
            }
            var controllers = GetAllControllers(gd.AllAssembly);
            gd.AllAccessUrls = GetAllAccessUrls(controllers);

            gd.SetModuleGetFunc(() =>
            {

                return GetAllModules(controllers);
            });

            gd.SetMenuGetFunc(() =>
            {
                var menus = new List<FrameworkMenu>();
                var cache = GlobalServices.GetService<IDistributedCache>();
                var menuCacheKey = "FFMenus";
                if (cache.TryGetValue(menuCacheKey, out List<FrameworkMenu> rv) == false)
                {
                    var data = GetAllMenus(gd.AllModule);
                    cache.Add(menuCacheKey, data);
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

        private static List<FrameworkMenu> GetAllMenus(List<FrameworkModule> allModule)
        {
            var ConfigInfo = GlobalServices.GetService<Configs>();
            var localizer = GlobalServices.GetService<IStringLocalizer<WalkingTec.Mvvm.Core.Program>>();
            var menus = new List<FrameworkMenu>();

            if (ConfigInfo.IsQuickDebug)
            {
                menus = new List<FrameworkMenu>();
                var areas = allModule.Where(x => x.NameSpace != "WalkingTec.Mvvm.Admin.Api").Select(x => x.Area?.AreaName).Distinct().ToList();
                foreach (var area in areas)
                {
                    var modelmenu = new FrameworkMenu
                    {
                        ID = Guid.NewGuid(),
                        PageName = area ?? localizer["DefaultArea"]
                    };
                    menus.Add(modelmenu);
                    var pages = allModule.Where(x => x.NameSpace != "WalkingTec.Mvvm.Admin.Api" && x.Area?.AreaName == area).SelectMany(x => x.Actions).Where(x => x.MethodName.ToLower() == "index").ToList();
                    foreach (var page in pages)
                    {
                        var url = page.Url;
                        menus.Add(new FrameworkMenu
                        {
                            ID = Guid.NewGuid(),
                            ParentId = modelmenu.ID,
                            PageName = page.Module.ActionDes == null ? page.Module.ModuleName : page.Module.ActionDes.Description,
                            Url = url
                        }) ;
                    }
                }
            }
            else
            {
                try
                {
                    using (var dc = ConfigInfo.ConnectionStrings.Where(x => x.Key.ToLower() == "default").FirstOrDefault().CreateDC())
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
        private static void SetDbContextCI(List<Assembly> AllAssembly, Configs con)
        {
            List<ConstructorInfo> cis = new List<ConstructorInfo>();
            foreach (var ass in AllAssembly)
            {
                try
                {
                    var t = ass.GetExportedTypes().Where(x => typeof(DbContext).IsAssignableFrom(x) && x.Name != "DbContext" && x.Name != "FrameworkContext" && x.Name != "EmptyContext").ToList();
                    foreach (var st in t)
                    {
                        var ci = st.GetConstructor(new Type[] { typeof(CS) });
                        if (ci != null)
                        {
                            cis.Add(ci);
                        }
                    }
                }
                catch { }
            }
            foreach (var item in con.ConnectionStrings)
            {
                string dcname = item.DbContext;
                if (string.IsNullOrEmpty(dcname))
                {
                    dcname = "DataContext";
                }
                item.DcConstructor = cis.Where(x => x.DeclaringType.Name.ToLower() == dcname.ToLower()).FirstOrDefault();
                if (item.DcConstructor == null)
                {
                    item.DcConstructor = cis.FirstOrDefault();
                }
                if (item.DbType == null)
                {
                    item.DbType = con.DbType;
                }
            }
        }

        /// <summary>
        /// 获取所有 Model
        /// </summary>
        /// <returns></returns>
        private static List<Type> GetAllModels(Configs con)
        {
            var models = new List<Type>();

            //获取所有模型
            var pros = con.ConnectionStrings.SelectMany(x => x.DcConstructor.DeclaringType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance));
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
                var pubattr1 = ctrl.GetCustomAttributes(typeof(PublicAttribute), false);
                var pubattr12 = ctrl.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
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
                if (pubattr1.Length > 0 || pubattr12.Length > 0 || rightattr.Length > 0 || debugattr.Length > 0)
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
                    var nameKey = ada.GetDescription(ctrl);
                    model.ModuleName = nameKey;
                    ada.SetLoccalizer(ctrl);
                    model.ActionDes = ada;
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
                    var pubattr22 = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
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
                        if (pubattr2.Length > 0 || pubattr22.Length > 0 || arattr2.Length > 0 || debugattr2.Length > 0)
                        {
                            action.IgnorePrivillege = true;
                        }

                        var attrs2 = method.GetCustomAttributes(typeof(ActionDescriptionAttribute), false);
                        if (attrs2.Length > 0)
                        {
                            var ada = attrs2[0] as ActionDescriptionAttribute;
                            ada.SetLoccalizer(ctrl);
                            action.ActionDes = ada;
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
                    var pubattr22 = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
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
                        if (pubattr2.Length > 0 || pubattr22.Length > 0 || arattr2.Length > 0 || debugattr2.Length > 0)
                        {
                            action.IgnorePrivillege = true;
                        }
                        var attrs2 = method.GetCustomAttributes(typeof(ActionDescriptionAttribute), false);
                        if (attrs2.Length > 0)
                        {
                            var ada = attrs2[0] as ActionDescriptionAttribute;
                            ada.SetLoccalizer(ctrl);
                            action.ActionDes = ada;
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
                var attrs22 = ctrl.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                var areaAttr = ctrl.GetCustomAttribute(typeof(AreaAttribute), false);
                if (areaAttr != null)
                {
                    area = (areaAttr as AreaAttribute).RouteValue;
                }
                if (attrs.Length > 0 || attrs2.Length > 0 || attrs22.Length > 0)
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
                            var attrs42 = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                            if (attrs3.Length > 0 || attrs4.Length > 0 || attrs42.Length > 0)
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
                            var attrs62 = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                            if (attrs5.Length > 0 || attrs6.Length > 0 || attrs62.Length > 0)
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
