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
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>            
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(x =>
                {
                    List<IDataPrivilege> pris = new List<IDataPrivilege>();
                    pris.Add(new DataPrivilegeInfo<School>("学校", y => y.SchoolName));
                    x.AddFrameworkService(dataPrivilegeSettings: pris);
                    //x.AddFrameworkService();
                    x.AddLayui();
                })
                .Configure(x =>
                {
                    x.UseFrameworkService();
                })
                .Build();

    }
}
