using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FunctionPrivilege
    /// </summary>
    [Table("FunctionPrivileges")]
    public class FunctionPrivilege : BasePoco
    {
        [Display(Name = "Role")]
        public string RoleCode { get; set; }

        [Display(Name = "MenuItem")]
        public Guid MenuItemId { get; set; }

        [Display(Name = "MenuItem")]
        public FrameworkMenu MenuItem { get; set; }

        [Display(Name = "Allowed")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public bool? Allowed { get; set; }
    }
}
