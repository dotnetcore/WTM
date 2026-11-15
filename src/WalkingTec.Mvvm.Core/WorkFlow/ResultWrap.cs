using System;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    /// <summary>
    /// 工作流执行结果包装类，包装执行后的流程实例、活动 ID 及执行状态信息
    /// </summary>
    public class ResultWrap
    {
        /// <summary>
        /// 工作流实例包装对象
        /// </summary>
        public InstanceWrap WorkflowInstance { get; set; }

        /// <summary>
        /// 当前活动 ID
        /// </summary>
        public string ActivityId { get; set; }
        /// <summary>
        /// 执行过程中抛出的异常（执行成功时为 null）
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// 活动是否已成功执行
        /// </summary>
        public bool Executed { get; set; }
    }
}
