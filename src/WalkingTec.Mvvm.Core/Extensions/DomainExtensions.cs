using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Core.Extensions
{
    public static class DomainExtensions
    {

        public static async Task<T> CallAPI<T>(this FrameworkDomain self, string url, HttpMethodEnum method ,HttpContent content,  ErrorObj error=null, string errormsg=null, int? timeout = null, string proxy = null)
        {
            var factory = GlobalServices.GetRequiredService<IHttpClientFactory>();
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return default(T);
                }
                //新建http请求
                var client = factory.CreateClient(self.Name);
                //如果配置了代理，则使用代理
                //设置超时
                if (timeout.HasValue)
                {
                    client.Timeout = new TimeSpan(0, 0, 0, timeout.Value, 0);
                }
                //填充表单数据
                HttpResponseMessage res = null;
                switch (method)
                {
                    case HttpMethodEnum.GET:
                        res = await client.GetAsync(url);
                        break;
                    case HttpMethodEnum.POST:
                        res = await client.PostAsync(url, content);
                        break;
                    case HttpMethodEnum.PUT:
                        res = await client.PutAsync(url, content);
                        break;
                    case HttpMethodEnum.DELETE:
                        res = await client.DeleteAsync(url);
                        break;
                    default:
                        break;
                }
                T rv = default(T);
                if(res == null)
                {
                    return rv;
                }
                if (res.IsSuccessStatusCode == true)
                {
                    rv = JsonConvert.DeserializeObject<T>(await res.Content.ReadAsStringAsync());
                }
                else
                {
                    if (res.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        error = JsonConvert.DeserializeObject<ErrorObj>(await res.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        errormsg = await res.Content.ReadAsStringAsync();
                    }
                }

                return rv;
            }
            catch (Exception ex)
            {
                errormsg = ex.ToString();
                return default(T);
            }
        }

        /// <summary>
        /// 使用Get方法调用api
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="url">调用地址</param>
        /// <param name="error">如果是框架识别的错误格式，将返回ErrorObj</param>
        /// <param name="errormsg">如果框架不识别错误格式，返回错误文本</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public static async Task<T> CallAPI<T>(this FrameworkDomain self, string url,ErrorObj error=null, string errormsg=null, int? timeout = null, string proxy = null)
        {
            HttpContent content = null;
            //填充表单数据
            return await CallAPI<T>(self, url, HttpMethodEnum.GET, content, error, errormsg, timeout, proxy);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="url">调用地址</param>
        /// <param name="method">调用方式</param>
        /// <param name="postdata">提交字段</param>
        /// <param name="error">如果是框架识别的错误格式，将返回ErrorObj</param>
        /// <param name="errormsg">如果框架不识别错误格式，返回错误文本</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public static async Task<T> CallAPI<T>(this FrameworkDomain self, string url, HttpMethodEnum method,IDictionary<string, string> postdata, ErrorObj error=null, string errormsg=null,int? timeout = null, string proxy = null)
        {
                HttpContent content = null;
                //填充表单数据
                if (!(postdata == null || postdata.Count == 0))
                {
                    List<KeyValuePair<string, string>> paras = new List<KeyValuePair<string, string>>();
                    foreach (string key in postdata.Keys)
                    {
                        paras.Add(new KeyValuePair<string, string>( key, postdata[key]));
                    }
                    content = new FormUrlEncodedContent(paras);
                }
                return await CallAPI<T>(self,url,method,content,error,errormsg,timeout,proxy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="url">调用地址</param>
        /// <param name="method">调用方式</param>
        /// <param name="postdata">提交的object，会被转成json提交</param>
        /// <param name="error">如果是框架识别的错误格式，将返回ErrorObj</param>
        /// <param name="errormsg">如果框架不识别错误格式，返回错误文本</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public static async Task<T> CallAPI<T>(this FrameworkDomain self, string url, HttpMethodEnum method, object postdata, ErrorObj error=null, string errormsg=null, int? timeout = null, string proxy = null)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(postdata), System.Text.Encoding.UTF8, "application/json");
            return await CallAPI<T>(self, url, method, content, error, errormsg, timeout, proxy);
        }




        public static async Task<string> CallAPI(this FrameworkDomain self, string url, HttpMethodEnum method, HttpContent content, ErrorObj error = null, string errormsg = null, int? timeout = null, string proxy = null)
        {
            var factory = GlobalServices.GetRequiredService<IHttpClientFactory>();
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return "";
                }
                //新建http请求
                var client = factory.CreateClient(self.Name);
                //如果配置了代理，则使用代理
                //设置超时
                if (timeout.HasValue)
                {
                    client.Timeout = new TimeSpan(0, 0, 0, timeout.Value, 0);
                }
                //填充表单数据
                HttpResponseMessage res = null;
                switch (method)
                {
                    case HttpMethodEnum.GET:
                        res = await client.GetAsync(url);
                        break;
                    case HttpMethodEnum.POST:
                        res = await client.PostAsync(url, content);
                        break;
                    case HttpMethodEnum.PUT:
                        res = await client.PutAsync(url, content);
                        break;
                    case HttpMethodEnum.DELETE:
                        res = await client.DeleteAsync(url);
                        break;
                    default:
                        break;
                }
                string rv = "";
                if (res == null)
                {
                    return rv;
                }
                if (res.IsSuccessStatusCode == true)
                {
                    rv =await res.Content.ReadAsStringAsync();
                }
                else
                {
                    if (res.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        error = JsonConvert.DeserializeObject<ErrorObj>(await res.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        errormsg = await res.Content.ReadAsStringAsync();
                    }
                }

                return rv;
            }
            catch (Exception ex)
            {
                errormsg = ex.ToString();
                return "";
            }
        }

        /// <summary>
        /// 使用Get方法调用api
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="url">调用地址</param>
        /// <param name="error">如果是框架识别的错误格式，将返回ErrorObj</param>
        /// <param name="errormsg">如果框架不识别错误格式，返回错误文本</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public static async Task<string> CallAPI(this FrameworkDomain self, string url, ErrorObj error = null, string errormsg = null, int? timeout = null, string proxy = null)
        {
            HttpContent content = null;
            //填充表单数据
            return await CallAPI(self, url, HttpMethodEnum.GET, content, error, errormsg, timeout, proxy);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="url">调用地址</param>
        /// <param name="method">调用方式</param>
        /// <param name="postdata">提交字段</param>
        /// <param name="error">如果是框架识别的错误格式，将返回ErrorObj</param>
        /// <param name="errormsg">如果框架不识别错误格式，返回错误文本</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public static async Task<string> CallAPI(this FrameworkDomain self, string url, HttpMethodEnum method, IDictionary<string, string> postdata, ErrorObj error = null, string errormsg = null, int? timeout = null, string proxy = null)
        {
            HttpContent content = null;
            //填充表单数据
            if (!(postdata == null || postdata.Count == 0))
            {
                List<KeyValuePair<string, string>> paras = new List<KeyValuePair<string, string>>();
                foreach (string key in postdata.Keys)
                {
                    paras.Add(new KeyValuePair<string, string>(key, postdata[key]));
                }
                content = new FormUrlEncodedContent(paras);
            }
            return await CallAPI(self, url, method, content, error, errormsg, timeout, proxy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="url">调用地址</param>
        /// <param name="method">调用方式</param>
        /// <param name="postdata">提交的object，会被转成json提交</param>
        /// <param name="error">如果是框架识别的错误格式，将返回ErrorObj</param>
        /// <param name="errormsg">如果框架不识别错误格式，返回错误文本</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public static async Task<string> CallAPI(this FrameworkDomain self, string url, HttpMethodEnum method, object postdata, ErrorObj error = null, string errormsg = null, int? timeout = null, string proxy = null)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(postdata), System.Text.Encoding.UTF8, "application/json");
            return await CallAPI(self, url, method, content, error, errormsg, timeout, proxy);
        }

        public static async Task<byte[]> CallStreamAPI(this FrameworkDomain self, string url, HttpMethodEnum method, object postdata, ErrorObj error = null, string errormsg = null, int? timeout = null, string proxy = null)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(postdata), System.Text.Encoding.UTF8, "application/json");
            var factory = GlobalServices.GetRequiredService<IHttpClientFactory>();
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return null;
                }
                //新建http请求
                var client = factory.CreateClient(self.Name);
                //如果配置了代理，则使用代理
                //设置超时
                if (timeout.HasValue)
                {
                    client.Timeout = new TimeSpan(0, 0, 0, timeout.Value, 0);
                }
                //填充表单数据
                HttpResponseMessage res = null;
                switch (method)
                {
                    case HttpMethodEnum.GET:
                        res = await client.GetAsync(url);
                        break;
                    case HttpMethodEnum.POST:
                        res = await client.PostAsync(url, content);
                        break;
                    case HttpMethodEnum.PUT:
                        res = await client.PutAsync(url, content);
                        break;
                    case HttpMethodEnum.DELETE:
                        res = await client.DeleteAsync(url);
                        break;
                    default:
                        break;
                }
                byte[] rv = null;
                if (res == null)
                {
                    return rv;
                }
                if (res.IsSuccessStatusCode == true)
                {
                    rv = await res.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    if (res.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        error = JsonConvert.DeserializeObject<ErrorObj>(await res.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        errormsg = await res.Content.ReadAsStringAsync();
                    }
                }

                return rv;
            }
            catch (Exception ex)
            {
                errormsg = ex.ToString();
                return null;
            }

        }
    }
}
