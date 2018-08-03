using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WalkingTec.Mvvm.ApiDemo
{
    /// <summary>
    /// 用户
    /// </summary>
    [Route("api/user")]
    public class UserController : BaseApiController
    {

        // GET: api/<controller>
        [HttpPost("Search")]
        public IActionResult Search(FrameworkUserSearcher searcher)
        {
            var vm = CreateVM<FrameworkUserListVM>();
            vm.Searcher = searcher;
            vm.DoSearch();
            var rv = new ContentResult
            {
                ContentType = "application/json;charset=utf-8",
                Content = $@"{{""Data"":{vm.GetDataJson()},""Count"":{vm.Searcher.Count},""Msg"":""success"",""Code"":{StatusCodes.Status200OK}}}",
            };
            return rv;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var vm = CreateVM<FrameworkUserVM>(id);
            return Ok(vm.Entity);
        }

        // POST api/<controller>
        [HttpPost]
        public IActionResult Post([FromBody]FrameworkUserVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                vm.DoAdd();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    return CreatedAtAction(nameof(Get), new { id = vm.Entity.ID }, vm);
                }
            }

        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public IActionResult Put([FromBody]FrameworkUserVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                vm.DoEdit();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    return CreatedAtAction(nameof(Get), new { id = vm.Entity.ID }, vm);
                }
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var vm = CreateVM<FrameworkUserVM>(id);
            vm.DoDelete();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                return Ok();
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

        [HttpPost("CheckExportExcel")]
        [ActionDescription("导出")]
        public IActionResult ExportExcel(List<Guid> ids)
        {
            var vm = CreateVM<FrameworkUserListVM>();
            vm.Ids = ids;
            vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            var data = vm.GenerateExcel();
            return File(data, "application/vnd.ms-excel", $"Export_FrameworkUser_{DateTime.Now.ToString("yyyy-MM-dd")}.xls");
        }
    }
}
