// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ActionDescription("MenuKey.UserManagement")]
    [ApiController]
    [Route("api/_FrameworkUserBase")]
    public class FrameworkUserController : BaseApiController
    {
        [ActionDescription("Sys.Search")]
        [HttpPost("[action]")]
        public string Search(FrameworkUserSearcher searcher)
        {
            var vm = Wtm.CreateVM<FrameworkUserListVM>();
            vm.Searcher = searcher;
            return vm.GetJson();
        }

        [ActionDescription("Sys.Get")]
        [HttpGet("{id}")]
        public FrameworkUserVM Get(Guid id)
        {
            var vm = Wtm.CreateVM<FrameworkUserVM>(id);
            return vm;
        }

        [ActionDescription("Sys.Create")]
        [HttpPost("[action]")]
        public async Task<IActionResult> Add(FrameworkUserVM vm)
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
        public async Task<IActionResult> Edit(FrameworkUserVM vm)
        {
            ModelState.Remove("Entity.Password");
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

        [ActionDescription("Login.ChangePassword")]
        [HttpPut("[action]")]
        public async Task<IActionResult> Password(FrameworkUserVM vm)
        {
            var keys = ModelState.Keys.ToList();
            foreach (var item in keys)
            {
                if (item != "Entity.Password")
                {
                    ModelState.Remove(item);
                }
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                await vm.ChangePassword();
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


        [HttpPost("BatchDelete")]
        [ActionDescription("Sys.Delete")]
        public async Task<IActionResult> BatchDelete(string[] ids)
        {
            var vm = Wtm.CreateVM<FrameworkUserBatchVM>();
            List<string> itcode = new List<string>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = ids;
                itcode = DC.Set<FrameworkUser>().CheckIDs(new List<string>(ids)).Select(x => x.ITCode).ToList();
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
                return Ok(ids.Count());
            }
        }

        [ActionDescription("Sys.Export")]
        [HttpPost("[action]")]
        public IActionResult ExportExcel(FrameworkUserSearcher searcher)
        {
            var vm = Wtm.CreateVM<FrameworkUserListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return vm.GetExportData();
        }

        [ActionDescription("Sys.ExportByIds")]
        [HttpPost("[action]")]
        public IActionResult ExportExcelByIds(string[] ids)
        {
            var vm = Wtm.CreateVM<FrameworkUserListVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            return vm.GetExportData();
        }

        [ActionDescription("Sys.DownloadTemplate")]
        [HttpGet("[action]")]
        public IActionResult GetExcelTemplate()
        {
            var vm = Wtm.CreateVM<FrameworkUserImportVM>();
            var qs = new Dictionary<string, string>();
            if (Request != null)
            {
                foreach (var item in Request.Query.Keys)
                {
                    qs.Add(item, Request.Query[item]);
                }
            }
            vm.SetParms(qs);
            var data = vm.GenerateTemplate(out string fileName);
            return File(data, "application/vnd.ms-excel", fileName);
        }

        [ActionDescription("Sys.Import")]
        [HttpPost("[action]")]
        public ActionResult Import(FrameworkUserImportVM vm)
        {
            if (vm != null && (vm.ErrorListVM.EntityList.Count > 0 || !vm.BatchSaveData()))
            {
                return BadRequest(vm.GetErrorJson());
            }
            else
            {
                return Ok(vm?.EntityList?.Count ?? 0);
            }
        }

        [HttpGet("GetFrameworkRoles")]
        [ActionDescription("GetRoles")]
        [AllRights]
        public ActionResult GetFrameworkRoles()
        {
            return Ok(DC.Set<FrameworkRole>().GetSelectListItems(Wtm, x => x.RoleName, x=>x.RoleCode));
        }

        [HttpGet("GetFrameworkGroups")]
        [ActionDescription("GetGroups")]
        [AllRights]
        public ActionResult GetFrameworkGroups()
        {
            return Ok(DC.Set<FrameworkGroup>().GetSelectListItems(Wtm,  x => x.GroupName, x=>x.GroupCode));
        }

    }
}
