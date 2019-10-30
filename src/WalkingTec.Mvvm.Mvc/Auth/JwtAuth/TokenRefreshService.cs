
using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth;

namespace WalkingTec.Mvvm.Mvc.Auth
{
    public class TokenRefreshService : ITokenRefreshService
    {
        private readonly ILogger _logger;
        private readonly IAuthService _authService;
        private readonly IDataContext _dc;

        public TokenRefreshService(
            ILogger<TokenRefreshService> logger,
            IAuthService authService
        )
        {
            _logger = logger;
            _authService = authService;
            _dc = _authService.DC;
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

                // 清理过期 refreshtoken
                var sql = $"DELETE FROM persistedgrants WHERE Expiration<'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
                _dc.RunSQL(sql);
                _logger.LogDebug("清理过期的refreshToken：【sql:{0}】", sql);

                // 颁发 token
                return await _authService.IssueToken(loginUserInfo);
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
