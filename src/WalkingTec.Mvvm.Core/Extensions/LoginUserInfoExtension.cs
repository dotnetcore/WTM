using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

using WalkingTec.Mvvm.Core.Auth;

namespace WalkingTec.Mvvm.Core.Extensions
{
    public static class LoginUserInfoExtension
    {
        private static IDistributedCache _cache;
        private static IDistributedCache Cache
        {
            get
            {
                if (_cache == null)
                {
                    _cache = GlobalServices.GetRequiredService<IDistributedCache>() as IDistributedCache;
                }
                return _cache;
            }
        }

        public static async Task RemoveUserCache(
                    this LoginUserInfo userInfo,
                    params string[] userIds)
        {
            foreach (var userId in userIds)
            {
                var key = $"{GlobalConstants.CacheKey.UserInfo}:{userId}";
                await Cache.DeleteAsync(key);
            }
        }

        public static ClaimsPrincipal CreatePrincipal(this LoginUserInfo self)
        {
            if (self.Id == Guid.Empty) throw new ArgumentException("Id is mandatory", nameof(self.Id));
            var claims = new List<Claim> { new Claim(AuthConstants.JwtClaimTypes.Subject, self.Id.ToString()) };

            if (!string.IsNullOrEmpty(self.Name))
            {
                claims.Add(new Claim(AuthConstants.JwtClaimTypes.Name, self.Name));
            }

            var id = new ClaimsIdentity(
                claims.Distinct(new ClaimComparer()),
                AuthConstants.AuthenticationType,
                AuthConstants.JwtClaimTypes.Name,
                AuthConstants.JwtClaimTypes.Role);
            return new ClaimsPrincipal(id);
        }
    }
}
