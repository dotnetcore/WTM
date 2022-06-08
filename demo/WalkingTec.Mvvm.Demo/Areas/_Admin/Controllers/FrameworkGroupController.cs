// WTM默认页面 Wtm buidin page
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
    [ActionDescription("MenuKey.GroupManagement")]
    public class FrameworkGroupController : BaseController
    {
        #region 查询
        [ActionDescription("Sys.Search")]
        public ActionResult Index()
        {
            var vm = Wtm.CreateVM<FrameworkGroupListVM>();
            return PartialView(vm);
        }

        [ActionDescription("Sys.Search")]
        [HttpPost]
        public IActionResult Search(FrameworkGroupSearcher searcher)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                searcher.IsPlainText = false;
                return Wtm.CallAPI<string>("mainhost", "/api/_frameworkgroup/search", HttpMethodEnum.POST, searcher).Result.ToActionResult();
            }
            var vm = Wtm.CreateVM<FrameworkGroupListVM>(passInit: true);
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

        [ActionDescription("Sys.Create")]
        public ActionResult Create()
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkGroupVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Create")]
        public ActionResult Create(FrameworkGroupVM vm)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
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

        [ActionDescription("Sys.Edit")]
        public ActionResult Edit(Guid id)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkGroupVM>(id);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Edit")]
        [ValidateFormItemOnly]
        public ActionResult Edit(FrameworkGroupVM vm)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
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

        [ActionDescription("Sys.Delete")]
        public ActionResult Delete(Guid id)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkGroupVM>(id);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Delete")]
        public async Task<ActionResult> Delete(Guid id, IFormCollection noUse)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkGroupVM>(id);
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

        [HttpPost]
        [ActionDescription("Sys.BatchDelete")]
        public ActionResult BatchDelete(Guid[] ids)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkGroupBatchVM>(Ids: ids);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchDelete")]
        public async Task<ActionResult> DoBatchDelete(FrameworkGroupBatchVM vm, IFormCollection noUse)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            List<string> GroupCode = DC.Set<FrameworkGroup>().CheckIDs(new List<string>(vm.Ids)).Select(x => x.GroupCode).ToList();
            if (!ModelState.IsValid || !vm.DoBatchDelete())
            {                
                return PartialView("BatchDelete", vm);
            }
            else
            {
                var gr = DC.Set<FrameworkUserGroup>().Where(x => GroupCode.Contains(x.GroupCode)).ToList();
                var itcodes = gr.Select(x => x.UserCode).ToArray();
                DC.Set<FrameworkUserGroup>().RemoveRange(gr);
                DC.SaveChanges();
                await Wtm.RemoveUserCacheByGroup(GroupCode.ToArray());
                await Wtm.RemoveGroupCache(Wtm.LoginUserInfo?.CurrentTenant);
                return FFResult().CloseDialog().RefreshGrid().Alert(Localizer["Sys.OprationSuccess"]);
            }
        }

        [ActionDescription("Sys.Import")]
        public ActionResult Import()
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkGroupImportVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Import")]
        public ActionResult Import(FrameworkGroupImportVM vm, IFormCollection nouse)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            if (vm.ErrorListVM.EntityList.Count > 0 || !vm.BatchSaveData())
            {
                return PartialView(vm);
            }
            else
            {
                Wtm.RemoveGroupCache(Wtm.LoginUserInfo.CurrentTenant).Wait();
                return FFResult().CloseDialog().RefreshGrid().Alert(Localizer["Sys.ImportSuccess", vm.EntityList.Count.ToString()]);
            }
        }

        [ActionDescription("_Admin.DataPrivilege")]
        public PartialViewResult DataFunction(string groupcode)
        {
            var role = Wtm.CreateVM<FrameworkGroupMDVM>(values: x=>x.GroupCode == groupcode);
            return PartialView(role);
        }

        [ActionDescription("_Admin.DataPrivilege")]
        [HttpPost]
        public ActionResult DataFunction(FrameworkGroupMDVM vm, IFormCollection noUse)
        {
            vm.DoChange();
            return FFResult().CloseDialog().Alert(Localizer["Sys.OprationSuccess"]);
        }

        [ActionDescription("Sys.Export")]
        [HttpPost]
        public IActionResult ExportExcel(FrameworkGroupListVM vm)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            return vm.GetExportData();
        }

        [AllRights]
        public IActionResult GetParents()
        {
            WalkingTec.Mvvm.Admin.Api.FrameworkGroupController userapi = new Mvvm.Admin.Api.FrameworkGroupController();
            userapi.Wtm = Wtm;
            var rv = userapi.GetParentsTree() as OkObjectResult;
            List<TreeSelectListItem> users = new List<TreeSelectListItem>();
            if (rv != null && rv.Value is string && rv.Value != null)
            {
                users = System.Text.Json.JsonSerializer.Deserialize<List<TreeSelectListItem>>(rv.Value.ToString());
            }
            else if (rv != null && rv.Value is List<TreeSelectListItem> c)
            {
                users = c;
            }
            return JsonMore(users);
        }

    }

}
