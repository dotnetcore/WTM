using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth.Attribute;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ActionDescription("UserManagement")]
    [ApiController]
    [Route("api/_FrameworkUserBase")]
    public class FrameworkUserController : BaseApiController
    {
        [ActionDescription("Search")]
        [HttpPost("[action]")]
        public string Search(FrameworkUserSearcher searcher)
        {
            var vm = CreateVM<FrameworkUserListVM>();
            vm.Searcher = searcher;
            return vm.GetJson();
        }

        [ActionDescription("Get")]
        [HttpGet("{id}")]
        public FrameworkUserVM Get(Guid id)
        {
            var vm = CreateVM<FrameworkUserVM>(id);
            return vm;
        }

        [ActionDescription("Create")]
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

        [ActionDescription("Edit")]
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

        [HttpPost("BatchDelete")]
        [ActionDescription("Delete")]
        public async Task<IActionResult> BatchDelete(string[] ids)
        {
            var vm = CreateVM<FrameworkUserBatchVM>();
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
                List<Guid?> tempids = new List<Guid?>();
                foreach (var item in vm?.Ids)
                {
                    tempids.Add(Guid.Parse(item));
                }
                var userids = DC.Set<FrameworkUserBase>().Where(x => tempids.Contains(x.ID)).Select(x => x.ID.ToString()).ToArray();
                await LoginUserInfo.RemoveUserCache(userids);
                return Ok(ids.Count());
            }
        }

        [ActionDescription("Export")]
        [HttpPost("[action]")]
        public IActionResult ExportExcel(FrameworkUserSearcher searcher)
        {
            var vm = CreateVM<FrameworkUserListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return vm.GetExportData();
        }

        [ActionDescription("ExportByIds")]
        [HttpPost("[action]")]
        public IActionResult ExportExcelByIds(string[] ids)
        {
            var vm = CreateVM<FrameworkUserListVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            return vm.GetExportData();
        }

        [ActionDescription("DownloadTemplate")]
        [HttpGet("[action]")]
        public IActionResult GetExcelTemplate()
        {
            var vm = CreateVM<FrameworkUserImportVM>();
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

        [ActionDescription("Import")]
        [HttpPost("[action]")]
        public ActionResult Import(FrameworkUserImportVM vm)
        {

            if (vm.ErrorListVM.EntityList.Count > 0 || !vm.BatchSaveData())
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
        public ActionResult GetFrameworkRoles()
        {
            return Ok(DC.Set<FrameworkRole>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, x => x.RoleName));
        }

        [HttpGet("GetFrameworkGroups")]
        [ActionDescription("GetGroups")]
        public ActionResult GetFrameworkGroups()
        {
            return Ok(DC.Set<FrameworkGroup>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, x => x.GroupName));
        }

        [AllRights]
        [HttpGet("[action]")]
        [ActionDescription("Login")]
        public async Task<IActionResult> GetUserInfo()
        {
            var user = await DC.Set<FrameworkUserBase>()
                            .Include(x => x.UserRoles)
                            .Include(x => x.UserGroups)
                            .Where(x => x.ID == LoginUserInfo.Id)
                            .SingleOrDefaultAsync();

            //如果没有找到则输出错误
            if (user == null)
            {
                ModelState.AddModelError(" login", "登录失败");
                return BadRequest(ModelState.GetErrorJson());
            }
            var roleIDs = user.UserRoles.Select(x => x.RoleId).ToList();
            var groupIDs = user.UserGroups.Select(x => x.GroupId).ToList();
            //查找登录用户的数据权限
            var dpris = DC.Set<DataPrivilege>()
                .Where(x => x.UserId == user.ID || (x.GroupId != null && groupIDs.Contains(x.GroupId.Value)))
                .ToList();

            //生成并返回登录用户信息
            LoginUserInfo rv = new LoginUserInfo();
            rv.Id = user.ID;
            rv.ITCode = user.ITCode;
            rv.Name = user.Name;
            rv.Roles = DC.Set<FrameworkRole>().Where(x => user.UserRoles.Select(y => y.RoleId).Contains(x.ID)).ToList();
            rv.Groups = DC.Set<FrameworkGroup>().Where(x => user.UserGroups.Select(y => y.GroupId).Contains(x.ID)).ToList();
            rv.DataPrivileges = dpris;
            rv.PhotoId = user.PhotoId;
            //查找登录用户的页面权限
            var pris = DC.Set<FunctionPrivilege>()
                .Where(x => x.UserId == user.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                .ToList();
            rv.FunctionPrivileges = pris;
            LoginUserInfo = rv;

            LoginUserInfo forapi = new LoginUserInfo();
            forapi.Id = user.ID;
            forapi.ITCode = user.ITCode;
            forapi.Name = user.Name;
            forapi.Roles = rv.Roles;
            forapi.Groups = rv.Groups;
            forapi.PhotoId = rv.PhotoId;
            List<SimpleMenu> ms = new List<SimpleMenu>();

            var menus = DC.Set<FunctionPrivilege>()
                .Where(x => x.UserId == user.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                .Select(x => x.MenuItem).Distinct()
                .Where(x => x.MethodName == null)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new SimpleMenu
                {
                    Id = x.ID.ToString().ToLower(),
                    ParentId = x.ParentId.ToString().ToLower(),
                    Text = x.PageName,
                    Url = x.Url,
                    Icon = x.ICon
                }).ToList();

            var folders = DC.Set<FrameworkMenu>().Where(x => x.FolderOnly == true).Select(x => new SimpleMenu
            {
                Id = x.ID.ToString().ToLower(),
                ParentId = x.ParentId.ToString().ToLower(),
                Text = x.PageName,
                Url = x.Url,
                Icon = x.ICon
            }).ToList();
            ms.AddRange(folders);
            foreach (var item in menus)
            {
                if (folders.Any(x => x.Id == item.Id) == false)
                {
                    ms.Add(item);
                }
            }

            List<string> urls = new List<string>();
            urls.AddRange(DC.Set<FunctionPrivilege>()
                .Where(x => x.UserId == user.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                .Select(x => x.MenuItem).Distinct()
                .Where(x => x.MethodName != null)
                .Select(x => x.Url)
                );
            urls.AddRange(GlobaInfo.AllModule.Where(x => x.IsApi == true).SelectMany(x => x.Actions).Where(x => (x.IgnorePrivillege == true || x.Module.IgnorePrivillege == true) && x.Url != null).Select(x => x.Url));
            forapi.Attributes = new Dictionary<string, object>();
            forapi.Attributes.Add("Menus", ms);
            forapi.Attributes.Add("Actions", urls);
            return Ok(forapi);
        }

    }
}
