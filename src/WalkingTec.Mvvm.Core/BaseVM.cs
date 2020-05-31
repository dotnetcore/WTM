
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

        #region Property

        public WTMContext WtmContext { get; set; }

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
        public string WindowIds { get => WtmContext?.WindowIds; }

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


        private IDataContext _dc;
        /// <summary>
        /// 数据库环境
        /// </summary>
        [JsonIgnore]
        [BindNever]       
        public IDataContext DC
        {
            get
            {
                if (_dc == null)
                {
                    return WtmContext?.DC;
                }
                else
                {
                    return _dc;
                }
            }
            set
            {
                _dc = value;
            }
        }

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
        public string CurrentCS { get => WtmContext?.CurrentCS; }

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
        public Configs ConfigInfo { get=> WtmContext?.ConfigInfo; }


        [JsonIgnore]
        public IUIService UIService { get=> WtmContext?.UIService; }

        /// <summary>
        /// 当前弹出层ID
        /// </summary>
        [JsonIgnore]
        public string CurrentWindowId { get => WtmContext?.CurrentWindowId; }

        /// <summary>
        /// 父级弹出层ID
        /// </summary>
        [JsonIgnore]
        public string ParentWindowId { get => WtmContext?.ParentWindowId; }

        [JsonIgnore]
        [BindNever]
        public object Controller { get; set; }

        [JsonIgnore]
        [BindNever]
        public IDistributedCache Cache { get => WtmContext?.Cache; }

        /// <summary>
        /// 当前登录人信息
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public LoginUserInfo LoginUserInfo { get=> WtmContext?.LoginUserInfo;}

        /// <summary>
        /// 当前Url
        /// </summary>
        [JsonIgnore]
        public string CurrentUrl { get => WtmContext?.BaseUrl; }

        /// <summary>
        /// Session信息
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public ISessionService Session { get => WtmContext?.Session; }

        /// <summary>
        /// Controller传递过来的ModelState信息
        /// </summary>
        [JsonIgnore]
        [BindNever]
        public IModelStateService MSD { get => WtmContext?.MSD; }

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
            WtmContext = vm.WtmContext;
            FC = vm.FC;
            CreatorAssembly = vm.CreatorAssembly;
        }

        #endregion

    }
}
