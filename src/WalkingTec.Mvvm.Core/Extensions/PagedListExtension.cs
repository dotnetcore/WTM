using System;
using System.Linq.Expressions;

namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// 分页列表 VM 相关的扩展函数，用于快速生成表格列定义
    /// </summary>
    public static class PagedListExtension
    {
        #region MakeGridColumn  生成GridColumn
        /// <summary>
        /// 生成GridColumn
        /// </summary>
        /// <typeparam name="T">继承自TopBasePoco的类</typeparam>
        /// <typeparam name="V">继承自ISearcher的类</typeparam>
        /// <param name="self">self</param>
        /// <param name="ColumnExp">指向T中字段的表达式</param>
        /// <param name="Format">格式化显示内容的委托函数，函数接受两个参数，第一个是整行数据，第二个是所选列的数据</param>
        /// <param name="Header">表头名称</param>
        /// <param name="Width">列宽</param>
        /// <param name="Flex">是否填充</param>
        /// <param name="AllowMultiLine">是否允许多行</param>
        /// <param name="NeedGroup">是否需要分组</param>
        /// <param name="ForeGroundFunc">设置前景色的委托函数</param>
        /// <param name="BackGroundFunc">设置背景色的委托函数</param>
        /// <returns>返回设置好的GridColumn类的实例</returns>
        public static GridColumn<T> MakeGridColumn<T, V>(this IBasePagedListVM<T, V> self
            , Expression<Func<T, object>> ColumnExp
            , ColumnFormatCallBack<T> Format = null
            , string Header = null
            , int? Width = null
            , int? Flex = null
            , bool AllowMultiLine = true
            , bool NeedGroup = false
            , Func<T, string> ForeGroundFunc = null
            , Func<T, string> BackGroundFunc = null)
            where T : TopBasePoco
            where V : ISearcher
        {
            GridColumn<T> rv = new GridColumn<T>(ColumnExp, Format, Header, Width, Flex, AllowMultiLine, NeedGroup, ForeGroundFunc, BackGroundFunc);
            return rv;
        }
        #endregion

    }
}