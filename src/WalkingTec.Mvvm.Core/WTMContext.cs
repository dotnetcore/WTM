using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using WalkingTec.Mvvm.Core.Auth;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core
{
    public class WTMContext
    {
        public HttpContext HttpContext { get; set; }
        public ClaimsPrincipal User { get; }

        private Configs _configInfo;
        public Configs ConfigInfo
        {
            get
            {
                if (_configInfo == null)
                {
                    _configInfo = (Configs)HttpContext.RequestServices.GetService(typeof(Configs));
                }
                return _configInfo;
            }
            set
            {
                _configInfo = value;
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
                if (User.Identity.IsAuthenticated && _loginUserInfo == null) // 用户认证通过后，当前上下文不包含用户数据
                {
                    var userIdStr = User.Claims.SingleOrDefault(x => x.Type == AuthConstants.JwtClaimTypes.Subject).Value;
                    Guid userId = Guid.Parse(userIdStr);
                    var cacheKey = $"{GlobalConstants.CacheKey.UserInfo}:{userIdStr}";
                    _loginUserInfo = Cache.Get<LoginUserInfo>(cacheKey);
                    if (_loginUserInfo == null || _loginUserInfo.Id != userId)
                    {
                        var userInfo = DC.Set<FrameworkUserBase>()
                                            .Include(x => x.UserRoles)
                                            .Include(x => x.UserGroups)
                                            .Where(x => x.ID == userId)
                                            .SingleOrDefault();
                        if (userInfo != null)
                        {
                            // 初始化用户信息
                            var roleIDs = userInfo.UserRoles.Select(x => x.RoleId).ToList();
                            var groupIDs = userInfo.UserGroups.Select(x => x.GroupId).ToList();
                            var dataPris = DC.Set<DataPrivilege>()
                                            .Where(x => x.UserId == userInfo.ID || (x.GroupId != null && groupIDs.Contains(x.GroupId.Value)))
                                            .ToList();

                            //查找登录用户的页面权限
                            var funcPrivileges = DC.Set<FunctionPrivilege>()
                                .Where(x => x.UserId == userInfo.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                                .ToList();

                            _loginUserInfo = new LoginUserInfo
                            {
                                Id = userInfo.ID,
                                ITCode = userInfo.ITCode,
                                Name = userInfo.Name,
                                PhotoId = userInfo.PhotoId,
                                Roles = DC.Set<FrameworkRole>().Where(x => userInfo.UserRoles.Select(y => y.RoleId).Contains(x.ID)).ToList(),
                                Groups = DC.Set<FrameworkGroup>().Where(x => userInfo.UserGroups.Select(y => y.GroupId).Contains(x.ID)).ToList(),
                                DataPrivileges = dataPris,
                                FunctionPrivileges = funcPrivileges
                            };
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
                    Cache.Add($"{GlobalConstants.CacheKey.UserInfo}:{_loginUserInfo.Id}", value);
                    _loginUserInfo = value;
                }
                else
                {
                    _loginUserInfo = value;
                    Cache.Add($"{GlobalConstants.CacheKey.UserInfo}:{_loginUserInfo.Id}", value);
                }
            }
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

    }
}
