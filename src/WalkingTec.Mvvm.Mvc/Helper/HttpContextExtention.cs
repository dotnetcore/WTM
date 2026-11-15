using System.Linq;
using Microsoft.AspNetCore.Http;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// HttpContext 扩展类，提供获取客户端 IP 地址等方法
    /// </summary>
    public static class HttpContextExtention
    {
        /// <summary>
        /// 客户端 Ip 头
        /// </summary>
        public const string REMOTE_IP_HEADER = "X-Forwarded-For";

        /// <summary>
        /// 获取客户端真实 IP 地址
        /// </summary>
        /// <param name="self">当前 HTTP 上下文</param>
        /// <returns>客户端 IP 地址；存在反向代理时返回代理转发的原始客户端 IP</returns>
        public static string GetRemoteIpAddress(this HttpContext self)
        {
            // 优先从 X-Forwarded-For 请求头获取经过反向代理转发的客户端 IP
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
