using Microsoft.AspNetCore.Authentication.Cookies;

namespace WalkingTec.Mvvm.Core.Auth
{
    public class CookieOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int Expires { get; set; }
        public bool SlidingExpiration { get; set; } = true;
        public string LoginPath { get; set; } = CookieAuthenticationDefaults.LoginPath;
        public string LogoutPath { get; set; } = CookieAuthenticationDefaults.LogoutPath;
        public string AccessDeniedPath { get; set; } = CookieAuthenticationDefaults.AccessDeniedPath;
        public string ReturnUrlParameter { get; set; } = CookieAuthenticationDefaults.ReturnUrlParameter;
    }
}
