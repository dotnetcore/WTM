using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WalkingTec.Mvvm.Core.Auth.Attribute
{
    public class AuthorizeJwtAttribute : AuthorizeAttribute
    {
        public AuthorizeJwtAttribute()
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }
    }

}
