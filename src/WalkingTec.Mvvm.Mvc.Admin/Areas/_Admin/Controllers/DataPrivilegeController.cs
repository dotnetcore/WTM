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
    [ActionDescription("数据权限")]
    public class DataPrivilegeController : BaseController
    {
        [ActionDescription("搜索")]
        public ActionResult Index()
        {
            var vm = CreateVM<DataPrivilegeListVM>();
            vm.Searcher.TableNames = ConfigInfo.DataPrivilegeSettings.ToListItems(x => x.PrivillegeName, x => x.ModelName);
            return PartialView(vm);
        }

        [ActionDescription("新建")]
        public ActionResult Create()
        {
            var vm = CreateVM<DataPrivilegeVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("新建")]
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

        [ActionDescription("修改")]
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

        [ActionDescription("修改")]
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

        [ActionDescription("删除")]
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
    }
}
