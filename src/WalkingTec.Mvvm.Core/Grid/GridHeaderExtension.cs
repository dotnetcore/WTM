using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// GridColumn Extension
    /// </summary>
    public static class GridHeaderExtension
    {
        /// <summary>
        /// 创建表头
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="self"></param>
        /// <param name="columnExp">绑定猎头表达式</param>
        /// <param name="width">宽度</param>
        /// <returns></returns>
        public static GridColumn<T> MakeGridHeader<T, V>(this IBasePagedListVM<T, V> self
            , Expression<Func<T, object>> columnExp
            , int? width = null
        )
            where T : TopBasePoco
            where V : ISearcher
        {
            //从表达式中提取MemberExpression，兼容直接传入成员表达式和Lambda表达式两种形式
            MemberExpression me = null;
            if (columnExp is MemberExpression)
            {
                me = columnExp as MemberExpression;
            }
            else if (columnExp is LambdaExpression)
            {
                var le = columnExp as LambdaExpression;
                if (le.Body is MemberExpression)
                {
                    me = le.Body as MemberExpression;
                }
            }
            //默认居中对齐；字符串类型的列自动改为左对齐
            var alignType = GridColumnAlignEnum.Center;
            if (me != null)
            {
                var propType = me.Type;
                if (propType == typeof(string))
                {
                    alignType = GridColumnAlignEnum.Left;
                }
            }

            return new GridColumn<T>(columnExp, width) { ColumnType = GridColumnTypeEnum.Normal, Align = alignType };
        }

        /// <summary>
        /// 创建一个间隙列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static GridColumn<T> MakeGridHeaderSpace<T, V>(this IBasePagedListVM<T, V> self)
            where T : TopBasePoco
            where V : ISearcher
        {
            return new GridColumn<T>() { ColumnType = GridColumnTypeEnum.Space };
        }

        /// <summary>
        /// 创建一个父级表头
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="self"></param>
        /// <param name="title">标题</param>
        /// <returns></returns>
        public static GridColumn<T> MakeGridHeaderParent<T, V>(this IBasePagedListVM<T, V> self, string title
        )
            where T : TopBasePoco
            where V : ISearcher
        {
            return new GridColumn<T>() { Title = title };
        }

        /// <summary>
        /// 创建一个操作列，默认固定在表格右侧，用于放置行级动作按钮
        /// </summary>
        /// <typeparam name="T">继承自TopBasePoco的类</typeparam>
        /// <typeparam name="V">继承自ISearcher的类</typeparam>
        /// <param name="self">self</param>
        /// <param name="title">列标题，不指定时使用系统默认的“操作”文字</param>
        /// <param name="width">列宽，默认160</param>
        /// <param name="rowspan">纵向跨越的单元格数</param>
        /// <returns>操作列</returns>
        public static GridColumn<T> MakeGridHeaderAction<T, V>(this IBasePagedListVM<T, V> self
            , string title = null
            , int? width = 160
            , int? rowspan = null
        )
            where T : TopBasePoco
            where V : ISearcher
        {
            return new GridColumn<T>()
            {
                ColumnType = GridColumnTypeEnum.Action,
                Width = width,
                Fixed = GridColumnFixedEnum.Right,
                Title = title ?? CoreProgram._localizer?["Sys.Operation"]
            };
        }

        #region GridColumn Property Setter

        /// <summary>
        /// 设定字段名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="field">字段名的设定非常重要，是表格数据列的唯一标识，默认属性对应的名字</param>
        /// <returns></returns>
        public static GridColumn<T> SetField<T>(this GridColumn<T> self, string field)
            where T : TopBasePoco
        {
            self.Field = field;
            return self;
        }

        /// <summary>
        /// 设定标题名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="title">即表头各列的标题，默认属性对应的 DisplayName 或 属性名 </param>
        /// <returns></returns>
        public static GridColumn<T> SetTitle<T>(this GridColumn<T> self, string title)
            where T : TopBasePoco
        {
            self.Title = title;
            return self;
        }

        /// <summary>
        /// 设定列宽
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="width">列宽的设定也通常是必须的（“特殊列”除外，如：复选框列、工具列等），它关系到表格的整体美观程度。</param>
        /// <returns></returns>
        public static GridColumn<T> SetWidth<T>(this GridColumn<T> self, int width)
            where T : TopBasePoco
        {
            self.Width = width;
            return self;
        }
        /// <summary>
        /// 单元格事件名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="eEvent"></param>
        /// <returns></returns>
        public static GridColumn<T> SetEvent<T>(this GridColumn<T> self, string eEvent) where T : TopBasePoco
        {
            self.Event = eEvent;
            return self;
        }

        /// <summary>
        /// 设定是否允许排序 (ASCII码排序)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="sort">如果设置 true，则在对应的表头显示排序icon，从而对列开启排序功能。</param>
        /// <returns></returns>
        public static GridColumn<T> SetSort<T>(this GridColumn<T> self, bool sort = true)
            where T : TopBasePoco
        {
            self.Sort = sort;
            return self;
        }

        /// <summary>
        /// 设定是否固定列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="fixed">如果设置 Left 或 Right，则对应的列将会被固定在左或右，不随滚动条而滚动。</param>
        /// <returns></returns>
        public static GridColumn<T> SetFixed<T>(this GridColumn<T> self, GridColumnFixedEnum? @fixed)
            where T : TopBasePoco
        {
            self.Fixed = @fixed;
            return self;
        }

        /// <summary>
        /// 设定对齐方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="align"></param>
        /// <returns></returns>
        public static GridColumn<T> SetAlign<T>(this GridColumn<T> self, GridColumnAlignEnum align)
            where T : TopBasePoco
        {
            self.Align = align;
            return self;
        }

        /// <summary>
        /// 该方法已被弃用，请使用SetHide代替，调用后不产生任何效果
        /// </summary>
        /// <typeparam name="T">列表的数据源类</typeparam>
        /// <param name="self">目标列</param>
        /// <param name="hidden">是否隐藏</param>
        /// <returns>列对象</returns>
        [Obsolete("该方法已经被弃用，请使用 SetHide 代替")]
        public static GridColumn<T> SetHidden<T>(this GridColumn<T> self, bool hidden = true) where T : TopBasePoco
        {
            return self;
        }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="hide"></param>
        /// <returns></returns>
        public static GridColumn<T> SetHide<T>(this GridColumn<T> self, bool hide = true) where T : TopBasePoco
        {
            self.Hide = hide;
            return self;
        }

        /// <summary>
        /// 设定列宽不可改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="unresize"></param>
        /// <returns></returns>
        public static GridColumn<T> SetUnResize<T>(this GridColumn<T> self, bool unresize = true)
            where T : TopBasePoco
        {
            self.UnResize = unresize;
            return self;
        }

        /// <summary>
        /// 设定单元格编辑类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="editType">单元格编辑类型</param>
        /// <param name="listitems">listitems</param>
        /// <param name="dateType"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public static GridColumn<T> SetEditType<T>(this GridColumn<T> self, EditTypeEnum editType = EditTypeEnum.Text, List<ComboSelectListItem> listitems = null, DateTimeTypeEnum dateType = DateTimeTypeEnum.DateTime, bool readOnly = false)
            where T : TopBasePoco
        {
            self.EditType = editType;
            self.ListItems = listitems;
            self.DateType = dateType;
            self.IsReadOnly = readOnly;
            return self;
        }

        #endregion
    }
}
