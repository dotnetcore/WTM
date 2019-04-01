using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs;

namespace WalkingTec.Mvvm.Mvc.Admin.Controllers
{
    [Area("_Admin")]
    [ActionDescription("菜单管理")]
    public class FrameworkMenuController : BaseController
    {
        #region 搜索
        [ActionDescription("搜索")]
        public ActionResult Index()
        {
            var vm = CreateVM<FrameworkMenuListVM>();
            return PartialView(vm);
        }
        #endregion

        #region 新建
        [ActionDescription("新建")]
        public ActionResult Create(Guid? id)
        {
            var vm = CreateVM<FrameworkMenuVM>();
            if (id != null)
            {
                vm.Entity.ParentId = id;
            }
            vm.Entity.IsInside = true;
            vm.Entity.IsPublic = false;
            vm.Entity.FolderOnly = false;
            vm.Entity.ShowOnMenu = true;
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("新建")]
        public ActionResult Create(FrameworkMenuVM vm)
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
            var vm = CreateVM<FrameworkMenuVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("修改")]
        [HttpPost]
        public ActionResult Edit(FrameworkMenuVM vm)
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
                    return FFResult().CloseDialog().RefreshGrid();
                }
            }
        }
        #endregion

        #region 删除
        [ActionDescription("删除")]
        public ActionResult Delete(Guid id)
        {
            var vm = CreateVM<FrameworkMenuVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("删除")]
        [HttpPost]
        public ActionResult Delete(Guid id, IFormCollection noUser)
        {
            var vm = CreateVM<FrameworkMenuVM>(id);
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
        public PartialViewResult Details(Guid id)
        {
            var v = CreateVM<FrameworkMenuVM>(id);
            return PartialView("Details", v);
        }
        #endregion

        #region 同步模块
        [ActionDescription("同步模块")]
        [FixConnection(DBOperationEnum.Write)]
        public async Task<ActionResult> SyncModel()
        {
            SycModelAndAction();
            var alld = DC.Set<FrameworkDomain>().DPWhere(LoginUserInfo.DataPrivileges, x => x.ID).ToList();
            foreach (var d in alld)
            {
                try
                {
                    await APIHelper.CallAPI(d.Address + "/WebApi/Pub/SyncModel");
                }
                catch { }
            }
            return FFResult().Alert("操作成功");
        }
        #endregion

        #region 未设置页面
        [ActionDescription("检查页面")]
        public ActionResult UnsetPages()
        {
            var vm = CreateVM<FrameworkActionListVM>();
            return PartialView(vm);
        }
        #endregion

        #region 刷新菜单
        [ActionDescription("刷新菜单")]
        public ActionResult RefreshMenu()
        {
            var cache = GlobalServices.GetService<IMemoryCache>();
            cache.Remove("FFMenus");
            return FFResult().Alert("操作成功");
        }
        #endregion

        [ActionDescription("获取动作")]
        public JsonResult GetActionsByModelId(string Id)
        {
            var modules = GlobalServices.GetRequiredService<GlobalData>().AllModule;
            var m = modules.Where(x => x.FullName == Id && x.IsApi == false).SelectMany(x => x.Actions).Where(x => x.MethodName != "Index" && x.IgnorePrivillege == false).ToList();
            var AllActions = m.ToListItems(y => y.ActionName, y => y.Url);
            AllActions.ForEach(x => x.Selected = true);
            return Json(AllActions);
        }

        [ActionDescription("同步模块")]
        protected void SycModelAndAction()
        {
            var allModules = GlobaInfo.AllModule.Where(x => x.IgnorePrivillege == false);
            using (var DC = CreateDC())
            {
                var dbModules = DC.Set<FrameworkModule>().Include(x => x.Actions).ToList();
                var ToRemove = new List<FrameworkModule>();
                var ToAdd = new List<FrameworkModule>();
                var ToRemove2 = new List<FrameworkAction>();
                var ToAdd2 = new List<FrameworkAction>();
                foreach (var oldItem in dbModules)
                {
                    bool exist = false;
                    foreach (var newItem in allModules)
                    {
                        if (oldItem.ClassName == newItem.ClassName && oldItem.NameSpace == newItem.NameSpace)
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (exist == false)
                    {
                        ToRemove.Add(oldItem);
                        ToRemove2.AddRange(oldItem.Actions);
                    }
                }
                foreach (var newItem in allModules)
                {
                    bool exist = false;
                    foreach (var oldItem in dbModules)
                    {
                        if (oldItem.ClassName == newItem.ClassName && oldItem.NameSpace == newItem.NameSpace)
                        {
                            oldItem.ModuleName = newItem.ModuleName;
                            SycActions(newItem.Actions.Where(x => x.IgnorePrivillege == false).ToList(), oldItem.Actions, oldItem, DC);
                            exist = true;
                            break;
                        }
                    }
                    if (exist == false)
                    {
                        ToAdd.Add(newItem);
                        ToAdd2.AddRange(newItem.Actions);
                    }
                }
                foreach (var remove in ToRemove2)
                {
                    DC.Set<FrameworkAction>().Remove(remove);
                }
                foreach (var remove in ToRemove)
                {
                    DC.Set<FrameworkModule>().Remove(remove);
                }
                foreach (var add in ToAdd)
                {
                    DC.Set<FrameworkModule>().Add(add);
                }
                DC.SaveChanges();
            }
        }

        /// <summary>
        /// 同步Actions
        /// </summary>
        /// <param name="newActions"></param>
        /// <param name="oldActions"></param>
        /// <param name="model"></param>
        /// <param name="DC"></param>
        private static void SycActions(List<FrameworkAction> newActions, List<FrameworkAction> oldActions, FrameworkModule model, IDataContext DC)
        {
            var ToRemove = new List<FrameworkAction>();
            var ToAdd = new List<FrameworkAction>();
            foreach (var oldItem in oldActions)
            {
                bool exist = false;
                foreach (var newItem in newActions)
                {
                    if (oldItem.MethodName == newItem.MethodName)
                    {
                        exist = true;
                        break;
                    }
                }
                if (exist == false)
                {
                    ToRemove.Add(oldItem);
                }
            }
            foreach (var newItem in newActions)
            {
                bool exist = false;
                foreach (var oldItem in oldActions)
                {
                    if (oldItem.MethodName == newItem.MethodName)
                    {
                        oldItem.ActionName = newItem.ActionName;
                        exist = true;
                        break;
                    }
                }
                if (exist == false)
                {
                    ToAdd.Add(newItem);
                }
            }
            foreach (var remove in ToRemove)
            {
                DC.Set<FrameworkAction>().Remove(remove);
            }
            foreach (var add in ToAdd)
            {
                add.ModuleId = model.ID;
                FrameworkAction act = new FrameworkAction();
                act.ModuleId = model.ID;
                act.MethodName = add.MethodName;
                act.Parameter = add.Parameter;
                act.ParasToRunTest = add.ParasToRunTest;
                act.ActionName = add.ActionName;
                DC.Set<FrameworkAction>().Add(act);
            }
        }

    }

}
