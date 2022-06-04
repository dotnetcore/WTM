// WTM默认页面 Wtm buidin page
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;
using WalkingTec.Mvvm.Core.Extensions;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace WalkingTec.Mvvm.Mvc.Admin.Controllers
{
    [Area("_Admin")]
    [ActionDescription("MenuKey.UserManagement")]
    public class FrameworkUserController : BaseController
    {
        [ActionDescription("Sys.Search", IsPage = true)]
        public ActionResult Index()
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkUserListVM>();
           // vm.Searcher.IsValid = true;
            return PartialView(vm);
        }

        [ActionDescription("Sys.Search")]
        [HttpPost]
        public string Search(FrameworkUserSearcher searcher)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Localizer["_Admin.HasMainHost"];
            }
            var vm = Wtm.CreateVM<FrameworkUserListVM>(passInit: true);
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


        [ActionDescription("Sys.Create")]
        public ActionResult Create()
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkUserVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Create")]
        public async Task<ActionResult> Create(FrameworkUserVM vm)
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

        [ActionDescription("Sys.Edit")]
        public ActionResult Edit(string id)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkUserVM>(id);
            vm.Entity.Password = null;
            return PartialView(vm);
        }

        [ActionDescription("Sys.Edit")]
        [HttpPost]
        [ValidateFormItemOnly]
        public async Task<ActionResult> Edit(FrameworkUserVM vm)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
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

        [ActionDescription("Login.ChangePassword")]
        public ActionResult Password(Guid id)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkUserVM>(id, passInit: true);
            vm.Entity.Password = null;
            return PartialView(vm);
        }

        [ActionDescription("Login.ChangePassword")]
        [HttpPost]
        public ActionResult Password(FrameworkUserVM vm)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var keys = ModelState.Keys.ToList();
            foreach (var item in keys)
            {
                if (item != "Entity.Password")
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

        [ActionDescription("Sys.Delete")]
        public ActionResult Delete(Guid id)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkUserVM>(id);
            return PartialView(vm);
        }

        [ActionDescription("Sys.Delete")]
        [HttpPost]
        public async Task<ActionResult> Delete(Guid id, IFormCollection nouse)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkUserVM>(id);
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

        [ActionDescription("Sys.Details")]
        public PartialViewResult Details(Guid id)
        {
            var v = Wtm.CreateVM<FrameworkUserVM>(id);
            return PartialView("Details", v);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchEdit")]
        public ActionResult BatchEdit(string[] IDs)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkUserBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchEdit")]
        public ActionResult DoBatchEdit(FrameworkUserBatchVM vm, IFormCollection nouse)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            if (!ModelState.IsValid || !vm.DoBatchEdit())
            {
                return PartialView("BatchEdit", vm);
            }
            else
            {
                return FFResult().CloseDialog().RefreshGrid().Alert(Localizer["Sys.BatchEditSuccess", vm.Ids.Length]);
            }
        }

        [HttpPost]
        [ActionDescription("Sys.BatchDelete")]
        public ActionResult BatchDelete(string[] IDs)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkUserBatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.BatchDelete")]
        public async Task<ActionResult> DoBatchDelete(FrameworkUserBatchVM vm, IFormCollection nouse)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            List<string> itcode = new List<string>();
            itcode = DC.Set<FrameworkUser>().CheckIDs(new List<string>(vm.Ids)).Select(x => x.ITCode).ToList();
            if (!ModelState.IsValid || !vm.DoBatchDelete())
            {
                return PartialView("BatchDelete", vm);
            }
            else
            {
                using (var tran = DC.BeginTransaction())
                {
                    try
                    {
                        var ur = DC.Set<FrameworkUserRole>().Where(x => itcode.Contains(x.UserCode));
                        DC.Set<FrameworkUserRole>().RemoveRange(ur);
                        var ug = DC.Set<FrameworkUserGroup>().Where(x => itcode.Contains(x.UserCode));
                        DC.Set<FrameworkUserGroup>().RemoveRange(ug);
                        DC.SaveChanges();
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }

                await Wtm.RemoveUserCache(itcode.ToArray());
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
            var vm = Wtm.CreateVM<FrameworkUserImportVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Import")]
        public ActionResult Import(FrameworkUserImportVM vm, IFormCollection nouse)
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
                return FFResult().CloseDialog().RefreshGrid().Alert(Localizer["Sys.ImportSuccess", vm.EntityList.Count.ToString()]);
            }
        }

        [ActionDescription("Sys.Enable")]
        public ActionResult Enable(Guid id, bool enable)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            FrameworkUser user = new FrameworkUser { ID = id };
            user.IsValid = enable;
            DC.UpdateProperty(user, x => x.IsValid);
            DC.SaveChanges();
            return FFResult().RefreshGrid(CurrentWindowId);
        }

        [AllRights]
        public ActionResult GetUserById(string keywords)
        {
            WalkingTec.Mvvm.Admin.Api.FrameworkUserController userapi = new Mvvm.Admin.Api.FrameworkUserController();
            userapi.Wtm = Wtm;
            var rv = userapi.GetUserById(keywords) as OkObjectResult;
            List<ComboSelectListItem> users = new List<ComboSelectListItem>();
            if (rv != null && rv.Value is string && rv.Value != null)
            {
                users = System.Text.Json.JsonSerializer.Deserialize<List<ComboSelectListItem>>(rv.Value.ToString());
            }
            else if (rv != null && rv.Value is List<ComboSelectListItem> c)
            {
                users = c;
            }
            return JsonMore(users);

        }

        [ActionDescription("Sys.Export")]
        [HttpPost]
        public IActionResult ExportExcel(FrameworkUserListVM vm)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            return vm.GetExportData();
        }

        [AllRights]
        public IActionResult GetFrameworkRoles()
        {
            WalkingTec.Mvvm.Admin.Api.FrameworkUserController userapi = new Mvvm.Admin.Api.FrameworkUserController();
            userapi.Wtm = Wtm;
            var rv = userapi.GetFrameworkRoles() as OkObjectResult;
            List<ComboSelectListItem> users = new List<ComboSelectListItem>();
            if (rv != null && rv.Value is string && rv.Value != null)
            {
                users = System.Text.Json.JsonSerializer.Deserialize<List<ComboSelectListItem>>(rv.Value.ToString());
            }
            else if (rv != null && rv.Value is List<ComboSelectListItem> c)
            {
                users = c;
            }
            return JsonMore(users);
        }

        [AllRights]
        public IActionResult GetFrameworkGroups()
        {
            WalkingTec.Mvvm.Admin.Api.FrameworkUserController userapi = new Mvvm.Admin.Api.FrameworkUserController();
            userapi.Wtm = Wtm;
            var rv = userapi.GetFrameworkGroupsTree() as OkObjectResult;
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
