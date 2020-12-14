using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WalkingTec.Mvvm.Core.Attributes;

namespace WalkingTec.Mvvm.Core
{
    [MiddleTable]
    public class FrameworkUserRole : BasePoco
    {
        [JsonIgnore]
        public FrameworkUserBase User { get; set; }
        [JsonIgnore]
        public FrameworkRole Role { get; set; }
        [Required]
        public Guid? UserId { get; set; }
        [Required]
        [Display(Name = "Role")]
        public Guid? RoleId { get; set; }
    }
}
