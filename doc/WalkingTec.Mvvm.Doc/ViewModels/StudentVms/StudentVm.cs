using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Doc.Models;

namespace WalkingTec.Mvvm.Doc.ViewModels.StudentVms
{
    public class StudentVm : BaseCRUDVM<Student>
    {
        public List<ComboSelectListItem> AllSchools { get; set; }
        public string SelectedSchool { get; set; }
        public string SelectedSchool2 { get; set; }
        public string SelectedSchool3 { get; set; }
        [Display(Name = "学校")]
        public List<string> SelectedSchools { get; set; }
        public string SelectedMajor { get; set; }

        [Display(Name = "学校")]
        public List<string> TransferSelectedSchools { get; set; }

        [Display(Name = "年龄1")]
        public int Age1 { get; set; }

        [Display(Name = "年龄2")]
        public int Age2 { get; set; }

        [Display(Name = "学生人数1")]
        public int StuCount0 { get; set; }
        [Display(Name = "学生人数")]
        public int StuCount1 { get; set; }
        [Display(Name = "学生人数2")]
        public int StuCount2 { get; set; }
        [Display(Name = "学生人数")]
        public int StuCount3 { get; set; }

        public StudentVm()
        {
            TransferSelectedSchools = new List<string>() { "清华大学", "北京工业大学" };
            AllSchools = new List<ComboSelectListItem>()
            {
                new ComboSelectListItem{ Text = "清华大学", Value = "清华大学"},
                new ComboSelectListItem{ Text = "北京大学", Value = "北京大学"},
                new ComboSelectListItem{ Text = "北京工业大学", Value = "北京工业大学", Disabled = true},
                new ComboSelectListItem{ Text = "复旦大学", Value = "复旦大学"},
                new ComboSelectListItem{ Text = "浙江大学", Value = "浙江大学"},
           };

            StuCount0 = 40;
            StuCount1 = 60;
            StuCount2 = 40;
            StuCount3 = 60;
        }
    }
}
