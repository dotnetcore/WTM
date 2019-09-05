using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    public class JwtHelper
    {

        public static object BuildJwtToken(FrameworkUserBase user, List<FrameworkRole> roles)
        {
            if (user == null)
                return null;

            var now = DateTime.Now;
            var configuration = GlobalServices.GetRequiredService<Configs>();

            var jwtConfig = configuration.JwtConfig;
            var expiration = new TimeSpan(0, 0, jwtConfig.Expiration);
            var key = Encoding.ASCII.GetBytes(jwtConfig.SecurityKey);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var handler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, user.ID.ToString())
            };

            //if (roles != null)
            //    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.RoleCode)));

            var securityToken = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration),
                signingCredentials: credentials
            );

            var token = handler.WriteToken(securityToken);

            //打包返回前台
            var responseJson = new
            {
                success = true,
                name = user.Name,
                token = "Bearer " + token,
                expires_in = expiration.TotalSeconds
            };

            return responseJson;
        }
    }
}
