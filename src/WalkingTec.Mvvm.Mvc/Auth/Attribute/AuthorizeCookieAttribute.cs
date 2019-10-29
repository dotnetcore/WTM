using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace WalkingTec.Mvvm.Mvc.Auth.Attribute
{
    public class AuthorizeCookieAttribute : AuthorizeAttribute
    {
        public AuthorizeCookieAttribute()
        {
            AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme;
        }
    }

}
