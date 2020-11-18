using System;
using Microsoft.AspNetCore.Mvc;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.ViewModels.DataTableVMs;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Demo.Controllers
{
    [ActionDescription("使用存储过程")]
    public class DataTableController : BaseController
    {
        [ActionDescription("搜索")]
        public IActionResult Index()
        {
            var vm = Wtm.CreateVM<DatatableListVM>();
            return PartialView(vm);
        }

        [ActionDescription("搜索")]
        [HttpPost]
        public string Search(DatatableListVM vm)
        {
            return vm.GetJson(false);
        }
        [ActionDescription("Export")]
        [HttpPost]
        public IActionResult ExportExcel(DatatableListVM vm)
        {
            vm.SearcherMode = vm.Ids != null && vm.Ids.Count > 0 ? ListVMSearchModeEnum.CheckExport : ListVMSearchModeEnum.Export;
            var data = vm.GenerateExcel();
            return File(data, "application/vnd.ms-excel", $"Export_City_{DateTime.Now.ToString("yyyy-MM-dd")}.xls");
        }

    }
}
