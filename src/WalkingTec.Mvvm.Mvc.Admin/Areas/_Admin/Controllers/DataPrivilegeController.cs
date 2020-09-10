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
    [ActionDescription("DataPrivilege")]
    public class DataPrivilegeController : BaseController
    {
        [ActionDescription("Search")]
        public ActionResult Index()
        {
            var vm = CreateVM<DataPrivilegeListVM>();
            vm.Searcher.TableNames = ConfigInfo.DataPrivilegeSettings.ToListItems(x => x.PrivillegeName, x => x.ModelName);
            return PartialView(vm);
        }

        [HttpPost]
        public ActionResult Index(DataPrivilegeListVM vm)
        {
            vm.Searcher.TableNames = ConfigInfo.DataPrivilegeSettings.ToListItems(x => x.PrivillegeName, x => x.ModelName);
            return PartialView(vm);
        }


        [ActionDescription("Search")]
        [HttpPost]
        public string Search(DataPrivilegeListVM vm)
        {
            return vm.GetJson(false);
        }

        [ActionDescription("Create")]
        public ActionResult Create(DpTypeEnum Type)
        {
            var vm = CreateVM<DataPrivilegeVM>(values:x=>x.DpType == Type);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Create")]
        public async Task<ActionResult> Create(DataPrivilegeVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                await vm.DoAddAsync();
                return FFResult().CloseDialog().RefreshGrid();
            }
        }

        [ActionDescription("Edit")]
        public ActionResult Edit(string ModelName, Guid Id, DpTypeEnum Type)
        {
            DataPrivilegeVM vm = null;
            if (Type == DpTypeEnum.User)
            {
                vm = CreateVM<DataPrivilegeVM>(values: x => x.Entity.TableName == ModelName && x.Entity.UserId == Id && x.DpType == Type);
                vm.UserItCode = DC.Set<FrameworkUserBase>().Where(x => x.ID == Id).Select(x => x.ITCode).FirstOrDefault();
            }
            else
            {
                vm = CreateVM<DataPrivilegeVM>(values: x => x.Entity.TableName == ModelName && x.Entity.GroupId == Id && x.DpType == Type);
            }
            return PartialView(vm);
        }

        [ActionDescription("Edit")]
        [HttpPost]
        public async Task<ActionResult> Edit(DataPrivilegeVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                await vm.DoEditAsync();
                return FFResult().CloseDialog().RefreshGrid();
            }
        }

        [ActionDescription("Delete")]
        public async Task<ActionResult> Delete(string ModelName, Guid Id, DpTypeEnum Type)
        {
            DataPrivilegeVM vm = null;
            if (Type == DpTypeEnum.User)
            {
                vm = CreateVM<DataPrivilegeVM>(values: x => x.Entity.TableName == ModelName && x.Entity.UserId == Id && x.DpType == Type);
            }
            else
            {
                vm = CreateVM<DataPrivilegeVM>(values: x => x.Entity.TableName == ModelName && x.Entity.GroupId == Id && x.DpType == Type);
            }
            await vm.DoDeleteAsync();
            return FFResult().RefreshGrid();
        }

        [AllRights]
        public ActionResult GetPrivilegeByTableName(string table)
        {
            var AllItems = new List<ComboSelectListItem>();
            var dps = ConfigInfo.DataPrivilegeSettings.Where(x => x.ModelName == table).SingleOrDefault();
            if (dps != null)
            {
                AllItems = dps.GetItemList(DC, LoginUserInfo);
            }
            return Json(AllItems);
        }

        [ActionDescription("Export")]
        [HttpPost]
        public IActionResult ExportExcel(DataPrivilegeListVM vm)
        {
            return vm.GetExportData();
        }
    }
}
