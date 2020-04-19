using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth.Attribute;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ActionDescription("UserManagement")]
    [ApiController]
    [Route("api/_FrameworkUserBase")]
    public class FrameworkUserController : BaseApiController
    {
        [ActionDescription("Search")]
        [HttpPost("[action]")]
        public string Search(FrameworkUserSearcher searcher)
        {
            var vm = CreateVM<FrameworkUserListVM>();
            vm.Searcher = searcher;
            return vm.GetJson();
        }

        [ActionDescription("Get")]
        [HttpGet("{id}")]
        public FrameworkUserVM Get(Guid id)
        {
            var vm = CreateVM<FrameworkUserVM>(id);
            return vm;
        }

        [ActionDescription("Create")]
        [HttpPost("[action]")]
        public async Task<IActionResult> Add(FrameworkUserVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                await vm.DoAddAsync();
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
        public async Task<IActionResult> Edit(FrameworkUserVM vm)
        {
            ModelState.Remove("Entity.Password");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                await vm.DoEditAsync(false);
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
            var vm = CreateVM<FrameworkUserBatchVM>();
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
                List<Guid?> tempids = new List<Guid?>();
                foreach (var item in vm?.Ids)
                {
                    tempids.Add(Guid.Parse(item));
                }
                var userids = DC.Set<FrameworkUserBase>().Where(x => tempids.Contains(x.ID)).Select(x => x.ID.ToString()).ToArray();
                await WtmContext.LoginUserInfo.RemoveUserCache(userids);
                return Ok(ids.Count());
            }
        }

        [ActionDescription("Export")]
        [HttpPost("[action]")]
        public IActionResult ExportExcel(FrameworkUserSearcher searcher)
        {
            var vm = CreateVM<FrameworkUserListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            var data = vm.GenerateExcel();
            return File(data, "application/vnd.ms-excel", $"Export_FrameworkUse_{DateTime.Now.ToString("yyyy-MM-dd")}.xls");
        }

        [ActionDescription("ExportByIds")]
        [HttpPost("[action]")]
        public IActionResult ExportExcelByIds(string[] ids)
        {
            var vm = CreateVM<FrameworkUserListVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            var data = vm.GenerateExcel();
            return File(data, "application/vnd.ms-excel", $"Export_FrameworkUse_{DateTime.Now.ToString("yyyy-MM-dd")}.xls");
        }

        [ActionDescription("DownloadTemplate")]
        [HttpGet("[action]")]
        public IActionResult GetExcelTemplate()
        {
            var vm = CreateVM<FrameworkUserImportVM>();
            var qs = new Dictionary<string, string>();
            if (Request != null)
            {
                foreach (var item in Request.Query.Keys)
                {
                    qs.Add(item, Request.Query[item]);
                }
            }
            vm.SetParms(qs);
            var data = vm.GenerateTemplate(out string fileName);
            return File(data, "application/vnd.ms-excel", fileName);
        }

        [ActionDescription("Import")]
        [HttpPost("[action]")]
        public ActionResult Import(FrameworkUserImportVM vm)
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
        [ActionDescription("GetRoles")]
        public ActionResult GetFrameworkRoles()
        {
            return Ok(DC.Set<FrameworkRole>().GetSelectListItems(null, x => x.RoleName));
        }

        [HttpGet("GetFrameworkGroups")]
        [ActionDescription("GetGroups")]
        public ActionResult GetFrameworkGroups()
        {
            return Ok(DC.Set<FrameworkGroup>().GetSelectListItems( null, x => x.GroupName));
        }

    }
}
