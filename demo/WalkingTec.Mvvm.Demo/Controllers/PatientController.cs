using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.ViewModels.PatientVMs;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Demo.Controllers
{
    
    [ActionDescription("租户管理")]
    public partial class PatientController : BaseController
    {
        #region Search
        [ActionDescription("Sys.Search")]
        public async Task<IActionResult> Index()
        {
            var vm = Wtm.CreateVM<PatientListVM>();
            return PartialView(vm);
        }

        [ActionDescription("Sys.Search")]
        [HttpPost]
        public string Search(PatientSearcher searcher)
        {
            var vm = Wtm.CreateVM<PatientListVM>(passInit: true);
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
            var vm = Wtm.CreateVM<PatientVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Create")]
        public async Task<IActionResult> Create(PatientVM vm)
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
            var vm = Wtm.CreateVM<PatientVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("Sys.Edit")]
        [HttpPost]
        [ValidateFormItemOnly]
        public async Task<IActionResult> Edit(PatientVM vm)
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
            var vm = Wtm.CreateVM<PatientVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("Sys.Delete")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id, IFormCollection nouse)
        {
            var vm = Wtm.CreateVM<PatientVM>(id);
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
            var vm = Wtm.CreateVM<PatientVM>(id);
            return PartialView(vm);
        }
        #endregion

        #region BatchEdit
        [HttpPost]
        [ActionDescription("Sys.BatchEdit")]
        public async Task<IActionResult> BatchEdit(string[] IDs)
        {
            var vm = Wtm.CreateVM<PatientBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchEdit")]
        public async Task<IActionResult> DoBatchEdit(PatientBatchVM vm, IFormCollection nouse)
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
            var vm = Wtm.CreateVM<PatientBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchDelete")]
        public async Task<IActionResult> DoBatchDelete(PatientBatchVM vm, IFormCollection nouse)
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
            var vm = Wtm.CreateVM<PatientImportVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Import")]
        public async Task<IActionResult> Import(PatientImportVM vm, IFormCollection nouse)
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
        public async Task<IActionResult> ExportExcel(PatientListVM vm)
        {
            return await vm.GetExportData();
        }

    }
}
