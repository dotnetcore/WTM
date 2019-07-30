using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [Public]
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            LoginUserInfo = new LoginUserInfo { ITCode = "admin" };
            ViewData["title"] = "WalkingTec MVVM Framework";
            ViewData["menu"] = FFMenus?.AsQueryable().GetTreeSelectListItems(null, null, x => x.PageName, null,null, x => x.Url, SortByName: false);
            return View();
        }

        [Public]
        public IActionResult PIndex()
        {
            return View();
        }

        [AllRights]
        public IActionResult FrontPage()
        {
            return Redirect("/QuickStart/Intro");
        }

        [Public]
        [ActionDescription("捐赠名单")]
        public IActionResult DonateList()
        {
            return PartialView();
        }

    }
}
