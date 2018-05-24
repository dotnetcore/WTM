using System;
using System.Net;

namespace WalkingTec.Mvvm.Core
{
    public class WebProxy : IWebProxy
    {
        public WebProxy(string proxyUri)
        : this(new Uri(proxyUri))
        {
        }

        public WebProxy(Uri proxyUri)
        {
            this.ProxyUri = proxyUri;
        }

        public Uri ProxyUri { get; set; }

        public ICredentials Credentials { get; set; }

        public Uri GetProxy(Uri destination)
        {
            return this.ProxyUri;
        }

        public bool IsBypassed(Uri host)
        {
            return false; /* Proxy all requests */
        }
    }

}
