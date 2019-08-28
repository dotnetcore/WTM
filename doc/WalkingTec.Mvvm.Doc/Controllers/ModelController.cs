using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [Public]
    [ActionDescription("模型层")]
    public class ModelController : BaseController
    {
        [ActionDescription("创建模型")]
        public IActionResult Poco()
        {
            return PartialView();
        }

        [ActionDescription("模型属性")]
        public IActionResult Att()
        {
            return PartialView();
        }

        [ActionDescription("内置模型")]
        public IActionResult BuildIn()
        {
            return PartialView();
        }
    }
}
