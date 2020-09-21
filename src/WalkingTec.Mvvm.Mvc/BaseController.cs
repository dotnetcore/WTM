using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Mvc
{
    public abstract class BaseController : Controller, IBaseController
    {
        public BaseController()
        {
        }

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
            set
            {
                _cache = value;
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
        private BaseVM CreateVM(Type VMType, object Id = null, object[] Ids = null, Dictionary<string, object> values = null, bool passInit = false)
        {
            //Use reflection to create viewmodel
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
            rv.DataContextCI = ConfigInfo.ConnectionStrings.Where(x => x.Key.ToLower() == CurrentCS.ToLower()).Select(x => x.DcConstructor).FirstOrDefault();
            rv.DC = this.DC;
            rv.MSD = new ModelStateServiceProvider(ModelState);
            rv.FC = new Dictionary<string, object>();
            rv.CreatorAssembly = this.GetType().AssemblyQualifiedName;
            rv.CurrentCS = CurrentCS;
            rv.CurrentUrl = this.BaseUrl;
            rv.WindowIds = this.WindowIds;
            rv.UIService = this.UIService;
            rv.Log = this.Log;
            rv.Controller = this;
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
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
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
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
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
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
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
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
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
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
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
        /// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
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

        #region CreateDC
        /// <summary>
        /// Create a new datacontext with current connectionstring and current database type
        /// </summary>
        /// <param name="isLog">if true, use defaultlog connection string</param>
        /// <returns>data context</returns>
        public virtual IDataContext CreateDC(bool isLog = false)
        {
            string cs = CurrentCS??"default";
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
            return ConfigInfo.ConnectionStrings?.Where(x => x.Key.ToLower() == cs.ToLower()).FirstOrDefault()?.CreateDC();
        }

        /// <summary>
        /// Create DataContext
        /// </summary>
        /// <param name="csName">ConnectionString key, "default" will be used if not set</param>
        /// <returns>data context</returns>
        public virtual IDataContext CreateDC(string csName)
        {
            string cs = csName ?? "default";
            return ConfigInfo.ConnectionStrings?.Where(x => x.Key.ToLower() == cs.ToLower()).FirstOrDefault()?.CreateDC();
        }


        #endregion

        #region ReInit model
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

        protected T ReadFromCache<T>(string key, Func<T> setFunc, int? timeout = null)
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

       

        [NonAction]
        public FResult FFResult()
        {
            var rv = new FResult
            {
                Controller = this
            };
            try
            {
                rv.Controller.Response.Headers.Add("IsScript", "true");
            }
            catch { }
            return rv;
        }

        #region JsonResult

        private const string SUCCESS = "success";

        /// <summary>
        /// Creates a Microsoft.AspNetCore.Mvc.JsonResult object that serializes the specified
        /// data object to JSON.
        /// </summary>
        /// <param name="data">The object to serialize.</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.JsonResult that serializes the specified
        /// data to JSON format for the response.</returns>
        [NonAction]
        public virtual JsonResult JsonCustom(object data)
        {
            return base.Json(data);
        }

        /// <summary>
        /// Creates a Microsoft.AspNetCore.Mvc.JsonResult object that serializes the specified
        /// data object to JSON.
        /// </summary>
        /// <param name="data">The object to serialize.</param>
        /// <param name="serializerSettings">settings</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.JsonResult that serializes the specified
        /// data to JSON format for the response.</returns>
        [NonAction]
        public virtual JsonResult JsonCustom(object data, JsonSerializerSettings serializerSettings)
        {
            return base.Json(data, serializerSettings);
        }

        /// <summary>
        /// override Json method
        /// output format is {data:{object},code:200,msg:""}
        /// </summary>
        /// <param name="data">The object to serialize.</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.JsonResult that serializes the specified
        /// data to JSON format for the response.</returns>
        [NonAction]
        public override JsonResult Json(object data)
        {
            return Json(data, StatusCodes.Status200OK, SUCCESS);
        }

        /// <summary>
        /// override Json method
        /// output format is {data:{object},code:200,msg:""}
        /// </summary>
        /// <param name="data">The object to serialize.</param>
        /// <param name="serializerSettings">settings</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.JsonResult that serializes the specified
        /// data to JSON format for the response.</returns>
        [NonAction]
        public override JsonResult Json(object data, JsonSerializerSettings serializerSettings)
        {
            return Json(data, StatusCodes.Status200OK, SUCCESS, serializerSettings);
        }

        /// <summary>
        /// override Json method
        /// output format is {data:{object},code:200,msg:""}
        /// </summary>
        /// <param name="data">The object to serialize.</param>
        /// <param name="statusCode">status code</param>
        /// <param name="msg">message</param>
        /// <param name="serializerSettings">settings</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.JsonResult that serializes the specified
        /// data to JSON format for the response.</returns>
        [NonAction]
        public virtual JsonResult Json(object data, int statusCode = StatusCodes.Status200OK, string msg = SUCCESS, JsonSerializerSettings serializerSettings = null)
        {
            return new JsonResult(new JsonResultT<object> { Msg = msg, Code = statusCode, Data = data }) { SerializerSettings = serializerSettings };
        }

        #endregion

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

    }

}
