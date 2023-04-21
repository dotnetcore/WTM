using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkTenantVMs;
using System.Linq;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Mvc.Admin.Controllers
{
    [Area("_Admin")]
    [ActionDescription("MenuKey.FrameworkTenant")]
    [FixConnection(CsName = "default")]
    public partial class FrameworkTenantController : BaseController
    {
        #region Search
        [ActionDescription("Sys.Search")]
        public async Task<IActionResult> Index()
        {
            if(await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
            var vm = Wtm.CreateVM<FrameworkTenantListVM>();
            return PartialView(vm);
        }

        [ActionDescription("Sys.Search")]
        [HttpPost]
        public async Task<IActionResult> Search(FrameworkTenantSearcher searcher)
        {
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
            var vm = Wtm.CreateVM<FrameworkTenantListVM>(passInit: true);
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

        #endregion

        #region Create
        [ActionDescription("Sys.Create")]
        public async Task<IActionResult> Create()
        {
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
            var vm = Wtm.CreateVM<FrameworkTenantVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Create")]
        public async Task<IActionResult> Create(FrameworkTenantVM vm)
        {
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
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
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
            var vm = Wtm.CreateVM<FrameworkTenantVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("Sys.Edit")]
        [HttpPost]
        [ValidateFormItemOnly]
        public async Task<IActionResult> Edit(FrameworkTenantVM vm)
        {
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
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
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
            var vm = Wtm.CreateVM<FrameworkTenantVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("Sys.Delete")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id, IFormCollection nouse)
        {
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
            var vm = Wtm.CreateVM<FrameworkTenantVM>(id);
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
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
            var vm = Wtm.CreateVM<FrameworkTenantVM>(id);
            return PartialView(vm);
        }
        #endregion

        #region BatchEdit
        [HttpPost]
        [ActionDescription("Sys.BatchEdit")]
        public async Task<IActionResult> BatchEdit(string[] IDs)
        {
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
            var vm = Wtm.CreateVM<FrameworkTenantBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchEdit")]
        public async Task<IActionResult> DoBatchEdit(FrameworkTenantBatchVM vm, IFormCollection nouse)
        {
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
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
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
            var vm = Wtm.CreateVM<FrameworkTenantBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchDelete")]
        public async Task<IActionResult> DoBatchDelete(FrameworkTenantBatchVM vm, IFormCollection nouse)
        {
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
            if (!ModelState.IsValid || !await vm.DoBatchDelete())
            {
                return PartialView("BatchDelete",vm);
            }
            else
            {
                Cache.Delete(nameof(GlobalData.AllTenant));
                return FFResult().CloseDialog().RefreshGrid().Alert(Localizer["Sys.BatchDeleteSuccess", vm.Ids.Length]);
            }
        }
        #endregion

        #region Import
		[ActionDescription("Sys.Import")]
        public async Task<IActionResult> Import()
        {
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
            var vm = Wtm.CreateVM<FrameworkTenantImportVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Import")]
        public async Task<IActionResult> Import(FrameworkTenantImportVM vm, IFormCollection nouse)
        {
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
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
        public async Task<IActionResult> ExportExcel(FrameworkTenantListVM vm)
        {
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
            return await vm.GetExportData();
        }

        private async Task<bool> CanUseTenant()
        {
            var _loginUserInfo = await Wtm.GetLoginUserInfo ();
            if (_loginUserInfo != null && (_loginUserInfo.CurrentTenant == null || Wtm.GlobaInfo.AllTenant.Any(x=>x.TCode == _loginUserInfo.CurrentTenant && x.Enabled==true && x.EnableSub == true)))
            {
                return true;
            }
            return false;
        }
    }
}
