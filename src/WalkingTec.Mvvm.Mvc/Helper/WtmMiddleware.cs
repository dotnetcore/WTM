using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc
{
    public class WtmMiddleware
    {
        private readonly RequestDelegate _next;

        public WtmMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, WTMContext wtm)
        {
            var max = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
            if (max.IsReadOnly == false)
            {
                max.MaxRequestBodySize = wtm.ConfigInfo.FileUploadOptions.UploadLimit;
            }
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
            if (context.Request.Path == "/")
            {
                context.Response.Cookies.Append("pagemode", wtm.ConfigInfo.PageMode.ToString());
                context.Response.Cookies.Append("tabmode", wtm.ConfigInfo.TabMode.ToString());
            }
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
            if (context.Response.StatusCode == 404)
            {
                await context.Response.WriteAsync(string.Empty);
            }
        }

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

    public static class WtmMiddlewareExtensions
    {
        public static IApplicationBuilder UseWtm(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WtmMiddleware>();
        }
    }
}
