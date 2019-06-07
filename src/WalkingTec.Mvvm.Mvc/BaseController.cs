using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    public abstract class BaseController : Controller, IBaseController
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

        private IMemoryCache _cache;
        protected IMemoryCache Cache
        {
            get
            {
                if (_cache == null)
                {
                    _cache = (IMemoryCache)HttpContext.RequestServices.GetService(typeof(IMemoryCache));
                }
                return _cache;
            }
        }

        public BaseController()
        {
        }

        public string CurrentCS { get; set; }
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

        #region 数据库环境（属性）
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

        #region 域（属性）
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

        #region 当前用户（属性）
        public LoginUserInfo LoginUserInfo
        {
            get
            {
                return HttpContext.Session?.Get<LoginUserInfo>("UserInfo");
            }
            set
            {
                HttpContext.Session?.Set<LoginUserInfo>("UserInfo", value);
            }
        }
        #endregion

        #region GUID（属性）
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

        #region 菜单 （属性）
        public List<FrameworkMenu> FFMenus => GlobaInfo.AllMenus;
        #endregion


        #region URL
        public string BaseUrl { get; set; }
        #endregion

        public ActionLog Log { get; set; }

        //-------------------------------------------方法------------------------------------//

        #region CreateVM
        /// <summary>
        /// 创建一个ViewModel，将Controller层的Session，cache，DC等信息传递给ViewModel
        /// </summary>
        /// <param name="VMType">ViewModel的类</param>
        /// <param name="Id">ViewModel的Id，如果有Id，则自动获取该Id的数据</param>
        /// <param name="Ids">如果VM为BatchVM，则自动将Ids赋值</param>
        /// <param name="values"></param>
        /// <param name="passInit"></param>
        /// <returns>创建的ViewModel</returns>
        private BaseVM CreateVM(Type VMType, Guid? Id = null, Guid[] Ids = null, Dictionary<string, object> values = null, bool passInit = false)
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
            rv.DataContextCI = GlobaInfo?.DataContextCI;
            rv.DC = this.DC;
            rv.MSD = new ModelStateServiceProvider(ModelState);
            rv.FC = new Dictionary<string, object>();
            rv.CreatorAssembly = this.GetType().AssemblyQualifiedName;
            rv.CurrentCS = CurrentCS;
            rv.CurrentUrl = this.BaseUrl;
            rv.WindowIds = this.WindowIds;
            rv.UIService = this.UIService;
            rv.Log = this.Log;
            rv.ControllerName = this.GetType().FullName;
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
                cvm.SetEntityById(Id.Value);
            }
            //如果ViewModel T继承自IBaseBatchVM<BaseVM>，则自动为其中的ListVM和EditModel初始化数据
            if (rv is IBaseBatchVM<BaseVM> temp)
            {
                temp.Ids = Ids;
                if (temp.ListVM != null)
                {
                    temp.ListVM.CopyContext(rv);
                    temp.ListVM.Ids = Ids == null ? new List<Guid>() : Ids.ToList();
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
            //如果ViewModel是ListVM，则初始化Searcher并调用Searcher的InitVM方法
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

            //自动调用ViewMode的InitVM方法
            if (passInit == false)
            {
                rv.DoInit();
            }
            return rv;
        }

        /// <summary>
        /// 初始化一个新的ViewModel
        /// </summary>
        /// <typeparam name="T">VM的类</typeparam>
        /// <param name="Id">VM的主键，如果不为空则自动根据主键读取数据</param>
        /// <param name="Ids">VM的列表主键数组，针对ListVM和BatchVM等有列表的VM，如果不为空则根据数组读取数据</param>
        /// <param name="values">Lambda的表达式，使用时用类似Where条件的写法来写，比如CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b');会在新建VM后将Field1赋为a，Field2赋为b</param>
        /// <param name="passInit"></param>
        /// <returns></returns>
        public T CreateVM<T>(Guid? Id = null, Guid[] Ids = null, Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), Id, Ids, dir, passInit) as T;
        }

        public BaseVM CreateVM(string VmFullName, Guid? Id = null, Guid[] Ids = null, bool passInit = false)
        {
            return CreateVM(Type.GetType(VmFullName), Id, Ids, null, passInit);
        }
        #endregion

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
            return (IDataContext)GlobaInfo?.DataContextCI?.Invoke(new object[] { ConfigInfo?.ConnectionStrings?.Where(x => x.Key.ToLower() == cs).Select(x => x.Value).FirstOrDefault(), ConfigInfo.DbType });
        }

        #endregion

        #region 重新加载model
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

        #region 验证mode
        public bool RedoValidation(object item)
        {
            if (ControllerContext == null)
            {
                ControllerContext = new ControllerContext();
            }
            bool rv = TryValidateModel(item);
            var pros = item.GetType().GetProperties();
            foreach (var pro in pros)
            {
                if (pro.PropertyType.GetTypeInfo().IsSubclassOf(typeof(TopBasePoco)))
                {
                    if (pro.GetValue(item) is TopBasePoco bp)
                    {
                        rv = TryValidateModel(bp);
                    }
                }
                if (pro.PropertyType.GenericTypeArguments.Count() > 0)
                {
                    var ftype = pro.PropertyType.GenericTypeArguments.First();
                    if (ftype.GetTypeInfo().IsSubclassOf(typeof(TopBasePoco)))
                    {
                        if (pro.GetValue(item) is IEnumerable<TopBasePoco> list)
                        {
                            foreach (var li in list)
                            {
                                rv = RedoValidation(li);
                            }
                        }
                    }
                }
            }
            return rv;
        }
        #endregion

        #region 更新model
        /// <summary>
        /// 模拟MVC将FormCollection的值赋给ViewModel的相应字段的过程
        /// </summary>
        /// <param name="vm">ViewModel</param>
        /// <param name="prefix">prefix</param>
        /// <returns>成功返回True，失败返回False</returns>
        public bool RedoUpdateModel(object vm, string prefix = null)
        {
            try
            {
                BaseVM bvm = vm as BaseVM;
                //循环FormCollection
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

        protected T ReadFromCache<T>(string key, Func<T> setFunc,int? timeout = null)
        {
            if (Cache.TryGetValue(key, out T rv) == false)
            {
                T data = setFunc();
                if (timeout == null)
                {
                    Cache.Set(key, data);
                }
                else
                {
                    Cache.Set(key, data, DateTime.Now.AddSeconds(timeout.Value).Subtract(DateTime.Now));
                }
                return data;
            }
            else
            {
                return rv;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var ctrlActDes = context.ActionDescriptor as ControllerActionDescriptor;
            BaseUrl = $"/{ctrlActDes.ControllerName}/{ctrlActDes.ActionName}";
            BaseUrl += context.HttpContext.Request.QueryString.ToUriComponent();
            if (context.RouteData.Values["area"] != null)
            {
                BaseUrl = $"/{context.RouteData.Values["area"]}{BaseUrl}";
            }


            base.OnActionExecuting(context);
        }

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
        /// 替换默认的 Json 不添加任何外部属性
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
        /// 替换默认的 Json 不添加任何外部属性
        /// </summary>
        /// <param name="data"></param>
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        [NonAction]
        public virtual JsonResult JsonCustom(object data, JsonSerializerSettings serializerSettings)
        {
            return base.Json(data, serializerSettings);
        }

        /// <summary>
        /// 重写 Json方法
        /// 统一接口 Json 输出格式{data:{object},code:200,msg:""}
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        public override JsonResult Json(object data)
        {
            return Json(data, StatusCodes.Status200OK, SUCCESS);
        }

        /// <summary>
        /// 重写 Json方法
        /// 统一接口 Json 输出格式{data:{object},code:200,msg:""}
        /// </summary>
        /// <param name="data"></param>
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        [NonAction]
        public override JsonResult Json(object data, JsonSerializerSettings serializerSettings)
        {
            return Json(data, StatusCodes.Status200OK, SUCCESS, serializerSettings);
        }

        /// <summary>
        /// 重写 Json方法
        /// 统一接口 Json 输出格式{data:{object},code:200,msg:""}
        /// </summary>
        /// <param name="data"></param>
        /// <param name="statusCode"></param>
        /// <param name="msg"></param>
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        [NonAction]
        public virtual JsonResult Json(object data, int statusCode = StatusCodes.Status200OK, string msg = SUCCESS, JsonSerializerSettings serializerSettings = null)
        {
            return new JsonResult(new JsonResultT<object> { Msg = msg, Code = statusCode, Data = data }) { SerializerSettings = serializerSettings };
        }

        #endregion

    }

}
