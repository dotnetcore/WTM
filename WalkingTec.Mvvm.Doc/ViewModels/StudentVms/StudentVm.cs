using System;
using System.Collections.Generic;
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
        public List<string> SelectedSchools { get; set; }
        public string SelectedMajor { get; set; }
        protected override void InitVM()
        {
            AllSchools = new List<ComboSelectListItem>()
            {
                new ComboSelectListItem{ Text = "清华大学", Value="清华大学"},
                new ComboSelectListItem{ Text = "北京大学", Value="北京大学"},
                new ComboSelectListItem{ Text = "北京工业大学", Value="北京工业大学"},
                new ComboSelectListItem{ Text = "复旦大学", Value="复旦大学"},
                new ComboSelectListItem{ Text = "浙江大学", Value="浙江大学"},
           };
        }
    }
}
