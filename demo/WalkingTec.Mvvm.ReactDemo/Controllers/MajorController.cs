using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.ReactDemo.ViewModels.MajorVMs;
using WalkingTec.Mvvm.ReactDemo.Models;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.ReactDemo.Controllers
{
    
    [AuthorizeJwtWithCookie]
    [ActionDescription("专业管理")]
    [ApiController]
    [Route("api/Major")]
	public partial class MajorController : BaseApiController
    {
        [ActionDescription("Sys.Search")]
        [HttpPost("Search")]
		public async Task<IActionResult> Search(MajorSearcher searcher)
        {
            if (ModelState.IsValid)
            {
                var vm = Wtm.CreateVM<MajorListVM>();
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
        public MajorVM Get(string id)
        {
            var vm = Wtm.CreateVM<MajorVM>(id);
            return vm;
        }

        [ActionDescription("Sys.Create")]
        [HttpPost("Add")]
        public async Task<IActionResult> Add (MajorVM vm)
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
        public async Task<IActionResult> Edit (MajorVM vm)
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
        public async Task<IActionResult> BatchDelete (string[] ids)
        {
            var vm = Wtm.CreateVM<MajorBatchVM>();
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
        public async Task<IActionResult> ExportExcel (MajorSearcher searcher)
        {
            var vm = Wtm.CreateVM<MajorListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return await vm.GetExportData();
        }

        [ActionDescription("Sys.CheckExport")]
        [HttpPost("ExportExcelByIds")]
        public async Task<IActionResult> ExportExcelByIds (string[] ids)
        {
            var vm = Wtm.CreateVM<MajorListVM>();
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
            var vm = Wtm.CreateVM<MajorImportVM>();
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
        public async Task<IActionResult> Import (MajorImportVM vm)
        {

            if (vm.ErrorListVM.EntityList.Count > 0 || !await vm.BatchSaveData())
            {
                return BadRequest(vm.GetErrorJson());
            }
            else
            {
                return Ok(vm.EntityList.Count);
            }
        }


        [HttpGet("GetSchools")]
        public async Task<IActionResult> GetSchools ()
        {
            return Ok(await DC.Set<School>().GetSelectListItems(Wtm, x => x.SchoolName));
        }

        [HttpGet("GetStudents")]
        public async Task<IActionResult> GetStudents ()
        {
            return Ok(await DC.Set<Student>().GetSelectListItems(Wtm, x => x.Name));
        }

    }
}
