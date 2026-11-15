namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// JWT 认证配置，对应 appsettings.json 中的 JwtOptions 节，用于配置 Token 的颁发者、签名密钥及过期时间等
    /// </summary>
    public class JwtOption
    {
        /// <summary>
        /// 颁发者地址，默认 http://localhost
        /// </summary>
        public string Issuer { get; set; } = "http://localhost";
        /// <summary>
        /// 接收者（受众）地址，默认 http://localhost
        /// </summary>
        public string Audience { get; set; } = "http://localhost";
        /// <summary>
        /// Token 过期时间，单位秒，默认 3600（1 小时）
        /// </summary>
        public int Expires { get; set; } = 3600;
        private string _securiteKey= "wtmwtmwtmwtmwtmwtm";
        /// <summary>
        /// 签名安全密钥，默认 wtmwtmwtmwtmwtmwtm；设置值长度不足 32 位时会自动在末尾补 x 至 32 位，以满足签名算法对密钥长度的要求
        /// </summary>
        public string SecurityKey { get
            {
                return _securiteKey;
            }
            set {
                _securiteKey = value;
                // 密钥长度不足 32 位时，自动在末尾补充 x 直到满足 32 位
                if (_securiteKey.Length < 32)
                {
                    var count = 32 - _securiteKey.Length;
                    for (int i = 0; i < count; i++)
                    {
                        _securiteKey += "x";
                    }
                }

            }
        }
        /// <summary>
        /// 登录页面路径，未配置时默认为 null
        /// </summary>
        public string LoginPath { get; set; }
    }
}
