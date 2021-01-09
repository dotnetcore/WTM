using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WalkingTec.Mvvm.Core.Attributes;

namespace WalkingTec.Mvvm.Core
{
    [Table("FrameworkUserGroups")]
    public class FrameworkUserGroup : BasePoco
    {
        [Required]
        public string UserCode { get; set; }
        [Display(Name = "_Admin.Group")]
        [Required]
        public string GroupCode { get; set; }
    }

}
