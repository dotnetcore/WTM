using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.BlazorDemo.ViewModel.VirusData.VirusVMs;
using WalkingTec.Mvvm.Demo.Models.Virus;


namespace WalkingTec.Mvvm.BlazorDemo.Controllers
{
    [Area("VirusData")]
    [AuthorizeJwtWithCookie]
    [ActionDescription("新病毒管理")]
    [ApiController]
    [Route("api/Virus")]
	public partial class VirusController : BaseApiController
    {
        [ActionDescription("Sys.Search")]
        [HttpPost("Search")]
		public async Task<IActionResult> Search(VirusSearcher searcher)
        {
            if (ModelState.IsValid)
            {
                var vm = Wtm.CreateVM<VirusListVM>();
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
        public VirusVM Get(string id)
        {
            var vm = Wtm.CreateVM<VirusVM>(id);
            return vm;
        }

        [ActionDescription("Sys.Create")]
        [HttpPost("Add")]
        public async Task<IActionResult> Add(VirusVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                await vm.DoAdd();
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

        [ActionDescription("Sys.Edit")]
        [HttpPut("Edit")]
        public async Task<IActionResult> Edit(VirusVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                await vm.DoEdit(false);
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
        public async Task<IActionResult> BatchDelete(string[] ids)
        {
            var vm = Wtm.CreateVM<VirusBatchVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = ids;
            }
            else
            {
                return Ok();
            }
            if (!ModelState.IsValid || !await vm.DoBatchDelete())
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
        public async Task<IActionResult> ExportExcel(VirusSearcher searcher)
        {
            var vm = Wtm.CreateVM<VirusListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return await vm.GetExportData();
        }

        [ActionDescription("Sys.CheckExport")]
        [HttpPost("ExportExcelByIds")]
        public async Task<IActionResult> ExportExcelByIds(string[] ids)
        {
            var vm = Wtm.CreateVM<VirusListVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            return await vm.GetExportData();
        }

        [ActionDescription("Sys.DownloadTemplate")]
        [HttpGet("GetExcelTemplate")]
        public IActionResult GetExcelTemplate()
        {
            var vm = Wtm.CreateVM<VirusImportVM>();
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
        public async Task<IActionResult> Import(VirusImportVM vm)
        {
            if (vm!=null && (vm.ErrorListVM.EntityList.Count > 0 || !await vm.BatchSaveData()))
            {
                return BadRequest(vm.GetErrorJson());
            }
            else
            {
                return Ok(vm?.EntityList?.Count ?? 0);
            }
        }


        [HttpGet("GetPatients")]
        public ActionResult GetPatients()
        {
            return Ok(await DC.Set<Patient>().GetSelectListItems(Wtm, x => x.PatientName));
        }

    }
}
