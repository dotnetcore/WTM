using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [AllowAnonymous]
    [ActionDescription("Data")]
    public class DataController : BaseController
    {
        [ActionDescription("Intro")]
        public IActionResult Intro()
        {
            return PartialView();
        }

        [ActionDescription("Migration")]
        public IActionResult Migration()
        {
            return PartialView();
        }
    }
}
