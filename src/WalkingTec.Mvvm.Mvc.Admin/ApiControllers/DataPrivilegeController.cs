using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth.Attribute;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.DataPrivilegeVMs;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ActionDescription("DataPrivilege")]
    [ApiController]
    [Route("api/_[controller]")]
    public class DataPrivilegeController : BaseApiController
    {
        [ActionDescription("Search")]
        [HttpPost("[action]")]
        public string Search(DataPrivilegeSearcher searcher)
        {
            var vm = CreateVM<DataPrivilegeListVM>();
            vm.Searcher = searcher;
            return vm.GetJson();
        }

        [ActionDescription("Get")]
        [HttpGet("[action]")]
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

        [ActionDescription("Create")]
        [HttpPost("[action]")]
        public async Task<IActionResult> Add(DataPrivilegeVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                await vm.DoAddAsync();
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

        [ActionDescription("Edit")]
        [HttpPut("[action]")]
        public async Task<IActionResult> Edit(DataPrivilegeVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                await vm.DoEditAsync(false);
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


        [HttpPost("[action]")]
        [ActionDescription("Delete")]
        public async Task<ActionResult> Delete(SimpleDpModel dp)
        {
            DataPrivilegeVM vm = null;
            if (dp.Type == DpTypeEnum.User)
            {
                vm = CreateVM<DataPrivilegeVM>(values: x => x.Entity.TableName == dp.ModelName && x.Entity.UserId == dp.Id && x.DpType == dp.Type);
            }
            else
            {
                vm = CreateVM<DataPrivilegeVM>(values: x => x.Entity.TableName == dp.ModelName && x.Entity.GroupId == dp.Id && x.DpType == dp.Type);
            }
            await vm.DoDeleteAsync();
            return Ok(1);
        }



        [AllRights]
        [HttpGet("[action]")]
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
        [HttpGet("[action]")]
        public ActionResult GetPrivileges()
        {
            var rv = ConfigInfo.DataPrivilegeSettings.ToListItems(x => x.PrivillegeName, x => x.ModelName);
            return Ok(rv);
        }

        [AllRights]
        [HttpGet("[action]")]
        public ActionResult GetUserGroups()
        {
            var rv = DC.Set<FrameworkGroup>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, x => x.GroupName);
            return Ok(rv);
        }
    }

    public class SimpleDpModel
    {
        public string ModelName { get; set; }
        public Guid Id { get; set; }
        public DpTypeEnum Type { get; set; }
    }
}
