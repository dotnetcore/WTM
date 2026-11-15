using System;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    /// <summary>
    /// 角色的精简传输类，保存角色的基础信息，用于登录用户信息缓存和 API 输出
    /// </summary>
    [Serializable]
    public class SimpleRole
    {
        /// <summary>
        /// 主键 ID
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 角色编码
        /// </summary>
        public string RoleCode { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 租户标识
        /// </summary>
        public string Tenant { get; set; }

    }
}
