using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.LinkTestVMs
{
    public partial class LinkTestVM : BaseCRUDVM<LinkTest>
    {
        //public List<ComboSelectListItem> AllStudents { get; set; }
        //public List<ComboSelectListItem> AllSchools { get; set; }
        //public List<ComboSelectListItem> AllMajors { get; set; }

        public List<int?> SelectedSchool { get; set; }
        public List<Guid> SelectedMajor { get; set; }
        public LinkTestVM()
        {
            SetInclude(x => x.Student);
        }

        protected override void InitVM()
        {
            //AllStudents = DC.Set<Student>().GetSelectListItems(Wtm, y => y.Name);
            //AllSchools = DC.Set<School>().GetSelectListItems(Wtm, y => y.SchoolName);
            //AllMajors = DC.Set<Major>().GetSelectListItems(Wtm, y => y.MajorName);
            if (Entity.HasID())
            {
                //SelectedMajor = DC.Set<StudentMajor>().Where(x => x.StudentId == Entity.StudentId).Select(x => x.MajorId).ToList();
                //SelectedSchool = DC.Set<Major>().Where(y => SelectedMajor.Contains(y.ID)).Select(z => z.SchoolId).ToList();
                //AllMajors = DC.Set<Major>().Where(x => SelectedSchool.Contains(x.SchoolId)).GetSelectListItems(Wtm, x => x.MajorName);
                //AllStudents = DC.Set<Student>().Where(x => DC.Set<StudentMajor>().Where(y => SelectedMajor.Contains(y.MajorId)).Select(z => z.StudentId).Contains(x.ID)).GetSelectListItems(Wtm, x => x.Name);
            }
        }

        public override void DoAdd()
        {           
            base.DoAdd();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            base.DoEdit(updateAllFields);
        }

        public override void DoDelete()
        {
            base.DoDelete();
        }
    }
}
