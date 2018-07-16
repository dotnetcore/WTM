using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core
{
    public enum HttpMethodEnum { GET, POST, PUT, DELETE }

    /// <summary>
    /// 有关HTTP请求的辅助类
    /// </summary>
    public class APIHelper
    {
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        /// <summary>
        /// 调用远程地址
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="method">请求方式</param>
        /// <param name="postdata">Post数据</param>
        /// <param name="timeout">超时秒数</param>
        /// <param name="proxy">代理</param>
        /// <returns>远程方法返回的内容</returns>
        public static async Task<string> CallAPI(string url, HttpMethodEnum method = HttpMethodEnum.GET, IDictionary<string, string> postdata = null, int? timeout = null, string proxy = null)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return string.Empty;
                }
                //新建http请求
                var request = WebRequest.Create(url) as HttpWebRequest;
                //如果配置了代理，则使用代理
                if (string.IsNullOrEmpty(proxy) == false)
                {
                    request.Proxy = new WebProxy(proxy);
                }

                request.Method = method.ToString();
                //如果是Post请求，则设置表单
                if (method == HttpMethodEnum.POST || method == HttpMethodEnum.PUT)
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    if (postdata == null || postdata.Count == 0)
                    {
                        request.Headers[HttpRequestHeader.ContentLength] = "0";
                    }
                }
                request.Headers[HttpRequestHeader.UserAgent] = DefaultUserAgent;
                //设置超时
                if (timeout.HasValue)
                {
                    request.ContinueTimeout = timeout.Value;
                }
                request.CookieContainer = new CookieContainer();
                //填充表单数据
                if (!(postdata == null || postdata.Count == 0))
                {
                    var buffer = new StringBuilder();
                    var i = 0;
                    foreach (string key in postdata.Keys)
                    {
                        if (i > 0)
                        {
                            buffer.AppendFormat("&{0}={1}", key, postdata[key]);
                        }
                        else
                        {
                            buffer.AppendFormat("{0}={1}", key, postdata[key]);
                        }
                        i++;
                    }
                    var data = Encoding.UTF8.GetBytes(buffer.ToString());
                    using (var stream = await request.GetRequestStreamAsync())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }


                var res = await request.GetResponseAsync();
                var wsp = (HttpWebResponse)res;
                Stream st = null;
                //如果远程服务器采用了gzip，增进行相应的解压缩
                if (wsp.Headers[HttpResponseHeader.ContentEncoding]?.ToLower().Contains("gzip") == true)
                {
                    st = new GZipStream(st, CompressionMode.Decompress);
                }
                else
                {
                    st = wsp.GetResponseStream();
                }
                //设置编码
                var encode = Encoding.UTF8;
                if (!string.IsNullOrEmpty(wsp.Headers[HttpResponseHeader.ContentEncoding]))
                {
                    encode = Encoding.GetEncoding(wsp.Headers[HttpResponseHeader.ContentEncoding]);
                }
                //读取内容
                var sr = new StreamReader(st, encode);
                var ss = sr.ReadToEnd();
                sr.Dispose();
                wsp.Dispose();
                return ss;
            }
            catch
            {
                //返回失败信息
                ComboSelectListItem rv = new ComboSelectListItem()
                {
                    Text = "连接失败",
                    Value = null
                };
                return JsonSerialize(rv);
            }
        }

        /// <summary>
        /// 调用远程方法，返回强类型
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="url">地址</param>
        /// <param name="method">请求方式</param>
        /// <param name="postdata">Post数据</param>
        /// <param name="timeout">超时秒数</param>
        /// <param name="proxy">代理</param>
        /// <returns>强类型</returns>
        public static async Task<T> CallAPI<T>(string url, HttpMethodEnum method = HttpMethodEnum.GET, IDictionary<string, string> postdata = null, int? timeout = null, string proxy = null)
        {
            var s = await CallAPI(url, method, postdata, timeout, proxy);
            return JsonDeserialize<T>(s);
        }

        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns>Json</returns>
        public static string JsonSerialize(object input)
        {
            var s = JsonConvert.SerializeObject(input);
            if (s != null && s.StartsWith("[") == false)
            {
                s = "[" + s + "]";
            }
            return s;
        }

        /// <summary>
        /// Json反序列化
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="input">Json字符串</param>
        /// <returns>强类型</returns>
        public static T JsonDeserialize<T>(string input)
        {
            try
            {
                T rv = JsonConvert.DeserializeObject<T>(input);
                return rv;
            }
            catch
            {
                return default(T);
            }
        }
    }
}
