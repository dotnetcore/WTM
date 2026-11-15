using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core.Models
{
    /// <summary>
    /// Elsa 工作流引擎的工作流实例（WorkflowInstance）持久化实体。
    /// 存储正在运行或已结束的工作流实例的状态数据
    /// </summary>
    [Table("WorkflowInstances", Schema = "Elsa")]
    public class Elsa_WorkflowInstance:TopBasePoco
    {
        /// <summary>
        /// 工作流实例唯一标识（字符串主键）
        /// </summary>
        [Key]
        [Column("Id")]
        [StringLength(450)]
        public new string ID { get; set; }

        /// <summary>
        /// 所属工作流定义的ID
        /// </summary>
        [Required]
        [StringLength(450)]
        public string DefinitionId { get; set; }

        /// <summary>
        /// 租户标识
        /// </summary>
        [StringLength(450)]
        public string TenantId { get; set; }

        /// <summary>
        /// 所属工作流定义的版本号
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 工作流实例状态（枚举整数值，如运行中、已挂起、已完成、已取消、已出错等）
        /// </summary>
        public int WorkflowStatus { get; set; }

        /// <summary>
        /// 关联ID，用于将工作流实例与外部业务数据关联
        /// </summary>
        [StringLength(450)]
        [Required]
        public string CorrelationId { get; set; }

        /// <summary>
        /// 上下文类型名称，标识实例关联的业务上下文类型
        /// </summary>
        [StringLength(450)]
        public string ContextType { get; set; }

        /// <summary>
        /// 上下文ID，标识实例关联的具体业务数据
        /// </summary>
        [StringLength(450)]
        public string ContextId { get; set; }

        /// <summary>
        /// 工作流实例名称
        /// </summary>
        [StringLength(450)]
        public string Name { get; set; }

        /// <summary>
        /// 实例创建时间
        /// </summary>
        [Required]
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// 最近一次执行时间
        /// </summary>
        public DateTimeOffset? LastExecutedAt { get; set; }

        /// <summary>
        /// 实例完成时间
        /// </summary>
        public DateTimeOffset? FinishedAt { get; set; }
        /// <summary>
        /// 实例取消时间
        /// </summary>
        public DateTimeOffset? CancelledAt { get; set; }
        /// <summary>
        /// 实例出错（故障）时间
        /// </summary>
        public DateTimeOffset? FaultedAt { get; set; }
        /// <summary>
        /// 工作流实例的完整序列化数据（JSON），包括活动状态、变量等
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 最近一次执行的活动ID
        /// </summary>
        public string LastExecutedActivityId { get; set; }
        /// <summary>
        /// 所属工作流定义版本记录的ID，对应 Elsa_WorkflowDefinition 的 ID
        /// </summary>
        [StringLength(450)]
        [Required]
        public string DefinitionVersionId { get; set; }

    }
}
