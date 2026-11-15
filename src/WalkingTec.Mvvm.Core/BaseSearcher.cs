using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WalkingTec.Mvvm.Core.Extensions;

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
        [JsonIgnore]
        public long Count { get; set; }
        /// <summary>
        /// 分页数
        /// </summary>
        [JsonIgnore]
        public int PageCount { get; set; }
        #endregion

        /// <summary>
        /// 记录 Controller 中的表单数据
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, object> FC { get; set; }

        /// <summary>
        /// Controller传递过来的ModelState信息
        /// </summary>
        [JsonIgnore]
        public IModelStateService MSD { get => Wtm?.MSD; }

        /// <summary>
        /// 是否以纯文本方式输出数据
        /// </summary>
        public bool? IsPlainText { get; set; }
        /// <summary>
        /// 是否将枚举以字符串方式输出
        /// </summary>
        public bool? IsEnumToString { get; set; }
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

        private IDataContext _dc;
        /// <summary>
        /// 数据库环境
        /// </summary>
        [JsonIgnore]
        public IDataContext DC
        {
            get
            {
                if (_dc == null)
                {
                    return Wtm?.DC;
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
        /// Session信息
        /// </summary>
        [JsonIgnore]
        public ISessionService Session { get => Wtm?.Session; }

        /// <summary>
        /// 当前登录人信息
        /// </summary>
        [JsonIgnore]
        public LoginUserInfo LoginUserInfo { get => Wtm?.LoginUserInfo; }

        /// <summary>
        /// PartialView中主Div的Id
        /// </summary>
        [JsonIgnore]
        public string ViewDivId { get; set; }
        #region 未使用
        /// <summary>
        /// 排序信息
        /// </summary>
        public SortInfo SortInfo { get; set; }

        /// <summary>
        /// 前台搜索框是否展开
        /// </summary>
        [JsonIgnore]
        public bool? IsExpanded { get; set; }

        private Guid _uniqueId;
        /// <summary>
        /// Searcher实例的Id
        /// </summary>
        [JsonIgnore]
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
        /// 框架上下文，保存当前请求相关的环境信息（数据上下文、Session、登录用户、缓存等）
        /// </summary>
        [JsonIgnore]
        public WTMContext Wtm { get; set; }
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
        /// 验证函数，MVC会在提交数据的时候自动调用本函数
        /// </summary>
        public virtual void Validate()
        {

        }
        /// <summary>
        /// 将源 VM 的 FC 等内容复制到本VM中
        /// </summary>
        /// <param name="vm"></param>
        public void CopyContext(IBaseVM vm)
        {
            FC = vm.FC;
            this.Wtm = vm.Wtm;
            this.ViewDivId = vm.ViewDivId;
        }

        #endregion

    }
}
