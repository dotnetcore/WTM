using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Core
{
    public interface ITokenService
    {
        IDataContext DC { get; }

        /// <summary>
        /// Issue token
        /// </summary>
        /// <param name="loginUserInfo"></param>
        /// <returns></returns>
        Task<Token> IssueTokenAsync(LoginUserInfo loginUserInfo);

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
