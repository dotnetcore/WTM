using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.ApiDemo.ViewModels.FrameworkUserBaseVMs;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.ApiDemo.Controllers
{
    
    [ActionDescription("用户管理")]
    [ApiController]
    [Route("api/FrameworkUserBase")]
	[Public]
	public class FrameworkUserBaseController : BaseApiController
    {
        [ActionDescription("搜索")]
        [HttpPost("Search")]
		public string Search(FrameworkUserBaseSearcher searcher)
        {
            var vm = CreateVM<FrameworkUserBaseListVM>();
            vm.Searcher = searcher;
            return vm.GetJson();
        }

        [ActionDescription("获取")]
        [HttpGet("{id}")]
        public FrameworkUserBase Get(Guid id)
        {
            var vm = CreateVM<FrameworkUserBaseVM>(id);
            return vm.Entity;
        }

        [ActionDescription("新建")]
        [HttpPost("Add")]
        public IActionResult Add(FrameworkUserBaseVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                vm.DoAdd();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    return Ok(vm.Entity);
                }
            }

        }

        [ActionDescription("修改")]
        [HttpPut("Edit")]
        public IActionResult Edit(FrameworkUserBaseVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                vm.DoEdit(true);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    return Ok(vm.Entity);
                }
            }
        }

        [ActionDescription("删除")]
        [HttpGet("Delete/{id}")]
        public IActionResult Delete(Guid id)
        {
            var vm = CreateVM<FrameworkUserBaseVM>(id);
            vm.DoDelete();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                return Ok(vm.Entity);
            }

        }

		[HttpPost("BatchDelete")]
        [ActionDescription("批量删除")]
        public IActionResult BatchDelete(Guid[] ids)
        {
            var vm = CreateVM<FrameworkUserBaseBatchVM>();
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
                return BadRequest(ModelState);
            }
            else
            {
                return Ok(ids.Count());
            }
        }


        [ActionDescription("导出")]
        [HttpPost("ExportExcel")]
        public IActionResult ExportExcel(FrameworkUserBaseSearcher searcher)
        {
            var vm = CreateVM<FrameworkUserBaseListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            var data = vm.GenerateExcel();
            return File(data, "application/vnd.ms-excel", $"Export_FrameworkUserBase_{DateTime.Now.ToString("yyyy-MM-dd")}.xls");
        }

        [ActionDescription("勾选导出")]
        [HttpPost("ExportExcelByIds")]
        public IActionResult ExportExcelByIds(Guid[] ids)
        {
            var vm = CreateVM<FrameworkUserBaseListVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = new List<Guid>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            var data = vm.GenerateExcel();
            return File(data, "application/vnd.ms-excel", $"Export_FrameworkUserBase_{DateTime.Now.ToString("yyyy-MM-dd")}.xls");
        }

        [ActionDescription("下载导入模板")]
        [HttpGet("GetExcelTemplate")]
        public IActionResult GetExcelTemplate()
        {
            var vm = CreateVM<FrameworkUserBaseImportVM>();
            var qs = new Dictionary<string, string>();
            foreach (var item in Request.Query.Keys)
            {
                qs.Add(item, Request.Query[item]);
            }
            vm.SetParms(qs);
            var data = vm.GenerateTemplate(out string fileName);
            return File(data, "application/vnd.ms-excel", fileName);
        }

        [ActionDescription("导入")]
        [HttpPost("Import")]
        public ActionResult Import(FrameworkUserBaseImportVM vm)
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


        [HttpGet("GetFrameworkRoles")]
        public ActionResult GetFrameworkRoles()
        {
            return Ok(DC.Set<FrameworkRole>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, x => x.RoleName));
        }

        [HttpGet("GetFrameworkGroups")]
        public ActionResult GetFrameworkGroups()
        {
            return Ok(DC.Set<FrameworkGroup>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, x => x.GroupName));
        }

    }
}
