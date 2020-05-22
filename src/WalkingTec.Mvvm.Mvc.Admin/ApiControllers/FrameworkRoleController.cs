using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth.Attribute;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ActionDescription("RoleManagement")]
    [ApiController]
    [Route("api/_[controller]")]
    public class FrameworkRoleController : BaseApiController
    {
        [ActionDescription("Search")]
        [HttpPost("[action]")]
        public string Search(FrameworkRoleSearcher searcher)
        {
            var vm = CreateVM<FrameworkRoleListVM>();
            vm.Searcher = searcher;
            return vm.GetJson();
        }

        [ActionDescription("Get")]
        [HttpGet("{id}")]
        public FrameworkRoleVM Get(Guid id)
        {
            var vm = CreateVM<FrameworkRoleVM>(id);
            return vm;
        }

        [ActionDescription("GetPageActions")]
        [HttpGet("[action]/{id}")]
        public FrameworkRoleMDVM2 GetPageActions(Guid id)
        {
            var vm = CreateVM<FrameworkRoleMDVM2>(id);
            return vm;
        }

        [ActionDescription("PageFunction")]
        [HttpPut("[action]")]
        public async Task<IActionResult> EditPrivilege(FrameworkRoleMDVM2 vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                await vm.DoChangeAsync();
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

        [ActionDescription("Create")]
        [HttpPost("[action]")]
        public IActionResult Add(FrameworkRoleVM vm)
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

        [ActionDescription("Edit")]
        [HttpPut("[action]")]
        public IActionResult Edit(FrameworkRoleVM vm)
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
        [ActionDescription("Delete")]
        public async Task<IActionResult> BatchDelete(string[] ids)
        {
            var vm = CreateVM<FrameworkRoleBatchVM>();
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
                List<Guid?> roleids = new List<Guid?>();
                foreach (var item in vm?.Ids)
                {
                    roleids.Add(Guid.Parse(item));
                }
                var userids = DC.Set<FrameworkUserRole>().Where(x => roleids.Contains(x.RoleId)).Select(x => x.UserId.ToString()).ToArray();
                await LoginUserInfo.RemoveUserCache(userids);
                return Ok(ids.Count());
            }
        }

        [ActionDescription("Export")]
        [HttpPost("[action]")]
        public IActionResult ExportExcel(FrameworkRoleSearcher searcher)
        {
            var vm = CreateVM<FrameworkRoleListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return vm.GetExportData();
        }

        [ActionDescription("ExportByIds")]
        [HttpPost("[action]")]
        public IActionResult ExportExcelByIds(string[] ids)
        {
            var vm = CreateVM<FrameworkRoleListVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            return vm.GetExportData();
        }

        [ActionDescription("DownloadTemplate")]
        [HttpGet("[action]")]
        public IActionResult GetExcelTemplate()
        {
            var vm = CreateVM<FrameworkRoleImportVM>();
            var qs = new Dictionary<string, string>();
            foreach (var item in Request.Query.Keys)
            {
                qs.Add(item, Request.Query[item]);
            }
            vm.SetParms(qs);
            var data = vm.GenerateTemplate(out string fileName);
            return File(data, "application/vnd.ms-excel", fileName);
        }

        [ActionDescription("Import")]
        [HttpPost("[action]")]
        public ActionResult Import(FrameworkRoleImportVM vm)
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


    }
}
