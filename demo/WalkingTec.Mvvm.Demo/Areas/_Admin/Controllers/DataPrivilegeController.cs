// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.DataPrivilegeVMs;
using WalkingTec.Mvvm.Core.Extensions;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Mvc.Admin.Controllers
{
    [Area("_Admin")]
    [ActionDescription("MenuKey.DataPrivilege")]
    public class DataPrivilegeController : BaseController
    {
        [ActionDescription("Sys.Search")]
        public IActionResult Index ()
        {
            var vm = Wtm.CreateVM<DataPrivilegeListVM>();
            vm.Searcher.TableNames = Wtm.DataPrivilegeSettings.ToListItems(x => x.PrivillegeName, x => x.ModelName);
            return PartialView(vm);
        }

        [HttpPost]
        public IActionResult Index (DataPrivilegeListVM vm)
        {
            vm.Searcher.TableNames = Wtm.DataPrivilegeSettings.ToListItems(x => x.PrivillegeName, x => x.ModelName);
            return PartialView(vm);
        }


        [ActionDescription("Sys.Search")]
        [HttpPost]
        public string Search(DataPrivilegeSearcher searcher)
        {
            var vm = Wtm.CreateVM<DataPrivilegeListVM>(passInit: true);
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
        public IActionResult Create (DpTypeEnum Type)
        {
            var vm = Wtm.CreateVM<DataPrivilegeVM>(values:x=>x.DpType == Type);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Sys.Create")]
        public async Task<ActionResult> Create(DataPrivilegeVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                await vm.DoAdd();
                return FFResult().CloseDialog().RefreshGrid();
            }
        }

        [ActionDescription("Sys.Edit")]
        public IActionResult Edit (string ModelName, string Id, DpTypeEnum Type)
        {
            DataPrivilegeVM vm = null;
            if (Type == DpTypeEnum.User)
            {
                vm = Wtm.CreateVM<DataPrivilegeVM>(values: x => x.Entity.TableName == ModelName && x.Entity.UserCode == Id && x.DpType == Type);
            }
            else
            {
                vm = Wtm.CreateVM<DataPrivilegeVM>(values: x => x.Entity.TableName == ModelName && x.Entity.GroupCode == Id && x.DpType == Type);
            }
            return PartialView(vm);
        }

        [ActionDescription("Sys.Edit")]
        [HttpPost]
        public async Task<ActionResult> Edit(DataPrivilegeVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                await vm.DoEdit();
                return FFResult().CloseDialog().RefreshGrid();
            }
        }

        [ActionDescription("Sys.Delete")]
        public async Task<ActionResult> Delete(string ModelName, string Id, DpTypeEnum Type)
        {
            DataPrivilegeVM vm = null;
            if (Type == DpTypeEnum.User)
            {
                vm = Wtm.CreateVM<DataPrivilegeVM>(values: x => x.Entity.TableName == ModelName && x.Entity.UserCode == Id && x.DpType == Type);
            }
            else
            {
                vm = Wtm.CreateVM<DataPrivilegeVM>(values: x => x.Entity.TableName == ModelName && x.Entity.GroupCode == Id && x.DpType == Type);
            }
            await vm.DoDelete();
            return FFResult().RefreshGrid();
        }

        [AllRights]
        public async Task<IActionResult> GetPrivilegeByTableName (string table)
        {
            var AllItems = new List<ComboSelectListItem>();
            var dps = Wtm.DataPrivilegeSettings.Where(x => x.ModelName == table).SingleOrDefault();
            if (dps != null)
            {
                AllItems = await dps.GetItemList(Wtm);
            }
            return JsonMore(AllItems);
        }

        [ActionDescription("Sys.Export")]
        [HttpPost]
        public async Task<IActionResult> ExportExcel(DataPrivilegeListVM vm)
        {
            return await vm.GetExportData();
        }
        [AllRights]
        public async Task<IActionResult> GetUserGroups ()
        {
            WalkingTec.Mvvm.Admin.Api.DataPrivilegeController userapi = new Mvvm.Admin.Api.DataPrivilegeController();
            userapi.Wtm = Wtm;
            var rv = await userapi.GetUserGroupsTree() as OkObjectResult;
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
