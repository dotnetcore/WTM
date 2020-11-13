using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    public class ListItem
    {
        /// <summary>
        /// The value to display
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The value to be submitted
        /// </summary>
        public object Value { get; set; }

    }

    public class MenuItem : ListItem
    {
        /// <summary>
        /// ICon
        /// </summary>
        /// <value></value>
        public string ICon { get; set; }
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
        /// ICon
        /// </summary>
        /// <value></value>
        public string ICon { get; set; }

    }

    /// <summary>
    /// 树形下拉菜单项
    /// </summary>
    public class TreeSelectListItem
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Text { get; set; }
        public bool Expended { get; set; }
        public string Url { get; set; }
        public string ICon { get; set; }
        public string Tag { get; set; }
        public bool Leaf => Children == null || Children.Count == 0;
        public bool Selected { get; set; }
        public List<TreeSelectListItem> Children { get; set; }
    }
}
