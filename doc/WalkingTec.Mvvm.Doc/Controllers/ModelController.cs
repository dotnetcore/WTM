using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [AllowAnonymous]
    [ActionDescription("ModelLayer")]
    public class ModelController : BaseController
    {
        [ActionDescription("CreateModel")]
        public IActionResult Poco()
        {
            return PartialView();
        }

        [ActionDescription("Att")]
        public IActionResult Att()
        {
            return PartialView();
        }

        [ActionDescription("BuildInModel")]
        public IActionResult BuildIn()
        {
            return PartialView();
        }

        [ActionDescription("CustomKey")]
        public IActionResult CustomKey()
        {
            return PartialView();
        }
    }
}
