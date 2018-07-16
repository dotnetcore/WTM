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
        [Display(Name = "创建时间")]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// CreateBy
        /// </summary>
        [Display(Name = "创建人")]
        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        public string CreateBy { get; set; }
        /// <summary>
        /// UpdateTime
        /// </summary>
        [Display(Name = "修改时间")]
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// UpdateBy
        /// </summary>
        [Display(Name = "修改人")]
        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        public string UpdateBy { get; set; }
    }
}
