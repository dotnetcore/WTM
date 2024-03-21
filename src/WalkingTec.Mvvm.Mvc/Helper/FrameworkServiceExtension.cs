using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore.SpaServices.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.FileHandlers;
using WalkingTec.Mvvm.Core.Support.Json;
using WalkingTec.Mvvm.Mvc.Auth;
using WalkingTec.Mvvm.Mvc.Filters;
using WalkingTec.Mvvm.Mvc.Helper;
using WalkingTec.Mvvm.TagHelpers.LayUI;
using Microsoft.AspNetCore.SpaServices.Extensions;
using Microsoft.Extensions.FileProviders;
using WalkingTec.Mvvm.Core.Support.Quartz;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using WalkingTec.Mvvm.Core.WorkFlow;
using Elsa;
using Elsa.Providers.WorkflowContexts;
using Elsa.Options;
using Elsa.Persistence.EntityFramework.PostgreSql;
using Elsa.Server.Api.Mapping;
using Elsa.Server.Api.Services;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Threading.Tasks;
using DUWENINK.Captcha.DI;
using Elsa.Activities.Http;
using Elsa.Activities.Http.Services;
using Microsoft.IdentityModel.JsonWebTokens;

namespace WalkingTec.Mvvm.Mvc
{
    public static class FrameworkServiceExtension
    {

        //private static Configs _wtmconfigs;
        //private static Configs WtmConfigs
        //{
        //    get
        //    {
        //        if(_wtmconfigs == null)
        //        {
        //            ConfigurationBuilder cb = new ConfigurationBuilder();
        //            _wtmconfigs = cb.WTMConfig(null).Build().Get<Configs>();
        //        }
        //        return _wtmconfigs;
        //    }
        //}




        private static GlobalData GetGlobalData()
        {
            var gd = new GlobalData();
            gd.AllAssembly = Utils.GetAllAssembly();
            return gd;
        }

        private static List<SimpleMenu> GetAllMenus(List<SimpleModule> allModule, bool isQuickdebug, List<CS> connections)
        {
            var localizer = new ResourceManagerStringLocalizerFactory(Options.Create<LocalizationOptions>(new LocalizationOptions { ResourcesPath = "Resources" }), new Microsoft.Extensions.Logging.LoggerFactory()).Create(typeof(WalkingTec.Mvvm.Core.CoreProgram));
            var menus = new List<SimpleMenu>();

            if (isQuickdebug)
            {
                menus = new List<SimpleMenu>();
                var areas = allModule.Where(x => x.NameSpace != "WalkingTec.Mvvm.Admin.Api").Select(x => x.Area?.AreaName).Distinct().ToList();
                foreach (var area in areas)
                {
                    var modelmenu = new SimpleMenu
                    {
                        ID = Guid.NewGuid(),
                        PageName = area ?? localizer["Sys.DefaultArea"],
                        TenantAllowed = true,
                    };
                    menus.Add(modelmenu);
                    var cModules = allModule.Where(x => x.NameSpace != "WalkingTec.Mvvm.Admin.Api" && x.Area?.AreaName == area).ToList();
                    foreach (var item in cModules)
                    {
                        var pages = item.Actions.Where(x => x.MethodName.ToLower() == "index" || x.ActionDes?.IsPage == true).ToList();
                        foreach (var page in pages)
                        {
                            var url = page.Url;
                            menus.Add(new SimpleMenu
                            {
                                ID = Guid.NewGuid(),
                                ParentId = modelmenu.ID,
                                PageName = item.ActionDes == null ? item.ModuleName : item.ActionDes.Description,
                                Url = url,
                                TenantAllowed = true
                            });
                        }
                    }
                }
            }
            else
            {
                try
                {
                    using (var dc = connections.Where(x => x.Key.ToLower() == "default").FirstOrDefault().CreateDC())
                    {
                        menus.AddRange(dc?.Set<FrameworkMenu>()
                                .OrderBy(x => x.DisplayOrder)
                                .Select(x => new SimpleMenu
                                {
                                    ID = x.ID,
                                    ParentId = x.ParentId,
                                    PageName = x.PageName,
                                    Url = x.Url,
                                    DisplayOrder = x.DisplayOrder,
                                    ShowOnMenu = x.ShowOnMenu,
                                    Icon = x.Icon,
                                    IsPublic = x.IsPublic,
                                    FolderOnly = x.FolderOnly,
                                    MethodName = x.MethodName,
                                    IsInside = x.IsInside,
                                    TenantAllowed = x.TenantAllowed
                                })
                                .ToList());
                    }
                }
                catch { }
            }
            return menus;
        }

