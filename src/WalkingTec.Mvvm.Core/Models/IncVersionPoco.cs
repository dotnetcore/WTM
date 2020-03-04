using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 乐观锁Model类，使用此类会新加一个单调递增的版本标识
    /// </summary>
    public class IncVersionPoco : BasePoco
    {
        /// <summary>
        /// CreateTime
        /// </summary>
        [Display (Name = "Version")]
        public long IncVersion { get; set; }
    }
}
