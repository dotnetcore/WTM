using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 列表列的定义
    /// </summary>
    /// <typeparam name="T">列表的数据源类</typeparam>
    public class GridColumn<T> : IGridColumn<T> where T : TopBasePoco
    {
        /// <summary>
        /// 是否显示列汇总
        /// </summary>
        public bool? ShowTotal { get; set; }
        /// <summary>
        /// 构造函数，使用列表达式和列宽初始化列
        /// </summary>
        /// <param name="columnExp">列绑定的属性表达式</param>
        /// <param name="width">列宽</param>
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
                //未显式设定字段名时，优先取列表达式对应的属性名
                if (_field == null)
                {
                    _field = PI?.Name;
                    //属性名取不到时，编译列表达式并求值，将结果作为字段名
                    if (_field == null)
                    {
                        _field = CompiledCol?.Invoke(null).ToString();
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
            set
            {
                _title = value;
            }
        }

        /// <summary>
        /// 列宽
        /// </summary>
        public int? Width { get; set; }
        /// <summary>
        /// //监听单元格事件
        /// </summary>
        public string Event { get; set; }
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
        /// 是否禁止导出此列
        /// </summary>
        public bool DisableExport { get; set; }

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
                    //递归统计最底层子列数量：无子列的直接计数，有子列的递归累加
                    var len = 0;
                    if (Children != null && Children.Any())
                    {
                        len += Children.Where(x => x.Children == null || !x.Children.Any()).Count();
                        var tempChildren = Children.Where(x => x.Children != null && x.Children.Any()).ToList();
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

        /// <summary>
        /// 单元格编辑类型
        /// </summary>
        public EditTypeEnum? EditType { get; set; }

        /// <summary>
        /// 编辑类型为ComboBox时的下拉选项数据源
        /// </summary>
        public List<ComboSelectListItem> ListItems { get; set; }

        /// <summary>
        /// 编辑类型为Datetime时的日期格式
        /// </summary>
        public DateTimeTypeEnum? DateType { get; set; }

        /// <summary>
        /// 单元格是否只读
        /// </summary>
        public bool IsReadOnly { get; set; }

        #region 只读属性 生成 Excel 及其 表头用

        /// <summary>
        /// 递归获取子列数量最大层的子列数量
        /// </summary>
        public int MaxChildrenCount
        {
            get
            {
                int rv = 1;
                if (this.Children != null && Children.Any())
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
                if (this.Children != null && Children.Any())
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
        //列表达式对应的属性信息，首次访问时通过解析表达式获取并缓存
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
        //列表达式编译后的委托，用于直接读取数据源中该列的值；无表达式时返回空字符串
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

        /// <summary>
        /// 属性名称
        /// </summary>
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
        /// <summary>
        /// 是否锁定列
        /// </summary>
        public bool IsLocked { get; set; }
        /// <summary>
        /// 是否允许排序
        /// </summary>
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
                if (Children != null && Children.Any())
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
        /// 判断本列是否设置了格式化函数
        /// </summary>
        /// <returns>设置了格式化函数返回true，否则返回false</returns>
        public bool HasFormat()
        {
            if(Format != null)
            {
                return true;
            }
            else
            {
                return false;
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
            //编译列表达式，取得当前行的原始值
            var col = CompiledCol?.Invoke(source as T);
            //不需要格式化输出时，如果Format返回的是普通对象（非ColumnFormatInfo），直接返回该对象
            if(needFormat == false && Format != null)
            {
                var test = Format.Invoke(source as T, col);
                if(test is ColumnFormatInfo == false && test is List<ColumnFormatInfo> == false)
                {
                    return test??"";
                }
            }

            //没有格式化函数或不需要格式化时，按默认规则转换原始值
            if (Format == null || needFormat == false)
            {
                if (col == null)
                {
                    rv = "";
                }
                else if (col is DateTime || col is DateTime?)
                {
                    var datevalue = col as DateTime?;
                    //时间部分全为0时只输出日期，否则输出完整的日期时间
                    if(datevalue != null)
                    {
                        if (datevalue.Value.Hour == 0 && datevalue.Value.Minute == 0 && datevalue.Value.Second == 0)
                        {
                            rv = datevalue.Value.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            rv = datevalue.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                    else
                    {
                        rv = "";
                    }
                }
                else if (col.GetType().IsEnumOrNullableEnum())
                {
                    rv = col.ToString();
                }
                else if (col.GetType().Namespace.Equals("System") == false)
                {
                    //非System命名空间的复杂类型：需要原始值时序列化为Json字符串，页面显示时使用ToString
                    if (needFormat == false)
                    {
                        rv = JsonSerializer.Serialize(col, CoreProgram.DefaultJsonOption);
                    }
                    else
                    {
                        rv = col.ToString();
                    }
                }
                else
                {
                    rv = col.ToString();
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

        /// <summary>
        /// 获取单元格的原始值，不做任何格式化
        /// </summary>
        /// <param name="source">源数据</param>
        /// <returns>原始值</returns>
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
            return rv ?? "";
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
