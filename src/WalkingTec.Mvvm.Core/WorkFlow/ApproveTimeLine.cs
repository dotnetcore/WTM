namespace WalkingTec.Mvvm.Core.WorkFlow
{
    /// <summary>
    /// 审批时间线节点，用于在页面上展示流程每个步骤的审批轨迹
    /// </summary>
    public class ApproveTimeLine
    {
        /// <summary>
        /// 时间线节点 ID（通常为活动 ID）
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 操作类型（等待审批、同意、拒绝、_start 提交流程、_finish 流程结束）
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 审批备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 待审批人显示文本（多人等待审批时使用）
        /// </summary>
        public string Approvers { get; set; }
        /// <summary>
        /// 实际审批人显示文本
        /// </summary>
        public string Approved { get; set; }

        /// <summary>
        /// 根据操作类型生成时间线节点的显示文本（只读属性）
        /// </summary>
        public string Message
        {
            get
            {
                string rv = "";
                switch (Action)
                {
                    case "等待审批":
                        rv = "等待" + this.Approvers + "审批";
                        break;
                    case "同意":
                    case "拒绝":
                        rv = this.Approved + this.Action;
                        break;
                    case "_start":
                        rv = this.Approved + "提交审批";
                        break;
                    case "_finish":
                        rv = "流程结束";
                        break;
                }
                return rv;
            }
        }
    }
}
