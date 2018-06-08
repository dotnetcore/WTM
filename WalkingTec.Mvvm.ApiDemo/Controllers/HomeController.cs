using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.ApiDemo.ViewModels.HomeVMs;

namespace WalkingTec.Mvvm.ApiDemo.Controllers
{
    public class HomeController : BaseController
    {
        [AllRights]
        public IActionResult Index()
        {
            ViewData["title"] = "WalkingTec MVVM Framework";
            var vm = CreateVM<IndexVM>();
            vm.Menu = FFMenus.AsQueryable().GetTreeSelectListItems(null, null, x => x.PageName, null, null, x => x.Url, SortByName: false);
            return View(vm);
        }

        [Public]
        public IActionResult PIndex()
        {
            return View();
        }

        [AllRights]
        public IActionResult FrontPage()
        {
            return Content("首页");
        }

    }
}
