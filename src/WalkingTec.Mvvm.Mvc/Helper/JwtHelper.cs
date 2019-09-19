using System;
using System.Collections.Generic;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.ConfigOptions;

namespace WalkingTec.Mvvm.Mvc
{
    public class JwtHelper
    {

        public static string BuildJwtToken(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            var now = DateTime.Now;
            var configuration = GlobalServices.GetRequiredService<Configs>();

            var jwtConfig = configuration.JwtConfig;
            var expiration = TimeSpan.FromSeconds(jwtConfig.Expiration);
            var key = Encoding.ASCII.GetBytes(jwtConfig.SecurityKey);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var handler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, userId)
            };

            var securityToken = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration),
                signingCredentials: credentials
            );

            var token = handler.WriteToken(securityToken);

            return token;
        }

        public static JsonResult ToJwtTokenResult(string token, string name)
        {
            var configuration = GlobalServices.GetRequiredService<Configs>();

            var jwtConfig = configuration.JwtConfig;
            var expiration = TimeSpan.FromSeconds(jwtConfig.Expiration);

            var responseJson = new
            {
                success = true,
                name,
                token = HttpContextExtention.BEARER_PREFIX + token,
                expires_in = expiration.TotalSeconds
            };

            return new JsonResult(responseJson);
        }


        public static void CacheUer(LoginUserInfo userInfo, string token)
        {
            if (token == null)
                return;

            var jwtConfig = GlobalServices.GetRequiredService<Configs>().JwtConfig;
            var expiration = TimeSpan.FromSeconds(jwtConfig.Expiration + 60);

            var memoryCache = GlobalServices.GetRequiredService<IMemoryCache>();

            var key = string.Format(DefaultConfigConsts.DEFAULT_USER_SIGN_IN_KEY, userInfo.Id.ToString());

            if (memoryCache.TryGetValue(key, out var value))
                memoryCache.Remove(key);
            memoryCache.Set(key, new LoginUserInfoCache { UserInfo = userInfo, Token = token }, expiration);
        }

        public static bool Signin(string userId, string token, HttpContext httpContext)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                httpContext.Session?.Clear();
                return false;
            }

            var key = string.Format(DefaultConfigConsts.DEFAULT_USER_SIGN_IN_KEY, userId);

            var memoryCache = GlobalServices.GetRequiredService<IMemoryCache>();
            if (memoryCache.TryGetValue(key, out var cacheObj))
            {
                if (cacheObj is LoginUserInfoCache loginUserCache)
                {
                    if (token.Equals(loginUserCache.Token))
                    {
                        httpContext.Session?.Set("UserInfo", loginUserCache.UserInfo);
                        return true;
                    }
                    else
                    {
                        httpContext.Session?.Clear();
                        return false;
                    }
                }
            }
            var userInfo = GetUserById(userId);
            if (userInfo == null)
            {
                httpContext.Session?.Clear();
                return false;
            }
            CacheUer(userInfo, token);
            return Signin(userId, token, httpContext);
        }

        private class LoginUserInfoCache
        {
            public LoginUserInfo UserInfo { get; set; }
            public string Token { get; set; }
        }

        private static LoginUserInfo GetUserById(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;
            var id = Guid.Parse(userId);
            if (id == Guid.Empty)
                return null;

            var configs = GlobalServices.GetRequiredService<Configs>();
            var globalData = GlobalServices.GetRequiredService<GlobalData>();
            // 尝试 default
            var csName = Utils.GetCS("default", "Read", configs);
            var connectionStrings = configs.ConnectionStrings.Where(x => x.Key.ToLower() == csName).Select(x => x.Value);
            if (!connectionStrings.Any()) //default 不存在，遍历所有配置库
            {
                connectionStrings = configs.ConnectionStrings.Select(x => x.Value);
            }

            foreach (var connectionString in connectionStrings)
            {
                var dataContext = (IDataContext)globalData.DataContextCI.Invoke(new object[] { connectionString, configs.DbType });

                var user = dataContext.Set<Core.FrameworkUserBase>()
                    .Include(x => x.UserRoles).Include(x => x.UserGroups)
                    .Where(x => x.ID == id && x.IsValid).SingleOrDefault();

                if (user == null)
                    continue;

                var roleIDs = user.UserRoles.Select(x => x.RoleId).ToList();
                var groupIDs = user.UserGroups.Select(x => x.GroupId).ToList();

                //查找登录用户的角色
                var roles = dataContext.Set<FrameworkRole>().Where(x => roleIDs.Contains(x.ID)).ToList();

                //查找登录用户的组
                var groups = dataContext.Set<FrameworkGroup>().Where(x => groupIDs.Contains(x.ID)).ToList();

                //查找登录用户的数据权限
                var dataPrivileges = dataContext.Set<DataPrivilege>().Where(x => x.UserId == user.ID || (x.GroupId != null && groupIDs.Contains(x.GroupId.Value))).ToList();

                //查找登录用户的页面权限
                var functionPrivileges = dataContext.Set<FunctionPrivilege>().Where(x => x.UserId == user.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value))).ToList();

                //生成并返回登录用户信息
                var userInfo = new LoginUserInfo
                {
                    Id = user.ID,
                    ITCode = user.ITCode,
                    Name = user.Name,
                    Roles = roles,
                    Groups = groups,
                    DataPrivileges = dataPrivileges,
                    PhotoId = user.PhotoId,
                    FunctionPrivileges = functionPrivileges
                };

                return userInfo;
            }
            return null;
        }
    }
}
