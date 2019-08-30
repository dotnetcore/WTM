using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [Public]
    [ActionDescription("数据层")]
    public class DataController : BaseController
    {
        [ActionDescription("介绍")]
        public IActionResult Intro()
        {
            return PartialView();
        }

        [ActionDescription("数据库同步")]
        public IActionResult Migration()
        {
            return PartialView();
        }
    }
}
