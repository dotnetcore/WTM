using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkRole
    /// </summary>
    [Table("FrameworkRoles")]
    [SoftKey(nameof(FrameworkRole.RoleCode))]
    public class FrameworkRole : BasePoco,ITenant
    {
        /// <summary>
        /// 角色编码，唯一，仅允许数字，创建后不可编辑
        /// </summary>
        [Display(Name = "_Admin.RoleCode")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Validate.{0}number")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        [CanNotEdit]
        public string RoleCode { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Display(Name = "_Admin.RoleName")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public string RoleName { get; set; }

        /// <summary>
        /// 角色备注说明
        /// </summary>
        [Display(Name = "_Admin.Remark")]
        public string RoleRemark { get; set; }

        /// <summary>
        /// 租户编码，标识角色所属的租户
        /// </summary>
        [Display(Name = "_Admin.Tenant")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TenantCode { get; set; }


        /// <summary>
        /// 拥有该角色的用户数量，不映射到数据库，列表页查询统计时赋值
        /// </summary>
        [NotMapped]
        [Display(Name = "_Admin.UsersCount")]
        public int UsersCount { get; set; }


    }
}
