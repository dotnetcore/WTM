using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WalkingTec.Mvvm.Core.Attributes;

namespace WalkingTec.Mvvm.Core
{
    [Table("FrameworkUserRoles")]
    public class FrameworkUserRole : BasePoco, ITenant
    {
        [Required]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string UserCode { get; set; }
        [Required]
        [Display(Name = "_Admin.Role")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string RoleCode { get; set; }
        [Display(Name = "_Admin.Tenant")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TenantCode { get; set; }
    }
}
