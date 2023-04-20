using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkTenantVMs;
using System.Linq;

namespace WalkingTec.Mvvm.Mvc.Admin.Controllers
{
    [Area("_Admin")]
    [ActionDescription("MenuKey.FrameworkTenant")]
    [FixConnection(CsName = "default")]
    public partial class FrameworkTenantController : BaseController
    {
        #region Search
        [ActionDescription("Sys.Search")]
        public ActionResult Index()
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
        public IActionResult Search(FrameworkTenantSearcher searcher)
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
        public ActionResult Create()
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
        public ActionResult Create(FrameworkTenantVM vm)
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
                    vm.DoReInit();
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
        public ActionResult Edit(string id)
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
        public ActionResult Edit(FrameworkTenantVM vm)
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
                vm.DoEdit();
                if (!ModelState.IsValid)
                {
                    vm.DoReInit();
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
        public ActionResult Delete(string id)
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
        public ActionResult Delete(string id, IFormCollection nouse)
        {
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
            var vm = Wtm.CreateVM<FrameworkTenantVM>(id);
            vm.DoDelete();
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
        public ActionResult Details(string id)
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
        public ActionResult BatchEdit(string[] IDs)
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
        public ActionResult DoBatchEdit(FrameworkTenantBatchVM vm, IFormCollection nouse)
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
        public ActionResult BatchDelete(string[] IDs)
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
        public ActionResult DoBatchDelete(FrameworkTenantBatchVM vm, IFormCollection nouse)
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
        public ActionResult Import()
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
        public ActionResult Import(FrameworkTenantImportVM vm, IFormCollection nouse)
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
        public IActionResult ExportExcel(FrameworkTenantListVM vm)
        {
            if (await CanUseTenant() == false)
            {
                return Content(Localizer["_Admin.TenantNotAllowed"]);
            }
            return await vm.GetExportData();
        }

        private async Task<bool> CanUseTenant()
        {
            if((await Wtm.GetLoginUserInfo ()) != null && ((await Wtm.GetLoginUserInfo ()).CurrentTenant == null || Wtm.GlobaInfo.AllTenant.Any(x=>x.TCode == (await Wtm.GetLoginUserInfo ()).CurrentTenant && x.Enabled==true && x.EnableSub == true)))
            {
                return true;
            }
            return false;
        }
    }
}
