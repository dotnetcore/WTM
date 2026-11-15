using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// HTTP 请求相关扩展类，支持把当前请求转发到主站并将接口调用结果转换为 MVC 响应
    /// </summary>
    public static class RequestExtension
    {
        /// <summary>
        /// 将当前请求转发到主站（mainhost 域名）并将远端响应转换为 IActionResult 返回，
        /// 用于多租户等场景下把子站请求交给主站统一处理
        /// </summary>
        /// <param name="baserequest">原始 HTTP 请求，为空时使用 wtm 中保存的当前请求</param>
        /// <param name="wtm">WTM 上下文，提供 CallAPI 远程调用能力</param>
        /// <param name="url">目标地址，为空时使用当前请求的路径</param>
        /// <returns>远端接口结果转换后的 IActionResult</returns>
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
            // GET 请求直接调用主站接口
            if (method == HttpMethodEnum.GET)
            {
                rv = await wtm.CallAPI("mainhost", url);
            }
            else
            {
                // 表单请求读取 Form 集合并以表单方式提交；其他请求读取缓存的请求体并以 JSON 内容提交
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

        /// <summary>
        /// 将接口调用结果转换为与状态码对应的 ActionResult（200 返回数据，401/403/400 返回相应错误结果）
        /// </summary>
        /// <param name="self">接口调用结果对象</param>
        /// <returns>与状态码对应的 ActionResult</returns>
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
