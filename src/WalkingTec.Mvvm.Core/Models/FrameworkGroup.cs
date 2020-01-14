using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace WalkingTec.Mvvm.Core
{

    [Table("FrameworkGroups")]
    public class FrameworkGroup : BasePoco
    {
        [Display(Name = "GroupCode")]
        [Required(ErrorMessage = "{0}required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "{0}number")]
        [StringLength(100, ErrorMessage = "{0}stringmax{1}")]
        public string GroupCode { get; set; }

        [Display(Name = "GroupName")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        [Required(ErrorMessage = "{0}required")]
        public string GroupName { get; set; }

        [Display(Name = "Remark")]
        public string GroupRemark { get; set; }

        [Display(Name = "User")]
        [JsonIgnore]
        public List<FrameworkUserGroup> UserGroups { get; set; }

        [NotMapped]
        [Display(Name = "UsersCount")]
        public int UsersCount { get; set; }

    }
}
