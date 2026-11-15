using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// Grid Column Content Fixed Enum
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GridColumnFixedEnum
    {
        /// <summary>
        /// 规定在左侧
        /// </summary>
        Left = 0,
        /// <summary>
        /// 规定在右侧
        /// </summary>
        Right
    }

    /// <summary>
    /// Grid Column Edit Type Enum
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EditTypeEnum
    {
        /// <summary>
        /// 文本
        /// </summary>
        Text,
        /// <summary>
        /// 文本框
        /// </summary>
        TextBox,
        /// <summary>
        /// 下拉框
        /// </summary>
        ComboBox,
        /// <summary>
        /// 日期时间
        /// </summary>
        Datetime,
        /// <summary>
        /// 复选框
        /// </summary>
        CheckBox
    }

    /// <summary>
    /// Grid Column Content Align Enum
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GridColumnAlignEnum
    {
        /// <summary>
        /// Center
        /// </summary>
        Center = 0,
        /// <summary>
        /// Left
        /// </summary>
        Left,
        /// <summary>
        /// Right
        /// </summary>
        Right
    }

    /// <summary>
    /// Grid Column Type Enum
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GridColumnTypeEnum
    {
        /// <summary>
        /// 正常列
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 空列
        /// </summary>
        Space,
        /// <summary>
        /// 操作列
        /// </summary>
        Action
    }

    /// <summary>
    /// IGridColumn
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGridColumn<out T>
    {
        /// <summary>
        /// 表头类型
        /// </summary>
        GridColumnTypeEnum ColumnType { get; set; }

        /// <summary>
        /// 设定字段名
        /// </summary>
        string Field { get; set; }

        /// <summary>
        /// 标题名称
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 列宽
        /// </summary>
        int? Width { get; set; }
        /// <summary>
        /// //监听单元格事件
        /// </summary>
        string Event { get; set; }
        /// <summary>
        /// 是否允许排序
        /// </summary>
        bool? Sort { get; set; }

        /// <summary>
        /// 是否固定列
        /// </summary>
        GridColumnFixedEnum? Fixed { get; set; }

        /// <summary>
        /// 对齐方式
        /// </summary>
        GridColumnAlignEnum Align { get; set; }

        /// <summary>
        /// 是否可改变列宽
        /// </summary>
        bool? UnResize { get; set; }

        /// <summary>
        /// 隐藏列
        /// </summary>
        bool? Hide { get; set; }

        /// <summary>
        /// 是否显示汇总
        /// </summary>
        bool? ShowTotal { get; set; }

        /// <summary>
        /// 子列
        /// </summary>
        IEnumerable<IGridColumn<T>> Children { get; }

        /// <summary>
        /// 底层子列数量
        /// </summary>
        int ChildrenLength { get; }

        /// <summary>
        /// 单元格编辑类型
        /// </summary>
        EditTypeEnum? EditType { get; set; }

        /// <summary>
        /// 编辑类型为ComboBox时的下拉选项数据源
        /// </summary>
        List<ComboSelectListItem> ListItems { get; set; }

        /// <summary>
        /// 编辑类型为Datetime时的日期格式
        /// </summary>
        DateTimeTypeEnum? DateType { get; set; }

        /// <summary>
        /// 单元格是否只读
        /// </summary>
        bool IsReadOnly { get; set; }

        #region 只读属性 生成 Excel 及其 表头用

        /// <summary>
        /// 最大子列数量
        /// </summary>
        int MaxChildrenCount { get; }

        /// <summary>
        /// 多表头的最大层数
        /// </summary>
        int MaxLevel { get; }

        /// <summary>
        /// 最下层列
        /// </summary>
        IEnumerable<IGridColumn<T>> BottomChildren { get; }

        /// <summary>
        /// 最大深度
        /// </summary>
        int MaxDepth { get; }

        #endregion


        #region 暂时没有用

        /// <summary>
        /// 列Id
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// 是否需要分组
        /// </summary>
        bool NeedGroup { get; set; }

        /// <summary>
        /// 是否锁定列
        /// </summary>
        bool IsLocked { get; set; }

        /// <summary>
        /// 是否允许排序
        /// </summary>
        bool Sortable { get; set; }
        /// <summary>
        /// 是否允许多行
        /// </summary>
        bool AllowMultiLine { get; set; }
        /// <summary>
        /// 是否填充
        /// </summary>
        int? Flex { get; set; }

        /// <summary>
        /// 获取列值域类型
        /// </summary>
        Type FieldType { get; }

        /// <summary>
        /// 获取属性名称
        /// </summary>
        string FieldName { get; }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="needFormat">是否适用format</param>
        /// <returns>内容</returns>
        object GetText(object source, bool needFormat = true);

        /// <summary>
        /// 获取单元格的原始值，不做任何格式化
        /// </summary>
        /// <param name="source">源数据</param>
        /// <returns>原始值</returns>
        object GetObject(object source);
        /// <summary>
        /// 获取前景色
        /// </summary>
        /// <param name="source">源数据</param>
        /// <returns>前景色</returns>
        string GetForeGroundColor(object source);
        /// <summary>
        /// 获取背景色
        /// </summary>
        /// <param name="source">源数据</param>
        /// <returns>背景色</returns>
        string GetBackGroundColor(object source);
        /// <summary>
        /// 判断本列是否设置了格式化函数
        /// </summary>
        /// <returns>设置了格式化函数返回true，否则返回false</returns>
        bool HasFormat();
        #endregion
    }

}
