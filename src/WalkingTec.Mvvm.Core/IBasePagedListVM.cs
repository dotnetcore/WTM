using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// ListVM接口
    /// </summary>
    /// <typeparam name="T">ListVM中的Model类</typeparam>
    /// <typeparam name="S">ListVM使用的Searcher类</typeparam>
    public interface IBasePagedListVM<out T, out S> : IBaseVM
        where T : TopBasePoco
        where S : ISearcher
    {

        /// <summary>
        /// 多级表头深度  默认 1级
        /// </summary>
        int ChildrenDepth { get; }

        /// <summary>
        /// GetHeaders
        /// </summary>
        /// <returns></returns>

        IEnumerable<IGridColumn<T>> GetHeaders();

        /// <summary>
        /// 页面动作
        /// </summary>
        List<GridAction> GridActions { get; }

        /// <summary>
        /// 查询并生成Excel
        /// </summary>
        /// <returns>Excel文件</returns>
        byte[] GenerateExcel();

        string TotalText { get; set; }
        #region Old
        event Action<IBasePagedListVM<T, S>> OnAfterInitList;
        /// <summary>
        ///记录批量操作时列表中选择的Id
        /// </summary>
        List<Guid> Ids { get; set; }

        /// <summary>
        /// 获取Model集合
        /// </summary>
        /// <returns>Model集合</returns>
        IEnumerable<T> GetEntityList();

        void ClearEntityList();
        /// <summary>
        /// 获取Searcher
        /// </summary>
        S Searcher { get; }
        /// <summary>
        /// GetIsSelected
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool GetIsSelected(object item);

        /// <summary>
        /// 每页行数
        /// </summary>
        [Obsolete("弃用，改用 DataTableHelper上的Limit")]
        int RecordsPerPage { get; set; }

        /// <summary>
        /// 是否已经搜索过
        /// </summary>
        bool IsSearched { get; set; }

        /// <summary>
        /// PassSearch
        /// </summary>
        bool PassSearch { get; set; }

        /// <summary>
        /// 搜索模式
        /// </summary>
        ListVMSearchModeEnum SearcherMode { get; set; }

        /// <summary>
        /// 是否需要分页
        /// </summary>
        bool NeedPage { get; set; }

        /// <summary>
        /// 移除操作列
        /// </summary>
        void RemoveActionColumn(object root = null);

        void RemoveAction();

        /// <summary>
        /// 填加错误信息列，用于批量操作的列表
        /// </summary>
        void AddErrorColumn();

        /// <summary>
        /// 搜索条件Panel的Id
        /// </summary>
        string SearcherDivId { get; }

        /// <summary>
        /// GetSearchQuery
        /// </summary>
        /// <returns></returns>
        IOrderedQueryable<T> GetSearchQuery();
        /// <summary>
        /// DoSearch
        /// </summary>
        void DoSearch();
        /// <summary>
        /// CopyContext
        /// </summary>
        /// <param name="vm"></param>
        void CopyContext(BaseVM vm);

        /// <summary>
        /// ReplaceWhere
        /// </summary>
        Expression<Func<TopBasePoco, bool>> ReplaceWhere { get; set; }

        /// <summary>
        /// SetFullRowColor
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        string SetFullRowColor(object entity);
        /// <summary>
        /// SetFullRowBgColor
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        string SetFullRowBgColor(object entity);

        T CreateEmptyEntity();

        /// <summary>
        /// 用于为子表生成可编辑Grid时，内部控件名称前缀
        /// </summary>
        string DetailGridPrix { get; set; }

        void DoInitListVM();
        #endregion

    }

}
