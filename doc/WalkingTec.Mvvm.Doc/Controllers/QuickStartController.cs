using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [ActionDescription("快速开始")]
    [Public]
    public class QuickStartController : BaseController
    {
        [ActionDescription("介绍")]
        public IActionResult Intro()
        {
            return PartialView();
        }


        [ActionDescription("第一个项目")]
        public IActionResult FirstProject()
        {
            return PartialView();
        }

        [ActionDescription("第一个模块")]
        public IActionResult FirstModule()
        {
            return PartialView();
        }

        [ActionDescription("前后端分离项目")]
        public IActionResult React()
        {
            return PartialView();
        }

        [ActionDescription("FAQ")]
        public IActionResult FAQ()
        {
            return PartialView();
        }

        [ActionDescription("自定义用户")]
        public IActionResult CustomUser()
        {
            return PartialView();
        }
    }
}
