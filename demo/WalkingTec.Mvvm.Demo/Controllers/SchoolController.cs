using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.Util;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo.ViewModels.SchoolVMs;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Binders;

namespace WalkingTec.Mvvm.Demo.Controllers
{
    [AuthorizeJwtWithCookie]
    public class SchoolController : BaseController
    {
        #region 搜索
        [ActionDescription("Search")]
        public ActionResult Index()
        {
            var vm = Wtm.CreateVM<SchoolListVM>();
            return PartialView(vm);
        }

        [ActionDescription("Search")]
        [HttpPost]
        public ActionResult Index(SchoolListVM vm)
        {
            vm.DoInit();
            return PartialView(vm);
        }

        [ActionDescription("Search")]
        [HttpPost]
        public string Search(SchoolSearcher searcher)
        {
            var vm = Wtm.CreateVM<SchoolListVM>(passInit: true);
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


        [ActionDescription("搜索并修改某字段")]
        public ActionResult EditIndex()
        {
            var vm = Wtm.CreateVM<SchoolListVM2>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("搜索并修改某字段")]
        public ActionResult EditIndex(SchoolListVM2 vm)
        {
            if (!ModelState.IsValid)
            {
                vm.ProcessListError(vm.EntityList);
                return PartialView(vm);
            }
            else
            {
                vm.UpdateEntityList();
            }
            return PartialView(vm);
        }


        #region 新建
        [ActionDescription("新建")]
        public ActionResult Create()
        {
            var vm = Wtm.CreateVM<SchoolVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("新建")]
        public ActionResult Create(SchoolVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                vm.DoAdd();
                var rv = vm.StartWorkflowAsync("学校审批").Result;
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

        #region 修改
        [ActionDescription("修改")]
        public ActionResult Edit(string id)
        {
            var vm = Wtm.CreateVM<SchoolVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("修改")]
        [HttpPost]
        public ActionResult Edit(SchoolVM vm)
        {
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

        #region 删除
        [ActionDescription("删除")]
        public ActionResult Delete(int id)
        {
            var vm = Wtm.CreateVM<SchoolVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("删除")]
        [HttpPost]
        public ActionResult Delete(int id, IFormCollection nouse)
        {
            var vm = Wtm.CreateVM<SchoolVM>(id);
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

        #region 详细
        [ActionDescription("详细")]
        public ActionResult Details(int id)
        {
            var vm = Wtm.CreateVM<SchoolVM>(id);
            return PartialView(vm);
        }
        #endregion

        #region 批量修改
        [HttpPost]
        [ActionDescription("批量修改")]
        public ActionResult BatchEdit(string[] IDs)
        {
            var vm = Wtm.CreateVM<SchoolBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("批量修改")]
        public ActionResult DoBatchEdit(SchoolBatchVM vm, IFormCollection nouse)
        {
            if (!ModelState.IsValid || !vm.DoBatchEdit())
            {
                return PartialView("BatchEdit", vm);
            }
            else
            {
                return FFResult().RefreshGrid().CloseDialog().Alert("操作成功，共有" + vm.Ids.Length + "条数据被修改");
            }
        }
        #endregion

        #region 批量删除
        [HttpPost]
        [ActionDescription("批量删除")]
        public ActionResult BatchDelete(string[] ids)
        {
            var vm = Wtm.CreateVM<SchoolBatchVM>();
            if (ids != null && ids.Length > 0)
            {
                vm.Ids = ids;
            }
            else
            {
                return Ok();
            }
            if (!ModelState.IsValid || !vm.DoBatchDelete())
            {
                return FFResult().Alert(ModelState.GetErrorJson().GetFirstError());
            }
            else
            {
                return FFResult().RefreshGrid().Alert(Localizer["Sys.BatchDeleteSuccess",vm.Ids.Length]);
            }
        }

        #endregion

        #region 导入
        [ActionDescription("导入")]
        public ActionResult Import()
        {
            var vm = Wtm.CreateVM<SchoolImportVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("导入")]
        public ActionResult Import(SchoolImportVM vm, IFormCollection nouse)
        {
            if (vm.ErrorListVM.EntityList.Count > 0 || !vm.BatchSaveData())
            {
                return PartialView(vm);
            }
            else
            {
                return FFResult().RefreshGrid().CloseDialog().Alert(Localizer["Sys.ImportSuccess", vm.EntityList.Count.ToString()]);
            }
        }
        #endregion


        #region 主子表新建
        [ActionDescription("主子表新建")]
        public ActionResult Create2()
        {
            var vm = Wtm.CreateVM<SchoolVM>();
            vm.MajorList.DetailGridPrix = "Entity.Majors";
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("主子表新建")]
        public ActionResult Create2(SchoolVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.MajorList.ProcessListError(vm.Entity.Majors);
                return PartialView(vm);
            }
            else
            {
                if (vm.Entity.Majors == null)
                {
                    vm.Entity.Majors = new System.Collections.Generic.List<Major>();
                }
                vm.DoAdd();
                if (!ModelState.IsValid)
                {
                    vm.MajorList.ProcessListError(vm.Entity.Majors);
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


        #region 主子表修改
        [ActionDescription("主子表修改")]
        public ActionResult Edit2(int id)
        {
            var vm = Wtm.CreateVM<SchoolVM>(id);
            vm.MajorList.DetailGridPrix = "Entity.Majors";
            vm.MajorList.Searcher.SchoolId = id;
            return PartialView(vm);
        }

        [ActionDescription("主子表修改")]
        [HttpPost]
        public ActionResult Edit2(SchoolVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.MajorList.ProcessListError(vm.Entity.Majors);
                return PartialView(vm);
            }
            else
            {
                if (vm.Entity.Majors == null)
                {
                    vm.Entity.Majors = new System.Collections.Generic.List<Major>();
                }
                vm.DoEdit();
                if (!ModelState.IsValid)
                {
                    vm.DoReInit();
                    vm.MajorList.ProcessListError(vm.Entity.Majors);
                    return PartialView(vm);
                }
                else
                {
                    return FFResult().CloseDialog().RefreshGridRow(vm.Entity.ID);
                }
            }
        }
        #endregion

        [ActionDescription("Export")]
        [HttpPost]
        public ActionResult ExportExcel(SchoolListVM vm)
        {
            vm.ExportMaxCount = 5; //自定义每个Excel最多数据行数，默认是100万
            vm.ExportTitleBackColor = HSSFColor.Black.Index;
            vm.ExportTitleFontColor = HSSFColor.White.Index;
            return vm.GetExportData();
        }

        [HttpPost]
        public IActionResult Download(long id, long[] ids)
        {
            return File(Array.Empty<byte>(), "application/vnd.ms-excel", $"Export_ActionLog_{DateTime.Now:yyyy-MM-dd}.xls");
        }
    }
}
