using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WalkingTec.Mvvm.Mvc.Auth.Attribute
{
    public class AuthorizeJwtWithCookieAttribute : AuthorizeAttribute
    {
        public AuthorizeJwtWithCookieAttribute()
        {
            AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}";
        }
    }
}
