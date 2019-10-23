using Microsoft.AspNetCore.Mvc;
using System;
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
        public string Search(ActionLogListVM vm)
        {
            return vm.GetJson(false);
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
            vm.SearcherMode = vm.Ids != null && vm.Ids.Count > 0 ? ListVMSearchModeEnum.CheckExport : ListVMSearchModeEnum.Export;
            var data = vm.GenerateExcel();
            return File(data, "application/vnd.ms-excel", $"Export_ActionLog_{DateTime.Now.ToString("yyyy-MM-dd")}.xls");
        }

    }
}
