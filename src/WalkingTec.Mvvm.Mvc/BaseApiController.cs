using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Implement;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Mvc
{
    public abstract class BaseApiController : ControllerBase, IBaseController
    {
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
            set
            {
                _cache = value;
            }
        }

        public string CurrentCS { get; set; }

        public DBTypeEnum? CurrentDbType { get; set; }

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

        private LoginUserInfo _loginUserInfo;
        public LoginUserInfo LoginUserInfo
        {
            get
            {
                if (User?.Identity?.IsAuthenticated == true && _loginUserInfo == null) // 用户认证通过后，当前上下文不包含用户数据
                {
                    var userIdStr = User.Claims.FirstOrDefault(x => x.Type == AuthConstants.JwtClaimTypes.Subject).Value;
                    Guid userId = Guid.Parse(userIdStr);
                    var cacheKey = $"{GlobalConstants.CacheKey.UserInfo}:{userIdStr}";
                    _loginUserInfo = Cache.Get<LoginUserInfo>(cacheKey);
                    if (User?.Identity?.AuthenticationType != AuthConstants.AuthenticationType)
                    {
                        if (_loginUserInfo == null || _loginUserInfo.Id != userId)
                        {
                            _loginUserInfo = this.GetLoginUserInfo(userId);
                            if (_loginUserInfo != null)
                            {
                                Cache.Add(cacheKey, _loginUserInfo);
                            }
                        }
                    }
                }
                return _loginUserInfo;
            }
            set
            {
                if (value == null)
                {
                    Cache.Delete($"{GlobalConstants.CacheKey.UserInfo}:{_loginUserInfo.Id}");
                }
                else
                {
                    Cache.Add($"{GlobalConstants.CacheKey.UserInfo}:{value.Id}", value);
                }
                _loginUserInfo = value;
            }
        }

        public string BaseUrl { get; set; }
        private IStringLocalizer _localizer;
        public IStringLocalizer Localizer
        {
            get
            {
                if (_localizer == null)
                {
                    var programtype = this.GetType().Assembly.GetTypes().Where(x => x.Name == "Program").FirstOrDefault();
                    if (programtype != null)
                    {
                        try
                        {
                            _localizer = GlobalServices.GetRequiredService(typeof(IStringLocalizer<>).MakeGenericType(programtype)) as IStringLocalizer;
                        }
                        catch { }
                    }
                    if (_localizer == null)
                    {
                        _localizer = WalkingTec.Mvvm.Core.Program._localizer;
                    }
                }
                return _localizer;
            }
        }

        public SimpleLog Log { get; set; }

        //-------------------------------------------方法------------------------------------//

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
        [NonAction]
        private BaseVM CreateVM(Type VMType, object Id = null, object[] Ids = null, Dictionary<string, object> values = null, bool passInit = false)
        {
            //通过反射创建ViewModel并赋值
            var ctor = VMType.GetConstructor(Type.EmptyTypes);
            BaseVM rv = ctor.Invoke(null) as BaseVM;
            try
            {
                rv.Session = new SessionServiceProvider(HttpContext.Session);
            }
            catch { }
            rv.ConfigInfo = ConfigInfo;
            rv.Cache = Cache;
            rv.LoginUserInfo = LoginUserInfo;
            rv.DataContextCI = ConfigInfo.ConnectionStrings.Where(x => x.Key.ToLower() == CurrentCS.ToLower()).Select(x=>x.DcConstructor).FirstOrDefault();
            rv.DC = this.DC;
            rv.MSD = new ModelStateServiceProvider(ModelState);
            rv.FC = new Dictionary<string, object>();
            rv.CreatorAssembly = this.GetType().AssemblyQualifiedName;
            rv.CurrentCS = CurrentCS;
            rv.CurrentUrl = this.BaseUrl;
            rv.WindowIds = "";
            rv.UIService = new DefaultUIService();
            rv.Log = this.Log;
            rv.ControllerName = this.GetType().FullName;
            rv.Localizer = this.Localizer;
            if (HttpContext != null && HttpContext.Request != null)
            {
                try
                {
                    if (Request.QueryString != null)
                    {
                        foreach (var key in Request.Query.Keys)
                        {
                            if (rv.FC.Keys.Contains(key) == false)
                            {
                                rv.FC.Add(key, Request.Query[key]);
                            }
                        }
                    }
                    var f = HttpContext.Request.Form;
                    foreach (var key in f.Keys)
                    {
                        if (rv.FC.Keys.Contains(key) == false)
                        {
                            rv.FC.Add(key, f[key]);
                        }
                    }
                }
                catch { }
            }
            //如果传递了默认值，则给vm赋值
            if (values != null)
            {
                foreach (var v in values)
                {
                    PropertyHelper.SetPropertyValue(rv, v.Key, v.Value, null, false);
                }
            }
            //如果ViewModel T继承自BaseCRUDVM<>且Id有值，那么自动调用ViewModel的GetById方法
            if (Id != null && rv is IBaseCRUDVM<TopBasePoco> cvm)
            {
                cvm.SetEntityById(Id);
            }
            //如果ViewModel T继承自IBaseBatchVM<BaseVM>，则自动为其中的ListVM和EditModel初始化数据
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
                    //绑定ListVM的OnAfterInitList事件，当ListVM的InitList完成时，自动将操作列移除
                    temp.ListVM.OnAfterInitList += (self) =>
                    {
                        self.RemoveActionColumn();
                        self.RemoveAction();
                        if (temp.ErrorMessage.Count > 0)
                        {
                            self.AddErrorColumn();
                        }
                    };
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
                //temp.ListVM?.DoSearch();
            }
            //如果ViewModel是ListVM，则初始化Searcher并调用Searcher的InitVM方法
            if (rv is IBasePagedListVM<TopBasePoco, ISearcher> lvm)
            {
                var searcher = lvm.Searcher;
                searcher.CopyContext(rv);
                if (passInit == false)
                {
                    //获取保存在Cookie中的搜索条件的值，并自动给Searcher中的对应字段赋值
                    string namePre = ConfigInfo.CookiePre + "`Searcher" + "`" + rv.VMFullName + "`";
                    Type searcherType = searcher.GetType();
                    var pros = searcherType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).ToList();
                    pros.Add(searcherType.GetSingleProperty("IsValid"));

                    Dictionary<string, string> cookieDic = HttpContext.Session.Get<Dictionary<string, string>>("SearchCondition" + searcher.VMFullName);
                    if (cookieDic != null)
                    {
                        foreach (var pro in pros)
                        {
                            var name = namePre + pro.Name;

                            if (cookieDic.ContainsKey(name) && !string.IsNullOrEmpty(cookieDic[name]))
                            {
                                try
                                {
                                    if (cookieDic[name] == "`")
                                    {
                                        pro.SetValue(searcher, null);
                                    }
                                    else
                                    {
                                        PropertyHelper.SetPropertyValue(searcher, pro.Name, cookieDic[name], null, true);
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                    searcher.DoInit();
                }
            }
            if (rv is IBaseImport<BaseTemplateVM> tvm)
            {
                var template = tvm.Template;
                template.CopyContext(rv);
                template.DoInit();
            }

            //自动调用ViewMode的InitVM方法
            if (passInit == false)
            {
                rv.DoInit();
            }
            return rv;
        }

        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <typeparam name="T">The type of the viewmodel</typeparam>
        /// <param name="Id">If the viewmodel is a BaseCRUDVM, the data having this id will be fetched</param>
        /// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        [NonAction]
        public T CreateVM<T>(object Id = null, object[] Ids = null, Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), Id, Ids, dir, passInit) as T;
        }

        /// <summary>
        /// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        /// </summary>
        /// <param name="VmFullName">the fullname of the viewmodel's type</param>
        /// <param name="Id">If the viewmodel is a BaseCRUDVM, the data having this id will be fetched</param>
        /// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        /// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        /// <returns>ViewModel</returns>
        [NonAction]
        public BaseVM CreateVM(string VmFullName, object Id = null, object[] Ids = null, bool passInit = false)
        {
            return CreateVM(Type.GetType(VmFullName), Id, Ids, null, passInit);
        }
        #endregion

        #region CreateDC
        /// <summary>
        /// Create a new datacontext with current connectionstring and current database type
        /// </summary>
        /// <param name="isLog">if true, use defaultlog connection string</param>
        /// <returns>data context</returns>
        [NonAction]
        public virtual IDataContext CreateDC(bool isLog = false)
        {
            string cs = CurrentCS ?? "default";
            if (isLog == true && ConfigInfo.ConnectionStrings?.Where(x => x.Key.ToLower() == "defaultlog").FirstOrDefault() != null)
            {
                cs = "defaultlog";
            }
            return ConfigInfo.ConnectionStrings?.Where(x => x.Key.ToLower() == cs.ToLower()).FirstOrDefault()?.CreateDC();
        }

        /// <summary>
        /// Create DataContext
        /// </summary>
        /// <param name="csName">ConnectionString key, "default" will be used if not set</param>
        /// <returns>data context</returns>
        [NonAction]
        public virtual IDataContext CreateDC(string csName)
        {
            string cs = csName ?? "default";
            return ConfigInfo.ConnectionStrings?.Where(x => x.Key.ToLower() == cs.ToLower()).FirstOrDefault()?.CreateDC();
        }

        #endregion

        #region ReInit model
        [NonAction]
        private void SetReInit(ModelStateDictionary msd, BaseVM model)
        {
            var reinit = model.GetType().GetTypeInfo().GetCustomAttributes(typeof(ReInitAttribute), false).Cast<ReInitAttribute>().SingleOrDefault();

            if (ModelState.IsValid)
            {
                if (reinit != null && (reinit.ReInitMode == ReInitModes.SUCCESSONLY || reinit.ReInitMode == ReInitModes.ALWAYS))
                {
                    model.DoReInit();
                }
            }
            else
            {
                if (reinit == null || (reinit.ReInitMode == ReInitModes.FAILEDONLY || reinit.ReInitMode == ReInitModes.ALWAYS))
                {
                    model.DoReInit();
                }
            }
        }
        #endregion

        #region Validate model
        [NonAction]
        public Dictionary<string, string> RedoValidation(object item)
        {
            Dictionary<string, string> rv = new Dictionary<string, string>();
            TryValidateModel(item);

            foreach (var e in ControllerContext.ModelState)
            {
                if (e.Value.ValidationState == ModelValidationState.Invalid)
                {
                    rv.Add(e.Key, e.Value.Errors.Select(x => x.ErrorMessage).ToSpratedString());
                }
            }

            return rv;
        }
        #endregion

        #region update viewmodel
        /// <summary>
        /// Set viewmodel's properties to the matching items posted by user
        /// </summary>
        /// <param name="vm">ViewModel</param>
        /// <param name="prefix">prefix</param>
        /// <returns>true if success</returns>
        [NonAction]
        public bool RedoUpdateModel(object vm, string prefix = null)
        {
            try
            {
                BaseVM bvm = vm as BaseVM;
                foreach (var item in bvm.FC.Keys)
                {
                    PropertyHelper.SetPropertyValue(vm, item, bvm.FC[item], prefix, true);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        [NonAction]
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

        [NonAction]
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
            var dpsSetting = GlobalServices.GetService<Configs>().DataPrivilegeSettings;
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
        protected virtual LoginUserInfo GetLoginUserInfo(Guid userId)
        {
            FrameworkUserBase userInfo = null;
            if (DC != null)
            {
                try
                {
                    userInfo = DC.Set<FrameworkUserBase>()
                                        .Include(x => x.UserRoles)
                                        .Include(x => x.UserGroups)
                                        .Where(x => x.ID == userId && x.IsValid == true)
                                        .SingleOrDefault();
                }
                catch { }
            }
            if (userInfo != null)
            {
                // 初始化用户信息
                var roleIDs = userInfo.UserRoles.Select(x => x.RoleId).ToList();
                var groupIDs = userInfo.UserGroups.Select(x => x.GroupId).ToList();
                var dataPris = DC.Set<DataPrivilege>()
                                .Where(x => x.UserId == userInfo.ID || (x.GroupId != null && groupIDs.Contains(x.GroupId.Value)))
                                .ToList();

                ProcessTreeDp(dataPris);
                //查找登录用户的页面权限
                var funcPrivileges = DC.Set<FunctionPrivilege>()
                    .Where(x => x.UserId == userInfo.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                    .ToList();

                var rv = new LoginUserInfo
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
                return rv;
            }
            else
            {
                return null;
            }

        }


    }

}
