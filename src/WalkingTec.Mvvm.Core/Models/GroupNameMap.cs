using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// GroupName与地址头映射表
    /// </summary>
    [Table("GroupNameMap")]
    public class GroupNameMap : BasePoco
    {
        [Display(Name = "组名" )]
        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        [Required(ErrorMessage ="{0}是必填项")]
        public string GroupName { get; set; }

        [Display(Name = "内网地址头" )]
        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        [Required(ErrorMessage ="{0}是必填项")]
        public string IntranetUrlHeader { get; set; }

        [Display(Name = "外网地址头" )]
        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        [Required(ErrorMessage ="{0}是必填项")]
        public string ExtranetUrlHeader { get; set; }
    }
}
