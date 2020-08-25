using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Doc.FrameworkUserVms;
using WalkingTec.Mvvm.Doc.Models;
using WalkingTec.Mvvm.Doc.ViewModels.DepartmentVms;
using WalkingTec.Mvvm.Doc.ViewModels.MajorVms;
using WalkingTec.Mvvm.Doc.ViewModels.SchoolVms;
using WalkingTec.Mvvm.Doc.ViewModels.StudentVms;
using WalkingTec.Mvvm.Mvc;

namespace WalkingTec.Mvvm.Doc.Controllers
{
    [AllowAnonymous]
    [ActionDescription("ViewLayer")]
    public class UIController : BaseController
    {
        [ActionDescription("Intro")]
        public IActionResult Intro()
        {
            var vm = CreateVM<FrameworkAllVM>();
            return PartialView(vm);
        }

        [ActionDescription("Layout")]
        public IActionResult Layout()
        {
            var vm = CreateVM<FrameworkAllVM>();
            return PartialView(vm);
        }

        [ActionDescription("Form")]
        public IActionResult Form()
        {
            var vm = CreateVM<SchoolVm>();
            return PartialView(vm);
        }

        [ActionDescription("TextBox")]
        public IActionResult TextBox()
        {
            var vm = CreateVM<MajorVm>();
            return PartialView(vm);
        }

        [ActionDescription("ComboBox")]
        public IActionResult ComboBox()
        {
            var vm = CreateVM<StudentVm>();
            return PartialView(vm);
        }

        [ActionDescription("CheckBox")]
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

        [ActionDescription("UploadMulti")]
        public IActionResult UploadMulti()
        {
            var vm = CreateVM<SchoolVm>();
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

        [ActionDescription("UEditor")]
        public IActionResult UEditor()
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

        [ActionDescription("Grid")]
        public IActionResult Grid()
        {
            var vm = CreateVM<StudentListVm>();
            return PartialView(vm);
        }


        [ActionDescription("Js")]
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

        [ActionDescription("Tree")]
        public IActionResult Tree()
        {
            var vm = CreateVM<DepartmentVM>();
            return PartialView(vm);
        }

        [ActionDescription("TreeContainer")]
        public IActionResult TreeContainer()
        {
            var vm = CreateVM<StudentListVm3>();
            return PartialView(vm);
        }

        [ActionDescription("Button")]
        public IActionResult Button()
        {
            return PartialView();
        }

        [ActionDescription("ColorPicker")]
        public IActionResult ColorPicker()
        {
            var vm = CreateVM<MajorVm>();
            return PartialView(vm);
        }


        public IActionResult GetSchool(string keywords)
        {
            List<School> schools = new List<School>()
            {
                new School { SchoolName = Localizer["U1"]},
                new School { SchoolName = Localizer["U2"]},
                new School { SchoolName = Localizer["U3"]},
                new School { SchoolName = Localizer["U4"]},
                new School { SchoolName = Localizer["U5"]},
            };
            var rv = schools.Where(x=>x.SchoolName.StartsWith(keywords)).Select(x => new { Text = x.SchoolName, Value = x.SchoolName }).ToList();
            return Json(rv);
        }

        public IActionResult GetMajorBySchool(string id)
        {
            List<School> schools = new List<School>()
            {
                new School { SchoolName = Localizer["U1"].ToString().ToLower().Replace(" ",""), Majors = new List<Major>(){
                    new Major{ MajorName = Localizer["U11"]},
                    new Major{ MajorName = Localizer["U12"]},
                } },
                new School { SchoolName = Localizer["U2"].ToString().ToLower().Replace(" ",""), Majors = new List<Major>(){
                    new Major{ MajorName = Localizer["U21"]},
                    new Major{ MajorName = Localizer["U22"]},
                }},
                new School { SchoolName = Localizer["U3"].ToString().ToLower().Replace(" ",""), Majors = new List<Major>(){
                    new Major{ MajorName = Localizer["U31"]},
                    new Major{ MajorName = Localizer["U32"]},
                }},
                new School { SchoolName = Localizer["U4"].ToString().ToLower().Replace(" ",""), Majors = new List<Major>(){
                    new Major{ MajorName = Localizer["U41"]},
                    new Major{ MajorName = Localizer["U42"]},
                }},
                new School { SchoolName = Localizer["U5"].ToString().ToLower().Replace(" ",""), Majors = new List<Major>(){
                    new Major{ MajorName = Localizer["U51"]},
                    new Major{ MajorName = Localizer["U52"]},
                }},
            };

            var rv = schools.Where(x => x.SchoolName == id).SelectMany(x=>x.Majors).Select(x => new { Text = x.MajorName, Value = x.MajorName }).ToList();
            return Json(rv);
        }

        public IActionResult GetStudentByMajor(string id)
        {
            List<ComboSelectListItem> rv = new List<ComboSelectListItem>();
            for(int i = 1; i <= 5; i++)
            {
                rv.Add(new ComboSelectListItem
                {
                    Text = id + Localizer["Student"] + i
                }); ;
            }
            return Json(rv);
        }

    }
}
