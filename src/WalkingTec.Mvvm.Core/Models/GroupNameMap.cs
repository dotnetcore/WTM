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
        [StringLength(50,ErrorMessage ="{0}stringmax{1}")]
        [Required(ErrorMessage ="{0}required")]
        public string GroupName { get; set; }

        [StringLength(50,ErrorMessage ="{0}stringmax{1}")]
        [Required(ErrorMessage ="{0}required")]
        public string IntranetUrlHeader { get; set; }

        [StringLength(50,ErrorMessage ="{0}stringmax{1}")]
        [Required(ErrorMessage ="{0}required")]
        public string ExtranetUrlHeader { get; set; }
    }
}
