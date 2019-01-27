using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 搜索条件基类，一般和ListVM配合使用实现对ListVM的搜索功能。
    /// </summary>
    public class BaseSearcher : ISearcher
    {
        #region Property

        #region 分页相关
        /// <summary>
        /// 当前页
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// 每页数
        /// </summary>
        public int Limit { get; set; }
        /// <summary>
        /// 记录数
        /// </summary>
        public long Count { get; set; }
        /// <summary>
        /// 分页数
        /// </summary>
        public int PageCount { get; set; }
        #endregion

        /// <summary>
        /// 记录 Controller 中的表单数据
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, object> FC { get; set; }

        /// <summary>
        /// 获取VM的全名
        /// </summary>
        [JsonIgnore]
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
        /// 数据库环境
        /// </summary>
        [JsonIgnore]
        public IDataContext DC { get; set; }

        /// <summary>
        /// Session信息
        /// </summary>
        [JsonIgnore]
        public ISessionService Session { get; set; }
        /// <summary>
        /// 当前登录人信息
        /// </summary>
        [JsonIgnore]
        public LoginUserInfo LoginUserInfo
        {
            get
            {
                return Session?.Get<LoginUserInfo>("UserInfo");
            }
            set
            {
                Session?.Set("UserInfo", value);
            }

        }

        #region 未使用
        /// <summary>
        /// 排序信息
        /// </summary>
        public SortInfo SortInfo { get; set; }
        /// <summary>
        /// 是否搜索树形结构数据
        /// </summary>
        [JsonIgnore]
        public bool TreeMode { get; set; }
        /// <summary>
        /// 树形结构数据父Id
        /// </summary>
        [JsonIgnore]
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 是否有效，针对继承PersistPoco的Model
        /// </summary>
        [Display(Name = "有效")]
        public bool? IsValid { get; set; }
        /// <summary>
        /// 用于框架判断列表页是否全局刷新
        /// </summary>
        [JsonIgnore]
        public bool IsPostBack { get; set; }
        #endregion

        #endregion

        #region Event

        /// <summary>
        /// InitVM 完成后触发的事件
        /// </summary>
        public event Action<ISearcher> OnAfterInit;
        /// <summary>
        /// ReInitVM 完成后触发的事件
        /// </summary>
        public event Action<ISearcher> OnAfterReInit;

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
        /// 将源 VM 的 FC 等内容复制到本VM中
        /// </summary>
        /// <param name="vm"></param>
        public void CopyContext(IBaseVM vm)
        {
            FC = vm.FC;
            this.DC = vm.DC;
            this.Session = vm.Session;
            //var CurrentCS = vm.CurrentCS;
            //var CreatorAssembly = vm.CreatorAssembly;
        }

        #endregion

    }
}