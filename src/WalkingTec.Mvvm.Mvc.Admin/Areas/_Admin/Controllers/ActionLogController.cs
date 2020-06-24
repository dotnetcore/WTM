using System;

using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.ActionLogVMs;

namespace WalkingTec.Mvvm.Mvc.Admin.Controllers
{
    [Area("_Admin")]
    [ActionDescription("Log")]
    [FixConnection(CsName = "defaultlog")]
    public class ActionLogController : BaseController
    {
        [ActionDescription("Search")]
        public IActionResult Index()
        {
            var vm = CreateVM<ActionLogListVM>();
            return PartialView(vm);
        }

        [ActionDescription("Search")]
        [HttpPost]
        public string Search(ActionLogSearcher searcher)
        {
            var vm = CreateVM<ActionLogListVM>(passInit:true);
            if (ModelState.IsValid)
            {
                vm.Searcher = searcher;
                return vm.GetJson(false);
            }
            else
            {
                return vm.GetError();
            }
        }


        [HttpGet]
        [ActionDescription("Details")]
        public IActionResult Details(string id)
        {
            var vm = CreateVM<ActionLogVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("Export")]
        [HttpPost]
        public IActionResult ExportExcel(ActionLogListVM vm)
        {
            return vm.GetExportData();
        }

    }
}
