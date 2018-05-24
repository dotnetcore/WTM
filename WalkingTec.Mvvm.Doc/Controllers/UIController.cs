using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Doc.FrameworkUserVms;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [Public]
    [ActionDescription("页面层")]
    public class UIController : BaseController
    {
        [ActionDescription("介绍")]
        public IActionResult Intro()
        {
            var vm = CreateVM<FrameworkAllVM>();
            vm.Vm = CreateVM<FrameworkUserVM>();
            vm.ListVm = CreateVM<FrameworkUserListVM>();
            return PartialView(vm);
        }

        [ActionDescription("布局")]
        public IActionResult Layout()
        {
            var vm = CreateVM<FrameworkAllVM>();
            vm.Vm = CreateVM<FrameworkUserVM>();
            vm.ListVm = CreateVM<FrameworkUserListVM>();
            return PartialView(vm);
        }

        [ActionDescription("表单")]
        public IActionResult Form()
        {
            var vm = CreateVM<FrameworkAllVM>();
            vm.Vm = CreateVM<FrameworkUserVM>();
            vm.ListVm = CreateVM<FrameworkUserListVM>();
            return PartialView(vm);
        }
        [ActionDescription("数据表格")]
        public IActionResult Grid()
        {
            var vm = CreateVM<FrameworkAllVM>();
            vm.Vm = CreateVM<FrameworkUserVM>();
            vm.ListVm = CreateVM<FrameworkUserListVM>();
            return PartialView(vm);
        }
        [ActionDescription("Js函数")]
        public IActionResult Js()
        {
            return PartialView();
        }
    }
}
