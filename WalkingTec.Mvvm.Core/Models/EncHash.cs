using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// EncHash
    /// </summary>
    [Table("EncHashs")]
    public class EncHash : BasePoco
    {
        public Guid Key { get; set; }
        public int Hash { get; set; }
    }
}
