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
        [Column("DomainName")]
        public string Name { get; set; }

        [StringLength(50,ErrorMessage ="{0}stringmax{1}")]
        [Required(ErrorMessage ="{0}required")]
        [Display(Name = "Address")]
        [Column("DomainAddress")]
        public string Address { get; set; }

        [Display(Name = "Port")]
        [Range(1, 65535,ErrorMessage="{0}必须是{1}到{2}之间的整数")]
        [Column("DomainPort")]
        public int? Port { get; set; }

        [Display(Name = "EntryUrl")]
        public string EntryUrl { get; set; }

        [NotMapped]
        public string Url
        {
            get
            {
                var rv = Address;
                if(rv.ToLower().StartsWith("http://") == false && rv.ToLower().StartsWith("https://") == false)
                {
                    rv = "http://" + rv;
                }
                if (Port != null)
                {
                    rv += ":" + Port;
                }
                return rv;
            }
        }

    }

}
