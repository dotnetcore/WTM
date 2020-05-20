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
    [ActionDescription("GroupManagement")]
    public class FrameworkGroupController : BaseController
    {
        #region 查询
        [ActionDescription("Search")]
        public ActionResult Index()
        {
            var vm = CreateVM<FrameworkGroupListVM>();
            return PartialView(vm);
        }

        [ActionDescription("Search")]
        [HttpPost]
        public string Search(FrameworkGroupListVM vm)
        {
            return vm.GetJson(false);
        }
        #endregion

        #region 新建
        [ActionDescription("Create")]
        public ActionResult Create()
        {
            var vm = CreateVM<FrameworkGroupVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Create")]
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
        #endregion

        #region 修改
        [ActionDescription("Edit")]
        public ActionResult Edit(Guid id)
        {
            var vm = CreateVM<FrameworkGroupVM>(id);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Edit")]
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
        #endregion

        #region 删除
        [ActionDescription("Delete")]
        public ActionResult Delete(Guid id)
        {
            var vm = CreateVM<FrameworkGroupVM>(id);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Delete")]
        public async Task<ActionResult> Delete(Guid id, IFormCollection noUse)
        {
            var vm = CreateVM<FrameworkGroupVM>(id);
            vm.DoDelete();
            if (!ModelState.IsValid)
            {
                var userids = DC.Set<FrameworkUserGroup>().Where(x => x.GroupId == id).Select(x => x.UserId.ToString()).ToArray();
                await LoginUserInfo.RemoveUserCache(userids);
                return PartialView(vm);
            }
            else
            {
                return FFResult().CloseDialog().RefreshGrid();
            }
        }
        #endregion

        #region 批量删除
        [HttpPost]
        [ActionDescription("BatchDelete")]
        public ActionResult BatchDelete(Guid[] ids)
        {
            var vm = CreateVM<FrameworkGroupBatchVM>(Ids: ids);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("BatchDelete")]
        public async Task<ActionResult> DoBatchDelete(FrameworkGroupBatchVM vm, IFormCollection noUse)
        {
            if (!ModelState.IsValid || !vm.DoBatchDelete())
            {

                return PartialView("BatchDelete", vm);
            }
            else
            {
                List<Guid?> groupids = new List<Guid?>();
                foreach (var item in vm?.Ids)
                {
                    groupids.Add(Guid.Parse(item));
                }
                var userids = DC.Set<FrameworkUserGroup>().Where(x => groupids.Contains(x.GroupId)).Select(x => x.UserId.ToString()).ToArray();
                await LoginUserInfo.RemoveUserCache(userids);
                return FFResult().CloseDialog().RefreshGrid().Alert(Program._localizer["OprationSuccess"]);
            }
        }
        #endregion

        #region 导入
        [ActionDescription("Import")]
        public ActionResult Import()
        {
            var vm = CreateVM<FrameworkGroupImportVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Import")]
        public ActionResult Import(FrameworkGroupImportVM vm, IFormCollection nouse)
        {
            if (vm.ErrorListVM.EntityList.Count > 0 || !vm.BatchSaveData())
            {
                return PartialView(vm);
            }
            else
            {
                return FFResult().CloseDialog().RefreshGrid().Alert(Program._localizer["ImportSuccess", vm.EntityList.Count.ToString()]);
            }
        }
        #endregion

        #region 页面权限
        [ActionDescription("DataPrivilege")]
        public PartialViewResult DataFunction(Guid id)
        {
            var role = CreateVM<FrameworkGroupMDVM>(values: x=>x.GroupId == id);
            return PartialView(role);
        }

        [ActionDescription("DataPrivilege")]
        [HttpPost]
        public ActionResult DataFunction(FrameworkGroupMDVM vm, IFormCollection noUse)
        {
            vm.DoChange();
            return FFResult().CloseDialog().Alert("操作成功");
        }
        #endregion

        [ActionDescription("Export")]
        [HttpPost]
        public IActionResult ExportExcel(FrameworkGroupListVM vm)
        {
            return vm.GetExportData();
        }

    }

}