        /// <summary>
        /// 获取所有模块
        /// </summary>
        /// <param name="controllers"></param>
        /// <returns></returns>
        private static List<SimpleModule> GetAllModules(List<Type> controllers)
        {
            var modules = new List<SimpleModule>();

            foreach (var ctrl in controllers)
            {
                var pubattr1 = ctrl.GetCustomAttributes(typeof(PublicAttribute), false);
                var pubattr12 = ctrl.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                var rightattr = ctrl.GetCustomAttributes(typeof(AllRightsAttribute), false);
                var debugattr = ctrl.GetCustomAttributes(typeof(DebugOnlyAttribute), false);
                var areaattr = ctrl.GetCustomAttributes(typeof(AreaAttribute), false);
                var mainhostonlyattr = ctrl.GetCustomAttributes(typeof(MainTenantOnlyAttribute), false);
                var model = new SimpleModule
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
                if (mainhostonlyattr.Length > 0)
                {
                    model.MainHostOnly = true;
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
                model.Actions = new List<SimpleAction>();
                //循环所有方法
                foreach (var method in methods)
                {
                    var pubattr2 = method.GetCustomAttributes(typeof(PublicAttribute), false);
                    var pubattr22 = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                    var arattr2 = method.GetCustomAttributes(typeof(AllRightsAttribute), false);
                    var debugattr2 = method.GetCustomAttributes(typeof(DebugOnlyAttribute), false);
                    var postAttr = method.GetCustomAttributes(typeof(HttpPostAttribute), false);
                    var mainhostonlyattr2 = method.GetCustomAttributes(typeof(MainTenantOnlyAttribute), false);
                    //如果不是post的方法，则添加到controller的action列表里
                    if (postAttr.Length == 0)
                    {
                        var action = new SimpleAction
                        {
                            Module = model,
                            MethodName = method.Name,
                            IgnorePrivillege = model.IgnorePrivillege,
                            MainHostOnly = model.MainHostOnly
                        };
                        if (pubattr2.Length > 0 || pubattr22.Length > 0 || arattr2.Length > 0 || debugattr2.Length > 0)
                        {
                            action.IgnorePrivillege = true;
                        }
                        if (mainhostonlyattr2.Length > 0)
                        {
                            action.MainHostOnly = true;
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
                    var mainhostonlyattr2 = method.GetCustomAttributes(typeof(MainTenantOnlyAttribute), false);

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
                        var action = new SimpleAction
                        {
                            Module = model,
                            MethodName = method.Name,
                            IgnorePrivillege = model.IgnorePrivillege,
                            MainHostOnly = model.MainHostOnly
                        };
                        if (pubattr2.Length > 0 || pubattr22.Length > 0 || arattr2.Length > 0 || debugattr2.Length > 0)
                        {
                            action.IgnorePrivillege = true;
                        }
                        if (mainhostonlyattr2.Length > 0)
                        {
                            action.MainHostOnly = true;
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
                if (model.Actions != null && model.Actions.Count() > 0)
                {
                    if (areaattr.Length > 0)
                    {
                        string areaName = (areaattr[0] as AreaAttribute).RouteValue;
                        var existArea = modules.Where(x => x.Area?.AreaName == areaName).Select(x => x.Area).FirstOrDefault();
                        if (existArea == null)
                        {
                            model.Area = new SimpleArea
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
                        url = "/" + url;
                        bool needadd = false;
                        if (includeAll == true)
                        {
                            needadd = true;
                        }
                        else
                        {
                            var attrs3 = method.GetCustomAttributes(typeof(AllRightsAttribute), false);
                            var attrs4 = method.GetCustomAttributes(typeof(PublicAttribute), false);
                            var attrs42 = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                            if (attrs3.Length > 0 || attrs4.Length > 0 || attrs42.Length > 0)
                            {
                                needadd = true;
                            }
                        }
                        if (needadd)
                        {
                            rv.Add(url);
                            if (url.ToLower().EndsWith("/index"))
                            {
                                rv.Add(url[0..^6]);
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
                        url = "/" + url;
                        bool needadd = false;
                        if (includeAll == true)
                        {
                            needadd = true;
                        }
                        else
                        {
                            var attrs5 = method.GetCustomAttributes(typeof(AllRightsAttribute), false);
                            var attrs6 = method.GetCustomAttributes(typeof(PublicAttribute), false);
                            var attrs62 = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
                            if (attrs5.Length > 0 || attrs6.Length > 0 || attrs62.Length > 0)
                            {
                                needadd = true;
                            }
                        }
                        if (needadd)
                        {
                            rv.Add(url);
                            if (url.ToLower().EndsWith("/index"))
                            {
                                rv.Add(url[0..^6]);
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

        public static IServiceCollection AddWtmContext(this IServiceCollection services, IConfiguration config, Action<WtmContextOption> options = null)
        {
            var conf = config.Get<Configs>();
            WtmContextOption op = new WtmContextOption();
            options?.Invoke(op);
            services.Configure<Configs>(config);
            var gd = GetGlobalData();
            services.AddHttpContextAccessor();
            services.AddSingleton(gd);
            services.AddLayui();
            services.AddSingleton(op.DataPrivileges ?? new List<IDataPrivilege>());
            DataContextFilter._csfunc = op.CsSelector;
            WtmFileProvider._subDirFunc = op.FileSubDirSelector;
            WTMContext.ReloadUserFunc = op.ReloadUserFunc;
            services.TryAddScoped<IDataContext, NullContext>();
            services.AddScoped<WTMContext>();
            services.AddScoped<WtmFileProvider>();
            services.Configure<FormOptions>(y =>
            {
                y.ValueCountLimit = 5000;
                y.ValueLengthLimit = int.MaxValue - 20480;
                y.MultipartBodyLengthLimit = conf.FileUploadOptions.UploadLimit;
            });
            services.AddHostedService<QuartzHostService>();
            var cs = conf.Connections;
            foreach (var item in cs)
            {
                var dc = item.CreateDC();
                dc.EnsureCreate();
            }
            services.AddVersionedApiExplorer(o=>
            {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
            });
            services.AddApiVersioning(
             options =>
             {
                 options.ReportApiVersions = true;
                 options.DefaultApiVersion = ApiVersion.Default;
                 options.AssumeDefaultVersionWhenUnspecified = true;
             });

            return services;
        }

        public static IServiceCollection AddWtmWorkflow(this IServiceCollection services, IConfiguration config,string csName="default",Action<ElsaOptionsBuilder> options=null)
        {
            var elsaSection = config.GetSection("Workflow");
            var conf = config.Get<Configs>();
            services.AddSingleton<AuthenticationBasedHttpEndpointAuthorizationHandler>();

            #region Add Captcha

            services.AddDUWENINKCaptcha();//使用验证码

            #endregion

            services
                .AddElsa(elsa => {
                    var cs = conf.Connections.Where(x => x.Key == csName).FirstOrDefault();
                    switch (cs.DbType)
                    {
                        case DBTypeEnum.SqlServer:
                            var ver = 120;
                            if (string.IsNullOrEmpty(cs.Version) == false)
                            {
                                int.TryParse(cs.Version, out ver);
                            }
                            elsa.UseNonPooledEntityFrameworkPersistence(ef => ef.UseSqlServer(cs.Value, o => o.UseCompatibilityLevel(ver)));
                            break;
                        case DBTypeEnum.MySql:
                            ServerVersion sv = null;
                            if (string.IsNullOrEmpty(cs.Version) == false)
                            {
                                ServerVersion.TryParse(cs.Version, out sv);
                            }
                            if (sv == null)
                            {
                                sv = ServerVersion.AutoDetect(cs.Value);
                            }
                            elsa.UseNonPooledEntityFrameworkPersistence(ef => ef.UseMySql(cs.Value, sv));
                            break;
                        case DBTypeEnum.PgSql:
                            elsa.UseNonPooledEntityFrameworkPersistence(ef => ef.UsePostgreSql(cs.Value));
                            break;
                        case DBTypeEnum.Memory:
                            elsa.UseNonPooledEntityFrameworkPersistence(ef => ef.UseInMemoryDatabase(cs.Value));
                            break;
                        case DBTypeEnum.SQLite:
                            elsa.UseNonPooledEntityFrameworkPersistence(ef => ef.UseSqlite(cs.Value));
                            break;
                        case DBTypeEnum.Oracle:
                            elsa.UseNonPooledEntityFrameworkPersistence<WtmElsaContext>(ef => ef.UseOracle(cs.Value, op =>
                            {
                                switch (cs.Version)
                                {
                                    case "19":
                                        op.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19);
                                        break;
                                    case "21":
                                        op.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion21);
                                        break;
                                    case "23":
                                        op.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion23);
                                        break;
                                    default:
                                        op.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19);
                                        break;
                                }
                            }));
                            break;
                        case DBTypeEnum.DaMeng:
                            break;
                        case null:
                            break;
                        default:
                            break;
                    }

                    elsa
                    .AddConsoleActivities()
                    .AddActivity<WtmApproveActivity>()
                    .AddActivity<BackApproveActivity>()
                    .AddJavaScriptActivities()
                    .AddHttpActivities(x=> {
                        if (conf.Domains.ContainsKey("server")) {
                            x.BaseUrl = new Uri(conf.Domains["server"].Address);
                        }
                        x.HttpEndpointAuthorizationHandlerFactory = sp => sp.GetRequiredService<AuthenticationBasedHttpEndpointAuthorizationHandler>();
                    })
                    .AddEmailActivities(elsaSection.GetSection("Smtp").Bind)
                    .AddQuartzTemporalActivities()
                    .AddCustomTenantAccessor<ElsaTenantAccessor>();

                    options?.Invoke(elsa);
                }
                );
            services.AddElsaApiEndpoints();
            if (conf.Domains.ContainsKey("server"))
            {
                services.AddHttpClient(nameof(SendHttpRequest)).ConfigureHttpClient((s,x) =>
                {
                    x.BaseAddress = new Uri(conf.Domains["server"].Address);
                    var ss = s.CreateScope();
                    var _wtm = ss.ServiceProvider.GetRequiredService<WTMContext>();
                    x.DefaultRequestHeaders.Add("Authorization", "Bearer " + _wtm.LoginUserInfo?.RemoteToken);
                });
            }


            //        services
            //.AddSingleton<ConnectionConverter>()
            //.AddSingleton<ActivityBlueprintConverter>()
            //.AddScoped<IWorkflowBlueprintMapper, WorkflowBlueprintMapper>()
            //.AddSingleton<IEndpointContentSerializerSettingsProvider, EndpointContentSerializerSettingsProvider>()
            //.AddAutoMapperProfile<AutoMapperProfile>()
            //.AddSignalR();
            services.AddMvc(options =>
            {
                options.Conventions.Add(new MyNewtonsoftJsonConvention(null));
            });

            var allTypes = Utils.GetAllModels();

            foreach (var item in allTypes)
            {
                if (typeof(IWorkflow).IsAssignableFrom(item))
                {
                    var type = typeof(WorkflowRefresher<>).MakeGenericType(item);
                    services.AddTransient(typeof(IWorkflowContextProvider), type);
                }
            }
            services.AddBookmarkProvider<WtmApproveBookmarkProvider>();
            services.AddBookmarkProvider<BackApproveBookmarkProvider>();
            return services;
        }

        public static IServiceCollection AddWtmCrossDomain(this IServiceCollection services, IConfiguration config)
        {
            var conf = config.Get<Configs>();
            services.AddCors(options =>
            {
                if (conf.CorsOptions?.Policy?.Count > 0)
                {
                    foreach (var item in conf.CorsOptions.Policy)
                    {
                        string[] domains = item.Domain?.Split(',');
                        options.AddPolicy(item.Name,
                           builder =>
                           {
                               builder.WithOrigins(domains)
                                                   .AllowAnyHeader()
                                                   .AllowAnyMethod()
                                                   .AllowCredentials()
                                                   .WithExposedHeaders("Content-Disposition");
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
                                                .AllowCredentials()
                                                .WithExposedHeaders("Content-Disposition");
                        });
                }
            });
            return services;
        }
        public static IServiceCollection AddWtmSession(this IServiceCollection services, int timeout, IConfiguration config)
        {
            var conf = config.Get<Configs>();
            services.AddSession(options =>
            {
                options.Cookie.Name = conf.CookiePre + ".Session";
                options.IdleTimeout = TimeSpan.FromSeconds(timeout);
            });
            return services;
        }
        public static IServiceCollection AddWtmAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var conf = config.Get<Configs>();
            services.AddScoped<ITokenService, TokenService>();

            var jwtOptions = conf.JwtOptions;

            var cookieOptions = conf.CookieOptions;

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
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
                             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey)),
                             LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
                             {
                                 if (expires == null)
                                 {
                                     return true;
                                 }
                                 else
                                 {
                                     return expires.Value > DateTime.UtcNow;
                                 }
                             },
                             ValidateLifetime = true
                         };
                         options.Events = new JwtBearerEvents
                         {
                             OnMessageReceived = context =>
                             {
                                 var accessToken = context.Request.Query["access_token"];

                                 // If the request is for our hub...
                                 var path = context.HttpContext.Request.Path;
                                 if (!string.IsNullOrEmpty(accessToken))
                                 {
                                     // Read the token out of the query string
                                     context.Token = accessToken;
                                 }
                                 return Task.CompletedTask;
                             },
                             OnTokenValidated = (context) => {
                                 JsonWebToken token = context.SecurityToken as JsonWebToken;
                                 return Task.FromResult(token);
                             }
                            };
                     })
                   .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.Cookie.Name = CookieAuthenticationDefaults.CookiePrefix + conf.CookiePre + "." + AuthConstants.CookieAuthName;
                        options.Cookie.HttpOnly = true;
                        options.Cookie.SameSite = SameSiteMode.Strict;
                        options.Cookie.Domain = string.IsNullOrEmpty(cookieOptions.Domain) ? null : cookieOptions.Domain;
                        options.ClaimsIssuer = cookieOptions.Issuer;
                        options.SlidingExpiration = cookieOptions.SlidingExpiration;
                        options.ExpireTimeSpan = TimeSpan.FromSeconds(cookieOptions.Expires);
                        // options.SessionStore = new MemoryTicketStore();

                        options.LoginPath = cookieOptions.LoginPath;
                        options.LogoutPath = cookieOptions.LogoutPath;
                        options.ReturnUrlParameter = cookieOptions.ReturnUrlParameter;
                        options.AccessDeniedPath = cookieOptions.AccessDeniedPath;
                    });
            return services;
        }

        public static IServiceCollection AddWtmHttpClient(this IServiceCollection services, IConfiguration config)
        {
            var conf = config.Get<Configs>();
            services.AddHttpClient();
            if (conf.Domains != null)
            {
                foreach (var item in conf.Domains)
                {
                    services.AddHttpClient(item.Key, x =>
                    {
                        x.BaseAddress = new Uri(item.Value.Url);
                        x.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                        x.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
                    });
                }
            }
            return services;
        }

        public static IServiceCollection AddWtmSwagger(this IServiceCollection services, bool useFullName = false)
        {
            services.AddSwaggerGen(c =>
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
                c.SchemaFilter<SwaggerFilter>();
                if (useFullName == true)
                {
                    c.CustomSchemaIds(i => i.FullName);
                }
            });
            return services;
        }

        public static IServiceCollection AddWtmMultiLanguages(this IServiceCollection services, IConfiguration config, Action<WtmLocalizationOption> op = null)
        {
            var conf = config.Get<Configs>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(conf.SupportLanguages[0]);
                options.SupportedCultures = conf.SupportLanguages;
                options.SupportedUICultures = conf.SupportLanguages;
            });
            WtmLocalizationOption loc = new WtmLocalizationOption();
            op?.Invoke(loc);
            if (loc.LocalizationType != null)
            {
                services.AddSingleton<WtmLocalizationOption>(loc);
            }
            return services;
        }

