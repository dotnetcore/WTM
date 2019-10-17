using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkModule
    /// </summary>
    [Table("FrameworkModules")]
    public class FrameworkModule : BasePoco
    {
        [Display(Name = "ModuleName")]
        [StringLength(50,ErrorMessage ="{0}stringmax{1}")]
        [Required(ErrorMessage ="{0}required")]
        public string ModuleName { get; set; }

        [Required(ErrorMessage ="{0}required")]
        [StringLength(50,ErrorMessage ="{0}stringmax{1}")]
        [Display(Name = "ClassName")]
        public string ClassName { get; set; }

        [Display(Name = "Action")]
        public List<FrameworkAction> Actions { get; set; }

        [Display(Name = "Area")]
        public Guid? AreaId { get; set; }
        [Display(Name = "Area")]
        public FrameworkArea Area { get; set; }

        public string NameSpace { get; set; }

        [NotMapped]
        public bool IgnorePrivillege { get; set; }

        [NotMapped]
        public bool IsApi { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return this.NameSpace + "," + this.ClassName;
            }
        }
    }
}
