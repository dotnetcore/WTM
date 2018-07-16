using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// SearchCondition
    /// </summary>
    [Table("SearchConditions")]
    public class SearchCondition : BasePoco
    {
        [Display(Name = "条件名称" )]
        public string Name { get; set; }
        [Display(Name = "用户" )]
        public Guid UserId { get; set; }
        [Display(Name = "用户" )]
        public FrameworkUserBase User { get; set; }
        [Display(Name = "条件" )]
        public string Condition { get; set; }
        [Display(Name = "模块" )]
        public string VMName { get; set; }
    }
}
