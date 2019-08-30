using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [Public]
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            LoginUserInfo = new LoginUserInfo { ITCode = "admin" };
            ViewData["title"] = "WTM";
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

        [Public]
        [ActionDescription("Layout")]
        public IActionResult Layout()
        {
            return PartialView();
        }
    }
}
