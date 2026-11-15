using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DUWENINK.Captcha.DI;
using Elsa;
using Elsa.Activities.Http;
using Elsa.Activities.Http.Services;
using Elsa.Options;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.PostgreSql;
using Elsa.Providers.WorkflowContexts;
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
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.FileHandlers;
using WalkingTec.Mvvm.Core.Support.Json;
using WalkingTec.Mvvm.Core.Support.Quartz;
using WalkingTec.Mvvm.Core.WorkFlow;
using WalkingTec.Mvvm.Mvc.Auth;
using WalkingTec.Mvvm.Mvc.Filters;
using WalkingTec.Mvvm.Mvc.Helper;
using WalkingTec.Mvvm.TagHelpers.LayUI;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// WTM 框架服务注册与中间件配置核心扩展类，
    /// 提供 AddWtmContext、AddWtmWorkflow、AddWtmAuthentication 等服务注册方法，
    /// 以及 UseWtmContext、UseWtmCrossDomain 等中间件配置方法
    /// </summary>
    public static class FrameworkServiceExtension
    {

        /// <summary>
        /// 创建框架全局数据对象并加载所有程序集
        /// </summary>
        /// <returns>全局数据对象</returns>
        private static GlobalData GetGlobalData()
        {
            var gd = new GlobalData();
            gd.AllAssembly = Utils.GetAllAssembly();
            return gd;
        }

        /// <summary>
        /// 获取系统所有菜单：快速调试模式下按模块和区域自动生成菜单，
        /// 否则从默认连接的数据库中读取 FrameworkMenu 表
        /// </summary>
        /// <param name="allModule">所有模块列表</param>
        /// <param name="isQuickdebug">是否为快速调试模式</param>
        /// <param name="connections">所有数据库连接配置</param>
        /// <returns>菜单列表</returns>
        private static List<SimpleMenu> GetAllMenus(List<SimpleModule> allModule, bool isQuickdebug, List<CS> connections)
        {
            // 创建本地化工厂，用于默认区域名称等菜单文本的本地化
            var localizer = new ResourceManagerStringLocalizerFactory(Options.Create<LocalizationOptions>(new LocalizationOptions { ResourcesPath = "Resources" }), new Microsoft.Extensions.Logging.LoggerFactory()).Create(typeof(WalkingTec.Mvvm.Core.CoreProgram));
            var menus = new List<SimpleMenu>();

            // 快速调试模式：按区域（Area）和页面 Action 自动生成菜单
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
                // 正常模式：从默认连接的数据库中读取 FrameworkMenu 表生成菜单
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

        /// <summary>
        /// 按名称从依赖上下文中定位并加载运行时程序集
        /// </summary>
        /// <param name="name">程序集名称</param>
        /// <returns>加载的程序集，找不到时返回 null</returns>
        private static Assembly GetRuntimeAssembly(string name)
        {
            var path = Assembly.GetEntryAssembly().Location;
            var library = DependencyContext.Default.RuntimeLibraries.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();
            if (library == null)
            {
                return null;
            }
            // 依次从应用基目录、引用程序集路径和 NuGet 包目录解析程序集物理路径
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

        /// <summary>
        /// 注册 WTM 框架核心服务：读取配置、初始化全局数据、注册 WTMContext、数据权限、
        /// 文件处理、表单选项和 Quartz 后台任务，按配置初始化各数据库连接，
        /// 并注册大模型、知识库和嵌入等 AI 相关服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="config">应用配置</param>
        /// <param name="options">框架上下文配置委托，可设置连接串选择器、数据权限等</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddWtmContext(this IServiceCollection services, IConfiguration config, Action<WtmContextOption> options = null)
        {
            services.AddDUWENINKCaptcha();//使用验证码
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
            // 默认注册空实现的 DataContext，具体数据库上下文由框架按连接配置创建
            services.TryAddScoped<IDataContext, NullContext>();
            services.AddScoped<WTMContext>();
            services.AddScoped<WtmFileProvider>();
            // 放宽表单绑定限制，支持大量表单项和大文件上传
            services.Configure<FormOptions>(y =>
            {
                y.ValueCountLimit = 5000;
                y.ValueLengthLimit = int.MaxValue - 20480;
                y.MultipartBodyLengthLimit = conf.FileUploadOptions.UploadLimit;
            });
            services.AddHostedService<QuartzHostService>();
            // 遍历所有配置的连接，创建数据库并初始化表结构
            var cs = conf.Connections;
            foreach (var item in cs)
            {
                var dc = item.CreateDC();
                dc.EnsureCreate();
            }

            // Register apiVersion route constraint for Elsa workflow API endpoints
            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("apiVersion", typeof(ApiVersionRouteConstraint));
            });

            // 大模型对话服务
            if (conf.Llm?.Enabled == true)
            {
                services.AddSingleton(new LlmService(conf.Llm));
            }

            // AI代码功能：初始化执行超时配置
            WalkingTec.Mvvm.Core.Support.AiCode.AiCodeRunner.TimeoutMs = conf.AiCode?.TimeoutSeconds * 1000 ?? 5000;

            // 知识库后台处理服务
            services.AddHostedService<KnowledgeProcessingHostedService>();

            // 嵌入服务 — 根据 EmbeddingOptions 配置选择
            if (conf.EmbeddingOptions != null && !string.IsNullOrEmpty(conf.EmbeddingOptions.ApiKey))
            {
                services.AddScoped<IEmbeddingService>(sp => new OpenAIEmbeddingService(conf.EmbeddingOptions));
            }
            else
            {
                services.AddScoped<IEmbeddingService, BuiltInEmbeddingService>();
            }

            return services;
        }

        /// <summary>
        /// 注册 WTM 工作流（Elsa）相关服务：按指定连接配置 Elsa 持久化数据库，
        /// 注册控制台、审批、脚本、HTTP、邮件、定时等活动，
        /// 并为所有工作流类注册上下文提供程序和审批书签提供程序
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="config">应用配置</param>
        /// <param name="csName">工作流持久化使用的连接串名称，默认为 default</param>
        /// <param name="options">Elsa 配置委托，可追加自定义配置</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddWtmWorkflow(this IServiceCollection services, IConfiguration config,string csName="default",Action<ElsaOptionsBuilder> options=null)
        {
            var elsaSection = config.GetSection("Workflow");
            var conf = config.Get<Configs>();
            // 注册基于认证的 HTTP 端点授权处理器，供 Elsa 工作流 HTTP 活动使用
            services.AddSingleton<AuthenticationBasedHttpEndpointAuthorizationHandler>();

            services
                .AddElsa(elsa => {
                    var cs = conf.Connections.Where(x => x.Key == csName).FirstOrDefault();
                    // 根据连接的数据库类型选择对应的 Elsa EF Core 持久化提供程序
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
                        // Oracle 使用自定义的 WtmElsaContext（不使用独立 Schema），并按配置版本设置 SQL 兼容级别
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
                            // Elsa does not have an official DaMeng persistence package.
                            // DaMeng EF Core is registered in DataContext.OnConfiguring for the main app.
                            break;
                        case null:
                            break;
                        default:
                            break;
                    }

                    // 注册各类工作流活动：控制台、审批、JavaScript 脚本、HTTP、邮件、Quartz 定时，并使用自定义租户访问器
                    elsa
                    .AddConsoleActivities()
                    .AddActivity<WtmApproveActivity>()
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
            // 注册 Elsa 的 API 端点
            services.AddElsaApiEndpoints();
            // 为工作流 HTTP 活动注册指向主站的 HttpClient，并附加当前用户的远程令牌
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


            services.AddMvc(options =>
            {
                options.Conventions.Add(new MyNewtonsoftJsonConvention(null));
            });

            // 为所有实现 IWorkflow 的工作流类注册对应的 WorkflowRefresher 上下文提供程序
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

            return services;
        }

        /// <summary>
        /// 判断密钥是否为框架模板默认密钥（含按框架规则补位到32位后的比较）。
        /// 默认密钥公开在开源代码中，任何人可用其伪造JWT，正式环境必须更换
        /// </summary>
        /// <param name="key">配置中的密钥</param>
        /// <returns>是默认密钥或空密钥返回true</returns>
        private static bool IsKnownDefaultKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false; //空密钥由配置绑定时的其他校验处理，这里只管已知默认值
            //按 JwtOptions.SecurityKey 的补位规则补齐后比较
            var padded = key.PadRight(32, 'x');
            return padded == "superSecretKey@345xxxxxxxxxxxxxx" //模板appsettings默认值
                || padded == "wtmwtmwtmwtmwtmwtmxxxxxxxxxxxxxx"; //JwtOption属性默认值
        }

        /// <summary>
        /// 注册跨域（CORS）服务：按配置中的策略列表注册各域名的跨域策略，
        /// 未配置策略时注册允许所有来源的默认策略
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="config">应用配置</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddWtmCrossDomain(this IServiceCollection services, IConfiguration config)
        {
            var conf = config.Get<Configs>();
            services.AddCors(options =>
            {
                // 配置了跨域策略时按策略名逐个注册；否则注册允许任意来源的默认策略
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
                            //注意：这里故意不调用 AllowCredentials —— 反射任意Origin（SetIsOriginAllowed=true）
                            //与允许凭据组合等于向任意网站开放带Cookie/凭据的跨域读取（绕过同源策略窃取数据）。
                            //需要带凭据的跨域请配置 CorsOptions.Policy 白名单（主子站iframe嵌套不受影响，
                            //iframe属页面导航不受CORS管辖；子站页面在iframe内调用自身API为同源请求）
                            builder.SetIsOriginAllowed((a) => true)
                                                .AllowAnyHeader()
                                                .AllowAnyMethod()
                                                .WithExposedHeaders("Content-Disposition");
                        });
                }
            });
            return services;
        }
        /// <summary>
        /// 注册 Session 服务，并按配置设置会话 Cookie 名称和空闲超时时间
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="timeout">会话空闲超时时间（秒）</param>
        /// <param name="config">应用配置</param>
        /// <returns>服务集合</returns>
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
        /// <summary>
        /// 注册框架认证服务：默认使用 Cookie 认证，同时支持 JwtBearer 认证，
        /// 并按配置完成令牌校验参数和 Cookie 选项的设置
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="config">应用配置</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddWtmAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var conf = config.Get<Configs>();
            services.AddScoped<ITokenService, TokenService>();

            var jwtOptions = conf.JwtOptions;

            var cookieOptions = conf.CookieOptions;

            //安全检查：正式模式（非QuickDebug）下拒绝使用框架模板默认密钥。
            //默认密钥及其补位规则公开在开源代码中，攻击者可用其伪造任意用户身份的JWT（含永不过期的管理员token），
            //历史上曾以此实现零密码完全接管
            if (conf.IsQuickDebug == false)
            {
                if (IsKnownDefaultKey(jwtOptions?.SecurityKey))
                    throw new InvalidOperationException("JwtOptions.SecurityKey 使用了框架默认密钥，正式环境（IsQuickDebug=false）必须在 appsettings.json 中改为自定义的强密钥后再启动");
            }

            // 清除 JWT 入站声明类型的默认映射，保留令牌中的原始声明名称
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                     .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                     {
                         // 配置 JWT 校验参数：颁发者、受众、签名密钥以及生命周期校验
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
                         // 配置 JwtBearer 事件：支持从查询串读取令牌（如 WebSocket 场景），令牌验证通过后原样返回
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
                   // 配置 Cookie 认证：Cookie 名称、域、过期时间、滑动过期以及登录、登出、拒绝访问路径
                   .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.Cookie.Name = CookieAuthenticationDefaults.CookiePrefix + conf.CookiePre + "." + AuthConstants.CookieAuthName;
                        options.Cookie.HttpOnly = true;
                        options.Cookie.SameSite = SameSiteMode.Strict;
                        options.Cookie.Domain = string.IsNullOrEmpty(cookieOptions.Domain) ? null : cookieOptions.Domain;
                        options.ClaimsIssuer = cookieOptions.Issuer;
                        options.SlidingExpiration = cookieOptions.SlidingExpiration;
                        options.ExpireTimeSpan = TimeSpan.FromSeconds(cookieOptions.Expires);

                        options.LoginPath = cookieOptions.LoginPath;
                        options.LogoutPath = cookieOptions.LogoutPath;
                        options.ReturnUrlParameter = cookieOptions.ReturnUrlParameter;
                        options.AccessDeniedPath = cookieOptions.AccessDeniedPath;
                    });
            return services;
        }

        /// <summary>
        /// 注册 HttpClient 工厂，并为配置文件中的每个域名（Domains）注册对应的命名 HttpClient
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="config">应用配置</param>
        /// <returns>服务集合</returns>
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

        /// <summary>
        /// 注册 Swagger 文档生成服务，包含 JWT Bearer 安全定义和框架自定义的 Schema 过滤器
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="useFullName">是否使用类型的全名作为 SchemaId，避免不同命名空间的同名类冲突</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddWtmSwagger(this IServiceCollection services, bool useFullName = false)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // 配置 JWT Bearer 认证的安全定义与全局安全要求
                var securityScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Bearer"
                };
                c.AddSecurityDefinition("Bearer", securityScheme);

                var sr = new OpenApiSecurityRequirement();
                sr[new OpenApiSecuritySchemeReference("Bearer", null, null)] = new List<string>();
                c.AddSecurityRequirement(doc => sr);

                c.SchemaFilter<SwaggerFilter>();
                if (useFullName == true)
                {
                    c.CustomSchemaIds(i => i.FullName);
                }
            });
            return services;
        }

        /// <summary>
        /// 注册多语言服务：设置资源路径、默认文化与支持的文化列表，并可指定自定义本地化资源类型
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="config">应用配置</param>
        /// <param name="op">本地化选项配置委托</param>
        /// <returns>服务集合</returns>
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

        /// <summary>
        /// 为 MVC 启用数据注解本地化，并将所有注解的本地化资源统一指向指定类型所在的资源
        /// </summary>
        /// <param name="builder">MVC 构建器</param>
        /// <param name="programType">用于定位本地化资源的类型，通常为主程序的 Program 类</param>
        /// <returns>MVC 构建器</returns>
        public static IMvcBuilder AddWtmDataAnnotationsLocalization(this IMvcBuilder builder, Type programType)
        {
            builder.AddDataAnnotationsLocalization(options =>
             {
                 options.DataAnnotationLocalizerProvider = (type, factory) =>
                 {
                     return factory.Create(programType);
                 };
             });
            return builder;
        }

        /// <summary>
        /// 初始化 WTM 框架运行环境：设置核心本地化、扫描控制器生成模块与 URL、
        /// 配置菜单和租户的缓存获取函数、汇总免权限地址、初始化文件提供程序，并执行数据库数据初始化
        /// </summary>
        /// <param name="app">应用构建器</param>
        /// <param name="isspa">是否为 SPA（单页应用）模式</param>
        /// <returns>应用构建器</returns>
        public static IApplicationBuilder UseWtmContext(this IApplicationBuilder app, bool isspa = false)
        {
            var configs = app.ApplicationServices.GetRequiredService<IOptionsMonitor<Configs>>().CurrentValue;
            var lg = app.ApplicationServices.GetRequiredService<LinkGenerator>();
            var gd = app.ApplicationServices.GetRequiredService<GlobalData>();
            var localfactory = app.ApplicationServices.GetRequiredService<IStringLocalizerFactory>();
            var lop = app.ApplicationServices.GetService<WtmLocalizationOption>();

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
            // 设置菜单获取函数：优先读取分布式缓存，未命中时按当前模式生成菜单并缓存一小时
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
            // 设置租户获取函数：启用多租户时从默认数据库读取启用的租户并缓存一小时
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
                            // 支持自定义租户类型：把自定义租户及其扩展属性一并读入
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
            // 为所有模块的 Action 生成访问地址，并处理带 id 参数和可选参数路由的情况
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

            // 汇总免权限验证的地址和仅限主站（主租户）访问的地址
            gd.AllAccessUrls = gd.AllModule.SelectMany(x => x.Actions).Where(x => x.IgnorePrivillege == true || x.Module.IgnorePrivillege == true).Select(x => x.Url).ToList();
            gd.AllMainTenantOnlyUrls = gd.AllModule.SelectMany(x => x.Actions).Where(x => x.MainHostOnly == true || x.Module.MainHostOnly == true).Select(x => x.Url).ToList();
            WtmFileProvider.Init(configs, gd);
            // 执行框架数据初始化；若容器中注册的是 NullContext，则逐个连接分别执行初始化
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
        /// <summary>
        /// 启用请求本地化中间件：按配置设置默认文化与支持的文化，并将当前线程文化设为默认文化
        /// </summary>
        /// <param name="app">应用构建器</param>
        /// <returns>应用构建器</returns>
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
        /// <summary>
        /// 启用跨域中间件，仅在配置允许跨域时按策略挂载 CORS
        /// </summary>
        /// <param name="app">应用构建器</param>
        /// <returns>应用构建器</returns>
        public static IApplicationBuilder UseWtmCrossDomain(this IApplicationBuilder app)
        {
            var configs = app.ApplicationServices.GetRequiredService<IOptionsMonitor<Configs>>().CurrentValue;
            // 启用跨域时挂载 CORS 中间件：有配置策略时使用第一个策略，否则使用默认策略
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

        /// <summary>
        /// 启用框架内置静态文件服务，将 Mvc 程序集的内嵌资源映射到 /_js 虚拟路径
        /// </summary>
        /// <param name="app">应用构建器</param>
        /// <returns>应用构建器</returns>
        public static IApplicationBuilder UseWtmStaticFiles(this IApplicationBuilder app)
        {
            // 将 Mvc 程序集中的内嵌资源映射到 /_js 虚拟路径，提供框架内置静态文件
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = new PathString("/_js"),
                FileProvider = new EmbeddedFileProvider(
                    typeof(_CodeGenController).GetTypeInfo().Assembly,
                    "WalkingTec.Mvvm.Mvc")
            });
            return app;
        }


        /// <summary>
        /// 启用 Swagger 中间件和 SwaggerUI 页面
        /// </summary>
        /// <param name="app">应用构建器</param>
        /// <param name="showInDebugOnly">是否仅在快速调试模式下显示，默认为 true</param>
        /// <returns>应用构建器</returns>
        public static IApplicationBuilder UseWtmSwagger(this IApplicationBuilder app, bool showInDebugOnly = true)
        {
            var configs = app.ApplicationServices.GetRequiredService<IOptions<Configs>>().Value;
            // 快速调试模式或明确要求显示时才启用 Swagger
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

        /// <summary>
        /// 配置 React 单页应用：启用 Spa 静态文件服务，开发环境下代理到 React 开发服务器
        /// </summary>
        /// <param name="app">应用构建器</param>
        /// <returns>应用构建器</returns>
        public static IApplicationBuilder UseReact(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetService<IWebHostEnvironment>();
            app.UseSpaStaticFiles();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                // 开发环境下启动 React 开发服务器（npm start），生产环境使用已构建的静态文件
                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

            return app;
        }

    }


}
