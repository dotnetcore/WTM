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
        public List<Guid> SelectedStudentMajorIDs { get; set; }

        public StudentVM()
        {
            SetInclude(x => x.StudentMajor);
        }

        protected override void InitVM()
        {
            SelectedStudentMajorIDs = Entity.StudentMajor?.Select(x => x.MajorId).ToList();
        }

        public override void DoAdd()
        {
            Entity.StudentMajor = new List<StudentMajor>();
            if (SelectedStudentMajorIDs != null)
            {
                foreach (var id in SelectedStudentMajorIDs)
                {
                    Entity.StudentMajor.Add(new StudentMajor { MajorId = id });
                }
            }
           
            base.DoAdd();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            Entity.StudentMajor = new List<StudentMajor>();
            if(SelectedStudentMajorIDs != null )
            {
                SelectedStudentMajorIDs.ForEach(x => Entity.StudentMajor.Add(new StudentMajor { MajorId = x }));
            }

            base.DoEdit(updateAllFields);
        }

        public override void DoDelete()
        {
            base.DoDelete();
        }
    }
}
