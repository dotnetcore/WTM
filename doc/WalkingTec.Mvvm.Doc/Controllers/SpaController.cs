using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [AllowAnonymous]
    [ActionDescription("Clientside")]
    public class SpaController : BaseController
    {
        [ActionDescription("介绍")]
        public IActionResult Intro()
        {
            return PartialView();
        }

        [ActionDescription("Global")]
        public IActionResult Global()
        {
            return PartialView();
        }

        [ActionDescription("Dir")]
        public IActionResult Dir()
        {
            return PartialView();
        }
    }
}
