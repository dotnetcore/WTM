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
        public ActionResult Index()
        {
            var vm = Wtm.CreateVM<FrameworkRoleListVM>();
            return PartialView(vm);
        }
        [ActionDescription("Sys.Search")]
        [HttpPost]
        public string Search(FrameworkRoleSearcher searcher)
        {
            var vm = Wtm.CreateVM<FrameworkRoleListVM>(passInit: true);
            if (ModelState.IsValid)
            {
                vm.Searcher = searcher;
                return vm.GetJson(false);
            }
            else
            {
                return vm.GetError();
            }
        }

        [ActionDescription("Sys.Create")]
        public ActionResult Create()
        {
            var vm = Wtm.CreateVM<FrameworkRoleVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Create")]
        public ActionResult Create(FrameworkRoleVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                vm.DoAdd();
                return FFResult().CloseDialog().RefreshGrid();
            }
        }

        [ActionDescription("Sys.Edit")]
        public ActionResult Edit(Guid id)
        {
            var vm = Wtm.CreateVM<FrameworkRoleVM>(id);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Edit")]
        [ValidateFormItemOnly]
        public ActionResult Edit(FrameworkRoleVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                vm.DoEdit();
                return FFResult().CloseDialog().RefreshGrid();
            }
        }

        [ActionDescription("Sys.Delete")]
        public ActionResult Delete(Guid id)
        {
            var vm = Wtm.CreateVM<FrameworkRoleVM>(id);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Delete")]
        public async Task<ActionResult> Delete(Guid id, IFormCollection noUse)
        {
            var vm = Wtm.CreateVM<FrameworkRoleVM>(id);
            await vm.DoDeleteAsync();
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
        public ActionResult BatchDelete(Guid[] ids)
        {
            var vm = Wtm.CreateVM<FrameworkRoleBatchVM>(Ids: ids);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchDelete")]
        public async Task<ActionResult> DoBatchDelete(FrameworkRoleBatchVM vm, IFormCollection noUse)
        {
            List<string> RoleCode = DC.Set<FrameworkRole>().CheckIDs(new List<string>(vm.Ids)).Select(x => x.RoleCode).ToList();
            if (!ModelState.IsValid || !vm.DoBatchDelete())
            {
                return PartialView("BatchDelete", vm);
            }
            else
            {
                var ur = DC.Set<FrameworkUserRole>().Where(x => RoleCode.Contains(x.RoleCode)).ToList();
                var itcodes = ur.Select(x => x.UserCode).ToArray();
                DC.Set<FrameworkUserRole>().RemoveRange(ur);
                DC.SaveChanges();
                await Wtm.RemoveUserCache(itcodes);
                return FFResult().CloseDialog().RefreshGrid();
            }
        }

        [ActionDescription("Sys.Import")]
        public ActionResult Import()
        {
            var vm = Wtm.CreateVM<FrameworkRoleImportVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Import")]
        public ActionResult Import(FrameworkRoleImportVM vm, IFormCollection nouse)
        {
            if (vm.ErrorListVM.EntityList.Count > 0 || !vm.BatchSaveData())
            {
                return PartialView(vm);
            }
            else
            {
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
            return FFResult().CloseDialog().Alert(Localizer["Sys.OprationSuccess"]);
        }

        [ActionDescription("Sys.Export")]
        [HttpPost]
        public IActionResult ExportExcel(FrameworkRoleListVM vm)
        {
            return vm.GetExportData();
        }
    }
}
