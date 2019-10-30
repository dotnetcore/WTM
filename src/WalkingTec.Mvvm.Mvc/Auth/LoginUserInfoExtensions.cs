using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth;

namespace WalkingTec.Mvvm.Mvc.Auth
{
    public static class LoginUserInfoExtensions
    {
        public static ClaimsPrincipal CreatePrincipal(this LoginUserInfo self)
        {
            if (self.Id == Guid.Empty) throw new ArgumentException("Id is mandatory", nameof(self.Id));
            var claims = new List<Claim> { new Claim(AuthConstants.JwtClaimTypes.Subject, self.Id.ToString()) };

            if (!string.IsNullOrEmpty(self.Name))
            {
                claims.Add(new Claim(AuthConstants.JwtClaimTypes.Name, self.Name));
            }

            // if (AuthenticationTime.HasValue)
            // {
            //     claims.Add(new Claim(AuthConstants.JwtClaimTypes.AuthenticationTime, new DateTimeOffset(AuthenticationTime.Value).ToUnixTimeSeconds().ToString()));
            // }

            // if (AuthenticationMethods.Any())
            // {
            //     foreach (var amr in AuthenticationMethods)
            //     {
            //         claims.Add(new Claim(AuthConstants.JwtClaimTypes.AuthenticationMethod, amr));
            //     }
            // }

            // claims.AddRange(AdditionalClaims);

            var id = new ClaimsIdentity(
                claims.Distinct(new ClaimComparer()),
                AuthConstants.AuthenticationType,
                AuthConstants.JwtClaimTypes.Name,
                AuthConstants.JwtClaimTypes.Role);
            return new ClaimsPrincipal(id);
        }
    }
}
