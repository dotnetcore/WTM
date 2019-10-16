using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkArea
    /// </summary>
    [Table("FrameworkAreas")]
    public class FrameworkArea : BasePoco
    {
        [Display(Name = "AreaName")]
        [StringLength(50,ErrorMessage ="{0}stringmax{1}")]
        [Required(ErrorMessage ="{0}required")]
        public string AreaName { get; set; }

        [Required(ErrorMessage ="{0}required")]
        [StringLength(50,ErrorMessage ="{0}stringmax{1}")]
        [Display(Name = "Prefix")]
        public string Prefix { get; set; }

        [Display(Name = "Module")]
        public List<FrameworkModule> Modules { get; set; }

    }

}
