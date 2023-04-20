// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.ActionLogVMs;

namespace WalkingTec.Mvvm.Admin.Api
{
    [AuthorizeJwtWithCookie]
    [ActionDescription("MenuKey.ActionLog")]
    [ApiController]
    [Route("api/_[controller]")]
    public class ActionLogController : BaseApiController
    {
        [ActionDescription("Sys.Search")]
        [HttpPost("[action]")]
        public IActionResult Search(ActionLogSearcher searcher)
        {
            if (ModelState.IsValid)
            {
                var vm = Wtm.CreateVM<ActionLogListVM>(passInit:true);
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
        public ActionLogVM Get(Guid id)
        {
            var vm = Wtm.CreateVM<ActionLogVM>(id);
            return vm;
        }

        [HttpPost("[action]")]
        [ActionDescription("Sys.Delete")]
        public async Task<IActionResult> BatchDelete (string[] ids)
        {
            var vm = Wtm.CreateVM<ActionLogBatchVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = ids;
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
                return Ok(ids.Count());
            }
        }


        [ActionDescription("Sys.Export")]
        [HttpPost("[action]")]
        public async Task<IActionResult> ExportExcel (ActionLogSearcher searcher)
        {
            var vm = Wtm.CreateVM<ActionLogListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return await vm.GetExportData();
        }

        [ActionDescription("Sys.ExportByIds")]
        [HttpPost("[action]")]
        public async Task<IActionResult> ExportExcelByIds (string[] ids)
        {
            var vm = Wtm.CreateVM<ActionLogListVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            return await vm.GetExportData();
        }
    }
}
