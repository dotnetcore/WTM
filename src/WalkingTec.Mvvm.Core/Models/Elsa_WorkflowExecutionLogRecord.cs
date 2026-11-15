using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core.Models
{
    /// <summary>
    /// Elsa 工作流引擎的工作流执行日志（WorkflowExecutionLogRecord）持久化实体。
    /// 记录工作流运行过程中每个活动的执行轨迹，用于跟踪和排查工作流执行情况
    /// </summary>
    [Table("WorkflowExecutionLogRecords", Schema = "Elsa")]
    public class Elsa_WorkflowExecutionLogRecord : TopBasePoco
    {
        /// <summary>
        /// 日志记录唯一标识（字符串主键）
        /// </summary>
        [Key]
        [Column("Id")]
        [StringLength(450)]
        public new string ID { get; set; }

        /// <summary>
        /// 租户标识
        /// </summary>
        [StringLength(450)]
        public string TenantId { get; set; }

        /// <summary>
        /// 所属工作流实例的ID
        /// </summary>
        [StringLength(450)]
        [Required]
        public string WorkflowInstanceId { get; set; }
        /// <summary>
        /// 活动类型
        /// </summary>
        [StringLength(450)]
        [Required]
        public string ActivityType { get; set; }

        /// <summary>
        /// 活动ID
        /// </summary>
        [StringLength(450)]
        [Required]
        public string ActivityId { get; set; }

        /// <summary>
        /// 日志记录时间
        /// </summary>
        [Required]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// 事件名称，标识活动的执行事件（如执行、完成等）
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// 日志消息内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 日志来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 附加的序列化数据（JSON），记录事件发生时的上下文信息
        /// </summary>
        public string Data { get; set; }


    }
}
