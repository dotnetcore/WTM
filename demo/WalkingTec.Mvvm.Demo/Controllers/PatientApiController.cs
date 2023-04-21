using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Demo.ViewModels.PatientVMs;
using WalkingTec.Mvvm.Demo.Models.Virus;
using WalkingTec.Mvvm.Demo.Models;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Demo.Controllers
{
    
    [AuthorizeJwtWithCookie]
    [ActionDescription("病例管理")]
    [ApiController]
    [Route("api/Patient")]
	public partial class PatientApiController : BaseApiController
    {
        [ActionDescription("Sys.Search")]
        [HttpPost("Search")]
		public async Task<IActionResult> Search(PatientApiSearcher searcher)
        {
            if (ModelState.IsValid)
            {
                var vm = Wtm.CreateVM<PatientApiListVM>(passInit: true);
                vm.Searcher = searcher;
                return Content(vm.GetJson());
            }
            else
            {
                return BadRequest(ModelState.GetErrorJson());
            }
        }

        [ActionDescription("Sys.Get")]
        [HttpGet("{id}")]
        public PatientApiVM Get(string id)
        {
            var vm = Wtm.CreateVM<PatientApiVM>(id);
            return vm;
        }

        [ActionDescription("Sys.Create")]
        [HttpPost("Add")]
        public async Task<IActionResult> Add(PatientApiVM vm)
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
        public async Task<IActionResult> Edit(PatientApiVM vm)
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
            var vm = Wtm.CreateVM<PatientApiBatchVM>();
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
        public async Task<IActionResult> ExportExcel(PatientApiSearcher searcher)
        {
            var vm = Wtm.CreateVM<PatientApiListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return await vm.GetExportData();
        }

        [ActionDescription("Sys.CheckExport")]
        [HttpPost("ExportExcelByIds")]
        public async Task<IActionResult> ExportExcelByIds(string[] ids)
        {
            var vm = Wtm.CreateVM<PatientApiListVM>();
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
            var vm = Wtm.CreateVM<PatientApiImportVM>();
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
        public async Task<IActionResult> Import(PatientApiImportVM vm)
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


        [HttpGet("GetCitys")]
        public async Task<IActionResult> GetCitys()
        {
            return Ok(await DC.Set<City>().GetSelectListItems(Wtm, x => x.Name));
        }

        [HttpGet("GetHospitals")]
        public async Task<IActionResult> GetHospitals()
        {
            return Ok(await DC.Set<Hospital>().GetSelectListItems(Wtm, x => x.Name));
        }

        [HttpGet("GetViruss")]
        public async Task<IActionResult> GetViruss()
        {
            return Ok(await DC.Set<Virus>().GetSelectListItems(Wtm, x => x.VirtusName));
        }

    }
}
