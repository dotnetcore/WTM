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
        [StringLength(50,ErrorMessage ="{0}stringmax{1}")]
        [Display(Name = "DomainName")]
        [Required(ErrorMessage ="{0}required")]
        public string DomainName { get; set; }

        [StringLength(50,ErrorMessage ="{0}stringmax{1}")]
        [Required(ErrorMessage ="{0}required")]
        [Display(Name = "Address")]
        public string DomainAddress { get; set; }

        [Display(Name = "Port")]
        [Range(1, 65535,ErrorMessage="{0}必须是{1}到{2}之间的整数")]
        public int? DomainPort { get; set; }

        [Display(Name = "EntryUrl")]
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
