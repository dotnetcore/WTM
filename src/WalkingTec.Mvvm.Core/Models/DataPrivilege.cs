using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// DataPrivilege
    /// </summary>
    [Table("DataPrivileges")]
    public class DataPrivilege : BasePoco
    {
        [Display(Name = "用户")]
        public Guid? UserId { get; set; }
        [Display(Name = "用户")]
        [JsonIgnore()]
        public FrameworkUserBase User { get; set; }

        [Display(Name = "用户组")]
        public Guid? GroupId { get; set; }
        [Display(Name = "用户组")]
        [JsonIgnore()]
        public FrameworkGroup Group { get; set; }

        [Required(ErrorMessage ="{0}是必填项")]
        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        [Display(Name = "数据权限" )]
        public string TableName { get; set; }
        public Guid? RelateId { get; set; }

        [Display(Name = "域" )]
        public Guid? DomainId { get; set; }

        [Display(Name = "域" )]
        [JsonIgnore()]
        public FrameworkDomain Domain { get; set; }

    }
}
