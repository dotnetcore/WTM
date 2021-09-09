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

        [ActionDescription("搜索", IsPage = true)]
        public IActionResult ChartsIndex()
        {
            var vm = Wtm.CreateVM<ChartVM>();
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
            return Json(rv);
        }

        public IActionResult GetCharts(ChartVM vm)
        {
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
            DateTime dateTime = new DateTime(1900, 1, 1);

            //.Select(x => new
            // {
            //     abc =
            //             x.CreateTime.Value.Year.ToString() + ","
            //             +
            //             (((SqlServerDbFunctionsExtensions.DateDiffMonth(EF.Functions, dateTime, x.CreateTime.Value) % 12 + 1) - 1) / 3 + 1).ToString(),
            //     x.MajorName,
            //     x.CreateTime
            // })

            //var data = Wtm.DC.Set<Major>().AsNoTracking().Include(x => x.School)
            //        .Select(x => new
            //        {
            //            abc =
            //            //x.CreateTime.Value.Year * 100 + ((x.CreateTime.Value.Month - 1) / 3 + 1),
            //            x.CreateTime.Value.DayOfYear / 7 + 1,
            //            x.MajorName,
            //            x.CreateTime
            //        })
            //        .GroupBy(x => new
            //        {
            //            x.abc,
            //            x.MajorName
            //        }).Select(x => new ChartData
            //        //.GroupBy(x => new { x.School.SchoolName, x.MajorType }, x => x.ID).Select(x => new ChartData
            //        {
            //            Series = x.Key.MajorName,
            //            //Category = x.Min(y => y.CreateTime).Value.ToString("yyyy.") + ((x.Min(y => y.CreateTime).Value.Month - 1) / 3 + 1),
            //            Category = x.Min(y => y.CreateTime).Value.ToString("yyyy.") + ((x.Min(y => y.CreateTime).Value.DayOfYear) / 7 + 1),
            //            //Category = x.Key.abc,
            //            Value = x.Count(),

            //            //yAvg= x.Average()
            //        }).ToList();

            //int group = int.Parse((max - min) / 2);
            var data = Wtm.DC.Set<ActionLog>().AsNoTracking()
                .CheckContain(vm.Searcher.ITCode, x => x.ITCode)
                .CheckContain(vm.Searcher.ActionUrl, x => x.ActionUrl)
                .CheckContain(vm.Searcher.LogType, x => x.LogType)
                .CheckContain(vm.Searcher.IP, x => x.IP)
                .CheckBetween(vm.Searcher.ActionTime?.GetStartTime(), vm.Searcher.ActionTime?.GetEndTime(), x => x.ActionTime, includeMax: false)
                .CheckWhere(vm.Searcher.Duration, x => x.Duration >= vm.Searcher.Duration)
                      .Select(x => new
                      {
                          abc =
                         //x.CreateTime.Value.Year * 100 + ((x.CreateTime.Value.Month - 1) / 3 + 1),
                         Convert.ToInt32(x.Duration / 2) + 1,
                          //x.CreateTime.Value.Year * 1000 + x.CreateTime.Value.DayOfYear,
                          ef = x.LogType,
                          x.Duration,
                      })
                      .GroupBy(x => new
                      {
                          x.abc,
                          x.ef
                      }, x => new { x.Duration }).OrderBy(x => x.Key.abc).Select(x => new ChartData
                      //.GroupBy(x => new { x.School.SchoolName, x.MajorType }, x => x.ID).Select(x => new ChartData
                      {
                          Series = x.Key.ef.ToString(),
                          //Category = x.Min(y => y.CreateTime).Value.ToString("yyyy.") + ((x.Min(y => y.CreateTime).Value.Month - 1) / 3 + 1),
                          Category = ((x.Key.abc - 1) * 2).ToString() + "-" + (x.Key.abc * 2).ToString(),
                          //Category = x.Key.abc,
                          Value = x.Count(),

                          //yAvg= x.Average()
                      }).ToList();

            //var data1 = Wtm.DC.Set<Student>()
            //        .Join(Wtm.DC.Set<StudentMajor>(), a => a.ID, b => b.StudentId, (a, b) => new { a.Name, a.CreateTime, b.MajorId })
            //        .Join(Wtm.DC.Set<Major>(), a => a.MajorId, b => b.ID, (m, n) => new { m.Name, m.CreateTime, n.MajorName })
            //      .Select(x => new
            //      {
            //          abc =
            //         x.CreateTime.Value.Year * 100 + ((x.CreateTime.Value.Month - 1) / 3 + 1),
            //          Convert.ToInt32(x.Duration / 2) + 1,
            //          x.Name,
            //          x.MajorName,
            //      }).ToList();


            //var data = data1
            //       .GroupBy(x => new
            //       {
            //           x.abc,
            //           x.MajorName
            //       }).OrderBy(x => x.Key.abc).Select(x => new ChartData
            //      .GroupBy(x => new { x.School.SchoolName, x.MajorType }, x => x.ID).Select(x => new ChartData
            //      {
            //          Series = x.Key.MajorName.ToString(),
            //          Category = x.Min(y => y.CreateTime).Value.ToString("yyyy.") + ((x.Min(y => y.CreateTime).Value.Month - 1) / 3 + 1),
            //          Category = x.Key.abc,
            //          Category = x.Key.abc,
            //          Value = x.Count(),

            //          yAvg = x.Average()
            //      }).ToList();


            var rv = data.ToChartData();
            return Json(rv);
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
