// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.DataPrivilegeVMs;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ActionDescription("_Admin.DataPrivilegeApi")]
    [ApiController]
    [Route("api/_[controller]")]
    public class DataPrivilegeController : BaseApiController
    {
        [ActionDescription("Sys.Search")]
        [HttpPost("[action]")]
        public string Search(DataPrivilegeSearcher searcher)
        {
            var vm = Wtm.CreateVM<DataPrivilegeListVM>();
            vm.Searcher = searcher;
            return vm.GetJson();
        }

        [ActionDescription("Sys.Get")]
        [HttpGet("[action]")]
        public DataPrivilegeVM Get(string TableName, string TargetId, DpTypeEnum DpType)
        {
            DataPrivilegeVM vm = null;
            if (DpType == DpTypeEnum.User)
            {
                vm = Wtm.CreateVM<DataPrivilegeVM>(values: x => x.Entity.TableName == TableName && x.Entity.UserCode == TargetId && x.DpType == DpType);
            }
            else
            {
                vm = Wtm.CreateVM<DataPrivilegeVM>(values: x => x.Entity.TableName == TableName && x.Entity.GroupCode == TargetId && x.DpType == DpType);
            }
            return vm;
        }

        [ActionDescription("Sys.Create")]
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

        [ActionDescription("Sys.Edit")]
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
        [ActionDescription("Sys.Delete")]
        public async Task<ActionResult> Delete(SimpleDpModel dp)
        {
            DataPrivilegeVM vm = null;
            if (dp.Type == DpTypeEnum.User)
            {
                vm = Wtm.CreateVM<DataPrivilegeVM>(values: x => x.Entity.TableName == dp.ModelName && x.Entity.UserCode == dp.Id && x.DpType == dp.Type);
            }
            else
            {
                vm = Wtm.CreateVM<DataPrivilegeVM>(values: x => x.Entity.TableName == dp.ModelName && x.Entity.GroupCode == dp.Id && x.DpType == dp.Type);
            }
            await vm.DoDeleteAsync();
            return Ok(1);
        }



        [AllRights]
        [HttpGet("[action]")]
        public ActionResult GetPrivilegeByTableName(string table)
        {
            var AllItems = new List<ComboSelectListItem>();
            var dps =Wtm.DataPrivilegeSettings.Where(x => x.ModelName == table).SingleOrDefault();
            if (dps != null)
            {
                AllItems = dps.GetItemList(Wtm);
            }
            return Ok(AllItems);
        }

        [AllRights]
        [HttpGet("[action]")]
        public ActionResult GetPrivileges()
        {
            var rv = Wtm.DataPrivilegeSettings.ToListItems(x => x.PrivillegeName, x => x.ModelName);
            return Ok(rv);
        }

        [AllRights]
        [HttpGet("[action]")]
        public ActionResult GetUserGroups()
        {
            var rv = DC.Set<FrameworkGroup>().GetSelectListItems(Wtm, x => x.GroupName);
            return Ok(rv);
        }
    }

    public class SimpleDpModel
    {
        public string ModelName { get; set; }
        public string Id { get; set; }
        public DpTypeEnum Type { get; set; }
    }
}
