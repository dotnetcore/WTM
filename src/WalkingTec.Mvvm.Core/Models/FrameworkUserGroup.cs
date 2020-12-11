using System;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core.Attributes;

namespace WalkingTec.Mvvm.Core
{
    [MiddleTable]
    public class FrameworkUserGroup : BasePoco
    {
        public FrameworkUserBase User { get; set; }
        public FrameworkGroup Group { get; set; }
        [Required]
        public Guid? UserId { get; set; }
        [Display(Name = "Group")]
        [Required]
        public Guid? GroupId { get; set; }
    }

}
