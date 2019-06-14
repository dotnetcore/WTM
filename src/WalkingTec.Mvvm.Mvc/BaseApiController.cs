using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Implement;

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


        public string CurrentCS { get; set; }


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
        [NonAction]
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
            rv.WindowIds = "";
            rv.UIService = new DefaultUIService();
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
                temp.ListVM?.DoSearch();
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
                    pros.Add(searcherType.GetProperty("IsValid"));

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
        /// 初始化一个新的ViewModel
        /// </summary>
        /// <typeparam name="T">VM的类</typeparam>
        /// <param name="Id">VM的主键，如果不为空则自动根据主键读取数据</param>
        /// <param name="Ids">VM的列表主键数组，针对ListVM和BatchVM等有列表的VM，如果不为空则根据数组读取数据</param>
        /// <param name="values">Lambda的表达式，使用时用类似Where条件的写法来写，比如CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b');会在新建VM后将Field1赋为a，Field2赋为b</param>
        /// <returns></returns>
        [NonAction]
        public T CreateVM<T>(Guid? Id = null, Guid[] Ids = null, Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        {
            SetValuesParser p = new SetValuesParser();
            var dir = p.Parse(values);
            return CreateVM(typeof(T), Id, Ids, dir, passInit) as T;
        }

        [NonAction]
        public BaseVM CreateVM(string VmFullName, Guid? Id = null, Guid[] Ids = null, bool passInit = false)
        {
            return CreateVM(Type.GetType(VmFullName), Id, Ids, null, passInit);
        }
        #endregion

        #region CreateDC
        [NonAction]
        public virtual IDataContext CreateDC(bool isLog = false)
        {
            string cs = CurrentCS;
            if (isLog == true && ConfigInfo.ConnectionStrings?.Where(x => x.Key.ToLower() == "defaultlog").FirstOrDefault() != null)
            {
                cs = "defaultlog";
            }
            return (IDataContext)GlobaInfo?.DataContextCI?.Invoke(new object[] { ConfigInfo?.ConnectionStrings?.Where(x => x.Key.ToLower() == cs).Select(x => x.Value).FirstOrDefault(), ConfigInfo.DbType });
        }

        #endregion

        #region 重新加载model
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

        #region 验证mode
        [NonAction]
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
        [NonAction]
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

        [NonAction]
        protected T ReadFromCache<T>(string key, Func<T> setFunc, int? timeout = null)
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

        [NonAction]
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
