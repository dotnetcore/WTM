// WTM默认页面 Wtm buidin page
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs;

namespace WalkingTec.Mvvm.Mvc.Admin.Controllers
{
    [Area("_Admin")]
    [ActionDescription("MenuKey.RoleManagement")]
    public class FrameworkRoleController : BaseController
    {
        [ActionDescription("Sys.Search")]
        public async Task<IActionResult> Index()
        {
            var vm = Wtm.CreateVM<FrameworkRoleListVM>();
            return PartialView(vm);
        }
        [ActionDescription("Sys.Search")]
        [HttpPost]
        public async Task<IActionResult> Search(FrameworkRoleSearcher searcher)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                searcher.IsPlainText = false;
                return Wtm.CallAPI<string>("mainhost", "/api/_frameworkrole/search", HttpMethodEnum.POST, searcher).Result.ToActionResult();
            }
            var vm = Wtm.CreateVM<FrameworkRoleListVM>(passInit: true);
            if (ModelState.IsValid)
            {
                vm.Searcher = searcher;
                return Content(vm.GetJson(false));
            }
            else
            {
                return Content(vm.GetError());
            }
        }

        [ActionDescription("Sys.Create")]
        public async Task<IActionResult> Create()
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkRoleVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Create")]
        public async Task<IActionResult> Create(FrameworkRoleVM vm)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                await vm.DoAdd();
                return FFResult().CloseDialog().RefreshGrid();
            }
        }

        [ActionDescription("Sys.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkRoleVM>(id);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Edit")]
        [ValidateFormItemOnly]
        public async Task<IActionResult> Edit(FrameworkRoleVM vm)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                await vm.DoEdit();
                return FFResult().CloseDialog().RefreshGrid();
            }
        }

        [ActionDescription("Sys.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkRoleVM>(id);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Delete")]
        public async Task<ActionResult> Delete(Guid id, IFormCollection noUse)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkRoleVM>(id);
            await vm.DoDelete();
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                return FFResult().CloseDialog().RefreshGrid();
            }
        }

        [HttpPost]
        [ActionDescription("Sys.BatchDelete")]
        public async Task<IActionResult> BatchDelete(Guid[] ids)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkRoleBatchVM>(Ids: ids);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchDelete")]
        public async Task<ActionResult> DoBatchDelete(FrameworkRoleBatchVM vm, IFormCollection noUse)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            List<string> RoleCode = DC.Set<FrameworkRole>().CheckIDs(new List<string>(vm.Ids)).Select(x => x.RoleCode).ToList();
            if (!ModelState.IsValid || !await vm.DoBatchDelete())
            {
                return PartialView("BatchDelete", vm);
            }
            else
            {
                var ur = DC.Set<FrameworkUserRole>().Where(x => RoleCode.Contains(x.RoleCode)).ToList();
                var itcodes = ur.Select(x => x.UserCode).ToArray();
                DC.Set<FrameworkUserRole>().RemoveRange(ur);
                await DC.SaveChangesAsync();
                await Wtm.RemoveUserCacheByRole(RoleCode.ToArray());
                await Wtm.RemoveRoleCache((await Wtm.GetLoginUserInfo ()).CurrentTenant);
                return FFResult().CloseDialog().RefreshGrid();
            }
        }

        [ActionDescription("Sys.Import")]
        public async Task<IActionResult> Import()
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkRoleImportVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Import")]
        public async Task<IActionResult> Import(FrameworkRoleImportVM vm, IFormCollection nouse)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            if (vm.ErrorListVM.EntityList.Count > 0 || !await vm.BatchSaveData())
            {
                return PartialView(vm);
            }
            else
            {
                Wtm.RemoveRoleCache((await Wtm.GetLoginUserInfo ()).CurrentTenant).Wait();
                return FFResult().CloseDialog().RefreshGrid().Alert(Localizer["Sys.ImportSuccess", vm.EntityList.Count.ToString()]);
            }
        }

        [ActionDescription("Sys.Details")]
        public PartialViewResult Details(Guid id)
        {
            var role = Wtm.CreateVM<FrameworkRoleMDVM>(id);
            role.ListVM.SearcherMode = ListVMSearchModeEnum.Custom1;
            return PartialView(role);
        }

        [ActionDescription("_Admin.PageFunction")]
        public PartialViewResult PageFunction(Guid id)
        {
            var role = Wtm.CreateVM<FrameworkRoleMDVM>(id);
            role.ListVM.SearcherMode = ListVMSearchModeEnum.Custom2;
            return PartialView(role);
        }

        [ActionDescription("_Admin.PageFunction")]
        [HttpPost]
        public async Task<ActionResult> PageFunction(FrameworkRoleMDVM vm, IFormCollection noUse)
        {
            await vm.DoChangeAsync();
            //if(vm.MSD.IsValid == false)
            //{
            //    return FFResult().CloseDialog().Alert(vm.MSD.GetFirstError());
            //}
            return FFResult().CloseDialog().Alert(Localizer["Sys.OprationSuccess"]);
        }

        [ActionDescription("Sys.Export")]
        [HttpPost]
        public async Task<IActionResult> ExportExcel(FrameworkRoleListVM vm)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            return await vm.GetExportData();
        }
    }
}
