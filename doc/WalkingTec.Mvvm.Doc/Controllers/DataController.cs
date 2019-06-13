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
