using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [Public]
    [ActionDescription("框架配置")]
    public class GlobalController : BaseController
    {
        [FixConnection(DBOperationEnum.Read, CsName = "test")]
        [ActionDescription("配置文件")]
        public IActionResult Config()
        {
            var test = new FrameworkContext();
            return PartialView();
        }

        [ActionDescription("全局变量")]
        public IActionResult Global()
        {
            return PartialView();
        }

        [ActionDescription("数据库分库")]
        public IActionResult CS()
        {
            return PartialView();
        }

        [ActionDescription("数据权限")]
        public IActionResult DP()
        {
            return PartialView();
        }

        [ActionDescription("路由")]
        public IActionResult Route()
        {
            return PartialView();
        }


        [ActionDescription("发布")]
        public IActionResult Publish()
        {
            return PartialView();
        }
    }
}