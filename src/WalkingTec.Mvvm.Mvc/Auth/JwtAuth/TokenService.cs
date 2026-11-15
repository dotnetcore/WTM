using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Mvc.Auth
{
    /// <summary>
    /// JWT 令牌服务，负责根据登录用户信息签发 JWT 访问令牌
    /// </summary>
    public class TokenService : ITokenService
    {
        /// <summary>
        /// JWT 配置选项(安全密钥、签发者、受众、过期时间等)
        /// </summary>
        private readonly JwtOption _jwtOptions;

        /// <summary>
        /// 空令牌常量(保留，当前未使用)
        /// </summary>
        private const Token _emptyToken = null;


        /// <summary>
        /// 构造函数，从全局配置中读取 JWT 配置选项
        /// </summary>
        /// <param name="configs">全局配置的监控对象，用于获取当前生效的配置</param>
        public TokenService(
            IOptionsMonitor<Configs> configs
        )
        {
            _jwtOptions = configs.CurrentValue.JwtOptions;
        }

        /// <summary>
        /// 根据登录用户信息签发 JWT 访问令牌；将用户 ITCode 作为主体声明，
        /// 并在存在租户代码或远程令牌时附加对应的声明
        /// </summary>
        /// <param name="loginUserInfo">当前登录用户信息</param>
        /// <returns>签发完成的令牌，包含访问令牌、有效期和令牌类型</returns>
        public async Task<Token> IssueTokenAsync(LoginUserInfo loginUserInfo)
        {
            if (loginUserInfo == null)
                throw new ArgumentNullException(nameof(loginUserInfo));

            // 使用配置中的安全密钥构造 HMAC-SHA256 签名凭据
            var signinCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecurityKey)), SecurityAlgorithms.HmacSha256);


            // 组装声明(Claims)：主体声明为用户 ITCode
            var cls = new List<Claim>()
                {
                    new Claim(AuthConstants.JwtClaimTypes.Subject, loginUserInfo.ITCode)
                };
            // 多租户场景下附加租户代码声明
            if (string.IsNullOrEmpty(loginUserInfo.TenantCode) == false)
            {
                cls.Add(new Claim(AuthConstants.JwtClaimTypes.TenantCode, loginUserInfo.TenantCode));
            }
            // 存在远程令牌时附加声明，用于跨主机场景下的令牌透传
            if (string.IsNullOrEmpty(loginUserInfo.RemoteToken) == false)
            {
                cls.Add(new Claim(AuthConstants.JwtClaimTypes.RToken, loginUserInfo.RemoteToken));
            }
            // 构造 JWT：设置签发者、受众、声明集合、过期时间(UTC)和签名凭据
            var tokeOptions = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: cls,
                expires: DateTime.UtcNow.AddSeconds(_jwtOptions.Expires),
                signingCredentials: signinCredentials
            );

            // 将 JwtSecurityToken 序列化为字符串形式的访问令牌
            var token = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            return await Task.FromResult(new Token()
            {
                AccessToken = token,
                ExpiresIn = _jwtOptions.Expires,
                TokenType = AuthConstants.JwtTokenType,
                RefreshToken = ""
            });
        }
    }
}
