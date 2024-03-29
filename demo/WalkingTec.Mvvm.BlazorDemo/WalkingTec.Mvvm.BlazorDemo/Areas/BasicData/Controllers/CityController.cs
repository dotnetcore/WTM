using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.BlazorDemo.ViewModel.BasicData.CityVMs;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.BlazorDemo.ViewModel.BasicData;

namespace WalkingTec.Mvvm.BlazorDemo.Controllers
{
    [Area("BasicData")]
    [AuthorizeJwt]
    [ActionDescription("城市管理")]
    [ApiController]
    [Route("api/City")]
	public partial class CityController : BaseApiController
    {
        [ActionDescription("Sys.Search")]
        [HttpPost("Search")]
		public IActionResult Search(CitySearcher searcher)
        {
            if (ModelState.IsValid)
            {
                var vm = Wtm.CreateVM<CityListVM>();
                vm.Searcher = searcher;
                return Content(vm.GetJson(enumToString: false));
            }
            else
            {
                return BadRequest(ModelState.GetErrorJson());
            }
        }

        [ActionDescription("Sys.Get")]
        [HttpGet("{id}")]
        public CityVM Get(string id)
        {
            var vm = Wtm.CreateVM<CityVM>(id);
            return vm;
        }

        [ActionDescription("Sys.Get")]
        [HttpGet("GetGroup/{id}")]
        public GroupVMTest GetGroup(string id)
        {
            var vm = Wtm.CreateVM<GroupVMTest>(values:x=>x.EntityId == id);
            return vm;
        }


        [ActionDescription("Sys.Create")]
        [HttpPost("Add")]
        public IActionResult Add(CityVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                vm.DoAdd();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.GetErrorJson());
                }
                else
                {
                    return Ok(vm.Entity);
                }
            }
        }

        [ActionDescription("Sys.Create")]
        [HttpPost("addgroupvm")]
        public IActionResult AddGroupVM(GroupVMTest vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                vm.vm1.DoAdd();
                vm.vm2.DoAdd();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.GetErrorJson());
                }
                else
                {
                    return Ok(vm);
                }
            }
        }

        [ActionDescription("Sys.Create")]
        [HttpPut("editgroupvm")]
        public IActionResult EditGroupVM(GroupVMTest vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                vm.vm1.DoEdit();
                vm.vm2.DoEdit();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.GetErrorJson());
                }
                else
                {
                    return Ok(vm);
                }
            }
        }

        [ActionDescription("Sys.Edit")]
        [HttpPut("Edit")]
        public IActionResult Edit(CityVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                vm.DoEdit(false);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.GetErrorJson());
                }
                else
                {
                    return Ok(vm.Entity);
                }
            }
        }

		[HttpPost("BatchDelete")]
        [ActionDescription("Sys.Delete")]
        public IActionResult BatchDelete(string[] ids)
        {
            var vm = Wtm.CreateVM<CityBatchVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = ids;
            }
            else
            {
                return Ok();
            }
            if (!ModelState.IsValid || !vm.DoBatchDelete())
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                return Ok(ids.Count());
            }
        }


        [ActionDescription("Sys.Export")]
        [HttpPost("ExportExcel")]
        public IActionResult ExportExcel(CitySearcher searcher)
        {
            var vm = Wtm.CreateVM<CityListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return vm.GetExportData();
        }

        [ActionDescription("Sys.CheckExport")]
        [HttpPost("ExportExcelByIds")]
        public IActionResult ExportExcelByIds(string[] ids)
        {
            var vm = Wtm.CreateVM<CityListVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            return vm.GetExportData();
        }

        [ActionDescription("Sys.DownloadTemplate")]
        [HttpGet("GetExcelTemplate")]
        public IActionResult GetExcelTemplate()
        {
            var vm = Wtm.CreateVM<CityImportVM>();
            var qs = new Dictionary<string, string>();
            foreach (var item in Request.Query.Keys)
            {
                qs.Add(item, Request.Query[item]);
            }
            vm.SetParms(qs);
            var data = vm.GenerateTemplate(out string fileName);
            return File(data, "application/vnd.ms-excel", fileName);
        }

        [ActionDescription("Sys.Import")]
        [HttpPost("Import")]
        public ActionResult Import(CityImportVM vm)
        {

            if (vm.ErrorListVM.EntityList.Count > 0 || !vm.BatchSaveData())
            {
                return BadRequest(vm.GetErrorJson());
            }
            else
            {
                return Ok(vm.EntityList.Count);
            }
        }


        [HttpGet("GetCitys")]
        public ActionResult GetCitys()
        {
            return Ok(DC.Set<City>().GetSelectListItems(Wtm, x => x.Name));
        }

    }
}
