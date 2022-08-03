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
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Mvc.Auth
{
    public class TokenService : ITokenService
    {
        private readonly JwtOption _jwtOptions;

        private const Token _emptyToken = null;


        public TokenService(
            IOptionsMonitor<Configs> configs
        )
        {
            _jwtOptions = configs.CurrentValue.JwtOptions;
        }

        public async Task<Token> IssueTokenAsync(LoginUserInfo loginUserInfo)
        {
            if (loginUserInfo == null)
                throw new ArgumentNullException(nameof(loginUserInfo));

            var signinCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecurityKey)), SecurityAlgorithms.HmacSha256);


            var cls = new List<Claim>()
                {
                    new Claim(AuthConstants.JwtClaimTypes.Subject, loginUserInfo.ITCode)
                };
            if (string.IsNullOrEmpty(loginUserInfo.TenantCode) == false)
            {
                cls.Add(new Claim(AuthConstants.JwtClaimTypes.TenantCode, loginUserInfo.TenantCode));
            }
            if (string.IsNullOrEmpty(loginUserInfo.RemoteToken) == false)
            {
                cls.Add(new Claim(AuthConstants.JwtClaimTypes.RToken, loginUserInfo.RemoteToken));
            }
            var tokeOptions = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: cls,
                expires: DateTime.UtcNow.AddSeconds(_jwtOptions.Expires),
                signingCredentials: signinCredentials
            );

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
