namespace WalkingTec.Mvvm.Core.WorkFlow
{
    /// <summary>
    /// 审批通知接口，应用程序可实现该接口以便在审批流程开始和审批完成时接收回调通知
    /// </summary>
    public interface IApproveNotification
    {
        /// <summary>
        /// 审批流程开始（审批活动挂起等待审批）时回调
        /// </summary>
        /// <param name="info">审批相关信息</param>
        void OnStart(ApproveInfo info);
        /// <summary>
        /// 审批完成（同意或拒绝）流程恢复执行时回调
        /// </summary>
        /// <param name="info">审批相关信息</param>
        void OnResume(ApproveInfo info);
    }
}
