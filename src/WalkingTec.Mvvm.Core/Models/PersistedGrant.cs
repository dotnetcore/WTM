using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// PersistedGrant
    /// </summary>
    [Table("PersistedGrants")]
    public class PersistedGrant : TopBasePoco
    {
        [StringLength(50)]
        public string Type { get; set; }

        public Guid UserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        /// <value>The creation time.</value>
        public DateTime CreationTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 到期时间
        /// </summary>
        /// <value>The expiration.</value>
        public DateTime Expiration { get; set; }

        [StringLength(50)]
        public string RefreshToken { get; set; }

    }
}
