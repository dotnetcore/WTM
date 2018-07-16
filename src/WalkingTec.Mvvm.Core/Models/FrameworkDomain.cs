using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkDomain
    /// </summary>
    [Table("FrameworkDomains")]
    public class FrameworkDomain : BasePoco
    {
        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        [Display(Name = "域名称")]
        [Required(ErrorMessage ="{0}是必填项")]
        public string DomainName { get; set; }

        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        [Required(ErrorMessage ="{0}是必填项")]
        [Display(Name = "地址" )]
        public string DomainAddress { get; set; }

        [Display(Name = "端口" )]
        [Range(1, 65535,ErrorMessage="{0}必须是{1}到{2}之间的整数")]
        public int? DomainPort { get; set; }

        [Display(Name = "入口" )]
        public string EntryUrl { get; set; }

        [NotMapped]
        public string Address
        {
            get
            {
                var rv = "http://" + DomainAddress;
                if (DomainPort != null)
                {
                    rv += ":" + DomainPort;
                }
                return rv;
            }
        }

    }

}
