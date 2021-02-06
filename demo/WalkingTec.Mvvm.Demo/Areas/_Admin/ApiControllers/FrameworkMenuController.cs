// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.Json;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ActionDescription("_Admin.MenuApi")]
    [ApiController]
    [Route("api/_[controller]")]
    public class FrameworkMenuController : BaseApiController
    {
        [ActionDescription("Sys.Search")]
        [HttpPost("[action]")]
        public string Search(BaseSearcher searcher)
        {
            var vm = Wtm.CreateVM<FrameworkMenuListVM2>();
            vm.Searcher = searcher;
            return vm.GetJson();
        }

        [ActionDescription("Sys.Get")]
        [HttpGet("{id}")]
        public FrameworkMenuVM2 Get(Guid id)
        {
            var vm = Wtm.CreateVM<FrameworkMenuVM2>(id);
            return vm;
        }

        [ActionDescription("Sys.Create")]
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

        [ActionDescription("Sys.Edit")]
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
        [ActionDescription("Sys.Delete")]
        public IActionResult BatchDelete(string[] ids)
        {
            var vm = Wtm.CreateVM<FrameworkMenuBatchVM>();
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

        [ActionDescription("Sys.Export")]
        [HttpPost("[action]")]
        public IActionResult ExportExcel(BaseSearcher searcher)
        {
            var vm = Wtm.CreateVM<FrameworkMenuListVM2>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return vm.GetExportData();
        }

        [ActionDescription("Sys.ExportByIds")]
        [HttpPost("[action]")]
        public IActionResult ExportExcelByIds(string[] ids)
        {
            var vm = Wtm.CreateVM<FrameworkMenuListVM2>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            return vm.GetExportData();
        }

        [ActionDescription("_Admin.UnsetPages")]
        [HttpGet("[action]")]
        public string UnsetPages()
        {
            var vm = Wtm.CreateVM<FrameworkActionListVM>();
            return vm.GetJson();
        }

        [ActionDescription("_Admin.RefreshMenu")]
        [HttpGet("[action]")]
        public async Task<ActionResult> RefreshMenu()
        {
            Cache.Delete("FFMenus");
            var userids = DC.Set<FrameworkUser>().Select(x => x.ID.ToString().ToLower()).ToArray();
            await Wtm.RemoveUserCache(userids);
            return Ok(Localizer["Sys.OprationSuccess"]);
        }

        [ActionDescription("GetActionsByModelId")]
        [HttpGet("GetActionsByModel")]
        [AllRights]
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
        [AllRights]
        public ActionResult GetFolders()
        {
            var AllParents = DC.Set<FrameworkMenu>().Where(x => x.FolderOnly == true).OrderBy(x => x.DisplayOrder).GetSelectListItems(Wtm, x => x.PageName);
            foreach (var p in AllParents)
            {
                if (p.Text.StartsWith("MenuKey."))
                {
                    p.Text = Localizer[p.Text];
                }
            }

            return Ok(AllParents);
        }

    }

}
