using System.Collections.Generic;
using System.Linq;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 列表项类，包含显示文本与提交值
    /// </summary>
    public class ListItem
    {
        /// <summary>
        /// The value to display
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The value to be submitted
        /// </summary>
        public string Value { get; set; }

    }

    /// <summary>
    /// 菜单项类，在列表项基础上扩展了图标字段
    /// </summary>
    public class MenuItem : ListItem
    {
        /// <summary>
        /// Icon
        /// </summary>
        /// <value></value>
        public string Icon { get; set; }
    }

    /// <summary>
    /// 下拉菜单项
    /// </summary>
    public class ComboSelectListItem : ListItem
    {
        /// <summary>
        /// Whether it is selected
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Whether it is disabled
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// ParentId
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Icon
        /// </summary>
        /// <value></value>
        public string Icon { get; set; }

    }

    /// <summary>
    /// 树形下拉菜单项
    /// </summary>
    public class TreeSelectListItem: ComboSelectListItem
    {
        /// <summary>
        /// 节点是否展开
        /// </summary>
        public bool Expended { get; set; }
        /// <summary>
        /// 节点对应的链接地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 节点标签
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 节点ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 是否为叶子节点（无子节点时为真）
        /// </summary>
        public bool Leaf => Children == null || Children.Count() == 0;
        /// <summary>
        /// 子节点集合
        /// </summary>
        public List<TreeSelectListItem> Children { get; set; }
    }

}