        public static IMvcBuilder AddWtmDataAnnotationsLocalization(this IMvcBuilder builder, Type programType)
        {
            builder.AddDataAnnotationsLocalization(options =>
             {
                 options.DataAnnotationLocalizerProvider = (type, factory) =>
                 {
                     //if (Core.CoreProgram.Buildindll.Any(x => type.FullName.StartsWith(x)))
                     //{
                     //    return factory.Create(typeof(WalkingTec.Mvvm.Core.CoreProgram));
                     //}
                     //else
                     //{
                     return factory.Create(programType);
                     //}
                 };
             });
            return builder;
        }

        public static IApplicationBuilder UseWtmContext(this IApplicationBuilder app, bool isspa = false)
        {
            var configs = app.ApplicationServices.GetRequiredService<IOptionsMonitor<Configs>>().CurrentValue;
            var lg = app.ApplicationServices.GetRequiredService<LinkGenerator>();
            var gd = app.ApplicationServices.GetRequiredService<GlobalData>();
            var localfactory = app.ApplicationServices.GetRequiredService<IStringLocalizerFactory>();
            var lop = app.ApplicationServices.GetService<WtmLocalizationOption>();

            //获取所有程序集
            //var mvc = GetRuntimeAssembly("WalkingTec.Mvvm.Mvc");
            //if (mvc != null && gd.AllAssembly.Contains(mvc) == false)
            //{
            //    gd.AllAssembly.Add(mvc);
            //}
            //var core = GetRuntimeAssembly("WalkingTec.Mvvm.Core");
            //if (core != null && gd.AllAssembly.Contains(core) == false)
            //{
            //    gd.AllAssembly.Add(core);
            //}
            //var layui = GetRuntimeAssembly("WalkingTec.Mvvm.TagHelpers.LayUI");
            //if (layui != null && gd.AllAssembly.Contains(layui) == false)
            //{
            //    gd.AllAssembly.Add(layui);
            //}

            //set Core's _Callerlocalizer to use localizer point to the EntryAssembly's Program class
            Type programType = null;
            if (lop?.LocalizationType == null)
            {
                programType = Assembly.GetCallingAssembly()?.GetTypes()?.Where(x => x.Name == "Program").FirstOrDefault();
            }
            else
            {
                programType = lop.LocalizationType;
            }
            var programLocalizer = localfactory.Create(programType);
            Core.CoreProgram._localizer = programLocalizer;

            var controllers = gd.GetTypesAssignableFrom<IBaseController>();
            var test = app.ApplicationServices.GetService<ISpaStaticFileProvider>();
            gd.IsSpa = isspa == true || test != null;
            gd.AllModule = GetAllModules(controllers);
            var modules = Utils.ResetModule(gd.AllModule, false);
            gd.CustomUserType = gd.GetPocoTypesAssignableFrom<FrameworkUserBase>().Where(x => x.Name.ToLower() == "frameworkuser").FirstOrDefault();
            gd.SetMenuGetFunc(() =>
            {
                var menus = new List<SimpleMenu>();
                var cache = app.ApplicationServices.GetRequiredService<IDistributedCache>();
                var menuCacheKey = nameof(GlobalData.AllMenus);
                if (cache.TryGetValue(menuCacheKey, out List<SimpleMenu> rv) == false)
                {

                    var data = GetAllMenus(modules, configs.IsQuickDebug, configs.Connections);
                    cache.Add(menuCacheKey, data, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = new TimeSpan(1, 0, 0) });
                    menus = data;
                }
                else
                {
                    menus = rv;
                }

                return menus;
            });
            gd.SetTenantGetFunc(() =>
            {
                var tenants = new List<FrameworkTenant>();
                var cache = app.ApplicationServices.GetRequiredService<IDistributedCache>();
                var tenantsCacheKey = nameof(GlobalData.AllTenant);
                if (cache.TryGetValue(tenantsCacheKey, out tenants) == false)
                {
                    tenants = new List<FrameworkTenant>();
                    if (configs?.EnableTenant == true)
                    {
                        using (var dc = configs.Connections.Where(x => x.Key.ToLower() == "default").FirstOrDefault().CreateDC())
                        {
                            var cusTenantType = gd.GetPocoTypesAssignableFrom<FrameworkTenant>().FirstOrDefault();
                            if (cusTenantType != null)
                            {
                                var set = dc.GetType().GetMethod("Set", Type.EmptyTypes).MakeGenericMethod(cusTenantType);
                                var q = set.Invoke(dc, null) as IQueryable<FrameworkTenant>;
                                tenants = q.IgnoreQueryFilters().Where(x => x.Enabled).ToList();
                            }
                            var _all = dc.Set<FrameworkTenant>().IgnoreQueryFilters().Where(x => x.Enabled).ToList();
                            foreach (var item in _all)
                            {
                                if(tenants.Any(x=>x.ID == item.ID) == false)
                                {
                                    tenants.Add(item);
                                }
                            }
                            tenants = tenants.OrderBy(x => x.CreateTime).ToList();
                            foreach (var item in tenants)
                            {
                                if (string.IsNullOrEmpty(item.TDomain) == false)
                                {
                                    Regex r = new Regex("(http://|https://)?(.+?)(/)?$");
                                    var m = r.Match(item.TDomain);
                                    if (m.Success)
                                    {
                                        item.TDomain = m.Groups[2].Value;
                                    }
                                }
                                item.Attributes = new Dictionary<string, object>();
                                if (cusTenantType != null)
                                {
                                    var cuspros = cusTenantType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(x => x.PropertyType.IsListOf<TopBasePoco>() == false && typeof(TopBasePoco).IsAssignableFrom(x.PropertyType) == false).ToList();
                                    foreach (var pro in cuspros)
                                    {
                                        if (item.Attributes.ContainsKey(pro.Name) == false)
                                        {
                                            try
                                            {
                                                item.Attributes.Add(pro.Name, pro.GetValue(item));
                                            }
                                            catch { }
                                        }
                                    }

                                }

                            }
                        }
                    }
                    if (tenants == null)
                    {
                        tenants = new List<FrameworkTenant>();
                    }
                    cache.Add(tenantsCacheKey, tenants, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = new TimeSpan(1, 0, 0) });
                }
                return tenants;
            });
            foreach (var m in gd.AllModule)
            {
                if (isspa == false && m.IsApi == true)
                {
                    if (m.ModuleName.ToLower().EndsWith("api") == false)
                    {
                        m.ModuleName += "Api";
                    }
                }
                foreach (var a in m.Actions)
                {
                    string u = null;
                    if (a.ParasToRunTest != null && a.ParasToRunTest.Any(x => x.ToLower() == "id"))
                    {
                        u = lg.GetPathByAction(a.MethodName, m.ClassName, new { id = 0, area = m.Area?.AreaName });
                    }
                    else
                    {
                        u = lg.GetPathByAction(a.MethodName, m.ClassName, new { area = m.Area?.AreaName });
                    }
                    if (u != null && (u.EndsWith("/0")))
                    {
                        u = u.Substring(0, u.Length - 2);
                        if (m.IsApi == true)
                        {
                            u = u + "/{id}";
                        }
                    }
                    if (u != null && (u.ToLower().EndsWith("?id=0")))
                    {
                        u = u[0..^5];
                        if (m.IsApi == true)
                        {
                            u = u + "/{id}";
                        }
                    }

                    a.Url = u;
                }
            }

