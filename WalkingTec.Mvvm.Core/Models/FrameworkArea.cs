using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkArea
    /// </summary>
    [Table("FrameworkAreas")]
    public class FrameworkArea : BasePoco
    {
        [Display(Name = "区域名称")]
        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        [Required(ErrorMessage ="{0}是必填项")]
        public string AreaName { get; set; }

        [Required(ErrorMessage ="{0}是必填项")]
        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        [Display(Name = "前缀" )]
        public string Prefix { get; set; }

        [Display(Name = "模块" )]
        public List<FrameworkModule> Modules { get; set; }

    }

}
