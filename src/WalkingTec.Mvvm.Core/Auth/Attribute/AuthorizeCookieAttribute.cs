using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace WalkingTec.Mvvm.Core.Auth.Attribute
{
    public class AuthorizeCookieAttribute : AuthorizeAttribute
    {
        public AuthorizeCookieAttribute()
        {
            AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme;
        }
    }

}
