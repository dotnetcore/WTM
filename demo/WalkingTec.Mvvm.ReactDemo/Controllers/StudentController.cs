using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.ReactDemo.ViewModels.StudentVMs;
using WalkingTec.Mvvm.ReactDemo.Models;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.ReactDemo.Controllers
{
    
    [AuthorizeJwtWithCookie]
    [ActionDescription("学生管理")]
    [ApiController]
    [Route("api/Student")]
	public partial class StudentController : BaseApiController
    {
        [ActionDescription("Sys.Search")]
        [HttpPost("Search")]
		public async Task<IActionResult> Search(StudentSearcher searcher)
        {
            if (ModelState.IsValid)
            {
                var vm = Wtm.CreateVM<StudentListVM>();
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
        public StudentVM Get(string id)
        {
            var vm = Wtm.CreateVM<StudentVM>(id);
            return vm;
        }

        [ActionDescription("Sys.Create")]
        [HttpPost("Add")]
        public async Task<IActionResult> Add (StudentVM vm)
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
        public async Task<IActionResult> Edit (StudentVM vm)
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
            var vm = Wtm.CreateVM<StudentBatchVM>();
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
        public async Task<IActionResult> ExportExcel (StudentSearcher searcher)
        {
            var vm = Wtm.CreateVM<StudentListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return await vm.GetExportData();
        }

        [ActionDescription("Sys.CheckExport")]
        [HttpPost("ExportExcelByIds")]
        public async Task<IActionResult> ExportExcelByIds (string[] ids)
        {
            var vm = Wtm.CreateVM<StudentListVM>();
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
            var vm = Wtm.CreateVM<StudentImportVM>();
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
        public async Task<IActionResult> Import (StudentImportVM vm)
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


        [HttpGet("GetMajors")]
        public async Task<IActionResult> GetMajors ()
        {
            return Ok(await DC.Set<Major>().GetSelectListItems(Wtm, x => x.MajorName));
        }

        [HttpGet("GetCitys")]
        public async Task<IActionResult> GetCitys ()
        {
            return Ok(await DC.Set<City>().GetSelectListItems(Wtm, x => x.Name));
        }

        [HttpGet("GetSchoolsByCity/{id}")]
        public async Task<IActionResult> GetSchoolsByCity (Guid id)
        {
            return Ok(await DC.Set<School>().Where(x=>x.PlaceId == id).GetSelectListItems(Wtm, x => x.SchoolName));
        }

        [HttpGet("GetMajorsBySchool/{id}")]
        public async Task<IActionResult> GetMajorsBySchool (int id)
        {
            return Ok(await DC.Set<Major>().Where(x => x.SchoolId == id).GetSelectListItems(Wtm, x => x.MajorName));
        }

    }
}
