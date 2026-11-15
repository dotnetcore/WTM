using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{

    /// <summary>
    /// 用户组实体，继承 TreePoco 支持树形层级结构。
    /// 用户组用于按组管理用户，并配合数据权限（DataPrivilege）实现组级数据授权
    /// </summary>
    [Table("FrameworkGroups")]
    [SoftKey(nameof(FrameworkGroup.GroupCode))]
    public class FrameworkGroup : TreePoco<FrameworkGroup>,ITenant
    {
        /// <summary>
        /// 组编码，唯一，仅允许数字，创建后不可编辑
        /// </summary>
        [Display(Name = "_Admin.GroupCode")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Validate.{0}number")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        [CanNotEdit]
        public string GroupCode { get; set; }

        /// <summary>
        /// 组名称
        /// </summary>
        [Display(Name = "_Admin.GroupName")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public string GroupName { get; set; }

        /// <summary>
        /// 组备注说明
        /// </summary>
        [Display(Name = "_Admin.Remark")]
        public string GroupRemark { get; set; }

        /// <summary>
        /// 组管理员的登录账号（ITCode）
        /// </summary>
        [Display(Name = "_Admin.GroupManager")]
        public string Manager { get; set; }

        /// <summary>
        /// 组内用户数量，不映射到数据库，列表页查询统计时赋值
        /// </summary>
        [NotMapped]
        [Display(Name = "_Admin.UsersCount")]
        public int UsersCount { get; set; }

        /// <summary>
        /// 租户编码，标识用户组所属的租户
        /// </summary>
        [Display(Name = "_Admin.Tenant")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TenantCode { get; set; }

    }
}
