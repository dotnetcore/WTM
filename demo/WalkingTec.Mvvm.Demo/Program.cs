using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.TagHelpers.LayUI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WalkingTec.Mvvm.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return
                Host.CreateDefaultBuilder(args)
                 .ConfigureLogging((hostingContext, logging) =>
                 {
                     logging.ClearProviders();
                     logging.AddConsole();
                     logging.AddWTMLogger();
                 })
                .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder.UseStartup<Startup>();
                 });

        }

        public static string CSSelector(ActionExecutingContext context)
        {
            var userinfo = (context.Controller as IBaseController)?.WtmContext?.LoginUserInfo;
            if (userinfo == null)
            {
                return "default";
            }
            else
            {
                if (userinfo.ITCode.StartsWith("a"))
                {
                    return "default";
                }
                else
                {
                    return "default";
                }
            }
        }
    }

    public static class ConfigInfoExtension
    {
        public static string Key1(this Configs self)
        {
            return self.AppSettings["Key1"];
        }
    }
}
