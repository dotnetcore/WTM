using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ILogger _logger;
        private readonly JwtOptions _jwtOptions;

        private const Token _emptyToken = null;

        private readonly Configs _configs;
        private readonly IDataContext _dc;
        public IDataContext DC => _dc;

        public AuthService(
            ILogger<AuthService> logger,
            IOptions<JwtOptions> jwtOptions
        )
        {
            _configs = GlobalServices.GetRequiredService<Configs>();
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
            _dc = CreateDC();
        }

        public async Task<Token> IssueToken(LoginUserInfo loginUserInfo)
        {
            if (loginUserInfo == null)
                throw new ArgumentNullException(nameof(loginUserInfo));

            var signinCredentials = new SigningCredentials(_jwtOptions.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var tokeOptions = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: new List<Claim>()
                {
                    new Claim(AuthConstants.JwtClaimTypes.Subject, loginUserInfo.Id.ToString()),
                    new Claim(AuthConstants.JwtClaimTypes.Name, loginUserInfo.Name)
                },
                expires: DateTime.Now.AddSeconds(_jwtOptions.Expires),
                signingCredentials: signinCredentials
            );


            var refreshToken = new PersistedGrant()
            {
                UserId = loginUserInfo.Id,
                Type = "refresh_token",
                CreationTime = DateTime.Now,
                RefreshToken = Guid.NewGuid().ToString("N"),
                Expiration = DateTime.Now.AddSeconds(_jwtOptions.RefreshTokenExpires)
            };
            _dc.AddEntity(refreshToken);
            await _dc.SaveChangesAsync();

            return await Task.FromResult(new Token()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(tokeOptions),
                ExpiresIn = _jwtOptions.Expires,
                TokenType = AuthConstants.JwtTokenType,
                RefreshToken = refreshToken.RefreshToken
            });
        }

        private IDataContext CreateDC()
        {
            string cs = "default";
            var globalIngo = GlobalServices.GetRequiredService<GlobalData>();
            return (IDataContext)globalIngo.DataContextCI?.Invoke(new object[] { _configs.ConnectionStrings?.Where(x => x.Key.ToLower() == cs).Select(x => x.Value).FirstOrDefault(), _configs.DbType });
        }
    }
}
