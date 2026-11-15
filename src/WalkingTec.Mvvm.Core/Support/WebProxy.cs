using System;
using System.Net;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// Web 代理配置类，实现 IWebProxy 接口，将所有请求统一转发到指定代理地址
    /// </summary>
    public class WebProxy : IWebProxy
    {
        /// <summary>
        /// 使用代理地址字符串构造 Web 代理
        /// </summary>
        /// <param name="proxyUri">代理地址字符串</param>
        public WebProxy(string proxyUri)
        : this(new Uri(proxyUri))
        {
        }

        /// <summary>
        /// 使用代理地址构造 Web 代理
        /// </summary>
        /// <param name="proxyUri">代理地址</param>
        public WebProxy(Uri proxyUri)
        {
            this.ProxyUri = proxyUri;
        }

        /// <summary>
        /// 代理服务器地址
        /// </summary>
        public Uri ProxyUri { get; set; }

        /// <summary>
        /// 代理服务器的认证凭据
        /// </summary>
        public ICredentials Credentials { get; set; }

        /// <summary>
        /// 获取指定目标地址所使用的代理
        /// </summary>
        /// <param name="destination">请求的目标地址</param>
        /// <returns>代理服务器地址</returns>
        public Uri GetProxy(Uri destination)
        {
            return this.ProxyUri;
        }

        /// <summary>
        /// 判断指定主机是否绕过代理，本实现始终返回 false，即所有请求均经过代理
        /// </summary>
        /// <param name="host">请求的目标主机地址</param>
        /// <returns>是否绕过代理，恒为 false</returns>
        public bool IsBypassed(Uri host)
        {
            return false; /* Proxy all requests */
        }
    }

}
