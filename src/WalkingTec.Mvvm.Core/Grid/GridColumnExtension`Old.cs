using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// GridColumnExtension
    /// </summary>
    public static class GridColumnExtension
    {
        /// <summary>
        /// GetAllBottomColumns
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IEnumerable<IGridColumn<T>> GetAllBottomColumns<T>(this IEnumerable<IGridColumn<T>> self)
        {
            List<IGridColumn<T>> rv = new List<IGridColumn<T>>();
            foreach (var item in self)
            {
                rv.AddRange(item.BottomChildren);
            }
            return rv;
        }
        /// <summary>
        /// Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static GridColumn<T> SetId<T>(this GridColumn<T> self, string id) where T : TopBasePoco
        {
            self.Id = id;
            return self;
        }
        /// <summary>
        /// 列头
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static GridColumn<T> SetHeader<T>(this GridColumn<T> self, string header) where T : TopBasePoco
        {
            self.Title = header;
            return self;
        }
        /// <summary>
        /// 本列是否需要分组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="needGroup"></param>
        /// <returns></returns>
        public static GridColumn<T> SetNeedGroup<T>(this GridColumn<T> self, bool needGroup) where T : TopBasePoco
        {
            self.NeedGroup = needGroup;
            return self;
        }
        public static GridColumn<T> SetLocked<T>(this GridColumn<T> self, bool locked) where T : TopBasePoco
        {
            self.IsLocked = locked;
            return self;
        }
        public static GridColumn<T> SetSortable<T>(this GridColumn<T> self, bool sortable = false) where T : TopBasePoco
        {
            self.Sortable = sortable;
            return self;
        }
        /// <summary>
        /// 列宽
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static GridColumn<T> SetWidth<T>(this GridColumn<T> self, int? width) where T : TopBasePoco
        {
            self.Width = width;
            return self;
        }
        /// <summary>
        /// 是否允许换行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="allowMultiLine"></param>
        /// <returns></returns>
        public static GridColumn<T> SetAllowMultiLine<T>(this GridColumn<T> self, bool allowMultiLine) where T : TopBasePoco
        {
            self.AllowMultiLine = allowMultiLine;
            return self;
        }
        /// <summary>
        /// 设置某列是否应该尽量充满
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="flex"></param>
        /// <returns></returns>
        public static GridColumn<T> SetFlex<T>(this GridColumn<T> self, int? flex) where T : TopBasePoco
        {
            self.Flex = flex;
            return self;
        }
        ///// <summary>
        ///// 时间与数字类型Format
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="self"></param>
        ///// <param name="format"></param>
        ///// <returns></returns>
        //public static GridColumn<T> SetFormat<T>(this GridColumn<T> self, string format) where T : TopBasePoco
        //{
        //    self.StringFormat = format;
        //    return self;
        //}
        /// <summary>
        /// 列内容的格式化函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static GridColumn<T> SetFormat<T>(this GridColumn<T> self, ColumnFormatCallBack<T> format) where T : TopBasePoco
        {
            self.Format = format;
            return self;
        }
        /// <summary>
        /// 计算列值的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="columnExp"></param>
        /// <returns></returns>
        public static GridColumn<T> SetColumnExp<T>(this GridColumn<T> self, Expression<Func<T, object>> columnExp) where T : TopBasePoco
        {
            self.ColumnExp = columnExp;
            return self;
        }
        /// <summary>
        /// 子列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="childrens"></param>
        /// <returns></returns>
        public static GridColumn<T> SetChildren<T>(this GridColumn<T> self, params GridColumn<T>[] childrens) where T : TopBasePoco
        {
            List<GridColumn<T>> temp = new List<GridColumn<T>>();
            if (self.Children == null)
            {
                temp = new List<GridColumn<T>>();
            }
            else
            {
                temp = self.Children.Cast<GridColumn<T>>().ToList();
            }
            temp.AddRange(childrens);
            self.Children = temp;
            return self;
        }
        /// <summary>
        /// 本列前景色函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="foreGroundFunc"></param>
        /// <returns></returns>
        public static GridColumn<T> SetForeGroundFunc<T>(this GridColumn<T> self, Func<T, string> foreGroundFunc) where T : TopBasePoco
        {
            self.ForeGroundFunc = foreGroundFunc;
            return self;
        }
        /// <summary>
        /// 本列背景色函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="backGroundFunc"></param>
        /// <returns></returns>
        public static GridColumn<T> SetBackGroundFunc<T>(this GridColumn<T> self, Func<T, string> backGroundFunc) where T : TopBasePoco
        {
            self.BackGroundFunc = backGroundFunc;
            return self;
        }

        /// <summary>
        /// 设置本列是否显示汇总
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="show">是否显示</param>
        /// <returns></returns>
        public static GridColumn<T> SetShowTotal<T>(this GridColumn<T> self, bool show = true) where T : TopBasePoco
        {
            self.ShowTotal = show;
            return self;
        }

        /// <summary>
        /// 设置禁止导出此列数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static GridColumn<T> SetDisableExport<T>(this GridColumn<T> self) where T : TopBasePoco
        {
            self.DisableExport = true;
            return self;
        }
    }
}
