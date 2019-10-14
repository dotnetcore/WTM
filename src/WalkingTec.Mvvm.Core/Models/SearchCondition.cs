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
        [Display(Name = "ConditionName" )]
        public string Name { get; set; }
        [Display(Name = "User")]
        public Guid UserId { get; set; }
        [Display(Name = "User")]
        public FrameworkUserBase User { get; set; }
        [Display(Name = "Condition")]
        public string Condition { get; set; }
        [Display(Name = "模块" )]
        public string VMName { get; set; }
    }
}
