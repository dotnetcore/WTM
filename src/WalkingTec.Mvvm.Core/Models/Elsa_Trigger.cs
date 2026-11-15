using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core.Models
{
    /// <summary>
    /// Elsa 工作流引擎的触发器（Trigger）持久化实体。
    /// 触发器描述工作流定义中可触发工作流启动或恢复的活动（如消息、定时器、HTTP 请求等）
    /// </summary>
    [Table("Triggers", Schema = "Elsa")]
    public class Elsa_Trigger:TopBasePoco
    {
        /// <summary>
        /// 触发器唯一标识（字符串主键）
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
        /// 触发器哈希值，用于快速匹配和去重触发器
        /// </summary>
        [StringLength(450)]
        [Required]
        public string Hash { get; set; }

        /// <summary>
        /// 触发器的序列化模型数据（JSON）
        /// </summary>
        [Required]
        public string Model { get; set; }

        /// <summary>
        /// 触发器模型的类型名称
        /// </summary>
        [Required]
        public string ModelType { get; set; }

        /// <summary>
        /// 触发器所属活动的类型
        /// </summary>
        [StringLength(450)]
        [Required]
        public string ActivityType { get; set; }

        /// <summary>
        /// 触发器所属活动的ID
        /// </summary>
        [StringLength(450)]
        [Required]
        public string ActivityId { get; set; }

        /// <summary>
        /// 触发器所属工作流定义的ID
        /// </summary>
        [StringLength(450)]
        [Required]
        public string WorkflowDefinitionId { get; set; }

    }
}
