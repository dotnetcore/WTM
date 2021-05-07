using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.BasicData.MajorVMs
{
    public partial class MajorVM : BaseCRUDVM<Major>
    {
        [Display(Name = "学生")]
        public List<string> SelectedStudentMajorsIDs { get; set; }

        public MajorVM()
        {
            SetInclude(x => x.School);
            SetInclude(x => x.StudentMajors);
        }

        protected override void InitVM()
        {
            SelectedStudentMajorsIDs = Entity.StudentMajors?.Select(x => x.StudentId).ToList();
        }

        public override void DoAdd()
        {
            Entity.StudentMajors = new List<StudentMajor>();
            if (SelectedStudentMajorsIDs != null)
            {
                foreach (var id in SelectedStudentMajorsIDs)
                {
                    Entity.StudentMajors.Add(new StudentMajor { StudentId = id });
                }
            }
           
            base.DoAdd();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            Entity.StudentMajors = new List<StudentMajor>();
            if(SelectedStudentMajorsIDs != null )
            {
                SelectedStudentMajorsIDs.ForEach(x => Entity.StudentMajors.Add(new StudentMajor { StudentId = x }));
            }

            base.DoEdit(updateAllFields);
        }

        public override void DoDelete()
        {
            base.DoDelete();
        }
    }
}
