using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 用户用户组关联实体，记录用户与用户组之间的多对多对应关系
    /// </summary>
    [Table("FrameworkUserGroups")]
    public class FrameworkUserGroup : BasePoco, ITenant
    {
        /// <summary>
        /// 用户编码（登录账号），对应 FrameworkUser 的 ITCode
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string UserCode { get; set; }
        /// <summary>
        /// 用户组编码，对应 FrameworkGroup 的 GroupCode
        /// </summary>
        [Display(Name = "_Admin.Group")]
        [Required]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string GroupCode { get; set; }
        /// <summary>
        /// 租户编码，标识该关联所属的租户；为空表示不属于任何租户
        /// </summary>
        [Display(Name = "_Admin.Tenant")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TenantCode { get; set; }

    }

}
