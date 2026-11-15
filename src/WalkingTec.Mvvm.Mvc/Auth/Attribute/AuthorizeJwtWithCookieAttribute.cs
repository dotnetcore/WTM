using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// 混合认证授权特性，同时支持 Cookie 与 JWT Bearer 两种身份验证方案，任一方案验证通过即可完成授权
    /// </summary>
    public class AuthorizeJwtWithCookieAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 初始化特性，将身份验证方案设置为 Cookie 认证和 JWT Bearer 认证
        /// </summary>
        public AuthorizeJwtWithCookieAttribute()
        {
            AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{JwtBearerDefaults.AuthenticationScheme}";
        }
    }
}
