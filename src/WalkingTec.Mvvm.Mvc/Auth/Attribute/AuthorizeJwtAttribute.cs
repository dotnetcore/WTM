using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// JWT 认证授权特性，限定当前请求仅使用 JWT Bearer 身份验证方案进行授权
    /// </summary>
    public class AuthorizeJwtAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 初始化特性，将身份验证方案设置为 JWT Bearer 认证
        /// </summary>
        public AuthorizeJwtAttribute()
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }
    }

}
