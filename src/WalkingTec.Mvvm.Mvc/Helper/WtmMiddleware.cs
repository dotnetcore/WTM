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
            if (context.Request.Path == "/")
            {
                context.Response.Cookies.Append("pagemode", wtm.ConfigInfo.PageMode.ToString());
                context.Response.Cookies.Append("tabmode", wtm.ConfigInfo.TabMode.ToString());
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
            //if(wtm.ConfigInfo.Domains != null)
            //{
            //    var mainHost = wtm.ConfigInfo.Domains.Where(x=>x.Key== "mainhost").Select(x=>x.Value.Address).FirstOrDefault();
            //    if(string.IsNullOrEmpty(mainHost) == false)
            //    {
            //        if(context.Request.RouteValues["controller"]?.ToString()?.ToLower() == "login")
            //        {
            //            var test = await context.Request.RedirectCall(wtm, "mainhost");
            //        }
            //    }
            //}
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
