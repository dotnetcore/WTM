using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core.Models
{
    /// <summary>
    /// Elsa 工作流引擎的工作流定义（WorkflowDefinition）持久化实体。
    /// 存储工作流的版本化定义信息，同一定义可存在多个版本
    /// </summary>
    [Table("WorkflowDefinitions", Schema = "Elsa")]
    public class Elsa_WorkflowDefinition:TopBasePoco
    {
        /// <summary>
        /// 定义版本记录的唯一标识（字符串主键），每个版本对应一条记录
        /// </summary>
        [Key]
        [Column("Id")]
        [StringLength(450)]
        public new string ID { get; set; }

        /// <summary>
        /// 工作流定义ID，同一定义的不同版本共享该ID
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
        /// 工作流名称
        /// </summary>
        [StringLength(450)]
        public string Name { get; set; }

        /// <summary>
        /// 工作流显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 工作流描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 工作流版本号
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 是否单例模式，为真时同一定义同一时间只允许存在一个运行中的实例
        /// </summary>
        public bool IsSingleton { get; set; }

        /// <summary>
        /// 实例持久化行为（枚举整数值），控制工作流实例状态的保存时机
        /// </summary>
        public int PersistenceBehavior { get; set; }

        /// <summary>
        /// 工作流执行完成后是否删除其实例
        /// </summary>
        public bool DeleteCompletedInstances { get; set; }

        /// <summary>
        /// 是否已发布
        /// </summary>
        public bool IsPublished { get; set; }
        /// <summary>
        /// 是否为最新版本
        /// </summary>
        public bool IsLatest { get; set; }
        /// <summary>
        /// 自定义标签，用于对工作流分类或过滤
        /// </summary>
        [StringLength(450)]
        public string Tag { get; set; }
        /// <summary>
        /// 工作流定义的完整序列化数据（JSON），包含活动、变量、连接线等结构
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 该版本记录的创建时间
        /// </summary>
        [Required]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
