using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.ViewModels.CityVMs;
using WalkingTec.Mvvm.Demo.Models;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Demo.Controllers
{
    [ActionDescription("城市管理")]
    public partial class CityController : BaseController
    {
        #region Search
        [ActionDescription("Sys.Search")]
        public IActionResult Index ()
        {
            var vm = Wtm.CreateVM<CityVM>();
            return PartialView(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CitySearcher searcher)
        {
            var vm = Wtm.CreateVM<CityVM>();
            searcher.CopyContext(vm);
            await searcher.DoInit();
            vm.se = searcher;
            vm.list.Searcher = searcher;
            return PartialView(vm);
        }



        [ActionDescription("Codegen.FieldDes", IsPage = true)]
        public ActionResult CityPage()
        {
            var vm = Wtm.CreateVM<CityListVM>();
            return PartialView(vm);
        }

        //[ActionDescription("Codegen.Gen", IsPage = true)]
        //public ActionResult LookPage()
        //{
        //    var vm = Wtm.CreateVM<CityListVM>();
        //    return PartialView(vm);
        //}
        //[ActionDescription("Login.RememberMe", IsPage = true)]
        //public ActionResult LookMe()
        //{
        //    var vm = Wtm.CreateVM<CityListVM>();
        //    return PartialView(vm);
        //}



        [ActionDescription("Sys.Search")]
        [HttpPost]
        public string Search(CitySearcher searcher)
        {
            var vm = Wtm.CreateVM<CityListVM>(passInit: true);
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
        public IActionResult Create ()
        {
            var vm = Wtm.CreateVM<CityVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Create")]
        public async Task<IActionResult> Create(CityVM vm)
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

        [ActionDescription("Sys.Create")]
        public IActionResult CreateGroup ()
        {
            var vm = Wtm.CreateVM<GroupVMTest>();
            return PartialView(vm);
        }
        [HttpPost]
        [ActionDescription("Sys.Create")]
        public async Task<IActionResult> CreateGroup(GroupVMTest vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                await vm.vm1.DoAdd();
                await vm.vm2.DoAdd();
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
        public IActionResult Edit (string id)
        {
            var vm = Wtm.CreateVM<CityVM>(id);
            vm.CityChildrenList1.Searcher.ParentId = id;
            return PartialView(vm);
        }

        [ActionDescription("Sys.Edit")]
        public IActionResult EditGroup (string id)
        {
            var vm = Wtm.CreateVM<GroupVMTest>(values: x=>x.EntityId == id);           
            return PartialView(vm);
        }


        [ActionDescription("Sys.Edit")]
        [HttpPost]
        [ValidateFormItemOnly]
        public async Task<IActionResult> Edit(CityVM vm)
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
        [ActionDescription("Sys.Edit")]
        [HttpPost]
        [ValidateFormItemOnly]
        public async Task<IActionResult> EditGroup(GroupVMTest vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                await vm.vm1.DoEdit();
                await vm.vm2.DoEdit();
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

        #region Delete
        [ActionDescription("Sys.Delete")]
        public IActionResult Delete (string id)
        {
            var vm = Wtm.CreateVM<CityVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("Sys.Delete")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id, IFormCollection nouse)
        {
            var vm = Wtm.CreateVM<CityVM>(id);
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
        public IActionResult Details (string id)
        {
            var vm = Wtm.CreateVM<CityVM>(id);
            return PartialView(vm);
        }
        #endregion

        #region BatchEdit
        [HttpPost]
        [ActionDescription("Sys.BatchEdit")]
        public IActionResult BatchEdit (string[] IDs)
        {
            var vm = Wtm.CreateVM<CityBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchEdit")]
        public async Task<IActionResult> DoBatchEdit(CityBatchVM vm, IFormCollection nouse)
        {
            if (!ModelState.IsValid || !await vm.DoBatchEdit())
            {
                return PartialView("BatchEdit", vm);
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
        public IActionResult BatchDelete (string[] IDs)
        {
            var vm = Wtm.CreateVM<CityBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchDelete")]
        public async Task<IActionResult> DoBatchDelete(CityBatchVM vm, IFormCollection nouse)
        {
            if (!ModelState.IsValid || !await vm.DoBatchDelete())
            {
                return PartialView("BatchDelete", vm);
            }
            else
            {
                return FFResult().CloseDialog().RefreshGrid().Alert(Localizer["Sys.BatchDeleteSuccess", vm.Ids.Length]);
            }
        }
        #endregion

        #region Import
        [ActionDescription("Sys.Import")]
        public IActionResult Import ()
        {
            var vm = Wtm.CreateVM<CityImportVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Import")]
        public async Task<IActionResult> Import(CityImportVM vm, IFormCollection nouse)
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
        public async Task<IActionResult> ExportExcel(CityListVM vm)
        {
            return await vm.GetExportData();
        }
        public IActionResult GetAllCities()
        {
            var rv = DC.Set<City>().GetTreeSelectListItems(Wtm, x => x.Name);
            return JsonMore(rv);
        }

    }
}
