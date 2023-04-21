using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.ViewModels.SoftFacInfoVMs;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Demo.Controllers
{
    
    [ActionDescription("softf")]
    public partial class SoftFacInfoController : BaseController
    {
        #region Search
        [ActionDescription("Sys.Search")]
        public async Task<IActionResult> Index()
        {
            var vm = Wtm.CreateVM<SoftFacInfoListVM>();
            return PartialView(vm);
        }

        [ActionDescription("Sys.Search")]
        [HttpPost]
        public string Search(SoftFacInfoSearcher searcher)
        {
            var vm = Wtm.CreateVM<SoftFacInfoListVM>(passInit: true);
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

        #region Create
        [ActionDescription("Sys.Create")]
        public async Task<IActionResult> Create()
        {
            var vm = Wtm.CreateVM<SoftFacInfoVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Create")]
        public async Task<IActionResult> Create(SoftFacInfoVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                await vm.DoAdd();
                if (!ModelState.IsValid)
                {
                    await vm.DoReInit();
                    return PartialView(vm);
                }
                else
                {
                    return FFResult().CloseDialog().RefreshGrid();
                }
            }
        }
        #endregion

        #region Edit
        [ActionDescription("Sys.Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var vm = Wtm.CreateVM<SoftFacInfoVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("Sys.Edit")]
        [HttpPost]
        [ValidateFormItemOnly]
        public async Task<IActionResult> Edit(SoftFacInfoVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                await vm.DoEdit();
                if (!ModelState.IsValid)
                {
                    await vm.DoReInit();
                    return PartialView(vm);
                }
                else
                {
                    return FFResult().CloseDialog().RefreshGridRow(vm.Entity.ID);
                }
            }
        }
        #endregion

        #region Delete
        [ActionDescription("Sys.Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var vm = Wtm.CreateVM<SoftFacInfoVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("Sys.Delete")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id, IFormCollection nouse)
        {
            var vm = Wtm.CreateVM<SoftFacInfoVM>(id);
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
        #endregion

        #region Details
        [ActionDescription("Sys.Details")]
        public async Task<IActionResult> Details(string id)
        {
            var vm = Wtm.CreateVM<SoftFacInfoVM>(id);
            return PartialView(vm);
        }
        #endregion

        #region BatchEdit
        [HttpPost]
        [ActionDescription("Sys.BatchEdit")]
        public async Task<IActionResult> BatchEdit(string[] IDs)
        {
            var vm = Wtm.CreateVM<SoftFacInfoBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchEdit")]
        public async Task<IActionResult> DoBatchEdit(SoftFacInfoBatchVM vm, IFormCollection nouse)
        {
            if (!ModelState.IsValid || !await vm.DoBatchEdit())
            {
                return PartialView("BatchEdit",vm);
            }
            else
            {
                return FFResult().CloseDialog().RefreshGrid().Alert(Localizer["Sys.BatchEditSuccess", vm.Ids.Length]);
            }
        }
        #endregion

        #region BatchDelete
        [HttpPost]
        [ActionDescription("Sys.BatchDelete")]
        public async Task<IActionResult> BatchDelete(string[] IDs)
        {
            var vm = Wtm.CreateVM<SoftFacInfoBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchDelete")]
        public async Task<IActionResult> DoBatchDelete(SoftFacInfoBatchVM vm, IFormCollection nouse)
        {
            if (!ModelState.IsValid || !await vm.DoBatchDelete())
            {
                return PartialView("BatchDelete",vm);
            }
            else
            {
                return FFResult().CloseDialog().RefreshGrid().Alert(Localizer["Sys.BatchDeleteSuccess", vm.Ids.Length]);
            }
        }
        #endregion

        #region Import
		[ActionDescription("Sys.Import")]
        public async Task<IActionResult> Import()
        {
            var vm = Wtm.CreateVM<SoftFacInfoImportVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Import")]
        public async Task<IActionResult> Import(SoftFacInfoImportVM vm, IFormCollection nouse)
        {
            if (vm.ErrorListVM.EntityList.Count > 0 || !await vm.BatchSaveData())
            {
                return PartialView(vm);
            }
            else
            {
                return FFResult().CloseDialog().RefreshGrid().Alert(Localizer["Sys.ImportSuccess", vm.EntityList.Count.ToString()]);
            }
        }
        #endregion

        [ActionDescription("Sys.Export")]
        [HttpPost]
        public async Task<IActionResult> ExportExcel(SoftFacInfoListVM vm)
        {
            return await vm.GetExportData();
        }

    }
}
