using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core.Auth
{
    public class TokenService : ITokenService
    {
        private readonly ILogger _logger;
        private readonly JwtOptions _jwtOptions;

        private const Token _emptyToken = null;

        private readonly Configs _configs;
        private readonly IDataContext _dc;
        public IDataContext DC => _dc;

        public TokenService(
            ILogger<TokenService> logger,
            IOptions<JwtOptions> jwtOptions
        )
        {
            _configs = GlobalServices.GetRequiredService<Configs>();
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
            _dc = CreateDC();
        }

        public async Task<Token> IssueTokenAsync(LoginUserInfo loginUserInfo)
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
            return _configs.ConnectionStrings.Where(x=>x.Key.ToLower() == cs).FirstOrDefault().CreateDC();
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

                // 删除 refresh token
                _dc.DeleteEntity(persistedGrant);
                await _dc.SaveChangesAsync();

                // 校验 regresh token 有效期
                if (persistedGrant.Expiration < DateTime.Now)
                    throw new Exception("refresh token 已过期");

                var user = await _dc.Set<FrameworkUserBase>()
                                    .Include(x => x.UserRoles).Include(x => x.UserGroups)
                                    .Where(x => x.ID == persistedGrant.UserId)
                                    .SingleAsync();

                //var roleIDs = user.UserRoles.Select(x => x.RoleId).ToList();
                //var groupIDs = usesr.UserGroups.Select(x => x.GroupId).ToList();
                ////查找登录用户的数据权限
                //var dpris = await _dc.Set<DataPrivilege>()
                //                .Where(x => x.UserId == user.ID || (x.GroupId != null && groupIDs.Contains(x.GroupId.Value)))
                //                .ToListAsync();

                ////查找登录用户的页面权限
                //var funcPrivileges = await _dc.Set<FunctionPrivilege>()
                //                        .Where(x => x.UserId == user.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                //                        .ToListAsync();

                //生成并返回登录用户信息
                var loginUserInfo = new LoginUserInfo
                {
                    Id = user.ID,
                    ITCode = user.ITCode,
                    Name = user.Name,
                    PhotoId = user.PhotoId,
                    //Roles = await _dc.Set<FrameworkRole>().Where(x => user.UserRoles.Select(y => y.RoleId).Contains(x.ID)).ToListAsync(),
                    //Groups = await _dc.Set<FrameworkGroup>().Where(x => user.UserGroups.Select(y => y.GroupId).Contains(x.ID)).ToListAsync(),
                    //DataPrivileges = dpris,
                    //FunctionPrivileges = funcPrivileges
                };

                //// 清理过期 refreshtoken
                //var sql = $"DELETE FROM {_dc.GetTableName<PersistedGrant>().ToLower()} WHERE Expiration<'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
                //_dc.RunSQL(sql);
                //_logger.LogDebug("清理过期的refreshToken：【sql:{0}】", sql);

                // 颁发 token
                return await IssueTokenAsync(loginUserInfo);
            }
            else
                throw new Exception("非法的 refresh Token");
        }

        /// <summary>
        /// clear expired refresh tokens
        /// </summary>
        /// <returns></returns>
        public async Task ClearExpiredRefreshTokenAsync()
        {
            var dataTime = DateTime.Now;
            var mapping = _dc.Model.FindEntityType(typeof(PersistedGrant)).Relational();
            var sql = $"DELETE FROM {mapping.TableName} WHERE Expiration<=@dataTime";
            _dc.RunSQL(sql, new
            {
                dataTime = dataTime
            });
            await Task.CompletedTask;
        }

    }
}
