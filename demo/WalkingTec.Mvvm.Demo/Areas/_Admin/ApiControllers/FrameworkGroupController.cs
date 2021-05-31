// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkGroupVMs;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ActionDescription("_Admin.GroupApi")]
    [ApiController]
    [Route("api/_[controller]")]
    public class FrameworkGroupController : BaseApiController
    {
        [ActionDescription("Sys.Search")]
        [HttpPost("[action]")]
        public string Search(FrameworkGroupSearcher searcher)
        {
            var vm = Wtm.CreateVM<FrameworkGroupListVM>();
            vm.Searcher = searcher;
            return vm.GetJson();
        }

        [ActionDescription("Sys.Get")]
        [HttpGet("{id}")]
        public FrameworkGroupVM Get(Guid id)
        {
            var vm = Wtm.CreateVM<FrameworkGroupVM>(id);
            return vm;
        }

        [ActionDescription("Sys.Create")]
        [HttpPost("[action]")]
        public IActionResult Add(FrameworkGroupVM vm)
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
        public IActionResult Edit(FrameworkGroupVM vm)
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

        [HttpPost("BatchDelete")]
        [ActionDescription("Sys.Delete")]
        public async Task<IActionResult> BatchDelete(string[] ids)
        {
            var vm = Wtm.CreateVM<FrameworkGroupBatchVM>();
            List<string> GroupCode = new List<string>();
            if (ids != null && ids.Count() > 0)
            {
                GroupCode = DC.Set<FrameworkGroup>().CheckIDs(new List<string>(ids)).Select(x => x.GroupCode).ToList();
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
                var gr = DC.Set<FrameworkUserGroup>().Where(x => GroupCode.Contains(x.GroupCode)).ToList();
                var itcodes = gr.Select(x => x.UserCode).ToArray();
                DC.Set<FrameworkUserGroup>().RemoveRange(gr);
                DC.SaveChanges();
                await Wtm.RemoveUserCache(itcodes);
                return Ok(ids.Count());
            }
        }

        [ActionDescription("Sys.Export")]
        [HttpPost("[action]")]
        public IActionResult ExportExcel(FrameworkGroupSearcher searcher)
        {
            var vm = Wtm.CreateVM<FrameworkGroupListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return vm.GetExportData();
        }

        [ActionDescription("Sys.ExportByIds")]
        [HttpPost("[action]")]
        public IActionResult ExportExcelByIds(string[] ids)
        {
            var vm = Wtm.CreateVM<FrameworkGroupListVM>();
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
            var vm = Wtm.CreateVM<FrameworkGroupImportVM>();
            var qs = new Dictionary<string, string>();
            foreach (var item in Request.Query.Keys)
            {
                qs.Add(item, Request.Query[item]);
            }
            vm.SetParms(qs);
            var data = vm.GenerateTemplate(out string fileName);
            return File(data, "application/vnd.ms-excel", fileName);
        }

        [ActionDescription("Sys.Import")]
        [HttpPost("[action]")]
        public ActionResult Import(FrameworkGroupImportVM vm)
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

    }
}
