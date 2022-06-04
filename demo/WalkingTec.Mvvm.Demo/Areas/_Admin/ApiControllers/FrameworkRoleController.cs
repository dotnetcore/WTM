// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ActionDescription("MenuKey.RoleManagement")]
    [ApiController]
    [Route("api/_[controller]")]
    public class FrameworkRoleController : BaseApiController
    {
        [ActionDescription("Sys.Search")]
        [HttpPost("[action]")]
        public IActionResult Search(FrameworkRoleSearcher searcher)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Request.RedirectCall(Wtm).Result;
            }
            if (ModelState.IsValid)
            {
                var vm = Wtm.CreateVM<FrameworkRoleListVM>(passInit: true);
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
        public FrameworkRoleVM Get(Guid id)
        {
            var vm = Wtm.CreateVM<FrameworkRoleVM>(id);
            return vm;
        }

        [ActionDescription("GetPageActions")]
        [HttpGet("[action]/{id}")]
        [AllRights]
        public FrameworkRoleMDVM2 GetPageActions(string id)
        {
            var vm = Wtm.CreateVM<FrameworkRoleMDVM2>(id);
            return vm;
        }

        [ActionDescription("_Admin.PageFunction")]
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

        [ActionDescription("Sys.Create")]
        [HttpPost("[action]")]
        public IActionResult Add(FrameworkRoleVM vm)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
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

        [ActionDescription("Sys.Edit")]
        [HttpPut("[action]")]
        public IActionResult Edit(FrameworkRoleVM vm)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
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
        public async Task<IActionResult> BatchDelete(string[] ids)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkRoleBatchVM>();
            List<string> RoleCode = new List<string>();
            if (ids != null && ids.Count() > 0)
            {
                RoleCode = DC.Set<FrameworkRole>().CheckIDs(new List<string>(ids)).Select(x => x.RoleCode).ToList();
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
                var ur = DC.Set<FrameworkUserRole>().Where(x => RoleCode.Contains(x.RoleCode)).ToList();
                var itcodes = ur.Select(x => x.UserCode).ToArray();
                DC.Set<FrameworkUserRole>().RemoveRange(ur);
                DC.SaveChanges();
                await Wtm.RemoveUserCacheByRole(itcodes);
                await Wtm.RemoveRoleCache(Wtm.LoginUserInfo.CurrentTenant);
                return Ok(ids.Count());
            }
        }

        [ActionDescription("Sys.Export")]
        [HttpPost("[action]")]
        public IActionResult ExportExcel(FrameworkRoleSearcher searcher)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkRoleListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return vm.GetExportData();
        }

        [ActionDescription("Sys.ExportByIds")]
        [HttpPost("[action]")]
        public IActionResult ExportExcelByIds(string[] ids)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkRoleListVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            return vm.GetExportData();
        }

        [ActionDescription("Sys.DownloadTemplate")]
        [HttpGet("[action]")]
        public IActionResult GetExcelTemplate()
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkRoleImportVM>();
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
        [HttpPost("[action]")]
        public ActionResult Import(FrameworkRoleImportVM vm)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }

            if (vm.ErrorListVM.EntityList.Count > 0 || !vm.BatchSaveData())
            {
                return BadRequest(vm.GetErrorJson());
            }
            else
            {
                Wtm.RemoveRoleCache(Wtm.LoginUserInfo.CurrentTenant).Wait();
                return Ok(vm.EntityList.Count);
            }
        }
    }
}
