using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WalkingTec.Mvvm.Core.Extensions
{
    public static class RequestExtension
    {
        public static async Task<ApiResult<string>> RedirectCall(this HttpRequest request, WTMContext wtm, string domainname)
        {
            HttpMethodEnum method = Enum.Parse<HttpMethodEnum>(request.Method.ToString());
            ApiResult<string> rv = null;
            if (method == HttpMethodEnum.GET)
            {
                rv = await wtm.CallAPI("mainhost", request.Path.ToString());
            }
            else
            {
                if(request.HasFormContentType == true)
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    foreach (var item in request.Form)
                    {
                        data.Add(item.Key, item.Value);
                    }
                    rv = await wtm.CallAPI<string>("mainhost", request.Path.ToString(), method, data);
                }
                else
                {
                    HttpContent data = new StreamContent(request.Body);
                    rv = await wtm.CallAPI<string>("mainhost", request.Path.ToString(), method, data);
                }
            }
            return rv;
        }
    }
}
