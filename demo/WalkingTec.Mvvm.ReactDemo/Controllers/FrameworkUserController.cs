using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WalkingTec.Mvvm.ReactDemo.Controllers
{
    /// <summary>
    /// 用户
    /// </summary>
    [ApiController]
    [Route("api/FrameworkUser")]
    [AllowAnonymous]
    public class UserController : BaseApiController
    {

        // GET: api/<controller>
        [HttpPost("Search")]
        public string Search(FrameworkUserSearcher searcher)
        {
            var vm = CreateVM<FrameworkUserListVM>();
            vm.Searcher = searcher;
            return vm.GetJson();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public FrameworkUserBase Get(Guid id)
        {
            var vm = CreateVM<FrameworkUserVM>(id);
            return vm.Entity;
        }

        // POST api/<controller>
        [HttpPost("Add")]
        public IActionResult Add(FrameworkUserVM vm)
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

        // PUT api/<controller>/5
        [HttpPut("Edit")]
        public IActionResult Edit(FrameworkUserVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                vm.DoEdit(true);
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

        // DELETE api/<controller>/5
        [HttpGet("Delete/{id}")]
        public IActionResult Delete(Guid id)
        {
            var vm = CreateVM<FrameworkUserVM>(id);
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
        public IActionResult BatchDelete(string[] ids)
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
                return Ok(ids.Count());
            }
        }


        [HttpPost("ExportExcel")]
        [ActionDescription("导出")]
        public IActionResult ExportExcel(FrameworkUserSearcher searcher)
        {
            var vm = CreateVM<FrameworkUserListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            var data = vm.GenerateExcel();
            return File(data, "application/vnd.ms-excel", $"Export_FrameworkUser_{DateTime.Now.ToString("yyyy-MM-dd")}.xls");
        }

        [HttpPost("ExportExcelByIds")]
        [ActionDescription("导出")]
        public IActionResult ExportExcelByIds(string[] ids)
        {
            var vm = CreateVM<FrameworkUserListVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            var data = vm.GenerateExcel();
            return File(data, "application/vnd.ms-excel", $"Export_FrameworkUser_{DateTime.Now.ToString("yyyy-MM-dd")}.xls");
        }

        [HttpGet("GetExcelTemplate")]
        public IActionResult GetExcelTemplate()
        {
            var vm = CreateVM<FrameworkUserImportVM>();
            var qs = new Dictionary<string, string>();
            foreach (var item in Request.Query.Keys)
            {
                qs.Add(item, Request.Query[item]);
            }
            vm.SetParms(qs);
            var data = vm.GenerateTemplate(out string fileName);
            return File(data, "application/vnd.ms-excel", fileName);
        }

        [HttpPost("Import")]
        [ActionDescription("导入")]
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

        [HttpGet("GetUserRoles")]
        [ActionDescription("获取角色")]
        public ActionResult GetUserRoles()
        {
            var test = DC.Set<FrameworkRole>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, x => x.RoleName);
            return Ok(DC.Set<FrameworkRole>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, x => x.RoleName));
        }

        [HttpGet("GetUserGroups")]
        [ActionDescription("获取角色")]
        public ActionResult GetUserGroups()
        {
            return Ok(DC.Set<FrameworkGroup>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, x => x.GroupName));
        }

        [HttpGet("GetSex")]
        [ActionDescription("获取性别")]
        public ActionResult GetSex()
        {
            var test = typeof(SexEnum).ToListItems();
            return Ok(test);
        }


    }
}
