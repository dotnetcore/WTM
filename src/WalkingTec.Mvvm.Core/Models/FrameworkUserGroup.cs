using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WalkingTec.Mvvm.Core.Attributes;

namespace WalkingTec.Mvvm.Core
{
    [MiddleTable]
    public class FrameworkUserGroup : BasePoco
    {
        [JsonIgnore]
        public FrameworkUserBase User { get; set; }
        [JsonIgnore]
        public FrameworkGroup Group { get; set; }
        [Required]
        public Guid? UserId { get; set; }
        [Display(Name = "Group")]
        [Required]
        public Guid? GroupId { get; set; }
    }

}
