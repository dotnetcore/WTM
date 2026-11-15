using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// WTM 核心中间件：统一处理请求体大小限制、非表单请求体缓冲、
    /// Elsa 活动列表接口拦截与翻译、首页模式 Cookie 写入以及 404 空响应体输出
    /// </summary>
    public class WtmMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// 构造中间件实例
        /// </summary>
        /// <param name="next">管道中的下一个中间件委托</param>
        public WtmMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 执行中间件逻辑：设置上传大小限制、拦截 Elsa 活动列表接口并按需翻译、
        /// 首页写入页面模式 Cookie、缓冲非表单请求体供后续重复读取，最后调用管道下一环节
        /// </summary>
        /// <param name="context">当前 HTTP 上下文</param>
        /// <param name="wtm">WTM 上下文</param>
        /// <returns>异步任务</returns>
        public async Task InvokeAsync(HttpContext context, WTMContext wtm)
        {
            // 按框架文件上传配置调整请求体大小上限
            var max = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
            if (max.IsReadOnly == false)
            {
                max.MaxRequestBodySize = wtm.ConfigInfo.FileUploadOptions.UploadLimit;
            }
            // 拦截 Elsa 工作流活动列表接口：改为内部调用并按当前文化对返回内容做中文翻译
            if (context.Request.Path == "/v1/activities" && context.Request.QueryString.Value != "?inneruse=1")
            {
                var txt = wtm.CallAPI("", $"{wtm.HostAddress}/v1/activities?inneruse=1").Result.Data;
                var cul = Thread.CurrentThread.CurrentCulture;
                if (cul.Name?.ToLower() == "zh" || cul.Name?.ToLower() == "zh-cn")
                {
                    await context.Response.WriteAsync(this.TranslateWorkflow(txt));
                }
                else
                {
                    await context.Response.WriteAsync(txt);
                }
                return;
            }
            // 访问首页时写入页面模式和标签页模式 Cookie，供前端读取
            if (context.Request.Path == "/")
            {
                context.Response.Cookies.Append("pagemode", wtm.ConfigInfo.PageMode.ToString());
                context.Response.Cookies.Append("tabmode", wtm.ConfigInfo.TabMode.ToString());
            }
            // 缓冲并缓存非表单请求的请求体，供后续过滤器和模型绑定重复读取
            if (context.Request.ContentLength > 0 && context.Request.HasFormContentType == false)
            {
                try
                {
                    context.Request.EnableBuffering();
                    context.Request.Body.Position = 0;
                    StreamReader tr = new StreamReader(context.Request.Body);
                    string body = tr.ReadToEndAsync().Result;
                    context.Request.Body.Position = 0;
                    if (context.Items.ContainsKey("DONOTUSE_REQUESTBODY") == false)
                    {
                        context.Items.Add("DONOTUSE_REQUESTBODY", body);
                    }
                    else
                    {
                        context.Items["DONOTUSE_REQUESTBODY"] = body;
                    }
                }
                catch {
                    context.Request.Body.Position = 0;
                }
            }
            await _next(context);
            // 404 时输出空响应体，避免默认 404 页面内容干扰前端处理
            if (context.Response.StatusCode == 404)
            {
                await context.Response.WriteAsync(string.Empty);
            }
        }

        /// <summary>
        /// 将 Elsa 活动列表 JSON 中的英文显示文本替换为中文（基于简单字符串替换实现）
        /// </summary>
        /// <param name="txt">Elsa 活动列表接口返回的 JSON 文本</param>
        /// <returns>替换后的文本，入参为空时返回空字符串</returns>
        private string TranslateWorkflow(string txt)
        {
            if(txt == null)
            {
                return "";
            }
            string rv = txt
                .Replace("\"displayName\":\"Read Line\"", "\"displayName\":\"Read\"")
                .Replace("\"description\":\"Read text from standard in.\"", "\"description\":\"Read text from standard in.\"")
                .Replace("\"category\":\"Console\"", "\"category\":\"Console\"")
                .Replace("\"displayName\":\"Write Line\"", "\"displayName\":\"Write\"")
                .Replace("\"description\":\"Write text to standard out.\"", "\"description\":\"Write text to standard out.\"")
                .Replace("\"label\":\"Text\"", "\"label\":\"Text\"")
                .Replace("\"hint\":\"The text to write.\"", "\"hint\":\"The text to write.\"")
                .Replace("\"label\":\"Script\"", "\"label\":\"Script\"")
                .Replace("\"hint\":\"The JavaScript to run.\"", "\"hint\":\"The JavaScript to run.\"")
                .Replace("\"label\":\"Possible Outcomes\"", "\"label\":\"Possible Outcomes\"")
                .Replace("\"hint\":\"The possible outcomes that can be set by the script.\"", "\"hint\":\"The possible outcomes that can be set by the script.\"")
                .Replace("\"displayName\":\"HTTP Endpoint\"", "\"displayName\":\"HTTP Endpoint\"")
                .Replace("\"description\":\"Handle an incoming HTTP request.\"", "\"description\":\"Handle an incoming HTTP request.\"");

            return rv;
        }
    }

    /// <summary>
    /// WtmMiddleware 的 IApplicationBuilder 扩展类
    /// </summary>
    public static class WtmMiddlewareExtensions
    {
        /// <summary>
        /// 将 WtmMiddleware 注册到请求处理管道
        /// </summary>
        /// <param name="builder">应用构建器</param>
        /// <returns>应用构建器</returns>
        public static IApplicationBuilder UseWtm(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WtmMiddleware>();
        }
    }
}
