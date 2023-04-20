using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.BasicData.StudentVMs
{
    public partial class StudentVM : BaseCRUDVM<Student>
    {
        [Display(Name = "专业")]
        public List<string> SelectedStudentMajorIDs { get; set; }

        public StudentVM()
        {
            SetInclude(x => x.StudentMajor);
        }

        protected override async Task InitVM()
        {
            SelectedStudentMajorIDs = Entity.StudentMajor?.Select(x => x.MajorId.ToString()).ToList();
        }

        public override async Task DoAdd()
        {
            Entity.StudentMajor = new List<StudentMajor>();
            if (SelectedStudentMajorIDs != null)
            {
                foreach (var id in SelectedStudentMajorIDs)
                {
                     StudentMajor middle = new StudentMajor();
                    middle.SetPropertyValue("MajorId", id);
                    Entity.StudentMajor.Add(middle);
                }
            }
           
            await base.DoAdd();
        }

        public override async Task DoEdit(bool updateAllFields = false)
        {
            Entity.StudentMajor = new List<StudentMajor>();
            if(SelectedStudentMajorIDs != null )
            {
                 foreach (var item in SelectedStudentMajorIDs)
                {
                    StudentMajor middle = new StudentMajor();
                    middle.SetPropertyValue("MajorId", item);
                    Entity.StudentMajor.Add(middle);
                }
            }

            await base.DoEdit(updateAllFields);
        }

        public override async Task DoDelete()
        {
            base.DoDelete();
        }
    }
}
