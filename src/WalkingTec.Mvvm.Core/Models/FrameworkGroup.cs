using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace WalkingTec.Mvvm.Core
{

    [Table("FrameworkGroups")]
    public class FrameworkGroup : BasePoco
    {
        [Display(Name = "用户组编码")]
        [Required(ErrorMessage = "{0}是必填项")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "{0}必须是数字")]
        [StringLength(100, ErrorMessage = "{0}最大100位")]
        public string GroupCode { get; set; }

        [Display(Name = "用户组名称")]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        [Required(ErrorMessage = "{0}是必填项")]
        public string GroupName { get; set; }

        [Display(Name = "备注")]
        public string GroupRemark { get; set; }

        [Display(Name = "用户")]
        [JsonIgnore]
        public List<FrameworkUserGroup> UserGroups { get; set; }

        [NotMapped]
        [Display(Name = "包含用户")]
        public int UsersCount { get; set; }

    }
}
