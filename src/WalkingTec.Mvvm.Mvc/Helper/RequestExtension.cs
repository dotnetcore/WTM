using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    public static class RequestExtension
    {
        public static async Task<IActionResult> RedirectCall(this HttpRequest baserequest, WTMContext wtm, string url = null)
        {
            var request = baserequest ?? wtm.HttpContext.Request;
            HttpMethodEnum method = Enum.Parse<HttpMethodEnum>(request.Method.ToString());
            if (string.IsNullOrEmpty(url))
            {
                url = request.Path.ToString();
            }
            url += request.QueryString;
            ApiResult<string> rv = null;
            if (method == HttpMethodEnum.GET)
            {
                rv = await wtm.CallAPI("mainhost", url);
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
                    rv = await wtm.CallAPI<string>("mainhost", url, method, data);
                }
                else
                {
                    string s = "";
                    if (request.HttpContext?.Items?.ContainsKey("DONOTUSE_REQUESTBODY")==true)
                    {
                        s = request.HttpContext.Items["DONOTUSE_REQUESTBODY"].ToString();
                    }
                    HttpContent data = new StringContent(s, System.Text.Encoding.UTF8, "application/json");
                    rv = await wtm.CallAPI<string>("mainhost", url, method, data);
                }
            }
            return rv.ToActionResult();
        }

        public static IActionResult ToActionResult(this ApiResult<string> self)
        {
            if(self == null)
            {
                return new BadRequestResult();
            }
            if (self.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new OkObjectResult(self.Data);
            }
            else if (self.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return new UnauthorizedResult();
            }
            else if (self.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return new ForbidResult();
            }
            else if (self.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return new BadRequestObjectResult(self.Errors);
            }
            return new BadRequestResult();
        }
    }
}
