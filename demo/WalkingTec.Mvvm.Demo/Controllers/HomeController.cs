using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Demo.Controllers
{
    public class HomeController : BaseController
    {
        [AllRights]
        public IActionResult Index(string redirect)
        {
            ViewData["title"] = "WTM";
            return View();
        }


        [AllowAnonymous]
        public IActionResult PIndex()
        {
            return View();
        }

        [AllRights]
        [ActionDescription("FrontPage")]
        public IActionResult FrontPage()
        {

            return PartialView();
        }


        public IActionResult GetActionChart()
        {
            var areas = GlobaInfo.AllModule.Select(x => x.Area).Distinct();
            var data = new List<ChartData>();

            foreach (var area in areas)
            {
                var controllers = GlobaInfo.AllModule.Where(x => x.Area == area);
                data.Add(new ChartData
                {
                    Category = "Controllers",
                    Value = controllers.Count(),
                    Series = area?.AreaName ?? "Default"
                });
                data.Add(new ChartData
                {
                    Category = "Actions",
                    Value = controllers.SelectMany(x=>x.Actions).Count(),
                    Series = area?.AreaName ?? "Default"
                });
            }
            var rv = data.ToChartData();
            return Json(rv);
        }

        public IActionResult GetModelChart()
        {
            var models = new List<Type>();

            //获取所有模型
            var pros = Wtm.ConfigInfo.Connections.SelectMany(x => x.DcConstructor.DeclaringType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance));
            if (pros != null)
            {
                foreach (var pro in pros)
                {
                    if (pro.PropertyType.IsGeneric(typeof(DbSet<>)))
                    {
                        models.Add(pro.PropertyType.GetGenericArguments()[0]);
                    }
                }
            }
            var data = new List<ChartData>();

            foreach (var m in models)
            {
                data.Add(new ChartData
                {
                    Value = m.GetProperties().Count(),
                    Category = m.GetPropertyDisplayName(),
                    Series = "Model"
                }) ;
            }
            var rv = data.ToChartData();
            return Json(rv);
        }

        public IActionResult GetSampleChart()
        {
            var data = new List<ChartData>();
            Random r = new Random();
            int maxi = r.Next(3, 10);
            int maxy = r.Next(3, 10);
            for (int i = 0; i < maxi; i++)
            {
                for (int j = 0; j < maxy; j++)
                {
                    data.Add(new ChartData
                    {
                        Category = "x" + i,
                        Value = r.Next(100, 1000),
                        ValueX = r.Next(200, 2000),
                        Series = "y" + j,
                        Addition = r.Next(100, 1000),

                    });
                }
            }
            var rv = data.ToChartData();
            return Json(rv);
        }

        [AllRights]
        [ActionDescription("Layout")]
        public IActionResult Layout()
        {
            ViewData["debug"] = Wtm.ConfigInfo.IsQuickDebug;
            return PartialView();
        }

        [AllRights]
        public IActionResult UserInfo()
        {
            if (HttpContext.Request.Cookies.TryGetValue(CookieAuthenticationDefaults.CookiePrefix + AuthConstants.CookieAuthName, out string cookieValue))
            {
                var protectedData = Base64UrlTextEncoder.Decode(cookieValue);
                var dataProtectionProvider = HttpContext.RequestServices.GetRequiredService<IDataProtectionProvider>();
                var _dataProtector = dataProtectionProvider
                                        .CreateProtector(
                                            "Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware",
                                            CookieAuthenticationDefaults.AuthenticationScheme,
                                            "v2");
                var unprotectedData = _dataProtector.Unprotect(protectedData);

                string cookieData = Encoding.UTF8.GetString(unprotectedData);
                return JsonMore(cookieData);
            }
            else
                return JsonMore("No Data");
        }

        [AllowAnonymous]
        public github GetGithubInfo()
        {
            var rv = Wtm.ReadFromCache<github>("githubinfo", () =>
            {
                var s = Wtm.CallAPI<github>("github", "repos/dotnetcore/wtm", 60).Result;
                return s.Data;
            }, 1800);

            return rv;
        }

        public class github
        {
            public int stargazers_count { get; set; }
            public int forks_count { get; set; }
            public int subscribers_count { get; set; }
            public int open_issues_count { get; set; }
        }

    }


}
