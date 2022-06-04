using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Core
{
    public interface ITokenService
    {
        /// <summary>
        /// Issue token
        /// </summary>
        /// <param name="loginUserInfo"></param>
        /// <returns></returns>
        Task<Token> IssueTokenAsync(LoginUserInfo loginUserInfo);
    }
}
