using System;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    /// <summary>
    /// 页面功能权限的精简传输类，保存角色对某个菜单页面(或页面内指定 Url)的访问权限，
    /// 用于登录用户信息缓存和 API 输出
    /// </summary>
    [Serializable]
    public class SimpleFunctionPri
    {
        /// <summary>
        /// 主键 ID
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 角色编码
        /// </summary>
        public string RoleCode{ get; set; }

        /// <summary>
        /// 关联的菜单项 ID
        /// </summary>
        public Guid MenuItemId { get; set; }

        /// <summary>
        /// 权限针对的页面 Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 是否允许访问；为 null 时表示未设置
        /// </summary>
        public bool? Allowed { get; set; }

    }
}
