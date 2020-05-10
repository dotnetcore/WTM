using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [AllowAnonymous]
    [ActionDescription("Controller")]
    public class ControlController : BaseController
    {
        [ActionDescription("Intro")]
        public IActionResult Intro()
        {
            return PartialView();
        }

        [ActionDescription("Att")]
        public IActionResult Att()
        {
            return PartialView();
        }

    }
}
