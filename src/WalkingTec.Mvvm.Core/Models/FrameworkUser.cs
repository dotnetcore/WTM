using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkUser
    /// </summary>
    [Table("FrameworkUsers")]
    public  abstract class FrameworkUserBase : BasePoco,ITenant
    {
        /// <summary>
        /// 登录账号，唯一，创建后不可编辑
        /// </summary>
        [Display(Name = "_Admin.Account")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(50,ErrorMessage = "Validate.{0}stringmax{1}")]
        [CanNotEdit]
        public string ITCode { get; set; }

        /// <summary>
        /// 登录密码（加密存储）
        /// </summary>
        [Display(Name = "_Admin.Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Validate.{0}required")]
        [StringLength(32, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string Password { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [Display(Name = "_Admin.Name")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(50,ErrorMessage = "Validate.{0}stringmax{1}")]
        public string Name { get; set; }

        /// <summary>
        /// 账号是否有效（启用/禁用），默认为启用
        /// </summary>
        [Display(Name = "_Admin.IsValid")]
        public bool IsValid { get; set; } = true;

        /// <summary>
        /// 用户头像的文件附件ID
        /// </summary>
        [Display(Name = "_Admin.Photo")]
        public Guid? PhotoId { get; set; }

        /// <summary>
        /// 用户头像附件的导航属性，序列化时忽略
        /// </summary>
        [Display(Name = "_Admin.Photo")]
        [JsonIgnore]
        public FileAttachment Photo { get; set; }

        /// <summary>
        /// 租户编码，标识用户所属的租户
        /// </summary>
        [Display(Name = "_Admin.Tenant")]
        [StringLength(50,ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TenantCode { get; set; }
    }
}
