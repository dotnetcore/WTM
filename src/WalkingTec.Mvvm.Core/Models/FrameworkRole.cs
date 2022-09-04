using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkRole
    /// </summary>
    [Table("FrameworkRoles")]
    [SoftKey(nameof(FrameworkRole.RoleCode))]
    public class FrameworkRole : BasePoco,ITenant
    {
        [Display(Name = "_Admin.RoleCode")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Validate.{0}number")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        [CanNotEdit]
        public string RoleCode { get; set; }

        [Display(Name = "_Admin.RoleName")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public string RoleName { get; set; }

        [Display(Name = "_Admin.Remark")]
        public string RoleRemark { get; set; }

        [Display(Name = "_Admin.Tenant")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TenantCode { get; set; }


        [NotMapped]
        [Display(Name = "_Admin.UsersCount")]
        public int UsersCount { get; set; }


    }
}
