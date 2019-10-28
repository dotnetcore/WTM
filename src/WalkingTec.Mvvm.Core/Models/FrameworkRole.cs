using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkRole
    /// </summary>
    [Table("FrameworkRoles")]
    public class FrameworkRole : BasePoco
    {
        [Display(Name = "RoleCode")]
        [Required(ErrorMessage = "{0}required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "{0}number")]
        [StringLength(100, ErrorMessage = "{0}stringmax{1}")]
        public string RoleCode { get; set; }

        [Display(Name = "RoleName")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        [Required(ErrorMessage = "{0}required")]
        public string RoleName { get; set; }

        [Display(Name = "Remark")]
        public string RoleRemark { get; set; }

        [JsonIgnore]
        public List<FrameworkUserRole> UserRoles { get; set; }

        [NotMapped]
        [Display(Name = "UserCount")]
        public int UserCount { get; set; }
    }
}
