using System.ComponentModel.DataAnnotations;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 持久化模型接口，通过 IsValid 属性提供软删除标记，
    /// 实现该接口的模型不会被物理删除，而是标记为无效
    /// </summary>
    public interface IPersistPoco
    {
        /// <summary>
        /// 数据有效性标记；为 false 时表示数据已被软删除，不应再出现在业务查询结果中
        /// </summary>
        bool IsValid { get; set; }
    }

    /// <summary>
    /// 所有持久化model的基类，所有的不应被物理删除的model都应该继承这个类
    /// </summary>
    public abstract class PersistPoco : BasePoco, IPersistPoco
    {
        /// <summary>
        /// IsValid
        /// </summary>
        [Display(Name = "_Admin.IsValid")]
        public bool IsValid { get; set; } = true;
    }
}
