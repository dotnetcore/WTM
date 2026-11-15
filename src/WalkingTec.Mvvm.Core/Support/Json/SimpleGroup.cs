using System;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    /// <summary>
    /// 用户组的精简传输类，保存用户组的基础信息及树形层级关系，
    /// 用于登录用户信息缓存和 API 输出
    /// </summary>
    [Serializable]
    public class SimpleGroup
    {
        /// <summary>
        /// 主键 ID
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 用户组编码
        /// </summary>
        public string GroupCode { get; set; }
        /// <summary>
        /// 用户组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 组管理员账号
        /// </summary>
        public string Manager { get; set; }
        /// <summary>
        /// 父级用户组 ID，用于构成树形结构
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 租户标识
        /// </summary>
        public string Tenant { get; set; }
    }
}
