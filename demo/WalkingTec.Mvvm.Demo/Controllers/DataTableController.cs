using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;
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

        [ActionDescription("报表查询", IsPage = true)]
        public IActionResult ChartsIndex()
        {
            var vm = Wtm.CreateVM<DatatableListVM>();

            var data = Wtm.DC.Set<Major>().AsNoTracking().Include(x => x.School)
                    .GroupBy(x => new { x.School.SchoolName, x.MajorType }, x => x.ID).Select(x => new ChartData
                    {
                        Series = x.Key.SchoolName,
                        Category = x.Key.MajorType.ToString(),
                        Value = x.Count(),
                        //yAvg= x.Average()
                    }).ToList();
            //data.g

            //var charts = new List<ChartData>();
            //for (int i = 0; i < 5; i++)
            //{
            //    for (int j = 0; j < 5; j++)
            //    {
            //        charts.Add(new ChartData
            //        {
            //            Category = "a" + i,
            //            Value = new Random().Next(100, 1000),
            //            Series = "bbb" + j
            //        });
            //    }
            //}

            vm.charts = data;
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
