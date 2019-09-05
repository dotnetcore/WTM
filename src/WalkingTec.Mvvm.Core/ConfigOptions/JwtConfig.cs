namespace WalkingTec.Mvvm.Core.ConfigOptions
{
    /// <summary>
    /// JwtConfig
    /// </summary>
    public class JwtConfig
    {
        /// <summary>
        /// SecurityKey
        /// </summary>
        public string SecurityKey { get; set; }

        /// <summary>
        /// Issuer
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Audience
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Expiration
        /// </summary>
        public int Expiration { get; set; }
    }
}
