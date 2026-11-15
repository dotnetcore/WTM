using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 工作流待办实体，记录用户当前参与的工作流实例及所处流程节点的信息
    /// </summary>
    [Table("FrameworkWorkflows")]
    public class FrameworkWorkflow : TopBasePoco, ITenant
    {
        /// <summary>
        /// 当前待办理人的用户编码（登录账号）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string UserCode { get; set; }
        /// <summary>
        /// 待办标签，用于区分或归类同一流程实例下的多条待办记录
        /// </summary>
        [StringLength(50)]
        public string Tag { get; set; }
        /// <summary>
        /// 工作流名称（显示用）
        /// </summary>
        [StringLength(450)]
        public string WorkflowName { get; set; }
        /// <summary>
        /// 待办关联的业务模型（VM）类型全名
        /// </summary>
        [StringLength(450)]
        public string ModelType { get; set; }
        /// <summary>
        /// 待办关联的业务数据主键
        /// </summary>
        [StringLength(100)]
        public string ModelID { get; set; }
        /// <summary>
        /// 流程提交人（发起人）的用户编码
        /// </summary>
        [StringLength(50)]
        public string Submitter { get; set; }

        /// <summary>
        /// 工作流实例 ID
        /// </summary>
        [Required]
        [StringLength(50)]
        public string WorkflowId { get; set; }
        /// <summary>
        /// 当前流程活动（节点）ID
        /// </summary>
        [Required]
        [StringLength(50)]
        public string ActivityId { get; set; }
        /// <summary>
        /// 租户编码，标识该待办所属的租户
        /// </summary>
        [Display(Name = "_Admin.Tenant")]
        [StringLength(50)]
        public string TenantCode { get; set; }

        /// <summary>
        /// 流程实例的启动时间
        /// </summary>
        public DateTime? StartTime { get; set; }
    }

}
