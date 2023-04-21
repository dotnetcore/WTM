using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.ViewModels.不要用中文模型名VMs;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Demo.Controllers
{
    
    [ActionDescription("中文测试")]
    public partial class 不要用中文模型名Controller : BaseController
    {
        #region 搜索
        [ActionDescription("搜索")]
        public async Task<IActionResult> Index()
        {
            var vm = Wtm.CreateVM<不要用中文模型名ListVM>();
            return PartialView(vm);
        }

        [ActionDescription("搜索")]
        [HttpPost]
        public string Search(不要用中文模型名ListVM vm)
        {
            return vm.GetJson(false);
        }

        #endregion

        #region 新建
        [ActionDescription("新建")]
        public async Task<IActionResult> Create()
        {
            var vm = Wtm.CreateVM<不要用中文模型名VM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("新建")]
        public async Task<IActionResult> Create(不要用中文模型名VM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                await vm.DoAdd();
                if (!ModelState.IsValid)
                {
                    await vm.DoReInit();
                    return PartialView(vm);
                }
                else
                {
                    return FFResult().CloseDialog().RefreshGrid();
                }
            }
        }
        #endregion

        #region 修改
        [ActionDescription("修改")]
        public async Task<IActionResult> Edit(string id)
        {
            var vm = Wtm.CreateVM<不要用中文模型名VM>(id);
            return PartialView(vm);
        }

        [ActionDescription("修改")]
        [HttpPost]
        [ValidateFormItemOnly]
        public async Task<IActionResult> Edit(不要用中文模型名VM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                await vm.DoEdit();
                if (!ModelState.IsValid)
                {
                    await vm.DoReInit();
                    return PartialView(vm);
                }
                else
                {
                    return FFResult().CloseDialog().RefreshGridRow(vm.Entity.ID);
                }
            }
        }
        #endregion

        #region 删除
        [ActionDescription("删除")]
        public async Task<IActionResult> Delete(string id)
        {
            var vm = Wtm.CreateVM<不要用中文模型名VM>(id);
            return PartialView(vm);
        }

        [ActionDescription("删除")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id, IFormCollection nouse)
        {
            var vm = Wtm.CreateVM<不要用中文模型名VM>(id);
            await vm.DoDelete();
            if (!ModelState.IsValid)
            {
                return PartialView(vm);
            }
            else
            {
                return FFResult().CloseDialog().RefreshGrid();
            }
        }
        #endregion

        #region 详细
        [ActionDescription("详细")]
        public async Task<IActionResult> Details(string id)
        {
            var vm = Wtm.CreateVM<不要用中文模型名VM>(id);
            return PartialView(vm);
        }
        #endregion

        #region 批量修改
        [HttpPost]
        [ActionDescription("批量修改")]
        public async Task<IActionResult> BatchEdit(string[] IDs)
        {
            var vm = Wtm.CreateVM<不要用中文模型名BatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("批量修改")]
        public async Task<IActionResult> DoBatchEdit(不要用中文模型名BatchVM vm, IFormCollection nouse)
        {
            if (!ModelState.IsValid || !await vm.DoBatchEdit())
            {
                return PartialView("BatchEdit",vm);
            }
            else
            {
                return FFResult().CloseDialog().RefreshGrid().Alert("操作成功，共有"+vm.Ids.Length+"条数据被修改");
            }
        }
        #endregion

        #region 批量删除
        [HttpPost]
        [ActionDescription("批量删除")]
        public async Task<IActionResult> BatchDelete(string[] IDs)
        {
            var vm = Wtm.CreateVM<不要用中文模型名BatchVM>(Ids: IDs);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("批量删除")]
        public async Task<IActionResult> DoBatchDelete(不要用中文模型名BatchVM vm, IFormCollection nouse)
        {
            if (!ModelState.IsValid || !await vm.DoBatchDelete())
            {
                return PartialView("BatchDelete",vm);
            }
            else
            {
                return FFResult().CloseDialog().RefreshGrid().Alert("操作成功，共有"+vm.Ids.Length+"条数据被删除");
            }
        }
        #endregion

        #region 导入
		[ActionDescription("导入")]
        public async Task<IActionResult> Import()
        {
            var vm = Wtm.CreateVM<不要用中文模型名ImportVM>();
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("导入")]
        public async Task<IActionResult> Import(不要用中文模型名ImportVM vm, IFormCollection nouse)
        {
            if (vm.ErrorListVM.EntityList.Count > 0 || !await vm.BatchSaveData())
            {
                return PartialView(vm);
            }
            else
            {
                return FFResult().CloseDialog().RefreshGrid().Alert("成功导入 " + vm.EntityList.Count.ToString() + " 行数据");
            }
        }
        #endregion

        [ActionDescription("导出")]
        [HttpPost]
        public async Task<IActionResult> ExportExcel(不要用中文模型名ListVM vm)
        {
            vm.SearcherMode = vm.Ids != null && vm.Ids.Count > 0 ? ListVMSearchModeEnum.CheckExport : ListVMSearchModeEnum.Export;
            var data = await vm.GenerateExcel();
            return File(data, "application/vnd.ms-excel", $"Export_不要用中文模型名_{DateTime.Now.ToString("yyyy-MM-dd")}.xls");
        }

    }
}
