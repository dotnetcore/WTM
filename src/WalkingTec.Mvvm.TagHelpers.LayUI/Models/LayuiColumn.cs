using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    /// <summary>
    /// LayuiColumnTypeEnum
    /// </summary>
    public enum LayuiColumnTypeEnum
    {
        /// <summary>
        /// 常规列，无需设定
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 复选框列
        /// </summary>
        Checkbox,
        /// <summary>
        /// 空列
        /// </summary>
        Space,
        /// <summary>
        /// 序号列
        /// </summary>
        Numbers
    }

    /// <summary>
    /// LayuiColumn
    /// </summary>
    public class LayuiColumn
    {
        #region Interface Property

        #region Special

        /// <summary>
        /// 设定复选框列
        /// <para>与 checkbox 参数搭配使用，如果设置 true，则表示复选框默认全部选中。</para>
        /// </summary>
        [JsonProperty("LAY_CHECKED")]
        public bool? LAY_CHECKED { get; set; }

        /// <summary>
        /// 绑定工具条
        /// <para>通常你需要在表格的每一行加上 查看、编辑、删除 这样类似的操作按钮，</para>
        /// <para>而 tool 参数就是为此而生，你因此可以非常便捷地实现各种操作功能。</para>
        /// <para>tool 参数和 templet 参数的使用方式完全类似，通常接受的是一个选择器，也可以是一段HTML字符。</para>
        /// </summary>
        [JsonProperty("toolbar")]
        public string Toolbar { get; set; }

        #endregion

        /// <summary>
        /// 设定列类型。
        /// 可选值有：normal（常规列，无需设定）、
        /// checkbox（复选框列）、
        /// space（空列）、
        /// numbers（序号列）。
        /// 注意：该参数为 layui 2.2.0 新增
        /// </summary>
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter), true)]
        public LayuiColumnTypeEnum? Type { get; set; }

        /// <summary>
        /// 设定字段名
        /// <para>字段名的设定非常重要，且是表格数据列的唯一标识：</para>
        /// </summary>
        [JsonProperty("field")]
        public string Field { get; set; }

        /// <summary>
        /// 设定标题名称
        /// <para>即表头各列的标题</para>
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// 设定列宽
        /// <para>列宽的设定也通常是必须的（“特殊列”除外，如：复选框列、工具列等），它关系到表格的整体美观程度。</para>
        /// </summary>
        [JsonProperty("width")]
        public int? Width { get; set; }

        /// <summary>
        /// 即横跨的单元格数，这种情况下不用设置field和width
        /// </summary>
        [JsonProperty("colspan")]
        public int? Colspan { get; set; }

        /// <summary>
        /// 即纵向跨越的单元格数
        /// </summary>
        [JsonProperty("rowspan")]
        public int? Rowspan { get; set; }

        /// <summary>
        /// 是否允许排序 (ASCII码排序)
        /// <para>如果设置 true，则在对应的表头显示排序icon，从而对列开启排序功能。</para>
        /// </summary>
        [JsonProperty("sort")]
        public bool? Sort { get; set; }

        /// <summary>
        /// 是否固定列
        /// <para>如果设置 Left 或 Right，则对应的列将会被固定在左或右，不随滚动条而滚动。</para>
        /// </summary>
        [JsonProperty("fixed")]
        [JsonConverter(typeof(StringEnumConverter), true)]
        public GridColumnFixedEnum? Fixed { get; set; }

        /// <summary>
        /// 对齐方式
        /// </summary>
        [JsonProperty("align")]
        [JsonConverter(typeof(StringEnumConverter), true)]
        public GridColumnAlignEnum Align { get; set; }

        /// <summary>
        /// 是否允许编辑
        /// <para>如果设置 true，则对应列的单元格将会被允许编辑，目前只支持type="text"的input编辑。</para>
        /// </summary>
        [JsonProperty("edit")]
        [JsonConverter(typeof(StringEnumConverter), true)]
        public EditTypeEnum? EditType { get; set; }

        /// <summary>
        /// 自定义模板
        /// <para>在默认情况下，单元格的内容是完全按照数据接口返回的content原样输出的，</para>
        /// <para>如果你想对某列的单元格添加链接等其它元素，你可以借助该参数来轻松实现。</para>
        /// <para>这是一个非常实用的功能，你的表格内容会因此而丰富多样。</para>
        /// </summary>
        [JsonProperty("templet")]
        public JRaw Templet { get; set; }

        /// <summary>
        /// 列宽不可改变 默认false
        /// </summary>
        [JsonProperty("unresize")]
        public bool? UnResize { get; set; }

        /// <summary>
        /// 隐藏列
        /// </summary>
        [JsonProperty("hide")]
        public bool? Hide { get; set; }

        [JsonProperty("style")]
        public string Style { get; set; }

        [JsonProperty("totalRow")]
        public bool? ShowTotal { get; set; }

        [JsonProperty("totalRowText")]
        public string TotalRowText { get; set; }
        #endregion
    }
}
