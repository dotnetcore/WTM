using System;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    /// <summary>
    /// 数据权限的精简传输类，保存某用户或用户组在指定数据表上的数据权限设置，
    /// 用于登录用户信息缓存和 API 输出
    /// </summary>
    [Serializable]
    public class SimpleDataPri
    {
        /// <summary>
        /// 主键 ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 用户账号(ITCode)，标识数据权限所属的用户
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// 用户组编码，标识数据权限所属的用户组
        /// </summary>
        public string GroupCode { get; set; }

        /// <summary>
        /// 数据权限针对的数据表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 数据权限关联的数据 ID
        /// </summary>
        public string RelateId { get; set; }


    }
}
