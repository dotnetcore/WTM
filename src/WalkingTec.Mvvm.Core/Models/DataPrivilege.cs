using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// DataPrivilege
    /// </summary>
    [Table("DataPrivileges")]
    public class DataPrivilege : BasePoco,ITenant
    {
        /// <summary>
        /// 被授权的用户编码，与组编码互斥使用
        /// </summary>
        [Display(Name = "_Admin.User")]
        public string UserCode { get; set; }

        /// <summary>
        /// 被授权的用户组编码，与用户编码互斥使用
        /// </summary>
        [Display(Name = "_Admin.Group")]
        public string GroupCode { get; set; }

        /// <summary>
        /// 数据权限针对的数据表名称
        /// </summary>
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(50,ErrorMessage = "Validate.{0}stringmax{1}")]
        [Display(Name = "_Admin.TableName")]
        public string TableName { get; set; }
        /// <summary>
        /// 数据权限针对的具体数据ID，为空表示对该表全部数据授权
        /// </summary>
        public string RelateId { get; set; }

        /// <summary>
        /// 所属域，用于多数据库/多域场景下的数据隔离
        /// </summary>
        [Display(Name = "_Admin.Domain")]
        public string Domain { get; set; }
        /// <summary>
        /// 租户编码，标识数据权限所属的租户
        /// </summary>
        [Display(Name = "_Admin.Tenant")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TenantCode { get; set; }

    }
}
