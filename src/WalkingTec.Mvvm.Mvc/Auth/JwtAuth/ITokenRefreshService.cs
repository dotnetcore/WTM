using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Mvc.Auth
{
    public interface ITokenRefreshService
    {
        /// <summary>
        /// refresh token
        /// </summary>
        /// <param name="refreshToken">refreshToken</param>
        /// <returns></returns>
        Task<Token> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// clear expired refresh tokens
        /// </summary>
        /// <returns></returns>
        Task ClearExpiredRefreshTokenAsync();
    }
}
