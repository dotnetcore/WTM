using System.ComponentModel.DataAnnotations;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 所有持久化model的基类，所有的不应被物理删除的model都应该继承这个类
    /// </summary>
    public class PersistIncVersionPoco : PersistPoco {
        /// <summary>
        /// IncVersion
        /// </summary>
        [Display (Name = "Version")]
        public long IncVersion { get; set; }
    }
}
