using System;
using Elsa.Models;

namespace WalkingTec.Mvvm.Core.WorkFlow
{
    /// <summary>
    /// Elsa 工作流实例的包装类，使用 new 关键字隐藏并重新声明基类的各时间字段，
    /// 便于在 WTM 中以可写属性的方式使用和序列化这些时间信息
    /// </summary>
    public class InstanceWrap : WorkflowInstance
    {
        /// <summary>
        /// 流程实例创建时间
        /// </summary>
        public new DateTime CreatedAt { get; set; }
        /// <summary>
        /// 流程实例最后一次执行时间
        /// </summary>
        public new DateTime? LastExecutedAt { get; set; }
        /// <summary>
        /// 流程实例完成时间
        /// </summary>
        public new DateTime? FinishedAt { get; set; }
        /// <summary>
        /// 流程实例取消时间
        /// </summary>
        public new DateTime? CancelledAt { get; set; }
        /// <summary>
        /// 流程实例出错时间
        /// </summary>
        public new DateTime? FaultedAt { get; set; }

    }
}
