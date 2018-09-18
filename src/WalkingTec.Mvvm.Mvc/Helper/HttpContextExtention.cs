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

    }
}
