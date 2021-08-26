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
    public class FrameworkRole : BasePoco
    {
        [Display(Name = "_Admin.RoleCode")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Validate.{0}number")]
        [StringLength(100, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string RoleCode { get; set; }

        [Display(Name = "_Admin.RoleName")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public string RoleName { get; set; }

        [Display(Name = "_Admin.Remark")]
        public string RoleRemark { get; set; }

        [Display(Name = "_Admin.Tenant")]
        public string TenantCode { get; set; }


        [NotMapped]
        [Display(Name = "_Admin.UsersCount")]
        public int UsersCount { get; set; }


    }
}
