using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.MajorVMs
{
    public partial class MajorVM : BaseCRUDVM<Major>
    {
        public List<ComboSelectListItem> AllSchools { get; set; }
        public List<ComboSelectListItem> AllStudentMajorss { get; set; }
        [Display(Name = "学生")]
        [Required]
        public List<string> SelectedStudentMajorsIDs { get; set; }

        public MajorVM()
        {
            SetInclude(x => x.School);
            SetInclude(x => x.StudentMajors);
        }

        protected override async Task InitVM()
        {
            AllSchools = await DC.Set<School>().GetSelectListItems(Wtm, y => y.SchoolName);
            AllStudentMajorss = await DC.Set<Student>().GetSelectListItems(Wtm, y => y.Name);
            SelectedStudentMajorsIDs = Entity.StudentMajors?.Select(x => x.StudentId).ToList();
        }

        public override async Task DoAdd()
        {
            Entity.StudentMajors = new List<StudentMajor>();
            if (SelectedStudentMajorsIDs != null)
            {
                foreach (var id in SelectedStudentMajorsIDs)
                {
                    Entity.StudentMajors.Add(new StudentMajor { StudentId = id });
                }
            }
           
            await base.DoAdd();
        }

        public override async Task DoEdit(bool updateAllFields = false)
        {
            Entity.StudentMajors = new List<StudentMajor>();
            if(SelectedStudentMajorsIDs != null )
            {
                SelectedStudentMajorsIDs.ForEach(x => Entity.StudentMajors.Add(new StudentMajor { ID = Guid.NewGuid(), StudentId = x }));
            }

            await base.DoEdit(updateAllFields);
        }

        public override async Task DoDelete()
        {
            await base.DoDelete();
        }
    }
}
