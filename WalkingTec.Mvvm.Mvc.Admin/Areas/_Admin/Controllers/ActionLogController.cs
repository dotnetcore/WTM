using Microsoft.AspNetCore.Mvc;
using System;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.ActionLogVMs;

namespace WalkingTec.Mvvm.Mvc.Admin.Controllers
{
    [Area("_Admin")]
    [ActionDescription("日志")]
    [FixConnection(CsName = "defaultlog")]
    public class ActionLogController : BaseController
    {
        [ActionDescription("搜索")]
        public IActionResult Index()
        {
            var vm = CreateVM<ActionLogListVM>();
            return PartialView(vm);
        }

        [HttpGet]
        [ActionDescription("详细")]
        public IActionResult Details(Guid id)
        {
            var vm = CreateVM<ActionLogVM>(id);
            return PartialView(vm);
        }

    }
}
