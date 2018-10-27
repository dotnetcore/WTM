using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.TagHelpers.LayUI;

namespace WalkingTec.Mvvm.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>

            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices((hostingCtx, x) =>
                {
                    var pris = new List<IDataPrivilege>
                    {
                        new DataPrivilegeInfo<School>("学校", y => y.SchoolName),
                        new DataPrivilegeInfo<Major>("专业", y => y.MajorName)
                    };
                    x.AddFrameworkService(dataPrivilegeSettings: pris);
                    x.AddLayui();
                })
                .Configure(x =>
                {
                    x.UseFrameworkService();
                });

    }
}
