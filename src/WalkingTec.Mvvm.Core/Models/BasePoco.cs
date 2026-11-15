using System;
using System.ComponentModel.DataAnnotations;

namespace WalkingTec.Mvvm.Core
{

    /// <summary>
    /// 基础实体接口，定义模型应具备的创建/更新审计字段（时间与操作人）
    /// </summary>
    public interface IBasePoco
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime? CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        string CreateBy { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 最后更新人
        /// </summary>
        string UpdateBy { get; set; }
    }

    /// <summary>
    /// Model层的基类，所有的model都应该继承这个类。这会使所有的model层对应的数据库表都有一个自增主键
    /// </summary>
    public abstract class BasePoco : TopBasePoco, IBasePoco
    {
        /// <summary>
        /// CreateTime
        /// </summary>
        [Display(Name = "_Admin.CreateTime")]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// CreateBy
        /// </summary>
        [Display(Name = "_Admin.CreateBy")]
        [StringLength(50,ErrorMessage = "Validate.{0}stringmax{1}")]
        public string CreateBy { get; set; }
        /// <summary>
        /// UpdateTime
        /// </summary>
        [Display(Name = "_Admin.UpdateTime")]
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// UpdateBy
        /// </summary>
        [Display(Name = "_Admin.UpdateBy")]
        [StringLength(50,ErrorMessage = "Validate.{0}stringmax{1}")]
        public string UpdateBy { get; set; }
    }
}
