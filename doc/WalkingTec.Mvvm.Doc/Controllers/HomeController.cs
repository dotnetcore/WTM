using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            LoginUserInfo = new LoginUserInfo { ITCode = "admin" };
            ViewData["title"] = "WTM";
            return View();
        }

        public IActionResult PIndex()
        {
            return View();
        }

        public IActionResult FrontPage()
        {
            return Redirect("/QuickStart/Intro");
        }

        [ActionDescription("捐赠名单")]
        public IActionResult DonateList()
        {
            return PartialView();
        }

        [ActionDescription("Layout")]
        public IActionResult Layout()
        {
            return PartialView();
        }
    }
}
