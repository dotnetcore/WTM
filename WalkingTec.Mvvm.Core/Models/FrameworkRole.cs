using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkRole
    /// </summary>
    [Table("FrameworkRoles")]
    public class FrameworkRole : BasePoco
    {
        [Display(Name = "角色编号")]
        [Required(ErrorMessage = "{0}是必填项")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "{0}必须是数字")]
        [StringLength(100, ErrorMessage = "{0}最大100位")]
        public string RoleCode { get; set; }

        [Display(Name = "角色名称")]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        [Required(ErrorMessage = "{0}是必填项")]
        public string RoleName { get; set; }

        [Display(Name = "备注")]
        public string RoleRemark { get; set; }

        [JsonIgnore]
        public List<FrameworkUserRole> UserRoles { get; set; }

        [NotMapped]
        [Display(Name = "包含用户数")]
        public int UserCount { get; set; }
    }
}
