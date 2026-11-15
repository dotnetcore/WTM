using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 用户角色关联实体，记录用户与角色之间的多对多对应关系
    /// </summary>
    [Table("FrameworkUserRoles")]
    public class FrameworkUserRole : BasePoco, ITenant
    {
        /// <summary>
        /// 用户编码（登录账号），对应 FrameworkUser 的 ITCode
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string UserCode { get; set; }
        /// <summary>
        /// 角色编码，对应 FrameworkRole 的 RoleCode
        /// </summary>
        [Required]
        [Display(Name = "_Admin.Role")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string RoleCode { get; set; }
        /// <summary>
        /// 租户编码，标识该关联所属的租户；为空表示不属于任何租户
        /// </summary>
        [Display(Name = "_Admin.Tenant")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TenantCode { get; set; }
    }
}
