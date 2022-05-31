using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

using WalkingTec.Mvvm.Core.Auth;

namespace WalkingTec.Mvvm.Core.Extensions
{
    public static class LoginUserInfoExtension
    {
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
            var id = new ClaimsIdentity(
                claims.Distinct(new ClaimComparer()),
                AuthConstants.AuthenticationType,
                AuthConstants.JwtClaimTypes.Name,
                AuthConstants.JwtClaimTypes.TenantCode);
            return new ClaimsPrincipal(id);
        }
    }
}
