// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ActionDescription("MenuKey.UserManagement")]
    [ApiController]
    [Route("api/_FrameworkUser")]
    public class FrameworkUserController : BaseApiController
    {
        [ActionDescription("Sys.Search")]
        [HttpPost("[action]")]
        public async Task<IActionResult> Search(FrameworkUserSearcher searcher)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Request.RedirectCall(Wtm).Result;
            }
            if (ModelState.IsValid)
            {
                var vm = Wtm.CreateVM<FrameworkUserListVM>(passInit: true);
                vm.Searcher = searcher;
                return Content(vm.GetJson());
            }
            else
            {
                return BadRequest(ModelState.GetErrorJson());
            }
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
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                await vm.DoAdd();
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
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            ModelState.Remove("Entity.Password");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                await vm.DoEdit(false);
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
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkUserBatchVM>();
            List<string> itcode = new List<string>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = ids;
                itcode = await DC.Set<FrameworkUser>().CheckIDs(new List<string>(ids)).Select(x => x.ITCode).ToListAsync();
            }
            else
            {
                return Ok();
            }
            if (!ModelState.IsValid || !await vm.DoBatchDelete())
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
                        await DC.SaveChangesAsync();
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
        public async Task<IActionResult> ExportExcel(FrameworkUserSearcher searcher)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkUserListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return await vm.GetExportData();
        }

        [ActionDescription("Sys.ExportByIds")]
        [HttpPost("[action]")]
        public async Task<IActionResult> ExportExcelByIds(string[] ids)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            var vm = Wtm.CreateVM<FrameworkUserListVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            return await vm.GetExportData();
        }

        [ActionDescription("Sys.DownloadTemplate")]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetExcelTemplate()
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
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
        public async Task<ActionResult> Import (FrameworkUserImportVM vm)
        {

            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Content(Localizer["_Admin.HasMainHost"]);
            }
            if (vm.ErrorListVM.EntityList.Count > 0 || !await vm.BatchSaveData())
            {
                return BadRequest(vm.GetErrorJson());
            }
            else
            {
                return Ok(vm.EntityList.Count);
            }
        }

        [HttpGet("GetFrameworkRoles")]
        [ActionDescription("GetRoles")]
        [AllRights]
        public async Task<IActionResult> GetFrameworkRoles ()
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return await Request.RedirectCall(Wtm, "/api/_frameworkuser/GetFrameworkRoles");
            }
            return Ok(await DC.Set<FrameworkRole>().GetSelectListItems(Wtm, x => x.RoleName, x => x.RoleCode));
        }

        [HttpGet("GetFrameworkGroups")]
        [ActionDescription("GetGroups")]
        [AllRights]
        public async Task<IActionResult> GetFrameworkGroups ()
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return await Request.RedirectCall(Wtm, "/api/_frameworkuser/GetFrameworkGroups");
            }
            return Ok(await DC.Set<FrameworkGroup>().GetSelectListItems(Wtm, x => x.GroupName, x => x.GroupCode));
        }

        [HttpGet("GetFrameworkGroupsTree")]
        [ActionDescription("GetGroupsTree")]
        [AllRights]
        public async Task<IActionResult> GetFrameworkGroupsTree ()
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Request.RedirectCall(Wtm, "/api/_frameworkuser/GetFrameworkGroupsTree").Result;
            }
            return Ok(await DC.Set<FrameworkGroup>().GetTreeSelectListItems(Wtm, x => x.GroupName, x => x.GroupCode));
        }


        [HttpGet("GetUserById")]
        [AllRights]
        public async Task<IActionResult> GetUserById (string keywords)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return await Request.RedirectCall(Wtm, "/api/_frameworkuser/GetUserById");
            }
            var users = await DC.Set<FrameworkUser>().Where(x => x.ITCode.ToLower().StartsWith(keywords.ToLower())).GetSelectListItems(Wtm, x => x.Name + "(" + x.ITCode + ")", x => x.ITCode);
            return Ok(users);
        }

        [HttpGet("GetUserByGroup")]
        [AllRights]
        public async Task<IActionResult> GetUserByGroup (string keywords)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return await Request.RedirectCall(Wtm, "/api/_frameworkuser/GetUserByGroup");
            }
            var users = await DC.Set<FrameworkUserGroup>().Where(x => x.GroupCode == keywords).Select(x=>x.UserCode).ToListAsync();
            return Ok(users);
        }

        [HttpGet("GetUserByRole")]
        [AllRights]
        public async Task<IActionResult> GetUserByRole (string keywords)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return await Request.RedirectCall(Wtm, "/api/_frameworkuser/GetUserByRole");
            }
            var users = await DC.Set<FrameworkUserRole>().Where(x => x.RoleCode == keywords).Select(x => x.UserCode).ToListAsync();
            return Ok(users);
        }

        [ActionDescription("Login.ChangePassword")]
        [HttpPut("[action]")]
        public  IActionResult Password(FrameworkUserVM vm)
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
                vm.ChangePassword();
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

    }
}
