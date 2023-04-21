using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.ViewModels.LinkTestVMs;
using WalkingTec.Mvvm.Demo.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Demo.Controllers
{
    
    [ActionDescription("联动测试")]
    public partial class LinkTestController : BaseController
    {
        #region Search
        [ActionDescription("Sys.Search")]
        public async Task<IActionResult> Index()
        {
            var vm = Wtm.CreateVM<LinkTestListVM>();
            return PartialView(vm);
        }

        [ActionDescription("Sys.Search")]
        [HttpPost]
        public string Search(LinkTestSearcher searcher)
        {
            var vm = Wtm.CreateVM<LinkTestListVM>(passInit: true);
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
            var vm = Wtm.CreateVM<LinkTestVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Create")]
        public async Task<IActionResult> Create(LinkTestVM vm)
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
            var vm = Wtm.CreateVM<LinkTestVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("Sys.Edit")]
        [HttpPost]
        [ValidateFormItemOnly]
        public async Task<IActionResult> Edit(LinkTestVM vm)
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
            var vm = Wtm.CreateVM<LinkTestVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("Sys.Delete")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id, IFormCollection nouse)
        {
            var vm = Wtm.CreateVM<LinkTestVM>(id);
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
            var vm = Wtm.CreateVM<LinkTestVM>(id);
            return PartialView(vm);
        }
        #endregion

        #region BatchEdit
        [HttpPost]
        [ActionDescription("Sys.BatchEdit")]
        public async Task<IActionResult> BatchEdit(string[] IDs)
        {
            var vm = Wtm.CreateVM<LinkTestBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchEdit")]
        public async Task<IActionResult> DoBatchEdit(LinkTestBatchVM vm, IFormCollection nouse)
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
            var vm = Wtm.CreateVM<LinkTestBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchDelete")]
        public async Task<IActionResult> DoBatchDelete(LinkTestBatchVM vm, IFormCollection nouse)
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
            var vm = Wtm.CreateVM<LinkTestImportVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Import")]
        public async Task<IActionResult> Import(LinkTestImportVM vm, IFormCollection nouse)
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
        public async Task<IActionResult> ExportExcel(LinkTestListVM vm)
        {
            return await vm.GetExportData();
        }

        public IActionResult GetMajorBySchool(List<string> id)
        {
            var rv = DC.Set<Major>().CheckIDs(id, x => x.SchoolId).GetSelectListItems(Wtm,x=>x.MajorName);
            return JsonMore(rv);
        }

        public IActionResult GetStudentByMajor(List<string> id)
        {
            var tempid = DC.Set<StudentMajor>().CheckIDs(id, x => x.MajorId).Select(x => x.StudentId).ToList().Cast<string>().ToList();
            var rv = DC.Set<Student>().CheckIDs(tempid).GetSelectListItems(Wtm, x => x.Name);
            return JsonMore(rv);
        }

        public IActionResult GetAllSchools()
        {
            var rv = DC.Set<School>().GetSelectListItems(Wtm, x => x.SchoolName);
            return JsonMore(rv);
        }

        public IActionResult GetAllStudents()
        {
            var rv = DC.Set<Student>().GetSelectListItems(Wtm, x => x.Name);
            return JsonMore(rv);
        }

        public IActionResult GetStudentById(List<string> id)
        {

            var rv = DC.Set<Student>().CheckContain(id,x=>x.ID).GetSelectListItems(Wtm, x => x.Name);
            return JsonMore(rv);
        }
    }
}
