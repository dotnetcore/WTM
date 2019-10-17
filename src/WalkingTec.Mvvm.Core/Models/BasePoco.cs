using System;
using System.ComponentModel.DataAnnotations;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// Model层的基类，所有的model都应该继承这个类。这会使所有的model层对应的数据库表都有一个自增主键
    /// </summary>
    public class BasePoco : TopBasePoco
    {
        /// <summary>
        /// CreateTime
        /// </summary>
        [Display(Name = "CreateTime")]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// CreateBy
        /// </summary>
        [Display(Name = "CreateBy")]
        [StringLength(50,ErrorMessage ="{0}stringmax{1}")]
        public string CreateBy { get; set; }
        /// <summary>
        /// UpdateTime
        /// </summary>
        [Display(Name = "UpdateTime")]
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// UpdateBy
        /// </summary>
        [Display(Name = "UpdateBy")]
        [StringLength(50,ErrorMessage ="{0}stringmax{1}")]
        public string UpdateBy { get; set; }
    }
}