            gd.AllAccessUrls = gd.AllModule.SelectMany(x => x.Actions).Where(x => x.IgnorePrivillege == true || x.Module.IgnorePrivillege == true).Select(x => x.Url).ToList();
            gd.AllMainTenantOnlyUrls = gd.AllModule.SelectMany(x => x.Actions).Where(x => x.MainHostOnly == true || x.Module.MainHostOnly == true).Select(x => x.Url).ToList();
            WtmFileProvider.Init(configs, gd);
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var fixdc = scope.ServiceProvider.GetRequiredService<IDataContext>();
                if (fixdc is NullContext)
                {
                    var cs = configs.Connections;
                    foreach (var item in cs)
                    {
                        var dc = item.CreateDC();
                        dc.DataInit(gd.AllModule, isspa == true || test != null).Wait();
                    }
                }
                else
                {
                    fixdc.DataInit(gd.AllModule, isspa == true || test != null).Wait();
                }

            }
            return app;
        }
        public static IApplicationBuilder UseWtmMultiLanguages(this IApplicationBuilder app)
        {
            var configs = app.ApplicationServices.GetRequiredService<IOptionsMonitor<Configs>>().CurrentValue;
            if (string.IsNullOrEmpty(configs.Languages) == false)
            {
                app.UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture(configs.SupportLanguages[0]),
                    SupportedCultures = configs.SupportLanguages,
                    SupportedUICultures = configs.SupportLanguages
                });
                System.Threading.Thread.CurrentThread.CurrentCulture = configs.SupportLanguages[0];
                System.Threading.Thread.CurrentThread.CurrentUICulture = configs.SupportLanguages[0];
            }
            return app;
        }
        public static IApplicationBuilder UseWtmCrossDomain(this IApplicationBuilder app)
        {
            var configs = app.ApplicationServices.GetRequiredService<IOptionsMonitor<Configs>>().CurrentValue;
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
            return app;
        }

        public static IApplicationBuilder UseWtmStaticFiles(this IApplicationBuilder app)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = new PathString("/_js"),
                FileProvider = new EmbeddedFileProvider(
                    typeof(_CodeGenController).GetTypeInfo().Assembly,
                    "WalkingTec.Mvvm.Mvc")
            });
            return app;
        }


        public static IApplicationBuilder UseWtmSwagger(this IApplicationBuilder app, bool showInDebugOnly = true)
        {
            var configs = app.ApplicationServices.GetRequiredService<IOptions<Configs>>().Value;
            if (configs.IsQuickDebug == true || showInDebugOnly == false)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }
            return app;
        }

        public static IApplicationBuilder UseReact(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetService<IWebHostEnvironment>();
            app.UseSpaStaticFiles();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

            return app;
        }

    }


}
