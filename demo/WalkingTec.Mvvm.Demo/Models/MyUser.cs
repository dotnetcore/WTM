using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.Models
{
    [Table("FrameworkUsers")]
    public class MyUser : FrameworkUserBase
    {

        [Display(Name = "附加信息1")]
        public string Extra1 { get; set; }

        [Display(Name = "附加信息2")]
        public string Extra2 { get; set; }
    }
}
