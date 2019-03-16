using System.Collections.Generic;
using Newtonsoft.Json;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 下拉菜单项
    /// </summary>
    public class ComboSelectListItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }

        [JsonIgnore]
        public ComboSelectListItem Parent { get; set; }
        public string ParentPath
        {
            get
            {
                var c = this;
                string rv = "";
                while((c = c.Parent) != null)
                {
                    rv = c.Value + "," + rv;
                }
                if (rv.EndsWith(","))
                {
                    rv = rv.Remove(rv.Length - 1);
                }
                return rv;
            }
        }
        public string ChildrenUrl { get; set; }
        public List<ComboSelectListItem> Children { get; set; }
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
        public List<TreeSelectListItem> Children { get; set; }
    }
}
