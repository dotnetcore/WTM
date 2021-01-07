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
    [ActionDescription("MenuKey.MenuMangement")]
    public class FrameworkMenuController : BaseController
    {
        #region 搜索
        [ActionDescription("Sys.Search")]
        public ActionResult Index()
        {
            var vm = Wtm.CreateVM<FrameworkMenuListVM>();
            return PartialView(vm);
        }

        [ActionDescription("Sys.Search")]
        [HttpPost]
        public string Search(FrameworkMenuSearcher searcher)
        {
            var vm = Wtm.CreateVM<FrameworkMenuListVM>(passInit: true);
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

        #region 新建
        [ActionDescription("Sys.Create")]
        public ActionResult Create(Guid? id)
        {
            var vm = Wtm.CreateVM<FrameworkMenuVM>();
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
        [Public]
        [ActionDescription("Sys.Create")]
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
        [ActionDescription("Sys.Edit")]
        public ActionResult Edit(Guid id)
        {
            var vm = Wtm.CreateVM<FrameworkMenuVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("Sys.Edit")]
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
        [ActionDescription("Sys.Delete")]
        public ActionResult Delete(Guid id)
        {
            var vm = Wtm.CreateVM<FrameworkMenuVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("Sys.Delete")]
        [HttpPost]
        public ActionResult Delete(Guid id, IFormCollection noUser)
        {
            var vm = Wtm.CreateVM<FrameworkMenuVM>(id);
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
        [ActionDescription("Sys.Details")]
        public PartialViewResult Details(Guid id)
        {
            var v = Wtm.CreateVM<FrameworkMenuVM>(id);
            return PartialView("Details", v);
        }
        #endregion

        #region 未设置页面
        [ActionDescription("_Admin.UnsetPages")]
        public ActionResult UnsetPages()
        {
            var vm = Wtm.CreateVM<FrameworkActionListVM>();
            return PartialView(vm);
        }
        #endregion

        #region 刷新菜单
        [ActionDescription("_Admin.RefreshMenu")]
        public async Task<ActionResult> RefreshMenu()
        {
            Cache.Delete("FFMenus");
            var userids = DC.Set<FrameworkUserBase>().Select(x => x.ID.ToString().ToLower()).ToArray();
            await Wtm.RemoveUserCache(userids);
            return FFResult().Alert(Localizer["Sys.OprationSuccess"]);
        }
        #endregion

        [ActionDescription("GetActionsByModelId")]
        [AllRights]
        public JsonResult GetActionsByModelId(string Id)
        {
            var modules = Wtm.GlobaInfo.AllModule;
            var m = modules.Where(x => x.FullName == Id).SelectMany(x => x.Actions).Where(x => x.MethodName != "Index" && x.IgnorePrivillege == false).ToList();
            var AllActions = m.ToListItems(y => y.ActionName, y => y.Url);
            AllActions.ForEach(x => x.Selected = true);
            return JsonMore(AllActions);
        }

        /// <summary>
        /// GetIconFontItems
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 3600)]
        [AllRights]
        public IActionResult GetIconFontItems(string id)
        {
            if (!string.IsNullOrEmpty(id) && IconFontsHelper.IconFontDicItems.ContainsKey(id))
                return JsonMore(IconFontsHelper.IconFontDicItems[id]);
            else
                return JsonMore(null);
        }

    }

}
