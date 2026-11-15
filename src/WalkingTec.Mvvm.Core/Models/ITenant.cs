namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 多租户接口，实现该接口的模型将携带租户编码，
    /// 框架可据此实现数据按租户隔离
    /// </summary>
    public interface ITenant
    {
        /// <summary>
        /// 租户编码，标识数据所属的租户；为空表示不属于任何租户
        /// </summary>
        string TenantCode { get; set; }
    }
}
