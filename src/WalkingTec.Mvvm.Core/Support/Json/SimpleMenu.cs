using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    /// <summary>
    /// 菜单的精简传输类，保存菜单项的基础信息及树形层级关系，
    /// 用于登录用户信息缓存和 API 输出
    /// </summary>
    [Serializable]
    public class SimpleMenu
    {
        /// <summary>
        /// 主键 ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 关联的操作(Action) ID
        /// </summary>
        public Guid? ActionId { get; set; }

        /// <summary>
        /// 是否为公开页面(无需登录即可访问)
        /// </summary>
        public bool? IsPublic { get; set; }

        /// <summary>
        /// 菜单对应的页面 Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 父级菜单 ID，用于构成树形结构
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int? DisplayOrder { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 是否显示在菜单上
        /// </summary>
        public bool ShowOnMenu { get; set; }
        /// <summary>
        /// 是否为框架内部页面
        /// </summary>
        public bool? IsInside { get; set; }
        /// <summary>
        /// 是否仅作为文件夹节点(不指向具体页面)
        /// </summary>
        public bool FolderOnly { get; set; }
        /// <summary>
        /// 关联的动作方法名称
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 租户是否可使用该菜单
        /// </summary>
        public bool? TenantAllowed { get; set; }
        /// <summary>
        /// 判断本菜单及其所有上级菜单是否均设置为在菜单上显示
        /// </summary>
        /// <param name="all">全量菜单列表，用于在其中查找父级菜单</param>
        /// <returns>若本菜单及其全部上级菜单均显示在菜单上则返回 true，否则返回 false</returns>
        public bool IsParentShowOnMenu(List<SimpleMenu> all)
        {
            // 顶层菜单直接返回自身的 ShowOnMenu 设置
            if(this.ParentId == null)
            {
                return ShowOnMenu;
            }
            else
            {
                // 在全量菜单中查找父菜单
                var p = all.Where(x => x.ID == this.ParentId).FirstOrDefault();
                // 找不到父菜单时视为不显示
                if(p == null)
                {
                    return false;
                }
                else
                {
                    // 递归判断父菜单及其上级菜单是否显示
                    return p.IsParentShowOnMenu(all);
                }
            }
        }

        /// <summary>
        /// 计算本菜单在菜单树中所处的层级(顶层菜单为 0)
        /// </summary>
        /// <param name="all">全量菜单列表，用于逐级向上查找父菜单</param>
        /// <returns>菜单的层级深度，顶层菜单返回 0</returns>
        public  int GetLevel(List<SimpleMenu> all)
        {
            // 沿 ParentId 逐级向上查找父菜单，每上升一级层级加 1
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

    /// <summary>
    /// 菜单的 API 输出模型，仅保留前端渲染菜单所需的精简字段
    /// </summary>
    public class SimpleMenuApi
    {
        /// <summary>
        /// 菜单 ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 父级菜单 ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 菜单显示文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 菜单对应的页面 Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 是否显示在菜单上
        /// </summary>
        public bool ShowOnMenu { get; set; }
    }

    /// <summary>
    /// LayUI 风格的菜单树节点模型，按 LayUI 导航组件要求的 JSON 结构输出菜单树
    /// </summary>
    public class LayUIMenu
    {
        /// <summary>
        /// 菜单 ID(不参与 JSON 序列化)
        /// </summary>
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

        /// <summary>
        /// 子菜单节点列表
        /// </summary>
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
