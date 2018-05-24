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
        [Display(Name = "角色" )]
        public Guid? RoleId { get; set; }

        [Display(Name = "用户" )]
        public Guid? UserId { get; set; }

        [Display(Name = "菜单项" )]
        public Guid MenuItemId { get; set; }

        [Display(Name = "菜单项")]
        public FrameworkMenu MenuItem { get; set; }

        [Display(Name = "允许" )]
        [Required(ErrorMessage ="{0}是必填项")]
        public bool? Allowed { get; set; }
    }
}
