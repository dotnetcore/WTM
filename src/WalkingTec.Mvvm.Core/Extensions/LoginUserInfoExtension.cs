using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

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
                await Cache.RemoveAsync(key);
            }
        }
    }
}
