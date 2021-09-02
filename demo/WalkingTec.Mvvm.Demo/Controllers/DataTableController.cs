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

        [ActionDescription("图表实例", IsPage = true)]
        public IActionResult ChartsIndex()
        {
            var vm = Wtm.CreateVM<DatatableListVM>();

            //var data = Wtm.DC.Set<Major>().AsNoTracking().Include(x => x.School)
            //        .GroupBy(x => new { x.School.SchoolName, x.MajorType }, x => x.ID).Select(x => new ChartData
            //        {
            //            Series = x.Key.SchoolName,
            //            Category = x.Key.MajorType.ToString(),
            //            Value = x.Count(),
            //            //yAvg= x.Average()
            //        }).ToList();
            //data.g

            //var data = new List<ChartData>();
            //for (int i = 0; i < 5; i++)
            //{
            //    for (int j = 0; j < 5; j++)
            //    {
            //        data.Add(new ChartData
            //        {
            //            Category = "a" + i,
            //            Value = new Random().Next(100, 1000),
            //            Series = "bbb" + j,
            //            Addition = new Random().Next(100, 1000),
            //        });
            //    }
            //}

            //vm.charts = data;
            return PartialView(vm);
        }

        public IActionResult GetChartsFour()
        {
            var data = new List<ChartData>();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    data.Add(new ChartData
                    {
                        Category = "a" + i,
                        Value = new Random().Next(100, 1000),
                        ValueX = new Random().Next(200, 2000),
                        Series = "bbb" + j,
                        Addition = new Random().Next(100, 1000),

                    });
                }
            }
            var rv = data.ToChartData();
            return Json(new { dataset = rv.Item1, series = rv.Item2, legend = rv.Item3 });
        }

        public IActionResult GetCharts()
        {
            var data = new List<ChartData>();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    data.Add(new ChartData
                    {
                        Category = "a" + i,
                        Value = new Random().Next(100, 1000),
                        Series = "bbb" + j,
                        Addition = new Random().Next(100, 1000),

                    });
                }
            }
            var rv = data.ToChartData();
            return Json(new { dataset = rv.Item1, series = rv.Item2, legend = rv.Item3 });
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
