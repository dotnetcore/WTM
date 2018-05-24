using Microsoft.AspNetCore.Mvc;
using System;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc
{
    [DebugOnly]
    public class _CodeGenController : BaseController
    {
        [ActionDescription("代码生成器")]
        public IActionResult Index()
        {
            var vm = CreateVM<CodeGenVM>();
            vm.EntryDir = AppDomain.CurrentDomain.BaseDirectory;
            vm.AllModels = GlobaInfo.AllModels.ToListItems(x => x.Name, x => x.AssemblyQualifiedName);
            return PartialView(vm);
        }

        [HttpPost]
        [ActionDescription("配置字段")]
        public IActionResult SetField(CodeGenVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AllModels = GlobaInfo.AllModels.ToListItems(x => x.Name, x => x.AssemblyQualifiedName);
                return PartialView("Index", vm);

            }
            else
            {
                vm.FieldList.ModelFullName = vm.SelectedModel;
                return PartialView(vm);
            }
        }

        [HttpPost]
        [ActionDescription("生成确认")]
        public IActionResult Gen(CodeGenVM vm)
        {
            
            return PartialView(vm);
        }

        [HttpPost]
        public IActionResult DoGen(CodeGenVM vm)
        {
            vm.DoGen();
            return FFResult().CloseDialog().Alert("生成成功！");
        }

        [ActionDescription("预览")]
        [HttpPost]
        public IActionResult Preview(CodeGenVM vm)
        {
            if (vm.PreviewFile == "Controller")
            {
                ViewData["filename"] = vm.ModelName + "Controller.cs";
                ViewData["code"] = vm.GenerateController();
            }
            else if(vm.PreviewFile == "Searcher" || vm.PreviewFile.EndsWith("VM"))
            {
                ViewData["filename"] = vm.ModelName + vm.PreviewFile.Replace("CrudVM","VM") + ".cs";
                ViewData["code"] = vm.GenerateVM(vm.PreviewFile);
            }
            else if (vm.PreviewFile.EndsWith("View"))
            {
                ViewData["filename"] = vm.PreviewFile.Replace("ListView","Index").Replace("View","") + "Controller.cshtml";
                ViewData["code"] = vm.GenerateView(vm.PreviewFile);
            }
            return PartialView(vm);
        }
    }
}
