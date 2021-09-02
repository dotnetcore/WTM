using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.BlazorDemo.ViewModel.BasicData.CityVMs;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.BlazorDemo.Controllers
{
    [Area("BasicData")]
    [AuthorizeJwt]
    [ActionDescription("图表管理")]
    [ApiController]
    [Route("api/Chart")]
    public partial class ChartController : BaseApiController
    {
        [HttpGet("GetCitys")]
        public ActionResult GetCitys()
        {
            return Ok(DC.Set<City>().GetSelectListItems(Wtm, x => x.Name));
        }

        [HttpGet("GetCharts")]
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
            return Ok(data);
        }
    }
}
