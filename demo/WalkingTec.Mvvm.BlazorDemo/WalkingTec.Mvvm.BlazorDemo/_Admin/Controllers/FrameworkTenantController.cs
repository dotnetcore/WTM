// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkTenantVMs;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ActionDescription("MenuKey.FrameworkTenant")]
    [ApiController]
    [Route("api/_[controller]")]
    [FixConnection(CsName = "default")]
    public class FrameworkTenantController : BaseApiController
    {
        [ActionDescription("Sys.Search")]
        [HttpPost("[action]")]
        public async Task<IActionResult> Search(FrameworkTenantSearcher searcher)
        {
            if (await CanUseTenant() == false)
            {
                ModelState.Clear();
                ModelState.AddModelError(" TenantNotAllowed", Localizer["_Admin.TenantNotAllowed"]);
                return BadRequest(ModelState.GetErrorJson());
            }
            if (ModelState.IsValid)
            {
                var vm = Wtm.CreateVM<FrameworkTenantListVM>(passInit: true);
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
        public FrameworkTenantVM Get(Guid id)
        {
            var vm = Wtm.CreateVM<FrameworkTenantVM>(id);
            return vm;
        }

        [ActionDescription("Sys.Create")]
        [HttpPost("[action]")]
        public async Task<IActionResult> Add(FrameworkTenantVM vm)
        {
            if (await CanUseTenant() == false)
            {
                ModelState.Clear();
                ModelState.AddModelError(" TenantNotAllowed", Localizer["_Admin.TenantNotAllowed"]);
                return BadRequest(ModelState.GetErrorJson());
            }
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
        [HttpPut("[action]")]
        public async Task<IActionResult> Edit(FrameworkTenantVM vm)
        {
            if (await CanUseTenant() == false)
            {
                ModelState.Clear();
                ModelState.AddModelError(" TenantNotAllowed", Localizer["_Admin.TenantNotAllowed"]);
                return BadRequest(ModelState.GetErrorJson());
            }
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
            if (await CanUseTenant() == false)
            {
                ModelState.Clear();
                ModelState.AddModelError(" TenantNotAllowed", Localizer["_Admin.TenantNotAllowed"]);
                return BadRequest(ModelState.GetErrorJson());
            }
            var vm = Wtm.CreateVM<FrameworkTenantBatchVM>();
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
                Cache.Delete(nameof(GlobalData.AllTenant));
                return Ok(ids.Count());
            }
        }

        [ActionDescription("Sys.Export")]
        [HttpPost("[action]")]
        public async Task<IActionResult> ExportExcel(FrameworkTenantSearcher searcher)
        {
            if (await CanUseTenant() == false)
            {
                ModelState.Clear();
                ModelState.AddModelError(" TenantNotAllowed", Localizer["_Admin.TenantNotAllowed"]);
                return BadRequest(ModelState.GetErrorJson());
            }
            var vm = Wtm.CreateVM<FrameworkTenantListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return await vm.GetExportData();
        }

        [ActionDescription("Sys.ExportByIds")]
        [HttpPost("[action]")]
        public async Task<IActionResult> ExportExcelByIds(string[] ids)
        {
            if (await CanUseTenant() == false)
            {
                ModelState.Clear();
                ModelState.AddModelError(" TenantNotAllowed", Localizer["_Admin.TenantNotAllowed"]);
                return BadRequest(ModelState.GetErrorJson());
            }
            var vm = Wtm.CreateVM<FrameworkTenantListVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            return await vm.GetExportData();
        }

        [ActionDescription("Sys.DownloadTemplate")]
        [HttpGet("[action]")]
        public IActionResult GetExcelTemplate()
        {
            if (await CanUseTenant() == false)
            {
                ModelState.Clear();
                ModelState.AddModelError(" TenantNotAllowed", Localizer["_Admin.TenantNotAllowed"]);
                return BadRequest(ModelState.GetErrorJson());
            }
            var vm = Wtm.CreateVM<FrameworkTenantImportVM>();
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
        public async Task<IActionResult> Import(FrameworkTenantImportVM vm)
        {

            if (await CanUseTenant() == false)
            {
                ModelState.Clear();
                ModelState.AddModelError(" TenantNotAllowed", Localizer["_Admin.TenantNotAllowed"]);
                return BadRequest(ModelState.GetErrorJson());
            }
            if (vm.ErrorListVM.EntityList.Count > 0 || !await vm.BatchSaveData())
            {
                return BadRequest(vm.GetErrorJson());
            }
            else
            {
                return Ok(vm.EntityList.Count);
            }
        }

        [HttpGet("GetFrameworkTenants")]
        [ActionDescription("GetTenants")]
        [AllRights]
        public IActionResult GetFrameworkTenants(string parent)
        {
            if (parent == "")
            {
                parent = null;
            }
            return Ok(Wtm.GlobaInfo.AllTenant.AsQueryable().Where(x => x.TenantCode == parent).GetSelectListItems(Wtm, x => x.TName, x => x.TCode));
        }



        private async Task<bool> CanUseTenant()
        {
            if ((await Wtm.GetLoginUserInfo ()) != null && ((await Wtm.GetLoginUserInfo ()).CurrentTenant == null || Wtm.GlobaInfo.AllTenant.Any(x => x.TCode == (await Wtm.GetLoginUserInfo ()).CurrentTenant && x.Enabled == true && x.EnableSub == true)))
            {
                return true;
            }
            return false;
        }


    }
}
