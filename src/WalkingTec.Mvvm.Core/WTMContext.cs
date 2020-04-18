using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
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
    public class WTMContext
    {
        private HttpContext _httpContext;
        public HttpContext HttpContext { get => _httpContext; }

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
                if (_cache == null)
                {
                    _cache = (IDistributedCache)HttpContext.RequestServices.GetService(typeof(IDistributedCache));
                }
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
                if (HttpContext.User.Identity.IsAuthenticated && _loginUserInfo == null) // 用户认证通过后，当前上下文不包含用户数据
                {
                    var userIdStr = HttpContext.User.Claims.SingleOrDefault(x => x.Type == AuthConstants.JwtClaimTypes.Subject).Value;
                    Guid userId = Guid.Parse(userIdStr);
                    var cacheKey = $"{GlobalConstants.CacheKey.UserInfo}:{userIdStr}";
                    _loginUserInfo = Cache.Get<LoginUserInfo>(cacheKey);
                    if (_loginUserInfo == null || _loginUserInfo.Id != userId)
                    {
                        _loginUserInfo = LoadUserFromDB(userId).Result; 
                        if (_loginUserInfo != null)
                        {
                            Cache.Add(cacheKey, _loginUserInfo);
                        }
                        else
                        {
                            HttpContext.ChallengeAsync().Wait();
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
                    Cache.Delete($"{GlobalConstants.CacheKey.UserInfo}:{_loginUserInfo?.Id}");
                    _loginUserInfo = value;
                }
                else
                {
                    _loginUserInfo = value;
                    Cache.Add($"{GlobalConstants.CacheKey.UserInfo}:{_loginUserInfo.Id}", value);
                }
            }
        }

        /// <summary>
        /// 从数据库读取用户
        /// </summary>
        /// <param name="userId">用户ID，如果为空，则使用用户名和密码查询</param>
        /// <param name="itcode">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>用户信息</returns>
        public virtual async Task<LoginUserInfo> LoadUserFromDB(Guid? userId, string itcode=null, string password=null)
        {
            FrameworkUserBase userInfo = null;
            if(DC == null)
            {
                return null;
            }
            if (userId.HasValue)
            {
                userInfo = await DC.Set<FrameworkUserBase>()
                                    .Include(x => x.UserRoles)
                                    .Include(x => x.UserGroups)
                                    .Where(x => x.ID == userId)
                                    .SingleOrDefaultAsync();
            }
            else
            {
                userInfo = await DC.Set<FrameworkUserBase>()
                                    .Include(x => x.UserRoles)
                                    .Include(x => x.UserGroups)
                                    .Where(x => x.ITCode.ToLower() == itcode.ToLower() && x.Password == Utils.GetMD5String(password) && x.IsValid)
                                    .SingleOrDefaultAsync();

            }
            LoginUserInfo rv = null;
            if (userInfo != null)
            {
                // 初始化用户信息
                var roleIDs = userInfo.UserRoles.Select(x => x.RoleId).ToList();
                var groupIDs = userInfo.UserGroups.Select(x => x.GroupId).ToList();


                var dataPris = await DC.Set<DataPrivilege>().AsNoTracking()
                                .Where(x => x.UserId == userInfo.ID || (x.GroupId != null && groupIDs.Contains(x.GroupId.Value)))
                                .Distinct()
                                .ToListAsync();
                ProcessTreeDp(dataPris);

                //查找登录用户的页面权限
                var funcPrivileges = await DC.Set<FunctionPrivilege>().AsNoTracking()
                    .Where(x => x.UserId == userInfo.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                    .Distinct()
                    .ToListAsync();
                
                var roles = DC.Set<FrameworkRole>().AsNoTracking().Where(x => roleIDs.Contains(x.ID)).ToList();
                var groups = DC.Set<FrameworkGroup>().AsNoTracking().Where(x => groupIDs.Contains(x.ID)).ToList();

                rv = new LoginUserInfo()
                {
                    Id = userInfo.ID,
                    ITCode = userInfo.ITCode,
                    Name = userInfo.Name,
                    PhotoId = userInfo.PhotoId,
                    Roles = roles.Select(x=> new SimpleRole { ID = x.ID, RoleCode = x.RoleCode, RoleName = x.RoleName}).ToList(),
                    Groups =groups.Select(x => new SimpleGroup { ID = x.ID, GroupCode = x.GroupCode, GroupName = x.GroupName }).ToList(),
                    DataPrivileges = dataPris.Select(x=> new SimpleDataPri { ID = x.ID, RelateId = x.RelateId, TableName = x.TableName, UserId = x.UserId, GroupId = x.GroupId }).ToList(),
                    FunctionPrivileges = funcPrivileges.Select(x=> new SimpleFunctionPri { ID = x.ID, UserId = x.UserId, RoleId = x.RoleId, Allowed = x.Allowed, MenuItemId = x.MenuItemId}).ToList()
                };
            }
            return rv;
        }
        #endregion

        #region GUID
        public List<EncHash> EncHashs
        {
            get
            {
                return ReadFromCache<List<EncHash>>("EncHashs", () =>
                {
                    using (var dc = this.CreateDC())
                    {
                        return dc.Set<EncHash>().ToList();
                    }
                });
            }
        }
        #endregion

        #region Menus
        public List<FrameworkMenu> FFMenus => GlobaInfo.AllMenus;
        #endregion

        #region URL
        public string BaseUrl { get; set; }
        #endregion

        public SimpleLog Log { get; set; }


        public WTMContext(IOptions<Configs> _config, GlobalData _gd, IHttpContextAccessor _http, IUIService _ui)
        {
            _configInfo = _config.Value;
            _globaInfo = _gd;
            _httpContext = _http.HttpContext;
            _uiservice = _ui;
        }

        protected T ReadFromCache<T>(string key, Func<T> setFunc, int? timeout = null)
        {
            if (Cache.TryGetValue(key, out T rv) == false)
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
                        SlidingExpiration = new TimeSpan(0,0,timeout.Value)
                    });
                }
                return data;
            }
            else
            {
                return rv;
            }
        }

        #region CreateDC
        public virtual IDataContext CreateDC(bool isLog = false)
        {
            string cs = CurrentCS;
            if (isLog == true)
            {
                if (ConfigInfo.ConnectionStrings?.Where(x => x.Key.ToLower() == "defaultlog").FirstOrDefault() != null)
                {
                    cs = "defaultlog";
                }
                else
                {
                    cs = "default";
                }
            }
            if(cs == null)
            {
                cs = "default";
            }
            return ConfigInfo.ConnectionStrings.Where(x => x.Key.ToLower() == cs).FirstOrDefault().CreateDC();
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
            if (_configInfo.IsQuickDebug || string.IsNullOrEmpty(url))
            {
                return true;
            }
            //循环所有不限制访问的url，如果含有当前判断的url，则认为可以访问
            var publicActions = _globaInfo.AllAccessUrls;
            foreach (var au in publicActions)
            {
                if (new Regex(au + "[/\\?]?", RegexOptions.IgnoreCase).IsMatch(url))
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
            var menu = Utils.FindMenu(url);
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
        public bool IsAccessable(FrameworkMenu menu, List<FrameworkMenu> menus)
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



        public void DoLog(string msg, ActionLogTypesEnum logtype = ActionLogTypesEnum.Debug)
        {
            var log = this.Log.GetActionLog();
            log.LogType = logtype;
            log.ActionTime = DateTime.Now;
            log.Remark = msg;
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
            GlobalServices.GetRequiredService<ILogger<ActionLog>>().Log<ActionLog>(ll, new EventId(), log, null, (a, b) => {
                return $@"
===WTM Log===
内容:{a.Remark}
地址:{a.ActionUrl}
时间:{a.ActionTime}
===WTM Log===
";
            });
        }

        private void ProcessTreeDp(List<DataPrivilege> dps)
        {
            var dpsSetting = GlobaInfo.DataPrivilegeSettings;
            foreach (var ds in dpsSetting)
            {
                if (typeof(ITreeData).IsAssignableFrom(ds.ModelType))
                {
                    var ids = dps.Where(x => x.TableName == ds.ModelName).Select(x => x.RelateId).ToList();
                    if (ids.Count > 0 && ids.Contains(null) == false)
                    {
                        List<Guid> tempids = new List<Guid>();
                        foreach (var item in ids)
                        {
                            if (Guid.TryParse(item, out Guid g))
                            {
                                tempids.Add(g);
                            }
                        }
                        List<Guid> subids = new List<Guid>();
                        subids.AddRange(GetSubIds(tempids.ToList(), ds.ModelType));
                        subids = subids.Distinct().ToList();
                        subids.ForEach(x => dps.Add(new DataPrivilege
                        {
                            TableName = ds.ModelName,
                            RelateId = x.ToString()
                        }));
                    }
                }
            }
        }
        private IEnumerable<Guid> GetSubIds(List<Guid> p_id, Type modelType)
        {
            var basequery = DC.GetType().GetTypeInfo().GetMethod("Set").MakeGenericMethod(modelType).Invoke(DC, null) as IQueryable;
            var subids = basequery.Cast<ITreeData>().Where(x => p_id.Contains(x.ParentId.Value)).Select(x => x.ID).ToList();
            if (subids.Count > 0)
            {
                return subids.Concat(GetSubIds(subids, modelType));
            }
            else
            {
                return new List<Guid>();
            }
        }

    }

}
