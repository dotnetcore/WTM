
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Core
{

    /// <summary>
    /// 所有ViewModel的基类，提供了基本的功能
    /// </summary>
    public class BaseVM : IBaseVM
    {
        /// <summary>
        /// BaseVM
        /// </summary>
        public BaseVM()
        {
            FC = new Dictionary<string, object>();
        }

        /// <summary>
        /// BaseVM
        /// </summary>
        /// <param name="dc">使用的DataContext</param>
        public BaseVM(IDataContext dc)
        {
            DC = dc;
        }

        #region Property

        private Guid _uniqueId;
        /// <summary>
        /// VM实例的Id
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public string UniqueId
        {
            get
            {
                if (_uniqueId == Guid.Empty)
                {
                    _uniqueId = Guid.NewGuid();
                }
                return _uniqueId.ToNoSplitString();
            }
        }


        /// <summary>
        /// 前台传递过来的弹出窗口ID，多层弹出窗口用逗号分隔
        /// </summary>
        [JsonIgnore]
        public string WindowIds { get; set; }

        private string _viewdivid;
        /// <summary>
        /// PartialView中主Div的Id
        /// </summary>
        [JsonIgnore]
        public string ViewDivId
        {
            set { _viewdivid = value; }
            get
            {
                if (string.IsNullOrEmpty(_viewdivid))
                {
                    _viewdivid = "ViewDiv" + UniqueId;
                }
                return _viewdivid;
            }
        }

        /// <summary>
        /// 数据库环境
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public IDataContext DC { get; set; }

        /// <summary>
        /// 获取VM的全名
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public string VMFullName
        {
            get
            {
                var name = GetType().AssemblyQualifiedName;
                name = name.Substring(0, name.LastIndexOf(", Version="));
                return name;
            }
        }

        /// <summary>
        /// 获取VM所在Dll
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public string CreatorAssembly
        {
            get; set;
        }

        /// <summary>
        /// 获取当前使用的连接字符串
        /// </summary>
        [JsonIgnore]
        public string CurrentCS { get; set; }

        /// <summary>
        /// 指示是否使用固定连接字符串
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public bool FromFixedCon { get; set; }

        /// <summary>
        /// 记录Controller中传递过来的表单数据
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public Dictionary<string, object> FC { get; set; }

        /// <summary>
        /// 获取配置文件的信息
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public Configs ConfigInfo { get; set; }

        /// <summary>
        /// 获取DbContext构造函数
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public ConstructorInfo DataContextCI { get; set; }


        [JsonIgnore]
        public IUIService UIService { get; set; }

        /// <summary>
        /// 当前弹出层ID
        /// </summary>
        [JsonIgnore]
        public string CurrentWindowId
        {
            get
            {
                string rv = null;
                if (WindowIds != null)
                {
                    rv = WindowIds.Split(',').LastOrDefault();
                }

                return rv ?? "";
            }
        }

        /// <summary>
        /// 父级弹出层ID
        /// </summary>
        [JsonIgnore]
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

        [JsonIgnore]
        [BindNever]
        public object Controller { get; set; }

        [JsonIgnore]
        [BindNever]
        public IDistributedCache Cache { get; set; }

        /// <summary>
        /// 当前登录人信息
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public LoginUserInfo LoginUserInfo { get; set; }

        /// <summary>
        /// 当前Url
        /// </summary>
        [JsonIgnore]
        public string CurrentUrl { get; set; }

        /// <summary>
        /// Session信息
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public ISessionService Session { get; set; }

        /// <summary>
        /// Controller传递过来的ModelState信息
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public IModelStateService MSD { get; set; }

        /// <summary>
        /// 日志信息
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public SimpleLog Log { get; set; }

        /// <summary>
        /// 用于保存删除的附件ID
        /// </summary>
        [JsonIgnore]
        public List<Guid> DeletedFileIds { get; set; }

        [JsonIgnore]
        public string ControllerName { get; set; }

        [JsonIgnore]
        [BindNever]
        public IStringLocalizer Localizer { get; set; }
        #endregion

        #region Event

        /// <summary>
        /// InitVM完成后触发的事件
        /// </summary>
        public event Action<IBaseVM> OnAfterInit;
        /// <summary>
        /// ReInitVM完成后触发的事件
        /// </summary>
        public event Action<IBaseVM> OnAfterReInit;

        #endregion

        #region Method

        /// <summary>
        /// 调用 InitVM 并触发 OnAfterInit 事件
        /// </summary>
        public void DoInit()
        {
            InitVM();
            OnAfterInit?.Invoke(this);
        }

        /// <summary>
        /// 调用 ReInitVM 并触发 OnAfterReInit 事件
        /// </summary>
        public void DoReInit()
        {
            ReInitVM();
            OnAfterReInit?.Invoke(this);
        }



        /// <summary>
        /// 初始化ViewModel，框架会在创建VM实例之后自动调用本函数
        /// </summary>
        protected virtual void InitVM()
        {
        }

        /// <summary>
        /// 从新初始化ViewModel，框架会在验证失败时自动调用本函数
        /// </summary>
        protected virtual void ReInitVM()
        {
            InitVM();
        }

        /// <summary>
        /// 验证函数，MVC会在提交数据的时候自动调用本函数
        /// </summary>
        /// <returns></returns>
        public virtual void Validate()
        {
            return;
        }

        /// <summary>
        /// 将源VM的上数据库上下文，Session，登录用户信息，模型状态信息，缓存信息等内容复制到本VM中
        /// </summary>
        /// <param name="vm">复制的源</param>
        public void CopyContext(BaseVM vm)
        {
            DC = vm.DC;
            FC = vm.FC;
            CurrentCS = vm.CurrentCS;
            CreatorAssembly = vm.CreatorAssembly;
            MSD = vm.MSD;
            Session = vm.Session;
            ConfigInfo = vm.ConfigInfo;
            DataContextCI = vm.DataContextCI;
            UIService = vm.UIService;
            LoginUserInfo = vm.LoginUserInfo;
            Localizer = vm.Localizer;
        }

        /// <summary>
        /// Create DbContext
        /// </summary>
        /// <param name="csName">ConnectionString key, "default" will be used if not set</param>
        /// <param name="dbtype">DataBase type, appsettings dbtype will be used if not set</param>
        /// <returns>data context</returns>
        public virtual IDataContext CreateDC(string csName = null, DBTypeEnum? dbtype = null)
        {
            if (string.IsNullOrEmpty(csName))
            {
                csName = CurrentCS ?? "default";
            }
            var dbt = dbtype ?? ConfigInfo.DbType;
            return (IDataContext)DataContextCI?.Invoke(new object[] { ConfigInfo.ConnectionStrings.Where(x => x.Key.ToLower() == csName).Select(x => x.Value).FirstOrDefault(), dbt });
        }

        /// <summary>
        /// DoLog
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logtype"></param>
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

        #endregion

    }
}
