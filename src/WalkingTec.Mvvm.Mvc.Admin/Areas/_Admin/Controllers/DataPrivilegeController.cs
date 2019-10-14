using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.DataPrivilegeVMs;
using WalkingTec.Mvvm.Core.Extensions;

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

        [ActionDescription("Search")]
        [HttpPost]
        public string Search(DataPrivilegeListVM vm)
        {
            return vm.GetJson(false);
        }

        [ActionDescription("Create")]
        public ActionResult Create()
        {
            var vm = CreateVM<DataPrivilegeVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("Create")]
        public ActionResult Create(DataPrivilegeVM vm)
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
        public ActionResult Edit(DataPrivilegeVM vm)
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

        [ActionDescription("Delete")]
        public ActionResult Delete(string ModelName, Guid Id, DpTypeEnum Type)
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
            vm.DoDelete();
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
            vm.SearcherMode = vm.Ids != null && vm.Ids.Count > 0 ? ListVMSearchModeEnum.CheckExport : ListVMSearchModeEnum.Export;
            var data = vm.GenerateExcel();
            return File(data, "application/vnd.ms-excel", $"Export_DataPrivilege_{DateTime.Now.ToString("yyyy-MM-dd")}.xls");
        }
    }
}
