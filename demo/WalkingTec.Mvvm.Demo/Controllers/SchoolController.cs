using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Demo.ViewModels.SchoolVMs;
using WalkingTec.Mvvm.Mvc.Binders;
using WalkingTec.Mvvm.Demo.Models;

namespace WalkingTec.Mvvm.Demo.Controllers
{
    
    [ActionDescription("学校管理（单表）")]
    public class SchoolController : BaseController
    {
        #region 搜索
        [ActionDescription("搜索")]
        public ActionResult Index()
        {
            var vm = CreateVM<SchoolListVM>();
            return PartialView(vm);
        }

        [ActionDescription("搜索")]
        [HttpPost]
        public ActionResult Index(SchoolListVM vm)
        {
            return PartialView(vm);
        }
        #endregion


        [ActionDescription("搜索并修改某字段")]
        public ActionResult EditIndex()
        {
            var vm = CreateVM<SchoolListVM2>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("搜索并修改某字段")]
        public ActionResult EditIndex(SchoolListVM2 vm)
        {
            //由于只更新名称字段，其他必填字段并没有值也不影响            
            ModelState.Clear();
            foreach (var item in vm.EntityList)
            {
                //手动更新某个字段，由于没有使用BaseCRUDVM，如果有验证条件需要自己判断
                DC.UpdateProperty<School>(new School { ID = item.ID, SchoolName = item.SchoolName }, x => x.SchoolName);
            }
            DC.SaveChanges();
            return PartialView(vm);
        }


        #region 新建
        [ActionDescription("新建")]
        public ActionResult Create()
        {
            var vm = CreateVM<SchoolVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("新建")]
        [StringNeedLTGT]
        public ActionResult Create(SchoolVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                vm.DoAdd();
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
        public ActionResult Edit(Guid id)
        {
            var vm = CreateVM<SchoolVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("修改")]
        [HttpPost]
        [StringNeedLTGT]
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
        public ActionResult Delete(Guid id)
        {
            var vm = CreateVM<SchoolVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("删除")]
        [HttpPost]
        public ActionResult Delete(Guid id, IFormCollection nouse)
        {
            var vm = CreateVM<SchoolVM>(id);
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
        public ActionResult Details(Guid id)
        {
            var vm = CreateVM<SchoolVM>(id);
            return PartialView(vm);
        }
        #endregion

        #region 批量修改
        [HttpPost]
        [ActionDescription("批量修改")]
        public ActionResult BatchEdit(Guid[] IDs)
        {
            var vm = CreateVM<SchoolBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("批量修改")]
        public ActionResult DoBatchEdit(SchoolBatchVM vm, IFormCollection nouse)
        {
            if (!ModelState.IsValid || !vm.DoBatchEdit())
            {
                return PartialView("BatchEdit",vm);
            }
            else
            {
                return FFResult().RefreshGrid().CloseDialog().Alert("操作成功，共有"+vm.Ids.Length+"条数据被修改");
            }
        }
        #endregion

        #region 批量删除
        [HttpPost]
        [ActionDescription("批量删除")]
        public ActionResult BatchDelete(Guid[] IDs)
        {
            var vm = CreateVM<SchoolBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("批量删除")]
        public ActionResult DoBatchDelete(SchoolBatchVM vm, IFormCollection nouse)
        {
            if (!ModelState.IsValid || !vm.DoBatchDelete())
            {
                return PartialView("BatchDelete",vm);
            }
            else
            {
                return FFResult().RefreshGrid().CloseDialog().Alert("操作成功，共有"+vm.Ids.Length+"条数据被删除");
            }
        }
        #endregion

        #region 导入
		[ActionDescription("导入")]
        public ActionResult Import()
        {
            var vm = CreateVM<SchoolImportVM>();
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
                return FFResult().RefreshGrid().CloseDialog().Alert("成功导入 " + vm.EntityList.Count.ToString() + " 行数据");
            }
        }
        #endregion


        #region 主子表新建
        [ActionDescription("主子表新建")]
        public ActionResult Create2()
        {
            var vm = CreateVM<SchoolVM>();
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
        public ActionResult Edit2(Guid id)
        {
            var vm = CreateVM<SchoolVM>(id);
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

    }
}
