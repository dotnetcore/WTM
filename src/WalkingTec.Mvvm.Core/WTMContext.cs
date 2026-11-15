using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalkingTec.Mvvm.Core.Auth;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// WTM 框架的每请求上下文中枢，贯穿一次请求的完整生命周期。
    /// 集中持有数据上下文(DC)、当前登录用户(LoginUserInfo)、分布式缓存(Cache)、系统配置(ConfigInfo)、
    /// 全局数据(GlobaInfo)、会话(Session)、UI服务(UIService)等核心对象；
    /// 并提供 CreateVM 视图模型工厂、DoLogin 登录验证、CreateDC 多数据库上下文创建、
    /// CallAPI 远程接口调用等框架级 API，是 Controller 与 ViewModel 之间传递上下文的桥梁。
    /// </summary>
    public class WTMContext : IDisposable
    {
        private HttpContext _httpContext;
        /// <summary>
        /// 当前HTTP上下文
        /// </summary>
        public HttpContext HttpContext { get => _httpContext; }

        private IServiceProvider _serviceProvider;
        /// <summary>
        /// 服务提供者；未显式设置时回退使用当前HTTP请求的 RequestServices
        /// </summary>
        public IServiceProvider ServiceProvider { get => _serviceProvider ?? _httpContext?.RequestServices; }


        private List<IDataPrivilege> _dps;
        /// <summary>
        /// 系统中配置的数据权限(IDataPrivilege)列表，定义哪些模型参与数据权限过滤
        /// </summary>
        public List<IDataPrivilege> DataPrivilegeSettings { get => _dps; }

        private Configs _configInfo;
        /// <summary>
        /// 系统配置信息，对应 appsettings 中 WTM 节点的配置
        /// </summary>
        public Configs ConfigInfo { get => _configInfo; }

        private GlobalData _globaInfo;
        /// <summary>
        /// 全局数据，包含系统所有菜单、页面权限、数据权限、租户、程序集等静态信息
        /// </summary>
        public GlobalData GlobaInfo { get => _globaInfo; }

        private IUIService _uiservice;
        /// <summary>
        /// UI服务，按当前UI框架(如LayUI)提供前端相关操作的统一接口
        /// </summary>
        public IUIService UIService { get => _uiservice; }

        private IDistributedCache _cache;
        /// <summary>
        /// 分布式缓存，用于缓存用户信息、租户角色/用户组等数据
        /// </summary>
        public IDistributedCache Cache
        {
            get
            {
                return _cache;
            }
        }

        /// <summary>
        /// 当前使用的连接串Key；为空时使用 default 连接
        /// </summary>
        public string CurrentCS
        {
            get;
            set;
        }

        /// <summary>
        /// 当前使用的数据库类型；为空时使用连接配置中的默认数据库类型
        /// </summary>
        public DBTypeEnum? CurrentDbType { get; set; }

        /// <summary>
        /// 父窗口的WindowId；弹出窗口向打开它的页面回传数据时用来定位目标窗口
        /// </summary>
        public string ParentWindowId
        {
            get
            {
                string rv = null;
                if (WindowIds != null)
                {
                    //窗口链按逗号分隔，倒数第二个即父窗口
                    var ids = WindowIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (ids.Length > 1)
                    {
                        rv = ids[ids.Length - 2];
                    }
                }

                return rv ?? string.Empty;
            }
        }

        /// <summary>
        /// 当前窗口的WindowId
        /// </summary>
        public string CurrentWindowId
        {
            get
            {
                string rv = null;
                if (WindowIds != null)
                {
                    //窗口链按逗号分隔，最后一个即当前窗口
                    var ids = WindowIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (ids.Length > 0)
                    {
                        rv = ids[ids.Length - 1];
                    }
                }

                return rv ?? string.Empty;
            }
        }

        /// <summary>
        /// 当前弹窗链的窗口ID串（逗号分隔，存储在Cookie中）
        /// </summary>
        public string WindowIds
        {
            get
            {
                string rv = string.Empty;
                try
                {
                    //先从Cookie读取windowguid，再读取该guid对应的窗口链串
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

        /// <summary>
        /// 会话服务，提供对Session的读写操作
        /// </summary>
        public ISessionService Session { get; set; }

        /// <summary>
        /// 模型验证服务，用于向ModelState添加验证错误信息
        /// </summary>
        public IModelStateService MSD { get; set; }

        /// <summary>
        /// 外部注册的用户重载委托；设置后 ReloadUser 将优先调用它加载用户，用于自定义用户来源
        /// </summary>
        public static Func<WTMContext, string, LoginUserInfo> ReloadUserFunc { get; set; }

        #region DataContext

        private IDataContext _dc;
        /// <summary>
        /// 当前使用的数据上下文(DataContext)；未设置时延迟创建，自动按当前连接串和租户选择数据库
        /// </summary>
        public IDataContext DC
        {
            get
            {
                //延迟创建：尚未初始化时按当前连接串/租户信息自动创建 DataContext
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
        /// <summary>
        /// 当前登录用户信息；未加载时自动从缓存或数据库加载（支持JWT认证和远程令牌两种方式），赋值时会同步写入缓存
        /// </summary>
        public LoginUserInfo LoginUserInfo
        {
            get
            {
                if (_loginUserInfo == null && HttpContext?.User?.Identity?.IsAuthenticated == true) //用户认证通过后，当前上下文尚不包含用户数据，需要从Claims中提取并加载
                {
                    //从JWT的Claims中提取用户标识和租户编码
                    var userIdStr = HttpContext.User.Claims.Where(x => x.Type == AuthConstants.JwtClaimTypes.Subject).Select(x => x.Value).FirstOrDefault();
                    var tenant = HttpContext.User.Claims.Where(x => x.Type == AuthConstants.JwtClaimTypes.TenantCode).Select(x => x.Value).FirstOrDefault();
                    string usercode = userIdStr;
                    //以"用户名+租户"组合为缓存键，优先从缓存读取用户信息
                    var cacheKey = $"{GlobalConstants.CacheKey.UserInfo}:{userIdStr + "$`$" + tenant}";
                    _loginUserInfo = Cache.Get<LoginUserInfo>(cacheKey);
                    //缓存未命中时调用 ReloadUser 从数据库重新加载用户，并写回缓存
                    if (_loginUserInfo == null)
                    {
                        try
                        {
                            _loginUserInfo = ReloadUser(usercode);
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
                //未登录但请求携带 _remotetoken 参数（跨系统远程认证）时，尝试解析远程令牌获取用户
                if (_loginUserInfo == null && HttpContext?.Request.Query.Any(x => x.Key == "_remotetoken") == true)
                {
                    var remoteToken = HttpContext?.Request.Query["_remotetoken"][0];
                    //非主站点模式：直接解析远程JWT令牌，后续流程与上面的JWT认证相同
                    if (ConfigInfo.HasMainHost == false)
                    {
                        JwtSecurityToken token = new JwtSecurityToken();
                        try
                        {
                            token = new JwtSecurityToken(remoteToken);
                        }
                        catch { }
                        //从远程JWT令牌的Claims中提取用户标识和租户编码，同样先读缓存、未命中再 ReloadUser 加载
                        var userIdStr = token.Claims.Where(x => x.Type == AuthConstants.JwtClaimTypes.Subject).Select(x => x.Value).FirstOrDefault();
                        var tenant = token.Claims.Where(x => x.Type == AuthConstants.JwtClaimTypes.TenantCode).Select(x => x.Value).FirstOrDefault();
                        string usercode = userIdStr;
                        var cacheKey = $"{GlobalConstants.CacheKey.UserInfo}:{userIdStr + "$`$" + tenant}";
                        _loginUserInfo = Cache.Get<LoginUserInfo>(cacheKey);
                        if (_loginUserInfo == null)
                        {
                            try
                            {
                                _loginUserInfo = ReloadUser(usercode);
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
                    else if (string.IsNullOrEmpty(remoteToken) == false)
                    {
                        //主站点模式：携带远程令牌时，通过 ReloadUser 到主站点验证并加载用户信息
                        try
                        {
                            _loginUserInfo = ReloadUser("null");
                        }
                        catch { }
                        if (_loginUserInfo != null)
                        {
                            var cacheKey = $"{GlobalConstants.CacheKey.UserInfo}:{_loginUserInfo.ITCode + "$`$" + _loginUserInfo.TenantCode}";
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
                //赋值为null时清除缓存中的用户信息；赋具体值时写入缓存
                if (value == null)
                {
                    Cache.Delete($"{GlobalConstants.CacheKey.UserInfo}:{_loginUserInfo?.ITCode + "$`$" + _loginUserInfo?.TenantCode}");
                    _loginUserInfo = value;
                }
                else
                {
                    _loginUserInfo = value;
                    Cache.Add($"{GlobalConstants.CacheKey.UserInfo}:{_loginUserInfo.ITCode + "$`$" + _loginUserInfo?.TenantCode}", value);
                }
            }
        }

        //本地化资源来源类型；未配置时默认取入口程序集中的 Program 类
        private Type _localizerType;
        private IStringLocalizerFactory _stringLocalizerFactory;
        private IStringLocalizer _localizer;
        private ILoggerFactory _loggerFactory;
        /// <summary>
        /// 日志工厂
        /// </summary>
        public ILoggerFactory LoggerFactory { get { return _loggerFactory; } }
        /// <summary>
        /// 多语言本地化服务，用于读取多语言资源文本
        /// </summary>
        public IStringLocalizer Localizer
        {
            get
            {
                if (_localizer == null && _stringLocalizerFactory != null)
                {
                    //未显式指定本地化类型时，使用入口程序集的 Program 类型作为本地化来源
                    if (_localizerType == null)
                    {
                        _localizerType = Assembly.GetEntryAssembly().GetTypes().Where(x => x.Name == "Program").FirstOrDefault();
                    }
                    _localizer = _stringLocalizerFactory.Create(_localizerType);
                }
                //无法创建本地化实例时，退回使用框架的全局本地化器
                return _localizer ?? WalkingTec.Mvvm.Core.CoreProgram._localizer;
            }
        }

        /// <summary>
        /// 从数据库读取用户
        /// </summary>
        /// <param name="itcode">用户名</param>
        /// <returns>用户信息</returns>
        public virtual LoginUserInfo
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
            var user = DoLogin(itcode, null, null);
            return user;
        }

        #endregion

        #region URL
        /// <summary>
        /// 站点根地址
        /// </summary>
        public string BaseUrl { get; set; }
        #endregion

        /// <summary>
        /// 当前请求的主机地址（协议+域名+端口）；无请求上下文时返回空字符串
        /// </summary>
        public string HostAddress { get
            {
                if (this.HttpContext?.Request != null)
                {
                    return $"{this.HttpContext.Request.Scheme}://{this.HttpContext.Request.Host}";
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 当前请求关联的简易日志对象(SimpleLog)，用于暂存Action日志数据
        /// </summary>
        public SimpleLog Log { get; set; }

        /// <summary>
        /// ActionLog 专用日志记录器
        /// </summary>
        protected ILogger<ActionLog> Logger { get; set; }


        private IQueryable<FrameworkUserBase> _baseUserQuery;
        /// <summary>
        /// 系统自定义用户类型（继承自 FrameworkUserBase）的数据查询源，供登录验证等内部逻辑使用
        /// </summary>
        public IQueryable<FrameworkUserBase> BaseUserQuery
        {
            get
            {
                if (_baseUserQuery == null && this.GlobaInfo?.CustomUserType != null && DC != null)
                {
                    //自定义用户类型编译期未知，通过反射调用 DC 的泛型 Set 方法获取其查询源
                    var set = DC.GetType().GetMethod("Set", Type.EmptyTypes).MakeGenericMethod(GlobaInfo.CustomUserType);
                    _baseUserQuery = set.Invoke(DC, null) as IQueryable<FrameworkUserBase>;
                }
                return _baseUserQuery;
            }
        }

        /// <summary>
        /// 构造函数，由依赖注入容器调用，装配上下文所需的各种系统对象
        /// </summary>
        /// <param name="_config">系统配置（Options监视器）</param>
        /// <param name="_gd">全局数据</param>
        /// <param name="_http">HTTP上下文访问器</param>
        /// <param name="_ui">UI服务</param>
        /// <param name="_dp">数据权限设置列表</param>
        /// <param name="dc">已创建的数据上下文</param>
        /// <param name="stringLocalizer">多语言本地化工厂</param>
        /// <param name="loggerFactory">日志工厂</param>
        /// <param name="lop">本地化选项</param>
        /// <param name="cache">分布式缓存</param>
        /// <param name="sp">服务提供者</param>
        public WTMContext(IOptionsMonitor<Configs> _config, GlobalData _gd = null, IHttpContextAccessor _http = null, IUIService _ui = null, List<IDataPrivilege> _dp = null, IDataContext dc = null, IStringLocalizerFactory stringLocalizer = null, ILoggerFactory loggerFactory = null, WtmLocalizationOption lop = null, IDistributedCache cache = null, IServiceProvider sp=null)
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
            //NullContext 是容器注入的空实现占位，统一转为 null，让 DC 属性按需延迟创建
            if (dc is NullContext)
            {
                _dc = null;
            }
            else
            {
                _dc = dc;
            }
            _serviceProvider = sp;
        }

        /// <summary>
        /// 手动设置服务提供者，用于非依赖注入场景下补充服务定位能力
        /// </summary>
        /// <param name="sp">服务提供者</param>
        public void SetServiceProvider(IServiceProvider sp)
        {
            this._serviceProvider = sp;
        }

        /// <summary>
        /// 执行登录验证：主站点模式下转发到主站点完成认证；否则在本地数据库验证用户名密码，
        /// 加载用户完整信息（角色、权限、数据权限等）并签发JWT令牌
        /// </summary>
        /// <param name="username">用户名(ITCode)</param>
        /// <param name="password">密码明文，验证时转为MD5比对</param>
        /// <param name="tenant">租户编码，为空时取当前DataContext或当前登录用户的租户</param>
        /// <returns>登录成功返回用户信息(含令牌)，失败返回null</returns>
        public LoginUserInfo DoLogin(string username, string password, string tenant)
        {
            //未指定租户时，依次尝试从当前DataContext和已认证用户的Claims中获取
            if(string.IsNullOrEmpty(tenant))
            {
                tenant = DC.TenantCode;               
            }
            if (tenant == null && HttpContext.User.Identity.IsAuthenticated)
            {
                tenant = HttpContext.User.Claims.Where(x => x.Type == AuthConstants.JwtClaimTypes.TenantCode).Select(x => x.Value).FirstOrDefault() ?? tenant;
            }
            //主站点模式（本系统作为子站点，用户中心部署在主站点）：登录验证转发到主站点完成
            if (ConfigInfo.HasMainHost && string.IsNullOrEmpty(tenant) == true)
            {
                //优先取已保存的远程令牌，其次从请求的 _remotetoken 参数获取
                var remoteToken = _loginUserInfo?.RemoteToken ?? HttpContext?.Request.Query?.Where(x => x.Key == "_remotetoken").Select(x => x.Value.First()).FirstOrDefault();
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    remoteToken = HttpContext.User.Claims.Where(x => x.Type == AuthConstants.JwtClaimTypes.RToken).Select(x => x.Value).FirstOrDefault();
                }
                    LoginUserInfo rv = null;
                if (string.IsNullOrEmpty(remoteToken) == false)
                {
                    //携带远程令牌时，直接到主站点验证并获取用户信息
                    Dictionary<string, string> headers = new Dictionary<string, string>();
                    headers.Add("Authorization", "Bearer " + remoteToken);
                    var user = CallAPI<LoginUserInfo>("mainhost", "/api/admin/account/checkuserinfo?IsApi=false", HttpMethodEnum.GET, new { }, 10, headers: headers).Result;
                    rv = user.Data;
                    if (rv != null)
                    {
                        rv.RemoteToken = remoteToken;
                    }
                }
                else if(string.IsNullOrEmpty(password)==false)
                {
                    //无远程令牌但有密码时，先用账号密码到主站点登录换取JWT令牌
                    var loginjwt = CallAPI<Token>("mainhost", "/api/admin/account/loginjwt", HttpMethodEnum.POST, new { Account = username, Password = password }, 10).Result;
                    if (string.IsNullOrEmpty(loginjwt?.Data?.AccessToken) == false)
                    {
                        remoteToken = loginjwt?.Data?.AccessToken;
                        Dictionary<string, string> headers = new Dictionary<string, string>();
                        headers.Add("Authorization", "Bearer " + remoteToken);
                        var user = CallAPI<LoginUserInfo>("mainhost", "/api/admin/account/checkuserinfo?IsApi=false", HttpMethodEnum.GET, new { }, 10, headers: headers).Result;
                        rv = user.Data;
                        if (rv != null)
                        {
                            rv.RemoteToken = remoteToken;
                        }
                    }
                }
                if (rv != null)
                {
                        rv.LoadBasicInfoAsync(this).Wait();
                }
                return rv;
            }
            else
            {
                //本地模式：直接在当前（或租户独立的）数据库中验证用户并签发令牌
                bool exist = false;
                username = HttpContext.User.Claims.Where(x => x.Type == AuthConstants.JwtClaimTypes.Subject).Select(x => x.Value).FirstOrDefault() ?? username;
                var ct = GlobaInfo.AllTenant.Where(x => x.TCode == tenant).FirstOrDefault();
                //如果找不到指定的tenant，说明租户不存在，直接返回null
                if (ct == null && string.IsNullOrEmpty(tenant) == false)
                {
                    return null;
                }
                //租户配置了独立数据库时，切换到租户自己的DataContext
                if (ct != null)
                {
                    _dc = ct.CreateDC(this);
                }
                //已通过JWT认证的用户只需验证用户存在且有效；未认证用户需同时比对密码(MD5)
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    exist = BaseUserQuery.IgnoreQueryFilters().Any(x => x.ITCode == username && x.TenantCode == tenant && x.IsValid==true);
                }
                else
                {
                    exist = BaseUserQuery.IgnoreQueryFilters().Any(x => x.ITCode == username && x.Password == Utils.GetMD5String(password) && x.TenantCode == tenant && x.IsValid==true);
                }
                if (exist == false)
                {
                    return null;
                }

                LoginUserInfo user = new LoginUserInfo
                {
                    ITCode = username,
                    TenantCode = tenant
                };
                    //加载用户的角色、权限、数据权限等完整信息
                    user.LoadBasicInfoAsync(this).Wait();
                user.RemoteToken = null;
                    //为用户签发本系统的JWT令牌
                    var authService = HttpContext.RequestServices.GetService(typeof(ITokenService)) as ITokenService;
                var token = authService.IssueTokenAsync(user).Result;
                user.RemoteToken = token.AccessToken;
                return user;
            }
        }

        /// <summary>
        /// 刷新当前用户的JWT令牌
        /// </summary>
        /// <returns>新签发的令牌；未登录时返回null</returns>
        public Token RefreshToken()
        {
            if(LoginUserInfo == null)
            {
                return null;
            }
            string rt = null;
            //主站点模式且非租户用户：先到主站点刷新远程令牌；否则沿用当前用户的远程令牌
            if (ConfigInfo.HasMainHost && LoginUserInfo?.CurrentTenant == null)
            {
                var r = CallAPI<Token>("mainhost", $"/api/admin/account/RefreshToken", HttpMethodEnum.POST, new { }).Result;
                rt = r?.Data?.AccessToken;
            }
            else
            {
                rt = LoginUserInfo.RemoteToken;
           }
            var _authService = ServiceProvider.GetRequiredService<ITokenService>();
            //用当前用户信息重新签发本地令牌，并携带远程令牌
            var rv = _authService.IssueTokenAsync(new LoginUserInfo
            {
                ITCode = LoginUserInfo.ITCode,
                TenantCode = LoginUserInfo.TenantCode,
                RemoteToken = rt
            }).Result;
            return rv;
        }

        /// <summary>
        /// 读取缓存模式的封装：缓存命中直接返回，未命中时执行委托获取数据并写入缓存
        /// </summary>
        /// <typeparam name="T">缓存数据的类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="setFunc">缓存未命中时生成数据的委托</param>
        /// <param name="timeout">缓存过期时间（秒），为空则永久缓存</param>
        /// <returns>缓存或委托返回的数据</returns>
        public T ReadFromCache<T>(string key, Func<T> setFunc, int? timeout = null)
        {
            //缓存未命中时执行委托获取数据，并按是否指定过期时间写回缓存
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
                        AbsoluteExpirationRelativeToNow = new TimeSpan(0, 0, timeout.Value)
                    });
                }
                return data;
            }
            else
            {
                return rv;
            }
        }

        /// <summary>
        /// 移除指定用户的信息缓存，使下次请求重新从数据库加载用户
        /// </summary>
        /// <param name="userIds">用户ITCode数组</param>
        /// <returns>异步任务</returns>
        public async Task RemoveUserCache(
            params string[] userIds)
        {
            //按用户ITCode拼出缓存键并逐个删除
            foreach (var userId in userIds)
            {
                var key = $"{GlobalConstants.CacheKey.UserInfo}:{userId + "$`$" + LoginUserInfo?.CurrentTenant}";
                await Cache.DeleteAsync(key);
            }
        }

        /// <summary>
        /// 移除指定角色下所有用户的信息缓存
        /// </summary>
        /// <param name="rolecode">角色编码数组</param>
        /// <returns>异步任务</returns>
        public async Task RemoveUserCacheByRole(
    params string[] rolecode)
        {
            List<string> userids = new List<string>();
            //主站点模式：调用主站点接口按角色查用户；否则直接查询本地的 FrameworkUserRole 表
            if (ConfigInfo.HasMainHost && string.IsNullOrEmpty(LoginUserInfo?.CurrentTenant) == true)
            {
                foreach (var item in rolecode)
                {
                    var rv = await CallAPI<List<string>>("mainhost", $"/api/admin/frameworkuser/GetUserByRole?keywords={item}");
                    if (rv != null && rv.Data != null)
                    {
                        userids.AddRange(rv.Data);
                    }
                }
            }
            else
            {
                userids = DC.Set<FrameworkUserRole>().Where(x => rolecode.Contains(x.RoleCode)).Select(x => x.UserCode).ToList();
            }
            foreach (var userId in userids)
            {
                var key = $"{GlobalConstants.CacheKey.UserInfo}:{userId + "$`$" + LoginUserInfo?.CurrentTenant}";
                await Cache.DeleteAsync(key);
            }
        }

        /// <summary>
        /// 移除指定用户组下所有用户的信息缓存
        /// </summary>
        /// <param name="groupcode">用户组编码数组</param>
        /// <returns>异步任务</returns>
        public async Task RemoveUserCacheByGroup(
params string[] groupcode)
        {
            List<string> userids = new List<string>();
            //主站点模式：调用主站点接口按用户组查用户；否则直接查询本地的 FrameworkUserGroup 表
            if (ConfigInfo.HasMainHost && string.IsNullOrEmpty(LoginUserInfo?.CurrentTenant) == true)
            {
                foreach (var item in groupcode)
                {
                    var rv = await CallAPI<List<string>>("mainhost", $"/api/admin/frameworkuser/GetUserByGroup?keywords={item}");
                    if (rv != null && rv.Data != null)
                    {
                        userids.AddRange(rv.Data);
                    }
                }
            }
            else
            {
                userids = DC.Set<FrameworkUserGroup>().Where(x => groupcode.Contains(x.GroupCode)).Select(x => x.UserCode).ToList();
            }
            foreach (var userId in userids)
            {
                var key = $"{GlobalConstants.CacheKey.UserInfo}:{userId + "$`$" + LoginUserInfo?.CurrentTenant}";
                await Cache.DeleteAsync(key);
            }
        }

        /// <summary>
        /// 移除指定租户的用户组缓存
        /// </summary>
        /// <param name="tenant">租户编码</param>
        /// <returns>异步任务</returns>
        public async Task RemoveGroupCache(string tenant)
        {

                var key = $"{GlobalConstants.CacheKey.TenantGroups}:{tenant}";
                await Cache.DeleteAsync(key);
        }

        /// <summary>
        /// 移除指定租户的角色缓存
        /// </summary>
        /// <param name="tenant">租户编码</param>
        /// <returns>异步任务</returns>
        public async Task RemoveRoleCache(string tenant)
        {

                var key = $"{GlobalConstants.CacheKey.TenantRoles}:{tenant}";
                await Cache.DeleteAsync(key);
            
        }

        /// <summary>
        /// 获取指定租户的全部用户组（带缓存）
        /// </summary>
        /// <param name="tenant">租户编码</param>
        /// <returns>租户下的用户组列表</returns>
        public List<SimpleGroup> GetTenantGroups(string tenant)
        {
            var key = $"{GlobalConstants.CacheKey.TenantGroups}:{tenant}";
            var rv = ReadFromCache<List<SimpleGroup>>(key, () =>
            {
                
                List<SimpleGroup> groups = null;
                try
                {
                    //租户配置了独立数据库时使用租户连接，否则使用 default 连接；忽略全局过滤器读取该租户全部用户组
                    var dbtenant = GlobaInfo.AllTenant.Where(x => x.TCode == tenant && x.IsUsingDB == true).FirstOrDefault();
                    using (var dc = dbtenant == null ? ConfigInfo.Connections.Where(x => x.Key.ToLower() == "default").FirstOrDefault().CreateDC() : dbtenant.CreateDC(this))
                    {
                        groups = dc.Set<FrameworkGroup>().IgnoreQueryFilters().Where(x => x.TenantCode == tenant).Select(x => new SimpleGroup
                        {
                            ID = x.ID,
                            GroupCode = x.GroupCode,
                            GroupName = x.GroupName,
                            Manager = x.Manager,
                            ParentId = x.ParentId,
                            Tenant = x.TenantCode
                        }).ToList();
                    }
                }
                catch
                {
                    groups = new List<SimpleGroup>();
                }
                return groups;
            }, 360000);
            return rv;
        }

        /// <summary>
        /// 获取指定租户的全部角色（带缓存）
        /// </summary>
        /// <param name="tenant">租户编码</param>
        /// <returns>租户下的角色列表</returns>
        public List<SimpleRole> GetTenantRoles(string tenant)
        {
            var key = $"{GlobalConstants.CacheKey.TenantRoles}:{tenant}";
            var rv = ReadFromCache<List<SimpleRole>>(key, () =>
            {
                List<SimpleRole> roles = null;
                try
                {
                    //租户配置了独立数据库时使用租户连接，否则使用 default 连接；忽略全局过滤器读取该租户全部角色
                    var dbtenant = GlobaInfo.AllTenant.Where(x => x.TCode == tenant && x.IsUsingDB == true).FirstOrDefault();
                    using (var dc = dbtenant == null ? ConfigInfo.Connections.Where(x => x.Key.ToLower() == "default").FirstOrDefault().CreateDC() : dbtenant.CreateDC(this))
                    {
                        roles = dc.Set<FrameworkRole>().IgnoreQueryFilters().Where(x => x.TenantCode == tenant).Select(x => new SimpleRole
                        {
                            ID = x.ID,
                            RoleCode = x.RoleCode,
                            RoleName = x.RoleName,
                            Tenant = x.TenantCode
                        }).ToList();
                    }
                }
                catch
                {
                    roles = new List<SimpleRole>();
                }
                return roles;
            }, 360000);
            return rv;
        }


        /// <summary>
        /// 切换当前登录用户操作的租户
        /// </summary>
        /// <param name="tenant">目标租户编码</param>
        /// <returns>切换成功返回true，无权限切换返回false</returns>
        public bool SetCurrentTenant(string tenant)
        {
            if (LoginUserInfo != null)
            {
                //仅当用户未绑定租户、已属于该租户、或该租户是其所属主租户的子租户时才允许切换
                if (LoginUserInfo.TenantCode == null || LoginUserInfo.TenantCode == tenant || GlobaInfo.AllTenant.Any(x => x.TCode == tenant && x.TenantCode == LoginUserInfo.TenantCode))
                {
                    LoginUserInfo.CurrentTenant = tenant;
                    LoginUserInfo = LoginUserInfo;
                    return true;
                }
            }
            return false;
        }

        #region CreateDC
        /// <summary>
        /// 创建数据上下文：按连接串Key、当前租户等信息选择合适的数据库连接；
        /// 支持租户独立数据库、日志专用连接(defaultlog)等机制
        /// </summary>
        /// <param name="isLog">是否创建写日志用的上下文，为true时优先使用 defaultlog 连接</param>
        /// <param name="cskey">连接串Key，为空时依次尝试 CurrentCS、租户连接、default</param>
        /// <param name="logerror">是否为上下文设置日志工厂</param>
        /// <returns>创建的数据上下文</returns>
        public virtual IDataContext CreateDC(bool isLog = false, string cskey = null, bool logerror = true)
        {
            string cs = cskey ?? CurrentCS;
            string tenantCode = null;

            var tenants = GlobaInfo.AllTenant ?? new List<FrameworkTenant>();
            string tc = _loginUserInfo?.CurrentTenant;
            //当前用户未指定租户时，尝试从请求的 Referer 域名反查租户（支持租户独立域名）
            if (tc == null && HttpContext?.Request.Headers.ContainsKey("Referer")==true)
            {
                Regex r = new Regex("(http://|https://)?(.+?)(/)?$");
                var m = r.Match(HttpContext?.Request.Headers["Referer"]);
                string dom = "";
                if (m.Success)
                {
                    dom = m.Groups[2].Value;
                }
                tc = tenants.Where(x => x.TDomain != null && x.TDomain.ToLower() == dom.ToLower()).Select(x => x.TCode).FirstOrDefault();
            }
            if (tc != null)
            {
                var item = tenants.Where(x => x.TCode == tc).FirstOrDefault();
                tenantCode = tc;
                //如果租户指定了数据库，则返回
                if (string.IsNullOrEmpty(cs) && item?.IsUsingDB == true)
                {
                    return item.CreateDC(this);
                }
            }

            //写日志时若配置了 defaultlog 连接，则使用专用的日志数据库
            if (isLog == true)
            {
                if (ConfigInfo.Connections?.Where(x => x.Key.ToLower() == "defaultlog").FirstOrDefault() != null)
                {
                    cs = "defaultlog";
                }
            }
            //最终仍未指定连接时，回退使用 default 连接
            if (string.IsNullOrEmpty(cs))
            {
                cs = "default";
            }
            var rv = ConfigInfo.Connections.Where(x => x.Key.ToLower() == cs.ToLower()).FirstOrDefault().CreateDC();
            rv.IsDebug = ConfigInfo.IsQuickDebug;
            rv.SetTenantCode(tenantCode);
            if (logerror == true)
            {
                rv.SetLoggerFactory(_loggerFactory);
            }
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
            //租户用户不能访问标记[HostOnly]的方法
            if (_configInfo.EnableTenant == true)
            {
                if (LoginUserInfo?.TenantCode != null)
                {
                    var hostonly = _globaInfo.AllMainTenantOnlyUrls;
                    foreach (var au in hostonly)
                    {
                        if (new Regex("^" + au + "[/\\?]?", RegexOptions.IgnoreCase).IsMatch(url))
                        {
                            return false;
                        }
                    }
                }
            }
            //循环所有不限制访问的url，如果含有当前判断的url，则认为可以访问
            var publicActions = _globaInfo.AllAccessUrls;
            foreach (var au in publicActions)
            {                
                if (au != "/" && new Regex("^" + au + "[/\\?]?", RegexOptions.IgnoreCase).IsMatch(url))
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
            if (LoginUserInfo.CurrentTenant != null && menu.TenantAllowed == false)
            {
                return false;
            }
            //寻找当前菜单的页面权限
            var find = LoginUserInfo?.FunctionPrivileges.Where(x => x.MenuItemId == menu.ID && x.Allowed == true).FirstOrDefault();
            //如果能找到直接对应的页面权限
            if (find != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断某URL是否为公开地址（无需登录即可访问）
        /// </summary>
        /// <param name="url">url地址</param>
        /// <returns>true代表公开地址，false代表需要登录访问</returns>
        public bool IsUrlPublic(string url)
        {
            var isPublic = false;
            try
            {
                url = Regex.Replace(url, "/do(batch.*)", "/$1", RegexOptions.IgnoreCase);
                url = url.Trim();

                //#开头的临时地址视为公开
                if (url.StartsWith("#"))
                {
                    isPublic = true;
                }
                var menus = GlobaInfo.AllMenus;
                //查找url对应的菜单，菜单标记为公开则任何人可访问
                var menu = Utils.FindMenu(url, menus);
                if (menu != null && menu.IsPublic == true)
                {
                    isPublic = true;
                }
            }
            catch { }
            return isPublic;
        }

        /// <summary>
        /// 记录一条操作日志(ActionLog)，按日志类型映射日志级别并写入日志系统
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="logtype">日志类型（普通/异常/调试）</param>
        /// <param name="moduleName">模块名称</param>
        /// <param name="actionName">操作名称</param>
        /// <param name="ip">客户端IP</param>
        /// <param name="url">访问地址，为空时自动取当前请求路径</param>
        /// <param name="duration">执行耗时（毫秒）</param>
        public void DoLog(string msg, ActionLogTypesEnum logtype = ActionLogTypesEnum.Normal, string moduleName = "", string actionName = "", string ip = "", string url = "", double duration = 0)
        {
            //优先复用当前请求已记录的日志对象，否则新建
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
            log.ITCode = LoginUserInfo?.ITCode;
            log.IP = ip;
            if (string.IsNullOrEmpty(url) && this.HttpContext?.Request != null)
            {
                log.ActionUrl = this.HttpContext.Request.Path.ToString();
            }
            //按日志类型映射日志级别：普通=Information，异常=Error，调试=Debug
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

            //初始化FC(前端传值字典)，后续会把QueryString和表单数据填入，供VM读取前端原始提交值
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
            //递归初始化VM中所有BaseVM类型的子VM属性
            SetSubVm(rv, passInit);
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
            //导入VM：初始化其Excel模板VM和错误列表VM
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
        /// 递归初始化 vm 中所有 BaseVM 类型的子VM属性：
        /// 反射创建未初始化的子VM实例，复制上下文、建立父子关系，并按需调用其 DoInit
        /// </summary>
        /// <param name="vm">父VM</param>
        /// <param name="passInit">为true时不调用子VM的DoInit</param>
        private void SetSubVm(BaseVM vm, bool passInit)
        {
            //查找所有BaseVM类型的属性（排除ParentVM避免循环引用）
            var sub = vm.GetType().GetAllProperties().Where(x => typeof(BaseVM).IsAssignableFrom(x.PropertyType) && x.Name != "ParentVM");
            foreach (var prop in sub)
            {
                var subins = prop.GetValue(vm) as BaseVM;
                bool exist = subins == null ? false : true;
                if (subins == null)
                {
                    //子VM未实例化时，通过无参构造函数反射创建
                    subins = prop.PropertyType.GetConstructor(Type.EmptyTypes).Invoke(null) as BaseVM;
                }
                if (subins != null)
                {
                    //将父VM的上下文传递给子VM，并记录父子关系
                    subins.CopyContext(vm);
                    subins.ParentVM = vm;
                    subins.PropertyNameInParent = prop.Name;
                   if (passInit == false)
                    {
                        subins.DoInit();
                    }
                    if (exist == false)
                    {
                        vm.SetPropertyValue(prop.Name, subins);
                    }
                    SetSubVm(subins,passInit);
                }
            }

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
        /// <summary>
        /// 调用远程API的核心实现：创建HttpClient、附加认证头、发送请求并反序列化响应
        /// </summary>
        /// <typeparam name="T">响应数据的类型</typeparam>
        /// <param name="domainName">Appsettings中配置的Domain key，为空则创建未命名客户端</param>
        /// <param name="url">调用地址</param>
        /// <param name="method">http请求方式</param>
        /// <param name="content">请求内容</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <param name="headers">自定义http请求头</param>
        /// <returns>API调用结果（含状态码、数据或错误信息）</returns>
        public async Task<ApiResult<T>> CallAPI<T>(string domainName, string url, HttpMethodEnum method, HttpContent content, int? timeout = null, string proxy = null, Dictionary<string, string> headers = null) where T : class
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
                    client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
                }
                else
                {
                    client = factory.CreateClient(domainName);
                }
                //附加调用方自定义的http请求头
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        if (client.DefaultRequestHeaders.Any(x => x.Key == item.Key) == false)
                        {
                            client.DefaultRequestHeaders.Add(item.Key, item.Value);
                        }
                    }
                }
                //未显式指定Authorization时，自动附加当前登录用户的远程令牌
                if (client.DefaultRequestHeaders.Any(x => x.Key == "Authorization") == false && string.IsNullOrEmpty(LoginUserInfo?.RemoteToken) == false)
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + LoginUserInfo.RemoteToken);
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
                //调用成功：按目标类型反序列化响应内容（字节数组、字符串或JSON）
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
                    //调用失败：记录错误信息，BadRequest时尝试解析为结构化错误对象
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
        /// <param name="headers">http headers</param>
        /// <returns></returns>
        public async Task<ApiResult<T>> CallAPI<T>(string domainName, string url, int? timeout = null, string proxy = null, Dictionary<string, string> headers = null) where T : class
        {
            HttpContent content = null;
            //填充表单数据
            return await CallAPI<T>(domainName, url, HttpMethodEnum.GET, content, timeout, proxy, headers);
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
        /// <param name="headers">http headers</param>
        /// <returns></returns>
        public async Task<ApiResult<T>> CallAPI<T>(string domainName, string url, HttpMethodEnum method, IDictionary<string, string> postdata, int? timeout = null, string proxy = null, Dictionary<string, string> headers = null) where T : class
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
            return await CallAPI<T>(domainName, url, method, content, timeout, proxy, headers);
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
        /// <param name="headers">http headers</param>
        /// <returns></returns>
        public async Task<ApiResult<T>> CallAPI<T>(string domainName, string url, HttpMethodEnum method, object postdata, int? timeout = null, string proxy = null, Dictionary<string, string> headers = null) where T : class
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(postdata, CoreProgram.DefaultPostJsonOption), System.Text.Encoding.UTF8, "application/json");
            return await CallAPI<T>(domainName, url, method, content, timeout, proxy, headers);
        }

        /// <summary>
        /// 调用远程API并返回字符串响应（HttpContent版本）
        /// </summary>
        /// <param name="domainName">Appsettings中配置的Domain key</param>
        /// <param name="url">调用地址</param>
        /// <param name="method">http请求方式</param>
        /// <param name="content">请求内容</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <param name="headers">自定义http请求头</param>
        /// <returns>API调用结果（含状态码、数据或错误信息）</returns>
        public async Task<ApiResult<string>> CallAPI(string domainName, string url, HttpMethodEnum method, HttpContent content, int? timeout = null, string proxy = null, Dictionary<string, string> headers = null)
        {
            return await CallAPI<string>(domainName, url, method, content, timeout, proxy, headers);
        }

        /// <summary>
        /// 使用Get方法调用api
        /// </summary>
        /// <param name="domainName">Appsettings中配置的Domain key</param>
        /// <param name="url">调用地址</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <param name="headers">http headers</param>
        /// <returns></returns>
        public async Task<ApiResult<string>> CallAPI(string domainName, string url, int? timeout = null, string proxy = null, Dictionary<string, string> headers = null)
        {
            return await CallAPI<string>(domainName, url, timeout, proxy, headers);
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
        /// <param name="headers">自定义header</param>
        /// <returns></returns>
        public async Task<ApiResult<string>> CallAPI(string domainName, string url, HttpMethodEnum method, IDictionary<string, string> postdata, int? timeout = null, string proxy = null, Dictionary<string, string> headers = null)
        {
            return await CallAPI<string>(domainName, url, method, postdata, timeout, proxy, headers);

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
        /// <param name="headers">http headers</param>
        /// <returns></returns>
        public async Task<ApiResult<string>> CallAPI(string domainName, string url, HttpMethodEnum method, object postdata, int? timeout = null, string proxy = null, Dictionary<string, string> headers = null)
        {
            return await CallAPI<string>(domainName, url, method, postdata, timeout, proxy, headers);
        }


        /// <summary>
        /// 获取服务端对外地址：优先使用 Domains 中 serverpub/server 的配置，未配置时取当前请求地址
        /// </summary>
        /// <returns>服务端地址</returns>
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

        /// <summary>
        /// 释放资源，销毁当前持有的DataContext
        /// </summary>
        public void Dispose()
        {
            this._dc?.Dispose();
        }

        #endregion
    }

}
