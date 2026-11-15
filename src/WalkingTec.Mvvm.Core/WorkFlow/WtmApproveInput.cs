namespace WalkingTec.Mvvm.Core.WorkFlow
{
    /// <summary>
    /// WTM 审批输入参数，审批人提交审批意见时由前端传入，用于恢复审批活动的执行
    /// </summary>
    public class WtmApproveInput
    {
        /// <summary>
        /// 审批动作，取值为“同意”或“拒绝”
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 执行审批的当前登录用户信息
        /// </summary>
        public LoginUserInfo CurrentUser { get; set; }
        /// <summary>
        /// 审批备注
        /// </summary>
        public string Remark { get; set; }
    }
}
