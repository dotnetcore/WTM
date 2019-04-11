using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs;

namespace WalkingTec.Mvvm.Admin.Api
{

    [ActionDescription("菜单管理")]
    [ApiController]
    [Route("api/_FrameworkMenu")]
	public class FrameworkMenuController : BaseApiController
    {
        [ActionDescription("搜索")]
        [HttpPost("Search")]
		public string Search(FrameworkMenuSearcher searcher)
        {
            var vm = CreateVM<FrameworkMenuListVM2>();
            vm.Searcher = searcher;
            return vm.GetJson();
        }

        [ActionDescription("获取")]
        [HttpGet("{id}")]
        public FrameworkMenuVM2 Get(Guid id)
        {
            var vm = CreateVM<FrameworkMenuVM2>(id);
            return vm;
        }

        [ActionDescription("新建")]
        [HttpPost("Add")]
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

        [ActionDescription("修改")]
        [HttpPut("Edit")]
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
        [ActionDescription("删除")]
        public IActionResult BatchDelete(Guid[] ids)
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


        [ActionDescription("导出")]
        [HttpPost("ExportExcel")]
        public IActionResult ExportExcel(FrameworkMenuSearcher searcher)
        {
            var vm = CreateVM<FrameworkMenuListVM2>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            var data = vm.GenerateExcel();
            return File(data, "application/vnd.ms-excel", $"Export_ActionLog_{DateTime.Now.ToString("yyyy-MM-dd")}.xls");
        }

        [ActionDescription("勾选导出")]
        [HttpPost("ExportExcelByIds")]
        public IActionResult ExportExcelByIds(Guid[] ids)
        {
            var vm = CreateVM<FrameworkMenuListVM2>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = new List<Guid>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            var data = vm.GenerateExcel();
            return File(data, "application/vnd.ms-excel", $"Export_ActionLog_{DateTime.Now.ToString("yyyy-MM-dd")}.xls");
        }

        #region 同步模块
        [ActionDescription("同步模块")]
        [FixConnection(DBOperationEnum.Write)]
        [HttpGet("SyncModel")]
        public ActionResult SyncModel()
        {
            SycModelAndAction();
            return Ok();
        }
        #endregion

        #region 未设置页面
        [ActionDescription("检查页面")]
        [HttpGet("UnsetPages")]
        public string UnsetPages()
        {
            var vm = CreateVM<FrameworkActionListVM>();
            return vm.GetJson();
        }
        #endregion

        #region 刷新菜单
        [ActionDescription("刷新菜单")]
        [HttpGet("RefreshMenu")]
        public ActionResult RefreshMenu()
        {
            var cache = GlobalServices.GetService<IMemoryCache>();
            cache.Remove("FFMenus");
            return Ok("操作成功");
        }
        #endregion

        [ActionDescription("获取动作")]
        [HttpGet("GetActionsByModel")]
        public ActionResult GetActionsByModel(string ModelName)
        {
            var m = GlobaInfo.AllModule.Where(x => x.IsApi == true && x.ClassName.ToLower()==ModelName.ToLower()).SelectMany(x => x.Actions).ToList();
            List<FrameworkAction> toremove = new List<FrameworkAction>();
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

        [ActionDescription("获取目录")]
        [HttpGet("GetFolders")]
        public ActionResult GetFolders()
        {
            var m = DC.Set<FrameworkMenu>().Where(x => x.FolderOnly == true).OrderBy(x=>x.DisplayOrder).GetSelectListItems(LoginUserInfo.DataPrivileges, null, x => x.PageName);
            return Ok(m);
        }

        protected void SycModelAndAction()
        {
            var allModules = GlobaInfo.AllModule.Where(x => x.IgnorePrivillege == false);
            using (var DC = CreateDC())
            {
                var dbModules = DC.Set<FrameworkModule>().Include(x => x.Actions).ToList();
                var ToRemove = new List<FrameworkModule>();
                var ToAdd = new List<FrameworkModule>();
                var ToRemove2 = new List<FrameworkAction>();
                var ToAdd2 = new List<FrameworkAction>();
                foreach (var oldItem in dbModules)
                {
                    bool exist = false;
                    foreach (var newItem in allModules)
                    {
                        if (oldItem.ClassName == newItem.ClassName && oldItem.NameSpace == newItem.NameSpace)
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (exist == false)
                    {
                        ToRemove.Add(oldItem);
                        ToRemove2.AddRange(oldItem.Actions);
                    }
                }
                foreach (var newItem in allModules)
                {
                    bool exist = false;
                    foreach (var oldItem in dbModules)
                    {
                        if (oldItem.ClassName == newItem.ClassName && oldItem.NameSpace == newItem.NameSpace)
                        {
                            oldItem.ModuleName = newItem.ModuleName;
                            SycActions(newItem.Actions.Where(x => x.IgnorePrivillege == false).ToList(), oldItem.Actions, oldItem, DC);
                            exist = true;
                            break;
                        }
                    }
                    if (exist == false)
                    {
                        ToAdd.Add(newItem);
                        ToAdd2.AddRange(newItem.Actions);
                    }
                }
                foreach (var remove in ToRemove2)
                {
                    DC.Set<FrameworkAction>().Remove(remove);
                }
                foreach (var remove in ToRemove)
                {
                    DC.Set<FrameworkModule>().Remove(remove);
                }
                foreach (var add in ToAdd)
                {
                    DC.Set<FrameworkModule>().Add(add);
                }
                DC.SaveChanges();
            }
        }

        /// <summary>
        /// 同步Actions
        /// </summary>
        /// <param name="newActions"></param>
        /// <param name="oldActions"></param>
        /// <param name="model"></param>
        /// <param name="DC"></param>
        private static void SycActions(List<FrameworkAction> newActions, List<FrameworkAction> oldActions, FrameworkModule model, IDataContext DC)
        {
            var ToRemove = new List<FrameworkAction>();
            var ToAdd = new List<FrameworkAction>();
            foreach (var oldItem in oldActions)
            {
                bool exist = false;
                foreach (var newItem in newActions)
                {
                    if (oldItem.MethodName == newItem.MethodName)
                    {
                        exist = true;
                        break;
                    }
                }
                if (exist == false)
                {
                    ToRemove.Add(oldItem);
                }
            }
            foreach (var newItem in newActions)
            {
                bool exist = false;
                foreach (var oldItem in oldActions)
                {
                    if (oldItem.MethodName == newItem.MethodName)
                    {
                        oldItem.ActionName = newItem.ActionName;
                        exist = true;
                        break;
                    }
                }
                if (exist == false)
                {
                    ToAdd.Add(newItem);
                }
            }
            foreach (var remove in ToRemove)
            {
                DC.Set<FrameworkAction>().Remove(remove);
            }
            foreach (var add in ToAdd)
            {
                add.ModuleId = model.ID;
                FrameworkAction act = new FrameworkAction();
                act.ModuleId = model.ID;
                act.MethodName = add.MethodName;
                act.Parameter = add.Parameter;
                act.ParasToRunTest = add.ParasToRunTest;
                act.ActionName = add.ActionName;
                DC.Set<FrameworkAction>().Add(act);
            }
        }

    }

}
