using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FunctionPrivilege
    /// </summary>
    [Table("FunctionPrivileges")]
    public class FunctionPrivilege : BasePoco,ITenant
    {
        /// <summary>
        /// 角色编码，指定该功能权限所属的角色，对应 FrameworkRole 的 RoleCode
        /// </summary>
        [Display(Name = "_Admin.Role")]
        public string RoleCode { get; set; }

        /// <summary>
        /// 菜单（页面）ID，对应 FrameworkMenu 的主键
        /// </summary>
        [Display(Name = "_Admin.MenuItem")]
        public Guid MenuItemId { get; set; }

        /// <summary>
        /// 是否允许访问；true 表示授权访问，false 表示拒绝访问
        /// </summary>
        [Display(Name = "_Admin.Allowed")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public bool? Allowed { get; set; }
        /// <summary>
        /// 租户编码，标识该权限所属的租户
        /// </summary>
        [Display(Name = "_Admin.Tenant")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TenantCode { get; set; }

    }
}
