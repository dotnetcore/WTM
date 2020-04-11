using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WalkingTec.Mvvm.Core.Auth;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core
{
    public class WTMContext
    {
        public HttpContext HttpContext { get; set; }

        private Configs _configInfo;
        public Configs ConfigInfo
        {
            get
            {
                return _configInfo;
            }
        }

        private GlobalData _globaInfo;
        public GlobalData GlobaInfo
        {
            get
            {
                if (_globaInfo == null)
                {
                    _globaInfo = (GlobalData)HttpContext.RequestServices.GetService(typeof(GlobalData));
                }
                return _globaInfo;
            }
            set
            {
                _globaInfo = value;
            }
        }

        private IUIService _uiservice;
        public IUIService UIService
        {
            get
            {
                if (_uiservice == null)
                {
                    _uiservice = (IUIService)HttpContext.RequestServices.GetService(typeof(IUIService));
                }
                return _uiservice;
            }
            set
            {
                _uiservice = value;
            }
        }

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

        #region Domain

        public List<FrameworkDomain> Domains
        {
            get
            {
                return ReadFromCache<List<FrameworkDomain>>("Domains", () =>
                {
                    using (var dc = this.CreateDC())
                    {
                        return dc.Set<FrameworkDomain>().ToList();
                    }
                });
            }
        }
        public static Guid? DomainId { get; set; }

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

                rv = new LoginUserInfo
                {
                    Id = userInfo.ID,
                    ITCode = userInfo.ITCode,
                    Name = userInfo.Name,
                    PhotoId = userInfo.PhotoId,
                    Roles = DC.Set<FrameworkRole>().Where(x => roleIDs.Contains(x.ID)).ToList(),
                    Groups = DC.Set<FrameworkGroup>().Where(x => groupIDs.Contains(x.ID)).ToList(),
                    DataPrivileges = dataPris,
                    FunctionPrivileges = funcPrivileges
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

        public ActionLog Log { get; set; }

        public WTMContext(IOptions<Configs> _config, GlobalData _gd)
        {
            _configInfo = _config.Value;
            _globaInfo = _gd;
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
            return ConfigInfo.ConnectionStrings.Where(x => x.Key.ToLower() == cs).FirstOrDefault().CreateDC();
        }

        #endregion

        public void DoLog(string msg, ActionLogTypesEnum logtype = ActionLogTypesEnum.Debug)
        {
            var log = Log.Clone() as ActionLog;
            log.LogType = logtype;
            log.ActionTime = DateTime.Now;
            log.Remark = msg;
            using (var dc = CreateDC())
            {
                dc.Set<ActionLog>().Add(log);
                dc.SaveChanges();
            }
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
