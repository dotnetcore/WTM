using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;
using WalkingTec.Mvvm.Core.Extensions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WalkingTec.Mvvm.Mvc.Admin.Controllers
{
    [Area("_Admin")]
    [ActionDescription("UserManagement")]
    public class FrameworkUserController : BaseController
    {
        #region 搜索
        [ActionDescription("Search")]
        public ActionResult Index()
        {
            var vm = CreateVM<FrameworkUserListVM>();
            vm.Searcher.IsValid = true;
            return PartialView(vm);
        }

        [ActionDescription("Search")]
        [HttpPost]
        public string Search(FrameworkUserListVM vm)
        {
            return vm.GetJson(false);
        }

        #endregion

        #region 新建
        [ActionDescription("Create")]
        public ActionResult Create()
        {
            var vm = CreateVM<FrameworkUserVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Create")]
        public async Task<ActionResult> Create(FrameworkUserVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                await vm.DoAddAsync();
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
        [ActionDescription("Edit")]
        public ActionResult Edit(string id)
        {
            var vm = CreateVM<FrameworkUserVM>(id);
            vm.Entity.Password = null;
            return PartialView(vm);
        }

        [ActionDescription("Edit")]
        [HttpPost]
        [ValidateFormItemOnly]
        public async Task<ActionResult> Edit(FrameworkUserVM vm)
        {
            if (ModelState.Any(x => x.Key != "Entity.Password" && x.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid))
            {
                return PartialView(vm);
            }
            else
            {
                ModelState.Clear();
                await vm.DoEditAsync();
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

        #region 修改密码
        [ActionDescription("ChangePassword")]
        public ActionResult Password(Guid id)
        {
            var vm = CreateVM<FrameworkUserVM>(id,passInit:true);
            vm.Entity.Password = null;
            return PartialView(vm);
        }

        [ActionDescription("ChangePassword")]
        [HttpPost]
        public ActionResult Password(FrameworkUserVM vm)
        {
            var keys = ModelState.Keys.ToList();
            foreach (var item in keys)
            {
                if(item != "Entity.Password")
                {
                    ModelState.Remove(item);
                }
            }
            if (ModelState.IsValid == false)
            {
                return PartialView(vm);
            }
            else
            {
                vm.ChangePassword();
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
        [ActionDescription("Delete")]
        public ActionResult Delete(Guid id)
        {
            var vm = CreateVM<FrameworkUserVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("Delete")]
        [HttpPost]
        public async Task<ActionResult> Delete(Guid id, IFormCollection nouse)
        {
            var vm = CreateVM<FrameworkUserVM>(id);
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
        #endregion

        #region 详细
        [ActionDescription("Details")]
        public PartialViewResult Details(Guid id)
        {
            var v = CreateVM<FrameworkUserVM>(id);
            return PartialView("Details", v);
        }
        #endregion

        #region 批量删除
        [HttpPost]
        [ActionDescription("BatchDelete")]
        public ActionResult BatchDelete(string[] IDs)
        {
            var vm = CreateVM<FrameworkUserBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("BatchDelete")]
        public async Task<ActionResult> DoBatchDelete(FrameworkUserBatchVM vm, IFormCollection nouse)
        {
            if (!ModelState.IsValid || !vm.DoBatchDelete())
            {
                return PartialView("BatchDelete", vm);
            }
            else
            {
                List<Guid?> tempids = new List<Guid?>();
                foreach (var item in vm?.Ids)
                {
                    tempids.Add(Guid.Parse(item));
                }
                var userids = DC.Set<FrameworkUserBase>().Where(x => tempids.Contains(x.ID)).Select(x => x.ID.ToString()).ToArray();
                await LoginUserInfo.RemoveUserCache(userids);
                return FFResult().CloseDialog().RefreshGrid().Alert(Program._localizer?["OprationSuccess"]);
            }
        }
        #endregion

        #region 导入
        [ActionDescription("Import")]
        public ActionResult Import()
        {
            var vm = CreateVM<FrameworkUserImportVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Import")]
        public ActionResult Import(FrameworkUserImportVM vm, IFormCollection nouse)
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

        [ActionDescription("Enable")]
        public ActionResult Enable(Guid id, bool enable)
        {
            FrameworkUserBase user = new FrameworkUserBase { ID = id };
            user.IsValid = enable;
            DC.UpdateProperty(user, x => x.IsValid);
            DC.SaveChanges();
            return FFResult().RefreshGrid(CurrentWindowId);
        }

        [AllRights]
        public ActionResult GetUserById(string keywords)
        {
            var users = DC.Set<FrameworkUserBase>().Where(x => x.ITCode.ToLower().StartsWith(keywords.ToLower())).GetSelectListItems(LoginUserInfo.DataPrivileges,null, x=>x.CodeAndName, x => x.ITCode);
            return Json(users);

        }

        [ActionDescription("Export")]
        [HttpPost]
        public IActionResult ExportExcel(FrameworkUserListVM vm)
        {
            return vm.GetExportData();
        }
    }
}
