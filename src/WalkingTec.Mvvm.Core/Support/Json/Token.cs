
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    /// <summary>
    /// 认证令牌返回模型，封装认证接口返回的访问令牌、有效期及刷新令牌等信息
    /// </summary>
    public class Token
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// 令牌有效期(秒)
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 令牌类型(如 Bearer)
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// 刷新令牌，用于在访问令牌过期后获取新的令牌
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
