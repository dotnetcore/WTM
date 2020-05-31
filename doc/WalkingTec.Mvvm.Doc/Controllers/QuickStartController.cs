using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [AllowAnonymous]
    [ActionDescription("QuickStart")]
    public class QuickStartController : BaseController
    {
        [ActionDescription("介绍")]
        public IActionResult Intro()
        {
            return PartialView();
        }


        [ActionDescription("FirstProject")]
        public IActionResult FirstProject()
        {
            return PartialView();
        }

        [ActionDescription("FirstModule")]
        public IActionResult FirstModule()
        {
            return PartialView();
        }

        [ActionDescription("前后端分离项目")]
        public IActionResult React()
        {
            return PartialView();
        }

        [ActionDescription("FAQ")]
        public IActionResult FAQ()
        {
            return PartialView();
        }

        [ActionDescription("CustomUser")]
        public IActionResult CustomUser()
        {
            return PartialView();
        }
    }
}
