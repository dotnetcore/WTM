using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Options;
using WalkingTec.Mvvm.Core.Auth;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Json;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Core
{
    public class WTMContext
    {
        private HttpContext _httpContext;
        public HttpContext HttpContext { get => _httpContext; }

        private IServiceProvider _serviceProvider;
        public IServiceProvider ServiceProvider { get => _serviceProvider ?? _httpContext?.RequestServices; }


        private List<IDataPrivilege> _dps;
        public List<IDataPrivilege> DataPrivilegeSettings { get => _dps; }

        private Configs _configInfo;
        public Configs ConfigInfo { get => _configInfo; }

        private GlobalData _globaInfo;
        public GlobalData GlobaInfo { get => _globaInfo; }

        private IUIService _uiservice;
        public IUIService UIService { get => _uiservice; }

        private IDistributedCache _cache;
        public IDistributedCache Cache
        {
            get
            {
                return _cache;
            }
        }

        public string CurrentCS { get; set; }

        public DBTypeEnum? CurrentDbType { get; set; }

        public string ParentWindowId
        {
            get
            {
                string rv = null;
                if (WindowIds != null)
                {
                    var ids = WindowIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (ids.Length > 1)
                    {
                        rv = ids[ids.Length - 2];
                    }
                }

                return rv ?? string.Empty;
            }
        }

        public string CurrentWindowId
        {
            get
            {
                string rv = null;
                if (WindowIds != null)
                {
                    var ids = WindowIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (ids.Length > 0)
                    {
                        rv = ids[ids.Length - 1];
                    }
                }

                return rv ?? string.Empty;
            }
        }

        public string WindowIds
        {
            get
            {
                string rv = string.Empty;
                try
                {
                    if (HttpContext.Request.Cookies.TryGetValue($"{ConfigInfo?.CookiePre}windowguid", out string windowguid) == true)
                    {

                        if (HttpContext.Request.Cookies.TryGetValue($"{ConfigInfo?.CookiePre}{windowguid}windowids", out string windowid) == true)
                        {
                            rv = windowid;
                        }
                    }
                }
                catch { }
                return rv;
            }
        }

        public ISessionService Session { get; set; }

        public IModelStateService MSD { get; set; }

        public static Func<WTMContext, string, LoginUserInfo> ReloadUserFunc { get; set; }

        #region DataContext

        private IDataContext _dc;
        public IDataContext DC
        {
            get
            {
                if (_dc == null)
                {
                    _dc = this.CreateDC();
                }
                return _dc;
            }
            set
            {
                _dc = value;
            }
        }

        #endregion

        #region Current User

        private LoginUserInfo _loginUserInfo;
        public LoginUserInfo LoginUserInfo
        {
            get
            {
                if (_loginUserInfo == null && HttpContext?.User?.Identity?.IsAuthenticated == true ) // 用户认证通过后，当前上下文不包含用户数据
                {
                    var userIdStr = HttpContext.User.Claims.Where(x => x.Type == AuthConstants.JwtClaimTypes.Subject).Select(x => x.Value).FirstOrDefault();
                    var tenant = HttpContext.User.Claims.Where(x => x.Type == AuthConstants.JwtClaimTypes.TenantCode).Select(x=>x.Value).FirstOrDefault();
                    string usercode = userIdStr;
                    var cacheKey = $"{GlobalConstants.CacheKey.UserInfo}:{userIdStr + "$`$" + tenant}";
                    _loginUserInfo = Cache.Get<LoginUserInfo>(cacheKey);
                    if (_loginUserInfo == null)
                    {
                        try
                        {
                            _loginUserInfo = ReloadUser(usercode).Result;
                        }
                        catch { }
                        if (_loginUserInfo != null)
                        {
                            Cache.Add(cacheKey, _loginUserInfo);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                return _loginUserInfo;
            }
            set
            {
                if (value == null)
                {
                    Cache.Delete($"{GlobalConstants.CacheKey.UserInfo}:{_loginUserInfo?.ITCode + "$`$" + _loginUserInfo?.TenantCode}");
                    _loginUserInfo = value;
                }
                else
                {
                    _loginUserInfo = value;
                    Cache.Add($"{GlobalConstants.CacheKey.UserInfo}:{_loginUserInfo.ITCode+ "$`$" + _loginUserInfo?.TenantCode}", value);
                }
            }
        }

        private Type _localizerType;
        private IStringLocalizerFactory _stringLocalizerFactory;
        private IStringLocalizer _localizer;
        private ILoggerFactory _loggerFactory;
        public IStringLocalizer Localizer
        {
            get
            {
                if (_localizer == null && _stringLocalizerFactory != null)
                {
                    if(_localizerType == null)
                    {
                        _localizerType = Assembly.GetEntryAssembly().GetTypes().Where(x => x.Name == "Program").FirstOrDefault();
                    }
                    _localizer = _stringLocalizerFactory.Create(_localizerType);
                }
                return _localizer ?? WalkingTec.Mvvm.Core.CoreProgram._localizer;
            }
        }

        /// <summary>
        /// 从数据库读取用户
        /// </summary>
        /// <param name="itcode">用户名</param>
        /// <returns>用户信息</returns>
        public virtual async Task<LoginUserInfo>
            ReloadUser(string itcode)
        {
            if (ReloadUserFunc != null)
            {
                var reload = ReloadUserFunc.Invoke(this, itcode);
                if (reload != null)
                {
                    return reload;
                }
            }
            if (DC == null)
            {
                return null;
            }

            var password = await BaseUserQuery.Where(x => x.ITCode.ToLower() == itcode.ToLower()).Select(x =>x.Password).SingleOrDefaultAsync();
            if (this.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                var user = await CallAPI<LoginUserInfo>("", GetServerUrl() + "/api/_account/loginjwt", HttpMethodEnum.POST, new { Account = itcode, Password = password, IsReload=true });
                return user?.Data;
            }
            else
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("account", itcode);
                data.Add("password", password);
                data.Add("withmenu", "false");
                var user = await CallAPI<LoginUserInfo>("", this.HttpContext.Request.Scheme + "://" + this.HttpContext.Request.Host.ToString() + "/api/_account/login", HttpMethodEnum.POST, data);
                return user?.Data;
            }
        }

        #endregion

        #region URL
        public string BaseUrl { get; set; }
        #endregion

        public SimpleLog Log { get; set; }

        protected ILogger<ActionLog> Logger { get; set; }


        private IQueryable<FrameworkUserBase> _baseUserQuery;
        public IQueryable<FrameworkUserBase> BaseUserQuery
        {
            get
            {
                if(_baseUserQuery == null && this.GlobaInfo?.CustomUserType  != null && DC != null)
                {
                    var set = DC.GetType().GetMethod("Set", Type.EmptyTypes).MakeGenericMethod(GlobaInfo.CustomUserType);
                    _baseUserQuery = set.Invoke(DC, null) as IQueryable<FrameworkUserBase>;
                }
                return _baseUserQuery;
            }
        }

        public WTMContext(IOptionsMonitor<Configs> _config, GlobalData _gd = null, IHttpContextAccessor _http = null, IUIService _ui = null, List<IDataPrivilege> _dp = null, IDataContext dc = null, IStringLocalizerFactory stringLocalizer = null, ILoggerFactory loggerFactory = null, WtmLocalizationOption lop=null, IDistributedCache cache=null)
        {
            _configInfo = _config?.CurrentValue ?? new Configs();
            _globaInfo = _gd ?? new GlobalData();
            _httpContext = _http?.HttpContext;
            _cache = cache;
            _stringLocalizerFactory = stringLocalizer;
            _loggerFactory = loggerFactory;
            _localizerType = lop?.LocalizationType;
            this.Logger = loggerFactory?.CreateLogger<ActionLog>();
            if (_httpContext == null)
            {
                MSD = new BasicMSD();
            }
            _uiservice = _ui;
            if (_dp == null)
            {
                _dp = new List<IDataPrivilege>();
            }
            _dps = _dp;
            if (dc is NullContext)
            {
                _dc = null;
            }
            else
            {
                _dc = dc;
            }
        }

        public void SetServiceProvider(IServiceProvider sp)
        {
            this._serviceProvider = sp;
        }


        public T ReadFromCache<T>(string key, Func<T> setFunc, int? timeout = null)
        {
            if (Cache.TryGetValue(key, out T rv) == false || rv == null)
            {
                T data = setFunc();
                if (timeout == null)
                {
                    Cache.Add(key, data);
                }
                else
                {
                    Cache.Add(key, data, new DistributedCacheEntryOptions()
                    {
                        SlidingExpiration = new TimeSpan(0, 0, timeout.Value)
                    });
                }
                return data;
            }
            else
            {
                return rv;
            }
        }

        public async Task RemoveUserCache(
            params string[] userIds)
        {
            foreach (var userId in userIds)
            {
                var key = $"{GlobalConstants.CacheKey.UserInfo}:{userId+"$`$"+LoginUserInfo?.TenantCode}";
                await Cache.DeleteAsync(key);
            }
        }


        #region CreateDC
        public virtual IDataContext CreateDC(bool isLog = false, string cskey = null)
        {
            string cs = cskey ?? CurrentCS;
            if (isLog == true)
            {
                if (ConfigInfo.Connections?.Where(x => x.Key.ToLower() == "defaultlog").FirstOrDefault() != null)
                {
                    cs = "defaultlog";
                }
            }
            if (cs == null)
            {
                cs = "default";
            }
            var rv = ConfigInfo.Connections.Where(x => x.Key.ToLower() == cs.ToLower()).FirstOrDefault().CreateDC();
            rv.IsDebug = ConfigInfo.IsQuickDebug;
            rv.SetLoggerFactory(_loggerFactory);
            return rv;
        }

        #endregion

        /// <summary>
        /// 判断某URL是否有权限访问
        /// </summary>
        /// <param name="url">url地址</param>
        /// <returns>true代表可以访问，false代表不能访问</returns>
        public bool IsAccessable(string url)
        {
            // 如果是调试 或者 url 为 null or 空字符串
            if (_configInfo.IsQuickDebug || string.IsNullOrEmpty(url) || IsUrlPublic(url))
            {
                return true;
            }
            //循环所有不限制访问的url，如果含有当前判断的url，则认为可以访问
            var publicActions = _globaInfo.AllAccessUrls;
            foreach (var au in publicActions)
            {
                if (new Regex("^"+au + "[/\\?]?", RegexOptions.IgnoreCase).IsMatch(url))
                {
                    return true;
                }
            }
            //如果没有任何页面权限，则直接返回false
            if (LoginUserInfo?.FunctionPrivileges == null)
            {
                return false;
            }


            url = Regex.Replace(url, "/do(batch.*)", "/$1", RegexOptions.IgnoreCase);

            //如果url以#开头，一般是javascript使用的临时地址，不需要判断，直接返回true
            url = url.Trim();

            if (url.StartsWith("#"))
            {
                return true;
            }
            var menus = _globaInfo.AllMenus;
            var menu = Utils.FindMenu(url, GlobaInfo.AllMenus);
            //如果最终没有找到，说明系统菜单中并没有配置这个url，返回false
            if (menu == null)
            {
                return false;
            }
            //如果找到了，则继续验证其他权限
            else
            {
                return IsAccessable(menu, menus);
            }
        }

        /// <summary>
        /// 判断某菜单是否有权限访问
        /// </summary>
        /// <param name="menu">菜单项</param>
        /// <param name="menus">所有系统菜单</param>
        /// <returns>true代表可以访问，false代表不能访问</returns>
        protected bool IsAccessable(SimpleMenu menu, List<SimpleMenu> menus)
        {
            //寻找当前菜单的页面权限
            var find = LoginUserInfo?.FunctionPrivileges.Where(x => x.MenuItemId == menu.ID && x.Allowed == true).FirstOrDefault();
            //如果能找到直接对应的页面权限
            if (find != null)
            {
                return true;
            }
            return false;
        }

        public bool IsUrlPublic(string url)
        {
            var isPublic = false;
            try
            {
                url = Regex.Replace(url, "/do(batch.*)", "/$1", RegexOptions.IgnoreCase);
                url = url.Trim();

                if (url.StartsWith("#"))
                {
                    isPublic = true;
                }
                var menus = GlobaInfo.AllMenus;
                var menu = Utils.FindMenu(url, menus);
                if (menu != null && menu.IsPublic == true)
                {
                    isPublic = true;
                }
            }
            catch { }
            return isPublic;
        }

        public void DoLog(string msg, ActionLogTypesEnum logtype = ActionLogTypesEnum.Normal,string moduleName="", string actionName="", string ip="",string url = "", double duration = 0)
        {
            var log = this.Log?.GetActionLog();
            if (log == null)
            {
                log = new ActionLog();
            }
            log.LogType = logtype;
            log.ActionTime = DateTime.Now;
            log.Remark = msg;
            log.ActionUrl = url;
            log.Duration = duration;
            log.ModuleName = moduleName;
            log.ActionName = actionName;
            log.IP = ip;
            if(string.IsNullOrEmpty(url) && this.HttpContext?.Request != null)
            {
                log.ActionUrl = this.HttpContext.Request.Path.ToString();
            }
            LogLevel ll = LogLevel.Information;
            switch (logtype)
            {
                case ActionLogTypesEnum.Normal:
                    ll = LogLevel.Information;
                    break;
                case ActionLogTypesEnum.Exception:
                    ll = LogLevel.Error;
                    break;
                case ActionLogTypesEnum.Debug:
                    ll = LogLevel.Debug;
                    break;
                default:
                    break;
            }

            Logger?.Log<ActionLog>(ll, new EventId(), log, null, (a, b) =>
            {
                return $@"
===WTM Log===
内容:{a.Remark}
地址:{a.ActionUrl}
时间:{a.ActionTime}
===WTM Log===
";
            });
        }


        #region CreateVM
        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <param name="VMType">The type of the viewmodel</param>
        /// <param name="Id">If the viewmodel is a BaseCRUDVM, the data having this id will be fetched</param>
        /// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        /// <param name="values">properties of the viewmodel that you want to assign values</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        private BaseVM CreateVM(Type VMType, object Id = null, object[] Ids = null, Dictionary<string, object> values = null, bool passInit = false)
        {
            //Use reflection to create viewmodel
            var ctor = VMType.GetConstructor(Type.EmptyTypes);
            BaseVM rv = ctor.Invoke(null) as BaseVM;
            rv.Wtm = this;

            rv.FC = new Dictionary<string, object>();
            rv.CreatorAssembly = this.GetType().AssemblyQualifiedName;
            rv.ControllerName = this.HttpContext?.Request?.Path;
            if (HttpContext != null && HttpContext.Request != null)
            {
                try
                {
                    if (HttpContext.Request.QueryString != QueryString.Empty)
                    {
                        foreach (var key in HttpContext.Request.Query.Keys)
                        {
                            if (rv.FC.Keys.Contains(key) == false)
                            {
                                rv.FC.Add(key, HttpContext.Request.Query[key]);
                            }
                        }
                    }
                    if (HttpContext.Request.HasFormContentType)
                    {
                        var f = HttpContext.Request.Form;
                        foreach (var key in f.Keys)
                        {
                            if (rv.FC.Keys.Contains(key) == false)
                            {
                                rv.FC.Add(key, f[key]);
                            }
                        }
                    }
                }
                catch { }
            }
            //try to set values to the viewmodel's matching properties
            if (values != null)
            {
                foreach (var v in values)
                {
                    PropertyHelper.SetPropertyValue(rv, v.Key, v.Value, null, false);
                }
            }
            //if viewmodel is derrived from BaseCRUDVM<> and Id has value, call ViewModel's GetById method
            if (Id != null && rv is IBaseCRUDVM<TopBasePoco> cvm)
            {
                cvm.SetEntityById(Id);
            }
            //if viewmodel is derrived from IBaseBatchVM<>，set ViewMode's Ids property,and init it's ListVM and EditModel properties
            if (rv is IBaseBatchVM<BaseVM> temp)
            {
                temp.Ids = new string[] { };
                if (Ids != null)
                {
                    var tempids = new List<string>();
                    foreach (var iid in Ids)
                    {
                        tempids.Add(iid.ToString());
                    }
                    temp.Ids = tempids.ToArray();
                }
                if (temp.ListVM != null)
                {
                    temp.ListVM.CopyContext(rv);
                    temp.ListVM.Ids = Ids == null ? new List<string>() : temp.Ids.ToList();
                    temp.ListVM.SearcherMode = ListVMSearchModeEnum.Batch;
                    temp.ListVM.NeedPage = false;
                }
                if (temp.LinkedVM != null)
                {
                    temp.LinkedVM.CopyContext(rv);
                }
                if (temp.ListVM != null)
                {
                    //Remove the action columns from list
                    temp.ListVM.OnAfterInitList += (self) =>
                    {
                        self.RemoveActionColumn();
                        self.RemoveAction();
                        if (temp.ErrorMessage.Count > 0)
                        {
                            self.AddErrorColumn();
                        }
                    };
                    temp.ListVM.DoInitListVM();
                    if (temp.ListVM.Searcher != null)
                    {
                        var searcher = temp.ListVM.Searcher;
                        searcher.CopyContext(rv);
                        if (passInit == false)
                        {
                            searcher.DoInit();
                        }
                    }
                }
                temp.LinkedVM?.DoInit();
                //temp.ListVM.DoSearch();
            }
            //if the viewmodel is a ListVM, Init it's searcher
            if (rv is IBasePagedListVM<TopBasePoco, ISearcher> lvm)
            {
                var searcher = lvm.Searcher;
                searcher.CopyContext(rv);
                if (passInit == false)
                {
                    searcher.DoInit();
                }
                lvm.DoInitListVM();

            }
            if (rv is IBaseImport<BaseTemplateVM> tvm)
            {
                var template = tvm.Template;
                template.CopyContext(rv);
                template.DoInit();
                var errorlist = tvm.ErrorListVM;
                errorlist.CopyContext(rv);
            }

            //if passinit is not set, call the viewmodel's DoInit method
            if (passInit == false)
            {
                rv.DoInit();
            }
            return rv;
        }

        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <typeparam name="T">The type of the viewmodelThe type of the viewmodel</typeparam>
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example Wtm.CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        public T CreateVM<T>(Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), null, new object[] { }, dir, passInit) as T;
        }

        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <typeparam name="T">The type of the viewmodelThe type of the viewmodel</typeparam>
        /// <param name="Id">If the viewmodel is a BaseCRUDVM, the data having this id will be fetched</param>
        /// <param name="values">properties of the viewmodel that you want to assign values</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        public T CreateVM<T>(object Id, Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), Id, new object[] { }, dir, passInit) as T;
        }

        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <typeparam name="T">The type of the viewmodelThe type of the viewmodel</typeparam>
        /// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example Wtm.CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        public T CreateVM<T>(object[] Ids, Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), null, Ids, dir, passInit) as T;
        }


        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <typeparam name="T">The type of the viewmodelThe type of the viewmodel</typeparam>
        /// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example Wtm.CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        public T CreateVM<T>(Guid[] Ids, Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), null, Ids.Cast<object>().ToArray(), dir, passInit) as T;
        }

        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <typeparam name="T">The type of the viewmodelThe type of the viewmodel</typeparam>
        /// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example Wtm.CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        public T CreateVM<T>(int[] Ids, Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), null, Ids.Cast<object>().ToArray(), dir, passInit) as T;
        }

        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <typeparam name="T">The type of the viewmodelThe type of the viewmodel</typeparam>
        /// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example Wtm.CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        public T CreateVM<T>(long[] Ids, Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), null, Ids.Cast<object>().ToArray(), dir, passInit) as T;
        }
        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <typeparam name="T">The type of the viewmodelThe type of the viewmodel</typeparam>
        /// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example Wtm.CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        public T CreateVM<T>(string[] Ids, Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), null, Ids.Cast<object>().ToArray(), dir, passInit) as T;
        }

        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <param name="VmFullName">the fullname of the viewmodel's type</param>
        /// <param name="Id">If the viewmodel is a BaseCRUDVM, the data having this id will be fetched</param>
        /// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        public BaseVM CreateVM(string VmFullName, object Id = null, object[] Ids = null, bool passInit = false)
        {
            return CreateVM(Type.GetType(VmFullName), Id, Ids, null, passInit);
        }
        #endregion

        #region CallApi
        public async Task<ApiResult<T>> CallAPI<T>(string domainName, string url, HttpMethodEnum method, HttpContent content,  int? timeout = null, string proxy = null) where T:class
        {
            ApiResult<T> rv = new ApiResult<T>();
            try
            {
                var factory = this.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                if (string.IsNullOrEmpty(url))
                {
                    return rv;
                }
                //新建http请求
                HttpClient client = null;
                if (string.IsNullOrEmpty(domainName))
                {
                    client = factory.CreateClient();
                }
                else
                {
                    client = factory.CreateClient(domainName);
                }
                //如果配置了代理，则使用代理
                //设置超时
                if (timeout.HasValue)
                {
                    client.Timeout = new TimeSpan(0, 0, 0, timeout.Value, 0);
                }
                //填充表单数据
                HttpResponseMessage res = null;
                switch (method)
                {
                    case HttpMethodEnum.GET:
                        res = await client.GetAsync(url);
                        break;
                    case HttpMethodEnum.POST:
                        res = await client.PostAsync(url, content);
                        break;
                    case HttpMethodEnum.PUT:
                        res = await client.PutAsync(url, content);
                        break;
                    case HttpMethodEnum.DELETE:
                        res = await client.DeleteAsync(url);
                        break;
                    default:
                        break;
                }
                if (res == null)
                {
                    return rv;
                }
                rv.StatusCode = res.StatusCode;
                if (res.IsSuccessStatusCode == true)
                {
                    Type dt = typeof(T);
                    if (dt == typeof(byte[]))
                    {
                        rv.Data = await res.Content.ReadAsByteArrayAsync() as T;
                    }
                    else
                    {
                        string responseTxt = await res.Content.ReadAsStringAsync();
                        if (dt == typeof(string))
                        {
                            rv.Data = responseTxt as T;
                        }
                        else
                        {
                            rv.Data = JsonSerializer.Deserialize<T>(responseTxt, CoreProgram.DefaultJsonOption);
                        }
                    }
                }
                else
                {
                    string responseTxt = await res.Content.ReadAsStringAsync();
                    if (res.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {

                        try
                        {
                            rv.Errors = JsonSerializer.Deserialize<ErrorObj>(responseTxt, CoreProgram.DefaultJsonOption);
                        }
                        catch { }
                    }
                    rv.ErrorMsg = responseTxt;
                }

                return rv;
            }
            catch (Exception ex)
            {
                rv.ErrorMsg = ex.ToString();
                return rv;
            }
        }

        /// <summary>
        /// 使用Get方法调用api
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domainName">Appsettings中配置的Domain key</param>
        /// <param name="url">调用地址</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public async Task<ApiResult<T>> CallAPI<T>(string domainName, string url,  int? timeout = null, string proxy = null) where T : class
        {
            HttpContent content = null;
            //填充表单数据
            return await CallAPI<T>(domainName, url, HttpMethodEnum.GET, content, timeout, proxy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domainName">Appsettings中配置的Domain key</param>
        /// <param name="url">调用地址</param>
        /// <param name="method">调用方式</param>
        /// <param name="postdata">提交字段</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public async Task<ApiResult<T>> CallAPI<T>(string domainName, string url, HttpMethodEnum method, IDictionary<string, string> postdata, int? timeout = null, string proxy = null) where T : class
        {
            HttpContent content = null;
            //填充表单数据
            if (!(postdata == null || postdata.Count == 0))
            {
                List<KeyValuePair<string, string>> paras = new List<KeyValuePair<string, string>>();
                foreach (string key in postdata.Keys)
                {
                    paras.Add(new KeyValuePair<string, string>(key, postdata[key]));
                }
                content = new FormUrlEncodedContent(paras);
            }
            return await CallAPI<T>(domainName, url, method, content, timeout, proxy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domainName">Appsettings中配置的Domain key</param>
        /// <param name="url">调用地址</param>
        /// <param name="method">调用方式</param>
        /// <param name="postdata">提交的object，会被转成json提交</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public async Task<ApiResult<T>> CallAPI<T>(string domainName, string url, HttpMethodEnum method, object postdata,  int? timeout = null, string proxy = null) where T : class
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(postdata, CoreProgram.DefaultPostJsonOption), System.Text.Encoding.UTF8, "application/json");
            return await CallAPI<T>(domainName, url, method, content,  timeout, proxy);
        }

        public async Task<ApiResult<string>> CallAPI(string domainName, string url, HttpMethodEnum method, HttpContent content, int? timeout = null, string proxy = null) 
        {
            return await CallAPI<string>(domainName, url, method, content, timeout, proxy);
        }

        /// <summary>
        /// 使用Get方法调用api
        /// </summary>
        /// <param name="domainName">Appsettings中配置的Domain key</param>
        /// <param name="url">调用地址</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public async Task<ApiResult<string>> CallAPI(string domainName, string url,  int? timeout = null, string proxy = null)
        {
            return await CallAPI<string>(domainName, url, timeout, proxy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainName">Appsettings中配置的Domain key</param>
        /// <param name="url">调用地址</param>
        /// <param name="method">调用方式</param>
        /// <param name="postdata">提交字段</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public async Task<ApiResult<string>> CallAPI(string domainName, string url, HttpMethodEnum method, IDictionary<string, string> postdata,int? timeout = null, string proxy = null)
        {
            return await CallAPI<string>(domainName, url, method, postdata, timeout, proxy);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainName">Appsettings中配置的Domain key</param>
        /// <param name="url">调用地址</param>
        /// <param name="method">调用方式</param>
        /// <param name="postdata">提交的object，会被转成json提交</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public async Task<ApiResult<string>> CallAPI(string domainName, string url, HttpMethodEnum method, object postdata, int? timeout = null, string proxy = null)
        {
            return await CallAPI<string>(domainName, url, method, postdata, timeout, proxy);
        }


        private string GetServerUrl()
        {
            var server = ConfigInfo.Domains.Where(x => x.Key.ToLower() == "serverpub").Select(x => x.Value).FirstOrDefault();
            if (server == null)
            {
                server = ConfigInfo.Domains.Where(x => x.Key.ToLower() == "server").Select(x => x.Value).FirstOrDefault();
            }
            if (server != null && string.IsNullOrEmpty(server.Address) == false)
            {
                return server.Address.TrimEnd('/');
            }
            else
            {
                return this.HttpContext.Request.Scheme + "://" + this.HttpContext.Request.Host.ToString();
            }
        }

        #endregion
    }

}
