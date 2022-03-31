using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Auth
{
    public class TokenService : ITokenService
    {
        private readonly ILogger _logger;
        private readonly JwtOption _jwtOptions;

        private const Token _emptyToken = null;

        private readonly Configs _configs;
        private readonly IDataContext _dc;
        public IDataContext DC => _dc;

        public TokenService(
            ILogger<TokenService> logger,
            IOptionsMonitor<Configs> configs
        )
        {
            _configs = configs.CurrentValue;
            _jwtOptions = _configs.JwtOptions;
            _logger = logger;
            _dc = CreateDC();
        }

        public async Task<Token> IssueTokenAsync(LoginUserInfo loginUserInfo)
        {
            if (loginUserInfo == null)
                throw new ArgumentNullException(nameof(loginUserInfo));

            var signinCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecurityKey)), SecurityAlgorithms.HmacSha256);


            var cls = new List<Claim>()
                {
                    new Claim(AuthConstants.JwtClaimTypes.Subject, loginUserInfo.ITCode.ToString())
                };
            if (string.IsNullOrEmpty(loginUserInfo.TenantCode) == false)
            {
                cls.Add(new Claim(AuthConstants.JwtClaimTypes.TenantCode, loginUserInfo.TenantCode.ToString()));
            }
            var tokeOptions = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: cls,
                expires: DateTime.Now.AddSeconds(_jwtOptions.Expires),
                signingCredentials: signinCredentials
            );

            var refreshToken = new PersistedGrant()
            {
                UserCode = loginUserInfo.ITCode + "$`$" + loginUserInfo.TenantCode,
                Type = "refresh_token",
                CreationTime = DateTime.Now,
                RefreshToken = Guid.NewGuid().ToString("N"),
                Expiration = DateTime.Now.AddSeconds(_jwtOptions.RefreshTokenExpires)
            };
            _dc.AddEntity(refreshToken);
            await _dc.SaveChangesAsync();

            var token = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            return await Task.FromResult(new Token()
            {
                AccessToken = token,
                ExpiresIn = _jwtOptions.Expires,
                TokenType = AuthConstants.JwtTokenType,
                RefreshToken = refreshToken.RefreshToken
            });
        }

        private IDataContext CreateDC()
        {
            string cs = "tokendefault";
            if (_configs.Connections.Any(x => x.Key.ToLower() == cs) == false)
            {
                cs = "default";
            }
            return _configs.Connections.Where(x => x.Key.ToLower() == cs).FirstOrDefault().CreateDC();
        }


        /// <summary>
        /// refresh token
        /// </summary>
        /// <param name="refreshToken">refreshToken</param>
        /// <returns></returns>
        public async Task<Token> RefreshTokenAsync(string refreshToken)
        {
            // 获取 RefreshToken
            PersistedGrant persistedGrant = await _dc.Set<PersistedGrant>().Where(x => x.RefreshToken == refreshToken).SingleOrDefaultAsync();
            if (persistedGrant != null)
            {
                // 校验 regresh token 有效期
                if (persistedGrant.Expiration < DateTime.Now)
                    throw new Exception("refresh token 已过期");

                // 删除 refresh token
                _dc.DeleteEntity(persistedGrant);
                await _dc.SaveChangesAsync();

                var pair = persistedGrant.UserCode?.Split("$`$");
                //生成并返回登录用户信息
                var loginUserInfo = new LoginUserInfo()
                {
                    ITCode = pair[0],
                    TenantCode = pair[1],
                };

                // 颁发 token
                return await IssueTokenAsync(loginUserInfo);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// clear expired refresh tokens
        /// </summary>
        /// <returns></returns>
        public async Task ClearExpiredRefreshTokenAsync()
        {
            var dataTime = DateTime.Now;
            var mapping = _dc.GetTableName<PersistedGrant>();
            var sql = $"DELETE FROM {mapping} WHERE Expiration<=@dataTime";
            _dc.RunSQL(sql, new
            {
                dataTime = dataTime
            });
            await Task.CompletedTask;
        }

    }
}
