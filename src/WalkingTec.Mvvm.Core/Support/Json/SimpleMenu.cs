using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    [Serializable]
    public class SimpleMenu
    {
        public Guid ID { get; set; }

        public Guid? ActionId { get; set; }

        public bool? IsPublic { get; set; }

        public string Url { get; set; }

        public Guid? ParentId { get; set; }

        public string PageName { get; set; }

        public int? DisplayOrder { get; set; }

        public string Icon { get; set; }

        public bool ShowOnMenu { get; set; }
        public bool? IsInside { get; set; }
        public bool FolderOnly { get; set; }
        public string MethodName { get; set; }
        public bool? TenantAllowed { get; set; }
        public bool IsParentShowOnMenu(List<SimpleMenu> all)
        {
            if(this.ParentId == null)
            {
                return ShowOnMenu;
            }
            else
            {
                var p = all.Where(x => x.ID == this.ParentId).FirstOrDefault();
                if(p == null)
                {
                    return false;
                }
                else
                {
                    return p.IsParentShowOnMenu(all);
                }
            }
        }

        public  int GetLevel(List<SimpleMenu> all)
        {
            int level = 0;
            var self = this;
            while (self.ParentId != null)
            {
                level++;
                self = all.Where(x=>x.ID == self.ParentId).FirstOrDefault();
            }
            return level;
        }


    }

    public class SimpleMenuApi
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }

        public bool ShowOnMenu { get; set; }
    }

    public class LayUIMenu
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        /// <summary>
        /// Name
        /// 默认用不上name，但是 v1.2.1 有问题：“默认展开了所有节点，并将所有子节点标蓝”
        /// </summary>
        /// <value></value>
        [JsonPropertyName("name")]
        public string Name => Title;

        /// <summary>
        /// Title
        /// </summary>
        /// <value></value>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        /// <value></value>
        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// 是否展开节点
        /// </summary>
        /// <value></value>
        [JsonPropertyName("spread")]
        public bool? Expand { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        /// <value></value>
        [JsonPropertyName("jump")]
        public string Url { get; set; }

        [JsonPropertyName("list")]
        public List<LayUIMenu> Children { get; set; }

        /// <summary>
        /// order
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public int? Order { get; set; }

    }

}
