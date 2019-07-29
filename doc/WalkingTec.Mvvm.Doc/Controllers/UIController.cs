using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Doc.FrameworkUserVms;
using WalkingTec.Mvvm.Doc.Models;
using WalkingTec.Mvvm.Doc.ViewModels.MajorVms;
using WalkingTec.Mvvm.Doc.ViewModels.SchoolVms;
using WalkingTec.Mvvm.Doc.ViewModels.StudentVms;
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
            return PartialView(vm);
        }

        [ActionDescription("布局")]
        public IActionResult Layout()
        {
            var vm = CreateVM<FrameworkAllVM>();
            return PartialView(vm);
        }

        [ActionDescription("表单")]
        public IActionResult Form()
        {
            var vm = CreateVM<SchoolVm>();
            return PartialView(vm);
        }

        [ActionDescription("文本框")]
        public IActionResult TextBox()
        {
            var vm = CreateVM<MajorVm>();
            return PartialView(vm);
        }

        [ActionDescription("下拉框")]
        public IActionResult ComboBox()
        {
            var vm = CreateVM<StudentVm>();
            return PartialView(vm);
        }

        [ActionDescription("勾选框")]
        public IActionResult CheckBox()
        {
            var vm = CreateVM<StudentVm>();
            return PartialView(vm);
        }

        [ActionDescription("Radio")]
        public IActionResult Radio()
        {
            var vm = CreateVM<StudentVm>();
            return PartialView(vm);
        }

        [ActionDescription("DateTime")]
        public IActionResult DateTime()
        {
            var vm = CreateVM<StudentVm>();
            return PartialView(vm);
        }

        [ActionDescription("Upload")]
        public IActionResult Upload()
        {
            var vm = CreateVM<StudentVm>();
            return PartialView(vm);
        }

        [ActionDescription("Selector")]
        public IActionResult Selector()
        {
            var vm = CreateVM<MajorVm>();
            return PartialView(vm);
        }


        [ActionDescription("Rich")]
        public IActionResult Rich()
        {
            var vm = CreateVM<MajorVm>();
            return PartialView(vm);
        }

        [ActionDescription("Others")]
        public IActionResult Others()
        {
            var vm = CreateVM<StudentVm>();
            return PartialView(vm);
        }

        [ActionDescription("数据表格")]
        public IActionResult Grid()
        {
            var vm = CreateVM<StudentListVm>();
            return PartialView(vm);
        }


        [ActionDescription("Js函数")]
        public IActionResult Js()
        {
            return PartialView();
        }

        [ActionDescription("Transfer")]
        public IActionResult Transfer()
        {
            var vm = CreateVM<StudentVm>();
            return PartialView(vm);
        }

        [ActionDescription("Slider")]
        public IActionResult Slider()
        {
            var vm = CreateVM<StudentVm>();
            return PartialView(vm);
        }

        public IActionResult GetSchool(string keywords)
        {
            List<School> schools = new List<School>()
            {
                new School { SchoolName = "清华大学"},
                new School { SchoolName = "北京大学"},
                new School { SchoolName = "复旦大学"},
                new School { SchoolName = "北京工业大学"},
                new School { SchoolName = "浙江大学"},
            };
            var rv = schools.Where(x=>x.SchoolName.StartsWith(keywords)).Select(x => new { Text = x.SchoolName, Value = x.SchoolName }).ToList();
            return Json(rv);
        }

        public IActionResult GetMajorBySchool(string id)
        {
            List<School> schools = new List<School>()
            {
                new School { SchoolName = "清华大学", Majors = new List<Major>(){
                    new Major{ MajorName = "物理系"},
                    new Major{ MajorName = "数学系"},
                } },
                new School { SchoolName = "北京大学", Majors = new List<Major>(){
                    new Major{ MajorName = "文学系"},
                    new Major{ MajorName = "历史系"},
                }},
                new School { SchoolName = "复旦大学", Majors = new List<Major>(){
                    new Major{ MajorName = "生物系"},
                    new Major{ MajorName = "化学系"},
                }},
                new School { SchoolName = "北京工业大学", Majors = new List<Major>(){
                    new Major{ MajorName = "工业控制"},
                    new Major{ MajorName = "计算机软件"},
                }},
                new School { SchoolName = "浙江大学", Majors = new List<Major>(){
                    new Major{ MajorName = "人文系"},
                    new Major{ MajorName = "经济系"},
                }},
            };

            var rv = schools.Where(x => x.SchoolName == id).SelectMany(x=>x.Majors).Select(x => new { Text = x.MajorName, Value = x.ID }).ToList();
            return Json(rv);
        }
    }
}
