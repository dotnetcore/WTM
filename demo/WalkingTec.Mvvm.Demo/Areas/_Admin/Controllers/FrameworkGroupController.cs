// WTM默认页面 Wtm buidin page
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkGroupVMs;

namespace WalkingTec.Mvvm.Mvc.Admin.Controllers
{
    [Area("_Admin")]
    [ActionDescription("MenuKey.GroupManagement")]
    public class FrameworkGroupController : BaseController
    {
        #region 查询
        [ActionDescription("Sys.Search")]
        public ActionResult Index()
        {
            var vm = Wtm.CreateVM<FrameworkGroupListVM>();
            return PartialView(vm);
        }

        [ActionDescription("Sys.Search")]
        [HttpPost]
        public string Search(FrameworkGroupSearcher searcher)
        {
            var vm = Wtm.CreateVM<FrameworkGroupListVM>(passInit: true);
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
        #endregion

        [ActionDescription("Sys.Create")]
        public ActionResult Create()
        {
            var vm = Wtm.CreateVM<FrameworkGroupVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Create")]
        public ActionResult Create(FrameworkGroupVM vm)
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
            var vm = Wtm.CreateVM<FrameworkGroupVM>(id);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Edit")]
        [ValidateFormItemOnly]
        public ActionResult Edit(FrameworkGroupVM vm)
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
            var vm = Wtm.CreateVM<FrameworkGroupVM>(id);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Delete")]
        public async Task<ActionResult> Delete(Guid id, IFormCollection noUse)
        {
            var vm = Wtm.CreateVM<FrameworkGroupVM>(id);
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
            var vm = Wtm.CreateVM<FrameworkGroupBatchVM>(Ids: ids);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchDelete")]
        public async Task<ActionResult> DoBatchDelete(FrameworkGroupBatchVM vm, IFormCollection noUse)
        {
            List<string> GroupCode = DC.Set<FrameworkGroup>().CheckIDs(new List<string>(vm.Ids)).Select(x => x.GroupCode).ToList();
            if (!ModelState.IsValid || !vm.DoBatchDelete())
            {                
                return PartialView("BatchDelete", vm);
            }
            else
            {
                var gr = DC.Set<FrameworkUserGroup>().Where(x => GroupCode.Contains(x.GroupCode)).ToList();
                var itcodes = gr.Select(x => x.UserCode).ToArray();
                DC.Set<FrameworkUserGroup>().RemoveRange(gr);
                DC.SaveChanges();
                await Wtm.RemoveUserCache(itcodes);
                return FFResult().CloseDialog().RefreshGrid().Alert(Localizer["Sys.OprationSuccess"]);
            }
        }

        [ActionDescription("Sys.Import")]
        public ActionResult Import()
        {
            var vm = Wtm.CreateVM<FrameworkGroupImportVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Import")]
        public ActionResult Import(FrameworkGroupImportVM vm, IFormCollection nouse)
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

        [ActionDescription("_Admin.DataPrivilege")]
        public PartialViewResult DataFunction(string groupcode)
        {
            var role = Wtm.CreateVM<FrameworkGroupMDVM>(values: x=>x.GroupCode == groupcode);
            return PartialView(role);
        }

        [ActionDescription("_Admin.DataPrivilege")]
        [HttpPost]
        public ActionResult DataFunction(FrameworkGroupMDVM vm, IFormCollection noUse)
        {
            vm.DoChange();
            return FFResult().CloseDialog().Alert(Localizer["Sys.OprationSuccess"]);
        }

        [ActionDescription("Sys.Export")]
        [HttpPost]
        public IActionResult ExportExcel(FrameworkGroupListVM vm)
        {
            return vm.GetExportData();
        }

    }

}
