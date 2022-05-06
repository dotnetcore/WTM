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
        public static async Task<IActionResult> RedirectCall(this HttpRequest request, WTMContext wtm)
        {
            HttpMethodEnum method = Enum.Parse<HttpMethodEnum>(request.Method.ToString());
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + wtm.LoginUserInfo.RemoteToken);
            ApiResult<string> rv = null;
            if (method == HttpMethodEnum.GET)
            {
                rv = await wtm.CallAPI("mainhost", request.Path.ToString(),headers:headers);
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
                    rv = await wtm.CallAPI<string>("mainhost", request.Path.ToString(), method, data, headers: headers);
                }
                else
                {
                    string s = "";
                    if (request.HttpContext?.Items?.ContainsKey("DONOTUSE_REQUESTBODY")==true)
                    {
                        s = request.HttpContext.Items["DONOTUSE_REQUESTBODY"].ToString();
                    }
                    HttpContent data = new StringContent(s, System.Text.Encoding.UTF8, "application/json");
                    rv = await wtm.CallAPI<string>("mainhost", request.Path.ToString(), method, data, headers: headers);
                }
            }
            if(rv.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new OkObjectResult(rv.Data);
            }
            else if(rv.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return new UnauthorizedResult();
            }
            else if(rv.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return new ForbidResult();
            }
            else if(rv.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return new BadRequestObjectResult(rv.Errors);
            }
            return new BadRequestResult();
        }
    }
}
