using System;
using System.Collections.Generic;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// ISearcher
    /// </summary>
    public interface ISearcher
    {
        #region Property

        #region 分页相关
        /// <summary>
        /// 当前页
        /// </summary>
        int Page { get; set; }
        /// <summary>
        /// 每页数
        /// </summary>
        int Limit { get; set; }
        /// <summary>
        /// 记录数
        /// </summary>
        long Count { get; set; }
        /// <summary>
        /// 分页数
        /// </summary>
        int PageCount { get; set; }
        #endregion

        /// <summary>
        /// 记录 Controller 中的表单数据
        /// </summary>
        Dictionary<string, object> FC { get; set; }

        IDataContext DC { get; set; }
        /// <summary>
        /// VMFullName
        /// </summary>
        string VMFullName { get; }

        ISessionService Session { get; set; }

        LoginUserInfo LoginUserInfo { get; set; }
        #region 未使用
        /// <summary>
        /// 排序信息
        /// </summary>
        SortInfo SortInfo { get; set; }
        /// <summary>
        /// 是否搜索树形结构数据
        /// </summary>
        bool TreeMode { get; set; }
        /// <summary>
        /// 树形结构数据父Id
        /// </summary>
        Guid? ParentId { get; set; }
        /// <summary>
        /// 是否有效，针对继承PersistPoco的Model
        /// </summary>
        bool? IsValid { get; set; }
        /// <summary>
        /// 用于框架判断列表页是否全局刷新
        /// </summary>
        bool IsPostBack { get; set; }
        #endregion

        #endregion

        #region Event

        /// <summary>
        /// InitVM 完成后触发的事件
        /// </summary>
        event Action<ISearcher> OnAfterInit;
        /// <summary>
        /// ReInitVM 完成后触发的事件
        /// </summary>
        event Action<ISearcher> OnAfterReInit;

        #endregion

        #region Method
        /// <summary>
        /// 将源 VM 的 FC 等内容复制到本VM中
        /// </summary>
        /// <param name="vm"></param>
        void CopyContext(IBaseVM vm);

        /// <summary>
        /// 调用 InitVM 并触发 OnAfterInit 事件
        /// </summary>
        void DoInit();

        /// <summary>
        /// 调用 ReInitVM 并触发 OnAfterReInit 事件
        /// </summary>
        void DoReInit();

        #endregion

    }
}
