using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.DataPrivilegeVMs;

namespace WalkingTec.Mvvm.Admin.Api
{
    [ActionDescription("数据权限")]
    [ApiController]
    [Route("api/_DataPrivilege")]
    public class DataPrivilegeController : BaseApiController
    {
        [ActionDescription("搜索")]
        [HttpPost("Search")]
        public string Search(DataPrivilegeSearcher searcher)
        {
            var vm = CreateVM<DataPrivilegeListVM>();
            vm.Searcher = searcher;
            return vm.GetJson();
        }


        [ActionDescription("获取")]
        [HttpGet("Get")]
        public DataPrivilegeVM Get(string TableName, Guid TargetId, DpTypeEnum DpType)
        {
            DataPrivilegeVM vm = null;
            if (DpType == DpTypeEnum.User)
            {
                vm = CreateVM<DataPrivilegeVM>(values: x => x.Entity.TableName == TableName && x.Entity.UserId == TargetId && x.DpType == DpType);
                vm.UserItCode = DC.Set<FrameworkUserBase>().Where(x => x.ID == TargetId).Select(x => x.ITCode).FirstOrDefault();
            }
            else
            {
                vm = CreateVM<DataPrivilegeVM>(values: x => x.Entity.TableName == TableName && x.Entity.GroupId == TargetId && x.DpType == DpType);
            }
            return vm;
        }

        [ActionDescription("新建")]
        [HttpPost("Add")]
        public IActionResult Add(DataPrivilegeVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                vm.DoAdd();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.GetErrorJson());
                }
                else
                {
                    return Ok(vm.Entity);
                }
            }

        }

        [ActionDescription("修改")]
        [HttpPut("Edit")]
        public IActionResult Edit(DataPrivilegeVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                vm.DoEdit(false);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.GetErrorJson());
                }
                else
                {
                    return Ok(vm.Entity);
                }
            }
        }

        [ActionDescription("删除")]
        [HttpGet("Delete/{id}")]
        public IActionResult Delete(Guid id)
        {
            var vm = CreateVM<DataPrivilegeVM>(id);
            vm.DoDelete();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                return Ok(vm.Entity);
            }

        }

        [HttpPost("BatchDelete")]
        [ActionDescription("批量删除")]
        public IActionResult BatchDelete(Guid[] ids)
        {
            var vm = CreateVM<DataPrivilegeBatchVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = ids;
            }
            else
            {
                return Ok();
            }
            if (!ModelState.IsValid || !vm.DoBatchDelete())
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                return Ok(ids.Count());
            }
        }

        [AllRights]
        [HttpGet("GetPrivilegeByTableName")]
        public ActionResult GetPrivilegeByTableName(string table)
        {
            var AllItems = new List<ComboSelectListItem>();
            var dps = ConfigInfo.DataPrivilegeSettings.Where(x => x.ModelName == table).SingleOrDefault();
            if (dps != null)
            {
                AllItems = dps.GetItemList(DC, LoginUserInfo);
            }
            return Ok(AllItems);
        }

        [AllRights]
        [HttpGet("GetPrivileges")]
        public ActionResult GetPrivileges()
        {
            var rv = ConfigInfo.DataPrivilegeSettings.ToListItems(x => x.PrivillegeName, x => x.ModelName);
            return Ok(rv);
        }

        [AllRights]
        [HttpGet("GetUserGroups")]
        public ActionResult GetUserGroups()
        {
            var rv = DC.Set<FrameworkGroup>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, x => x.GroupName);
            return Ok(rv);
        }
    }
}
