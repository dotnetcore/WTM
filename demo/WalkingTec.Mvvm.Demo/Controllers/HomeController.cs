using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.ViewModels.HomeVMs;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Demo.Controllers
{
    public class HomeController : BaseController
    {
        [AllRights]
        public IActionResult Index()
        {
            ViewData["title"] = "WTM";
            var vm = CreateVM<IndexVM>();
            vm.AllMenu = FFMenus;
            return View(vm);
        }

        [Public]
        public IActionResult PIndex()
        {
            return View();
        }

        [AllRights]
        [ActionDescription("首页")]
        public IActionResult FrontPage()
        {
            var areas = GlobaInfo.AllModule.Select(x => x.Area).Distinct();
            var legend = new List<string>();
            var series = new List<object>();
            foreach (var area in areas)
            {
                var legendName = area?.AreaName ?? "默认";
                var controllers = GlobaInfo.AllModule.Where(x => x.Area == area);
                legend.Add(legendName);
                series.Add(new
                {
                    name = legendName,
                    type = "bar",
                    data = new int[] {
                        controllers.Count(),
                        controllers.SelectMany(x => x.Actions).Count()
                    },
                });
            }

            var otherLegend = new List<string>() { "legendName" };
            var otherSeries = new List<object>()
            {
                new {
                    name = "legendName",
                    type = "bar",
                    data = new int[] {
                        GlobaInfo.AllModels.Count(),
                        GlobaInfo.AllAssembly.Count(),
                        ConfigInfo.DataPrivilegeSettings.Count(),
                        ConfigInfo.ConnectionStrings.Count(),
                        ConfigInfo.AppSettings.Count()
                    },
                }
            };

            ViewData["controller.legend"] = legend;
            ViewData["controller.series"] = series;
            ViewData["other.legend"] = otherLegend;
            ViewData["other.series"] = otherSeries;

            return PartialView();
        }

        [AllRights]
        [ActionDescription("Layout")]
        public IActionResult Layout()
        {
            ViewData["debug"] = ConfigInfo.IsQuickDebug;
            return PartialView();
        }

    }
}
