namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 框架全局常量定义
    /// </summary>
    public static class GlobalConstants
    {
        /// <summary>
        /// 系统缓存 Key 常量
        /// </summary>
        public static class CacheKey
        {
            /// <summary>
            /// 用户信息缓存 Key
            /// </summary>
            public const string UserInfo = "WtmUserInfo";
            /// <summary>
            /// 租户组缓存 Key
            /// </summary>
            public const string TenantGroups = "WtmTGroups";
            /// <summary>
            /// 租户角色缓存 Key
            /// </summary>
            public const string TenantRoles = "WtmTRoles";
        }
    }
}
