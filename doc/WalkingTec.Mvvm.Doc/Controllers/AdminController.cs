using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [AllowAnonymous]
    [ActionDescription("Admin")]
    public class AdminController : BaseController
    {
        [ActionDescription("Intro")]
        public IActionResult Intro()
        {
            return PartialView();
        }

        [ActionDescription("Log")]
        public new IActionResult Log()
        {
            return PartialView();
        }

        [ActionDescription("UserManagement")]
        public new IActionResult User()
        {
            return PartialView();
        }

        [ActionDescription("RoleManagement")]
        public IActionResult Role()
        {
            return PartialView();
        }

        [ActionDescription("GroupManagement")]
        public IActionResult Group()
        {
            return PartialView();
        }

        [ActionDescription("MenuManagement")]
        public IActionResult Menu()
        {
            return PartialView();
        }

        [ActionDescription("Dataprivilege")]
        public IActionResult Dp()
        {
            return PartialView();
        }
    }
}
