using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    public class WtmMiddleware
    {
        private readonly RequestDelegate _next;

        public WtmMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IOptionsMonitor<Configs> configs)
        {
            var max = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
            if (max.IsReadOnly == false)
            {
                max.MaxRequestBodySize = configs.CurrentValue.FileUploadOptions.UploadLimit;
            }
            if (context.Request.Path == "/")
            {
                context.Response.Cookies.Append("pagemode", configs.CurrentValue.PageMode.ToString());
                context.Response.Cookies.Append("tabmode", configs.CurrentValue.TabMode.ToString());
            }
            if (context.Request.ContentLength > 0 && context.Request.ContentLength < 512000)
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
            await _next(context);
            if (context.Response.StatusCode == 404)
            {
                await context.Response.WriteAsync(string.Empty);
            }
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
