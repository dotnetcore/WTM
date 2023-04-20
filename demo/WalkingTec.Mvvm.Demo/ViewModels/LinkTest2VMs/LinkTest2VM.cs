using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.LinkTest2VMs
{
    public partial class LinkTest2VM : BaseCRUDVM<LinkTest2>
    {
        public List<ComboSelectListItem> AllStudents { get; set; }
        public List<ComboSelectListItem> AllSchools { get; set; }
        public List<ComboSelectListItem> AllMajors { get; set; }

        public List<int?> SelectedSchool { get; set; }
        public List<Guid> SelectedMajor { get; set; }

        public LinkTest2VM()
        {
            SetInclude(x => x.LinkStudent);
        }

        protected override async Task InitVM()
        {
            //AllStudents = DC.Set<Student>().GetSelectListItems(Wtm, y => y.Name);
            AllSchools = DC.Set<School>().GetSelectListItems(Wtm, y => y.SchoolName);
            //AllMajors = DC.Set<Major>().GetSelectListItems(Wtm, y => y.MajorName);
            if (Entity.HasID())
            {
                SelectedMajor = DC.Set<StudentMajor>().Where(x => Entity.LinkStudent.Select(y=>y.StudentId).Contains(x.StudentId)).Select(x => x.MajorId).ToList();
                SelectedSchool = DC.Set<Major>().Where(y => SelectedMajor.Contains(y.ID)).Select(z => z.SchoolId).ToList();
                AllMajors = DC.Set<Major>().Where(x => SelectedSchool.Contains(x.SchoolId)).GetSelectListItems(Wtm, x => x.MajorName);
                AllStudents = DC.Set<Student>().Where(x => DC.Set<StudentMajor>().Where(y => SelectedMajor.Contains(y.MajorId)).Select(z => z.StudentId).Contains(x.ID)).GetSelectListItems(Wtm, x => x.Name);
            }
        }


        public override async Task DoDelete()
        {
            base.DoDelete();
        }
    }
}
