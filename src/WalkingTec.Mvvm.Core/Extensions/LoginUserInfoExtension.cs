using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using WalkingTec.Mvvm.Core.Auth;

namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// 登录用户信息相关的扩展函数
    /// </summary>
    public static class LoginUserInfoExtension
    {
        /// <summary>
        /// 将登录用户信息转换为 ClaimsPrincipal，用于构建身份认证票据
        /// </summary>
        /// <param name="self">登录用户信息</param>
        /// <returns>包含用户声明信息的 ClaimsPrincipal 实例</returns>
        public static ClaimsPrincipal CreatePrincipal(this LoginUserInfo self)
        {
            if (string.IsNullOrEmpty(self.ITCode)) throw new ArgumentException("Id is mandatory", nameof(self.ITCode));
            var claims = new List<Claim> { new Claim(AuthConstants.JwtClaimTypes.Subject, self.ITCode) };

            if (!string.IsNullOrEmpty(self.Name))
            {
                claims.Add(new Claim(AuthConstants.JwtClaimTypes.Name, self.Name));
            }
            if (!string.IsNullOrEmpty(self.TenantCode))
            {
                claims.Add(new Claim(AuthConstants.JwtClaimTypes.TenantCode, self.TenantCode));
            }
            if (!string.IsNullOrEmpty(self.RemoteToken))
            {
                claims.Add(new Claim(AuthConstants.JwtClaimTypes.RToken, self.RemoteToken));
            }
            //利用声明列表构造 ClaimsIdentity，并指定认证类型及用户名、租户编号对应的声明类型
            var id = new ClaimsIdentity(
                claims.Distinct(new ClaimComparer()),
                AuthConstants.AuthenticationType,
                AuthConstants.JwtClaimTypes.Name,
                AuthConstants.JwtClaimTypes.TenantCode);
            return new ClaimsPrincipal(id);
        }
    }
}
