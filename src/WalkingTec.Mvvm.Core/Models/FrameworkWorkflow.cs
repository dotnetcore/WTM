using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WalkingTec.Mvvm.Core.Attributes;

namespace WalkingTec.Mvvm.Core
{
    [Table("FrameworkWorkflows")]
    public class FrameworkWorkflow : TopBasePoco, ITenant
    {
        [Required]
        [StringLength(50)]
        public string UserCode { get; set; }
        [StringLength(50)]
        public string Tag { get; set; }
        [StringLength(450)]
        public string WorkflowName { get; set; }
        [StringLength(450)]
        public string ModelType { get; set; }
        [StringLength(100)]
        public string ModelID { get; set; }
        [StringLength(50)]
        public string Submitter { get; set; }

        [Required]
        [StringLength(50)]
        public string WorkflowId { get; set; }
        [Required]
        [StringLength(50)]
        public string ActivityId { get; set; }
        [Display(Name = "_Admin.Tenant")]
        [StringLength(50)]
        public string TenantCode { get; set; }

        public DateTime? StartTime { get; set; }
    }

}
