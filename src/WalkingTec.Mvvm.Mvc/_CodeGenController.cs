using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// 代码生成器页面控制器（仅 Debug 模式下可访问），提供模型选择、字段配置、代码预览与生成等功能
    /// </summary>
    [DebugOnly]
    public class _CodeGenController : BaseController
    {
        /// <summary>
        /// 返回内嵌框架页视图
        /// </summary>
        /// <returns>视图结果</returns>
        public IActionResult Inner()
        {
            return View();

        }



        /// <summary>
        /// 代码生成器主页面，创建 CodeGenVM 并加载所有可选模型及程序入口目录
        /// </summary>
        /// <param name="ui">目标 UI 类型</param>
        /// <returns>主页面视图</returns>
        [ActionDescription("代码生成器")]
        public IActionResult Index(UIEnum ui)
        {
            var vm = Wtm.CreateVM<CodeGenVM>();
            vm.UI = ui;
            vm.EntryDir = AppDomain.CurrentDomain.BaseDirectory;
            vm.AllModels = GetAllModels().ToListItems(x => x.Name, x => x.AssemblyQualifiedName);
            return View(vm);
        }

        /// <summary>
        /// 字段配置页面；校验所选模型必须继承 TopBasePoco，校验失败重新返回主页面，成功则加载模型的字段列表
        /// </summary>
        /// <param name="vm">代码生成器 VM</param>
        /// <returns>字段配置页面视图，校验失败时返回主页面视图</returns>
        [HttpPost]
        [ActionDescription("配置字段")]
        public IActionResult SetField(CodeGenVM vm)
        {
            if (vm.SelectedModel != null)
            {
                Type modeltype = Type.GetType(vm.SelectedModel);
                //所选模型必须是 TopBasePoco 的子类，否则不允许进入下一步
                if(modeltype.IsSubclassOf(typeof(TopBasePoco)) == false)
                {
                    ModelState.AddModelError("SelectedModel", MvcProgram._localizer["Codegen.SelectedModelMustBeBasePoco"]);
                }
            }
            if (!ModelState.IsValid)
            {
                vm.AllModels = GetAllModels().ToListItems(x => x.Name, x => x.AssemblyQualifiedName);
                return View("Index", vm);

            }
            else
            {
                vm.FieldList.ModelFullName = vm.SelectedModel;
                return View(vm);
            }
        }

        /// <summary>
        /// 生成确认页面，汇总展示用户配置的各项生成选项供最终确认
        /// </summary>
        /// <param name="vm">代码生成器 VM</param>
        /// <returns>生成确认页面视图</returns>
        [HttpPost]
        [ActionDescription("生成确认")]
        public IActionResult Gen(CodeGenVM vm)
        {
            
            return View(vm);
        }

        /// <summary>
        /// 执行代码生成，完成后弹出成功提示并关闭当前弹窗
        /// </summary>
        /// <param name="vm">代码生成器 VM</param>
        /// <returns>前端脚本执行结果</returns>
        [HttpPost]
        public IActionResult DoGen(CodeGenVM vm)
        {
            vm.DoGen();
            return FFResult().Alert(MvcProgram._localizer["Codegen.Success"]);
        }

        /// <summary>
        /// 预览将要生成的代码；根据 PreviewFile 与所选 UI 类型，分别调用控制器、VM 或对应前端框架的生成方法
        /// </summary>
        /// <param name="vm">代码生成器 VM</param>
        /// <returns>包含文件名和代码内容的局部视图</returns>
        [ActionDescription("预览")]
        [HttpPost]
        public IActionResult Preview(CodeGenVM vm)
        {
            //预览控制器代码
            if (vm.PreviewFile == "Controller")
            {
                ViewData["filename"] = $"{vm.ModelName}{(vm.IsApi == true ? "Api" : "")}Controller.cs";
                ViewData["code"] = vm.GenerateController();
            }
            //预览 VM 代码（含 Searcher 和各 CRUD VM）
            else if(vm.PreviewFile == "Searcher" || vm.PreviewFile.EndsWith("VM"))
            {
                ViewData["filename"] = vm.ModelName + $"{(vm.IsApi == true ? "Api" : "")}" + vm.PreviewFile.Replace("CrudVM","VM") + ".cs";
                ViewData["code"] = vm.GenerateVM(vm.PreviewFile);
            }
            //React 前端代码预览
            else if(vm.UI == UIEnum.React)
            {
                if (vm.PreviewFile == "storeindex")
                {
                    ViewData["code"] = vm.GetResource("index.txt", "Spa.React.store").Replace("$modelname$", vm.ModelName.ToLower());
                }
                else if (vm.PreviewFile == "index")
                {
                    ViewData["code"] = vm.GetResource("index.txt", "Spa.React").Replace("$modelname$", vm.ModelName.ToLower());
                }
                else if (vm.PreviewFile == "style")
                {
                    ViewData["code"] = vm.GetResource("style.txt", "Spa.React").Replace("$modelname$", vm.ModelName.ToLower());
                }
                else
                {
                    ViewData["code"] = vm.GenerateReactView(vm.PreviewFile);
                }

            }
            //VUE 前端代码预览
            else if(vm.UI == UIEnum.VUE)
            {
                List<string> apineeded = new List<string>();
                ViewData["code"] = vm.GenerateVUEView(vm.PreviewFile,apineeded);
            }
            //VUE3 前端代码预览
            else if (vm.UI == UIEnum.VUE3)
            {
                List<string> apineeded = new List<string>();
                ViewData["code"] = vm.GenerateVue3View(vm.PreviewFile);
            }
            //Blazor 前端代码预览
            else if (vm.UI == UIEnum.Blazor)
            {
                ViewData["code"] = vm.GenerateBlazorView(vm.PreviewFile);
            }
            //LayUI 视图代码预览，并将视图名转换为页面名
            else if (vm.PreviewFile.EndsWith("View"))
            {
                ViewData["filename"] = vm.PreviewFile.Replace("ListView","Index").Replace("View","") + "cshtml";
                ViewData["code"] = vm.GenerateView(vm.PreviewFile);
            }
            return PartialView(vm);
        }

        /// <summary>
        /// 获取所有数据库上下文中以 DbSet 属性暴露的模型类型
        /// </summary>
        /// <returns>模型类型列表</returns>
        private  List<Type> GetAllModels()
        {
            var models = new List<Type>();
            
            //获取所有模型
            var pros = Wtm.ConfigInfo.Connections.SelectMany(x => x.DcConstructor.DeclaringType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance));
            if (pros != null)
            {
                foreach (var pro in pros)
                {
                    if (pro.PropertyType.IsGeneric(typeof(DbSet<>)))
                    {
                        models.Add(pro.PropertyType.GetGenericArguments()[0]);
                    }
                }
            }
            return models;
        }

    }
}
