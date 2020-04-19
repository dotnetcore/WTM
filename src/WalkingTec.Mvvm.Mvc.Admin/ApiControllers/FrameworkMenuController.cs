using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Auth.Attribute;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.Json;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ActionDescription("MenuMangement")]
    [ApiController]
    [Route("api/_[controller]")]
    public class FrameworkMenuController : BaseApiController
    {
        [ActionDescription("Search")]
        [HttpPost("[action]")]
        public string Search(FrameworkMenuSearcher searcher)
        {
            var vm = CreateVM<FrameworkMenuListVM2>();
            vm.Searcher = searcher;
            return vm.GetJson();
        }

        [ActionDescription("Get")]
        [HttpGet("{id}")]
        public FrameworkMenuVM2 Get(Guid id)
        {
            var vm = CreateVM<FrameworkMenuVM2>(id);
            return vm;
        }

        [ActionDescription("Create")]
        [HttpPost("[action]")]
        public IActionResult Add(FrameworkMenuVM2 vm)
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

        [ActionDescription("Edit")]
        [HttpPut("[action]")]
        public IActionResult Edit(FrameworkMenuVM2 vm)
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

        [HttpPost("BatchDelete")]
        [ActionDescription("Delete")]
        public IActionResult BatchDelete(string[] ids)
        {
            var vm = CreateVM<FrameworkMenuBatchVM>();
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

        [ActionDescription("Export")]
        [HttpPost("[action]")]
        public IActionResult ExportExcel(FrameworkMenuSearcher searcher)
        {
            var vm = CreateVM<FrameworkMenuListVM2>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            var data = vm.GenerateExcel();
            return File(data, "application/vnd.ms-excel", $"Export_ActionLog_{DateTime.Now.ToString("yyyy-MM-dd")}.xls");
        }

        [ActionDescription("ExportByIds")]
        [HttpPost("[action]")]
        public IActionResult ExportExcelByIds(string[] ids)
        {
            var vm = CreateVM<FrameworkMenuListVM2>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            var data = vm.GenerateExcel();
            return File(data, "application/vnd.ms-excel", $"Export_ActionLog_{DateTime.Now.ToString("yyyy-MM-dd")}.xls");
        }

        #region 未设置页面
        [ActionDescription("UnsetPages")]
        [HttpGet("[action]")]
        public string UnsetPages()
        {
            var vm = CreateVM<FrameworkActionListVM>();
            return vm.GetJson();
        }
        #endregion

        #region 刷新菜单
        [ActionDescription("RefreshMenu")]
        [HttpGet("[action]")]
        public async Task<ActionResult> RefreshMenu()
        {
            Cache.Delete("FFMenus");
            var userids = DC.Set<FrameworkUserBase>().Select(x => x.ID.ToString().ToLower()).ToArray();
            await WtmContext.LoginUserInfo.RemoveUserCache(userids);
            return Ok(Mvc.Admin.Program._localizer["OprationSuccess"]);
        }
        #endregion

        [ActionDescription("GetActionsByModelId")]
        [HttpGet("GetActionsByModel")]
        public ActionResult GetActionsByModel(string ModelName)
        {
            var m = GlobaInfo.AllModule.Where(x => x.IsApi == true && x.FullName.ToLower() == ModelName.ToLower()).SelectMany(x => x.Actions).ToList();
            List<SimpleAction> toremove = new List<SimpleAction>();
            foreach (var item in m)
            {
                if (item.IgnorePrivillege == true || item.Module.IgnorePrivillege == true)
                {
                    toremove.Add(item);
                }
            }
            toremove.ForEach(x => m.Remove(x));
            var actions = m.ToListItems(y => y.ActionName, y => y.MethodName);
            actions.ForEach(x => x.Selected = true);
            return Ok(actions);
        }

        [ActionDescription("GetFolders")]
        [HttpGet("GetFolders")]
        public ActionResult GetFolders()
        {
            var m = DC.Set<FrameworkMenu>().Where(x => x.FolderOnly == true).OrderBy(x => x.DisplayOrder).GetSelectListItems( null, x => x.PageName);
            return Ok(m);
        }

    }

}
