using Microsoft.AspNetCore.Http;
using System.Linq;

namespace WalkingTec.Mvvm.Mvc
{
    public static class HttpContextExtention
    {
        /// <summary>
        /// 客户端 Ip 头
        /// </summary>
        public const string REMOTE_IP_HEADER = "X-Forwarded-For";

        public static string GetRemoteIpAddress(this HttpContext self)
        {
            var proxyIp = self.Request?.Headers?[REMOTE_IP_HEADER].FirstOrDefault();
            if (!string.IsNullOrEmpty(proxyIp))
            {
                return proxyIp;
            }
            else
            {
                return self.Connection.RemoteIpAddress.ToString();
            }
        }

        /// <summary>
        /// 授权头
        /// </summary>
        public const string AUTHORIZATION_HEADER = "Authorization";

        /// <summary>
        /// Jwt 前缀
        /// </summary>
        public const string BEARER_PREFIX = "Bearer ";

        public static string GetJwtToken(this HttpContext self)
        {
           var token = self.Request?.Headers?[AUTHORIZATION_HEADER].FirstOrDefault();
            if (!string.IsNullOrEmpty(token) && token.StartsWith(BEARER_PREFIX))
                token = token.Substring(BEARER_PREFIX.Length).Trim();
            return token;
        }
    }
}
