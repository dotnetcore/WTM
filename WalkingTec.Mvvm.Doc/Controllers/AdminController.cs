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
    [ActionDescription("页面层")]
    public class AdminController : BaseController
    {
        [ActionDescription("介绍")]
        public IActionResult Intro()
        {
            return PartialView();
        }

        [ActionDescription("日志管理")]
        public IActionResult Log()
        {
            return PartialView();
        }

        [ActionDescription("用户管理")]
        public IActionResult User()
        {
            return PartialView();
        }
        [ActionDescription("角色管理")]
        public IActionResult Role()
        {
            return PartialView();
        }
        [ActionDescription("用户组管理")]
        public IActionResult Group()
        {
            return PartialView();
        }
        [ActionDescription("菜单管理")]
        public IActionResult Menu()
        {
            return PartialView();
        }
        [ActionDescription("数据权限管理")]
        public IActionResult Dp()
        {
            return PartialView();
        }
    }
}
