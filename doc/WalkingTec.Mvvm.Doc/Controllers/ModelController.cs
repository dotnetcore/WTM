using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
