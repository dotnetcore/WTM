using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [AllowAnonymous]
    [ActionDescription("FrameworkConfig")]
    public class GlobalController : BaseController
    {
        [FixConnection(DBOperationEnum.Read, CsName = "test")]
        [ActionDescription("ConfigFile")]
        public IActionResult Config()
        {
            var test = new FrameworkContext();
            return PartialView();
        }

        [ActionDescription("Global")]
        public IActionResult Global()
        {
            return PartialView();
        }

        [ActionDescription("MultiCS")]
        public IActionResult CS()
        {
            return PartialView();
        }

        [ActionDescription("Dataprivilege")]
        public IActionResult DP()
        {
            return PartialView();
        }

        [ActionDescription("Route")]
        public IActionResult Route()
        {
            return PartialView();
        }


        [ActionDescription("Publish")]
        public IActionResult Publish()
        {
            return PartialView();
        }

        [ActionDescription("MultiLanguages")]
        public IActionResult MultiLanguages()
        {
            return PartialView();
        }

        [ActionDescription("Jwt")]
        public IActionResult CookieAuthAndJwtAuth()
        {
            return PartialView("CookieAuth&JwtAuth");
        }
    }
}
