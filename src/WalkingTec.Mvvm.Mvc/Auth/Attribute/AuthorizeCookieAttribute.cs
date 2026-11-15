using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// Cookie 认证授权特性，限定当前请求仅使用 Cookie 身份验证方案进行授权
    /// </summary>
    public class AuthorizeCookieAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 初始化特性，将身份验证方案设置为 Cookie 认证
        /// </summary>
        public AuthorizeCookieAttribute()
        {
            AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme;
        }
    }

}
