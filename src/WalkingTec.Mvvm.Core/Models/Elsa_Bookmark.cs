using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core.Models
{
    /// <summary>
    /// Elsa 工作流引擎的书签（Bookmark）持久化实体。
    /// 书签用于记录工作流中处于挂起状态、等待外部事件或条件触发的活动，触发后工作流可从该书签处继续执行
    /// </summary>
    [Table("Bookmarks",Schema = "Elsa")]

    public class Elsa_Bookmark:TopBasePoco
    {
        /// <summary>
        /// 书签唯一标识（字符串主键）
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
        /// 书签哈希值，用于快速定位和去重书签
        /// </summary>
        [StringLength(450)]
        [Required]
        public string Hash { get; set; }

        /// <summary>
        /// 书签的序列化模型数据（JSON）
        /// </summary>
        [Required]
        public string Model { get; set; }

        /// <summary>
        /// 书签模型的类型名称
        /// </summary>
        [Required]
        public string ModelType { get; set; }

        /// <summary>
        /// 书签所属活动的类型
        /// </summary>
        [StringLength(450)]
        [Required]
        public string ActivityType { get; set; }

        /// <summary>
        /// 书签所属活动的ID
        /// </summary>
        [StringLength(450)]
        [Required]
        public string ActivityId { get; set; }

        /// <summary>
        /// 书签所属工作流实例的ID
        /// </summary>
        [StringLength(450)]
        [Required]
        public string WorkflowInstanceId { get; set; }

        /// <summary>
        /// 关联ID，用于将工作流实例与外部业务数据关联
        /// </summary>
        [StringLength(450)]
        [Required]
        public string CorrelationId { get; set; }

    }
}
