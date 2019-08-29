using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 列表列的定义
    /// </summary>
    /// <typeparam name="T">列表的数据源类</typeparam>
    public class GridColumn<T> : IGridColumn<T> where T : TopBasePoco
    {
        public bool? ShowTotal { get; set; }
        public GridColumn(Expression<Func<T, object>> columnExp, int? width)
        {
            ColumnExp = columnExp;
            Width = width;
        }

        /// <summary>
        /// 表头类型
        /// </summary>
        public GridColumnTypeEnum ColumnType { get; set; }
        private string _field;
        /// <summary>
        /// 设定字段名
        /// </summary>
        public string Field
        {
            get
            {
                if (_field == null)
                {
                    _field = PI?.Name;
                    if (_field == null)
                    {
                        _field = (ColumnExp?.Body as ConstantExpression)?.Value?.ToString();
                    }
                }
                return _field;
            }
            set
            {
                _field = value;
            }
        }

        private string _title;
        /// <summary>
        /// 标题名称
        /// </summary>
        public string Title
        {
            get
            {
                if (_title == null && PI != null)
                {
                    _title = PropertyHelper.GetPropertyDisplayName(PI) ?? string.Empty;
                }
                return _title;
            }
            set { _title = value; }
        }

        /// <summary>
        /// 列宽
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// 是否允许排序
        /// </summary>
        public bool? Sort { get; set; }

        /// <summary>
        /// 是否固定列
        /// </summary>
        public GridColumnFixedEnum? Fixed { get; set; }

        /// <summary>
        /// 对齐方式
        /// </summary>
        public GridColumnAlignEnum Align { get; set; }

        /// <summary>
        /// 是否可改变列宽
        /// </summary>
        public bool? UnResize { get; set; }

        /// <summary>
        /// 隐藏列
        /// </summary>
        public bool? Hide { get; set; }

        /// <summary>
        /// 子列
        /// </summary>
        public IEnumerable<IGridColumn<T>> Children { get; set; }
        private int? _childrenLen;
        /// <summary>
        /// 底层子列数量
        /// </summary>
        public int ChildrenLength
        {
            get
            {
                if (_childrenLen == null)
                {
                    var len = 0;
                    if (Children != null && Children.Count() > 0)
                    {
                        len += Children.Where(x => x.Children == null || x.Children.Count() == 0).Count();
                        var tempChildren = Children.Where(x => x.Children != null && x.Children.Count() > 0).ToList();
                        foreach (var item in tempChildren)
                        {
                            len += item.ChildrenLength;
                        }
                    }
                    _childrenLen = len;
                }
                return _childrenLen.Value;
            }
        }

        public EditTypeEnum? EditType { get; set; }

        public List<ComboSelectListItem> ListItems { get; set; }

        #region 只读属性 生成 Excel 及其 表头用

        /// <summary>
        /// 递归获取子列数量最大层的子列数量
        /// </summary>
        public int MaxChildrenCount
        {
            get
            {
                int rv = 1;
                if (this.Children != null && this.Children.Count() > 0)
                {
                    rv = 0;
                    foreach (var child in this.Children)
                    {
                        rv += child.MaxChildrenCount;
                    }
                }
                return rv;
            }
        }
        /// <summary>
        /// 获取最大层数
        /// </summary>
        public int MaxLevel
        {
            get
            {
                int rv = 1;
                if (this.Children != null && this.Children.Count() > 0)
                {
                    int max = 0;
                    foreach (var child in this.Children)
                    {
                        int temp = child.MaxLevel;
                        if (max < temp)
                        {
                            max = temp;
                        }
                    }
                    rv += max;
                }
                return rv;

            }
        }

        #endregion

        private PropertyInfo _pi;
        protected PropertyInfo PI
        {
            get
            {
                if (_pi == null && ColumnExp != null)
                {
                    _pi = PropertyHelper.GetPropertyInfo(ColumnExp);
                }
                return _pi;
            }
        }

        /// <summary>
        /// ColumnExp
        /// </summary>
        public Expression<Func<T, object>> ColumnExp { get; set; }

        private int? _maxDepth;

        /// <summary>
        /// 最大深度
        /// </summary>
        public int MaxDepth
        {
            get
            {
                if (_maxDepth == null)
                {
                    _maxDepth = 1;
                    if (Children?.Count() > 0)
                    {
                        _maxDepth += Children.Max(x => x.MaxDepth);
                    }
                }
                return _maxDepth.Value;
            }
        }

        #region 暂时没有用
        /// <summary>
        ///
        /// </summary>
        public string Id { get; set; }

        private Func<T, object> _compiledCol;
        protected Func<T, object> CompiledCol
        {
            get
            {
                if (_compiledCol == null)
                {
                    if (ColumnExp == null)
                    {
                        _compiledCol = (T) => "";
                    }
                    else
                    {
                        _compiledCol = ColumnExp.Compile();
                    }
                }
                return _compiledCol;
            }
        }


        private Type _fieldType;
        /// <summary>
        /// 获取值域类型
        /// </summary>
        /// <returns></returns>
        public Type FieldType
        {
            get
            {
                return _fieldType ?? PI?.PropertyType;
            }
            set
            {
                _fieldType = value;
            }
        }

        public string FieldName
        {
            get
            {
                return PI?.Name;
            }
        }

        /// <summary>
        /// 本列是否需要分组
        /// </summary>
        public bool NeedGroup { get; set; }
        public bool IsLocked { get; set; }
        public bool Sortable { get; set; }
        /// <summary>
        /// 是否允许换行
        /// </summary>
        public bool AllowMultiLine { get; set; }
        /// <summary>
        /// 设置某列是否应该尽量充满
        /// </summary>
        public int? Flex { get; set; }
        /// <summary>
        /// 列内容的格式化函数
        /// </summary>
        public ColumnFormatCallBack<T> Format { get; set; }

        /// <summary>
        /// 本列前景色函数
        /// </summary>
        public Func<T, string> ForeGroundFunc { get; set; }
        /// <summary>
        /// 本列背景色函数
        /// </summary>
        public Func<T, string> BackGroundFunc { get; set; }


        /// <summary>
        /// 获取最底层的子列
        /// </summary>
        public IEnumerable<IGridColumn<T>> BottomChildren
        {
            get
            {
                List<IGridColumn<T>> rv = new List<IGridColumn<T>>();
                if (Children != null && Children.Count() > 0)
                {
                    foreach (var child in Children)
                    {
                        rv.AddRange(child.BottomChildren);
                    }
                }
                else
                {
                    rv.Add(this);
                }
                return rv;
            }
        }

        /// <summary>
        /// 根据设置前景色的函数返回前景色
        /// </summary>
        /// <param name="source">源数据</param>
        /// <returns>前景色</returns>
        public string GetForeGroundColor(object source)
        {
            if (ForeGroundFunc == null)
            {
                return "";
            }
            else
            {
                return ForeGroundFunc.Invoke(source as T);
            }
        }

        /// <summary>
        /// 根据设置背景色的函数返回背景色
        /// </summary>
        /// <param name="source">源数据</param>
        /// <returns>背景色</returns>
        public string GetBackGroundColor(object source)
        {
            if (BackGroundFunc == null)
            {
                return "";
            }
            else
            {
                return BackGroundFunc.Invoke(source as T);
            }
        }

        /// <summary>
        /// 获取单元格要输出的内容
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="needFormat">是否使用format</param>
        /// <returns>Html内容</returns>
        public virtual object GetText(object source, bool needFormat = true)
        {
            object rv = null;
            var col = CompiledCol?.Invoke(source as T);
            if (Format == null || (needFormat == false && Format.Method.ReturnType != typeof(string)))
            {
                if (col == null)
                {
                    rv = null;
                }
                else if (col is DateTime dateTime)
                {
                    rv = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else if (col != null && col is DateTime?)
                {
                    rv = (col as DateTime?).Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else if (col is Enum)
                {
                    rv = (int)col;
                }
                else
                {
                    rv = col?.ToString();
                }
            }
            else
            {
                rv = Format.Invoke(source as T, col);
            }
            if (rv == null)
            {
                rv = "";
            }
            return rv;
        }

        public virtual object GetObject(object source)
        {
            object rv = CompiledCol?.Invoke(source as T);
            return rv;
        }

        /// <summary>
        /// 获取列头内容
        /// </summary>
        /// <returns>Html内容</returns>
        protected virtual string GetHeader()
        {
            string rv = PropertyHelper.GetPropertyDisplayName(PI);
            return rv == null ? "" : rv;
        }
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public GridColumn()
        {
            AllowMultiLine = true;
            this.Sortable = true;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ColumnExp"></param>
        /// <param name="Format"></param>
        /// <param name="Header"></param>
        /// <param name="Width"></param>
        /// <param name="Flex"></param>
        /// <param name="AllowMultiLine"></param>
        /// <param name="NeedGroup"></param>
        /// <param name="ForeGroundFunc"></param>
        /// <param name="BackGroundFunc"></param>
        /// <param name="sortable"></param>
        public GridColumn(Expression<Func<T, object>> ColumnExp, ColumnFormatCallBack<T> Format = null, string Header = null, int? Width = null, int? Flex = null, bool AllowMultiLine = true, bool NeedGroup = false, Func<T, string> ForeGroundFunc = null, Func<T, string> BackGroundFunc = null, bool sortable = true)
        {
            this.ColumnExp = ColumnExp;
            this.Format = Format;
            this.Title = Header;
            this.Width = Width;
            this.NeedGroup = NeedGroup;
            this.ForeGroundFunc = ForeGroundFunc;
            this.BackGroundFunc = BackGroundFunc;
            this.AllowMultiLine = AllowMultiLine;
            this.Flex = Flex;
            this.Sortable = sortable;
        }

        #endregion
    }
}
