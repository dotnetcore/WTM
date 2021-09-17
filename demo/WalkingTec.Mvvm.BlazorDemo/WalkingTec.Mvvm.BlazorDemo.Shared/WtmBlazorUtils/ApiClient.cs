using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Microsoft.JSInterop;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WtmBlazorUtils
{
    public class ApiClient
    {
        public HttpClient Client { get; }
        private IJSRuntime js { get; }
        public ApiClient(HttpClient client, IJSRuntime jsr)
        {
            Client = client;
            js = jsr;
        }

        public void SetToken(string token)
        {
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        #region CallApi
        public async Task<ApiResult<T>> CallAPI<T>(string url, HttpMethodEnum method, HttpContent content, int? timeout = null, string proxy = null) where T : class
        {
            var token = await js.InvokeAsync<string>("localStorageFuncs.get", "wtmtoken");
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            ApiResult<T> rv = new ApiResult<T>();
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return rv;
                }
                url = url.AppendQuery("culture=" + System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
                //如果配置了代理，则使用代理
                //设置超时
                if (timeout.HasValue)
                {
                    Client.Timeout = new TimeSpan(0, 0, 0, timeout.Value, 0);
                }
                //填充表单数据
                HttpResponseMessage res = null;
                switch (method)
                {
                    case HttpMethodEnum.GET:
                        res = await Client.GetAsync(url);
                        break;
                    case HttpMethodEnum.POST:
                        res = await Client.PostAsync(url, content);
                        break;
                    case HttpMethodEnum.PUT:
                        res = await Client.PutAsync(url, content);
                        break;
                    case HttpMethodEnum.DELETE:
                        res = await Client.DeleteAsync(url);
                        break;
                    default:
                        break;
                }
                if (res == null)
                {
                    return rv;
                }
                rv.StatusCode = res.StatusCode;
                if (res.IsSuccessStatusCode == true)
                {
                    Type dt = typeof(T);
                    if (dt == typeof(byte[]))
                    {
                        rv.Data = await res.Content.ReadAsByteArrayAsync() as T;
                    }
                    else
                    {
                        string responseTxt = await res.Content.ReadAsStringAsync();
                        if (dt == typeof(string))
                        {
                            rv.Data = responseTxt as T;
                        }
                        else
                        {
                            rv.Data = JsonSerializer.Deserialize<T>(responseTxt, CoreProgram.DefaultJsonOption);
                        }
                    }
                }
                else
                {
                    if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        await js.InvokeVoidAsync("localStorageFuncs.remove", "wtmtoken");
                        await js.InvokeVoidAsync("urlFuncs.redirect","/login");
                    }
                    string responseTxt = await res.Content.ReadAsStringAsync();
                    if (res.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        try
                        {
                            rv.Errors = JsonSerializer.Deserialize<ErrorObj>(responseTxt, CoreProgram.DefaultJsonOption);
                        }
                        catch { }
                    }
                    rv.ErrorMsg = responseTxt;
                }

                return rv;
            }
            catch (Exception ex)
            {
                rv.ErrorMsg = ex.ToString();
                return rv;
            }
        }

        /// <summary>
        /// 使用Get方法调用api
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domainName">Appsettings中配置的Domain key</param>
        /// <param name="url">调用地址</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public async Task<ApiResult<T>> CallAPI<T>(string url, int? timeout = null, string proxy = null) where T : class
        {
            HttpContent content = null;
            //填充表单数据
            return await CallAPI<T>(url, HttpMethodEnum.GET, content, timeout, proxy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domainName">Appsettings中配置的Domain key</param>
        /// <param name="url">调用地址</param>
        /// <param name="method">调用方式</param>
        /// <param name="postdata">提交字段</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public async Task<ApiResult<T>> CallAPI<T>(string url, HttpMethodEnum method, IDictionary<string, string> postdata, int? timeout = null, string proxy = null) where T : class
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
                if (paras.Any())
                {
                    url = url.AppendQuery(paras);
                }
                content = new FormUrlEncodedContent(paras);
            }
            return await CallAPI<T>(url, method, content, timeout, proxy);
        }

        public async Task<ApiResult<T>> CallAPI<T>(string url, HttpMethodEnum method, IDictionary<string, string> postdata, byte[] filedata, string filename, int? timeout = null, string proxy = null) where T : class
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            //填充表单数据
            if (!(postdata == null || postdata.Count == 0))
            {
                List<KeyValuePair<string, string>> paras = new List<KeyValuePair<string, string>>();
                foreach (string key in postdata.Keys)
                {
                    if (postdata[key] != null)
                    {
                        content.Add(new StringContent(postdata[key]), key);
                    }
                }
            }
            content.Add(new ByteArrayContent(filedata), "File", filename);

            return await CallAPI<T>(url, method, content, timeout, proxy);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domainName">Appsettings中配置的Domain key</param>
        /// <param name="url">调用地址</param>
        /// <param name="method">调用方式</param>
        /// <param name="postdata">提交的object，会被转成json提交</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public async Task<ApiResult<T>> CallAPI<T>(string url, HttpMethodEnum method, object postdata, int? timeout = null, string proxy = null) where T : class
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(postdata, CoreProgram.DefaultPostJsonOption), System.Text.Encoding.UTF8, "application/json");
            return await CallAPI<T>(url, method, content, timeout, proxy);
        }

        public async Task<ApiResult<string>> CallAPI(string url, HttpMethodEnum method, HttpContent content, int? timeout = null, string proxy = null)
        {
            return await CallAPI<string>(url, method, content, timeout, proxy);
        }

        /// <summary>
        /// 使用Get方法调用api
        /// </summary>
        /// <param name="domainName">Appsettings中配置的Domain key</param>
        /// <param name="url">调用地址</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public async Task<ApiResult<string>> CallAPI(string url, int? timeout = null, string proxy = null)
        {
            return await CallAPI<string>(url, timeout, proxy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainName">Appsettings中配置的Domain key</param>
        /// <param name="url">调用地址</param>
        /// <param name="method">调用方式</param>
        /// <param name="postdata">提交字段</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public async Task<ApiResult<string>> CallAPI(string url, HttpMethodEnum method, IDictionary<string, string> postdata, int? timeout = null, string proxy = null)
        {
            return await CallAPI<string>(url, method, postdata, timeout, proxy);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainName">Appsettings中配置的Domain key</param>
        /// <param name="url">调用地址</param>
        /// <param name="method">调用方式</param>
        /// <param name="postdata">提交的object，会被转成json提交</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public async Task<ApiResult<string>> CallAPI(string url, HttpMethodEnum method, object postdata, int? timeout = null, string proxy = null)
        {
            return await CallAPI<string>(url, method, postdata, timeout, proxy);
        }

        #endregion

        public async Task<ApiResult<WtmApiResult<T>>> CallSearchApi<T>(string url, BaseSearcher searcher, QueryPageOptions options) where T : class, new()
        {
            searcher.Page = options.PageIndex;
            searcher.Limit = options.PageItems;
            if (string.IsNullOrEmpty(options.SortName) == false && options.SortOrder != SortOrder.Unset)
            {
                searcher.SortInfo = new SortInfo
                {
                    Property = options.SortName,
                    Direction = options.SortOrder == SortOrder.Desc ? SortDir.Desc : SortDir.Asc
                };
            }
            else
            {
                searcher.SortInfo = null;
            }
            var rv = await CallAPI<WtmApiResult<T>>(url, HttpMethodEnum.POST, searcher);
            return rv;
        }

        public async Task<List<SelectedItem>> CallItemsApi(string url, HttpMethodEnum method = HttpMethodEnum.GET, object postdata = null, int? timeout = null, string proxy = null, string placeholder = null,List<string> values = null)
        {
            var result = await CallAPI<List<ComboSelectListItem>>(url, method, postdata, timeout, proxy);
            List<SelectedItem> rv = new List<SelectedItem>();
            if(values == null)
            {
                values = new List<string>();
            }
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (result.Data != null)
                {
                    foreach (var item in result.Data)
                    {
                        rv.Add(new SelectedItem
                        {
                            Text = item.Text,
                            Value = item.Value.ToString(),
                            Active = values.Any(x=>x.Contains(item.Value.ToString()))
                        });
                    }
                }
                if(string.IsNullOrEmpty(placeholder) == false)
                {
                    rv.Insert(0, new SelectedItem
                    {
                        Text = placeholder,
                        Value = ""
                    });
                }
            }
            return rv;
        }

    }

    public class WtmApiResult<T>
    {
        public List<T> Data { get; set; }
        public int Count { get; set; }
        public int Page { get; set; }
        public int PageCount { get; set; }
        public string Msg { get; set; }
        public int Code { get; set; }
    }
}
