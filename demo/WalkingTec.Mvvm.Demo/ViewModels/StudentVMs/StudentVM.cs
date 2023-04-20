using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo.ViewModels.MajorVMs;

namespace WalkingTec.Mvvm.Demo.ViewModels.StudentVMs
{
    public partial class StudentVM : BaseCRUDVM<Student>
    {
        public List<ComboSelectListItem> AllStudentMajors { get; set; }
        public MajorListVM MajorList { get; set; }
        [Display(Name = "专业")]
        public List<Guid> SelectedStudentMajorIDs { get; set; }

        public StudentVM()
        {
            SetInclude(x => x.StudentMajor);
            MajorList = new MajorListVM();
        }

        protected override async Task InitVM()
        {
            AllStudentMajors = DC.Set<Major>().GetSelectListItems(Wtm, y => y.MajorName);
            //SelectedStudentMajorIDs = Entity.StudentMajor?.Select(x => x.MajorId).ToList();
        }

        //public override async Task DoAdd()
        //{
        //    Entity.StudentMajor = new List<StudentMajor>();
        //    if (SelectedStudentMajorIDs != null)
        //    {
        //        foreach (var id in SelectedStudentMajorIDs)
        //        {
        //            Entity.StudentMajor.Add(new StudentMajor { MajorId = id });
        //        }
        //    }
           
        //    await base.DoAdd();
        //}

        //public override async Task DoEdit(bool updateAllFields = false)
        //{
        //    Entity.StudentMajor = new List<StudentMajor>();
        //    if(SelectedStudentMajorIDs != null )
        //    {
        //        SelectedStudentMajorIDs.ForEach(x => Entity.StudentMajor.Add(new StudentMajor { ID = Guid.NewGuid(), MajorId = x }));
        //    }
        //    using (var trans = DC.BeginTransaction())
        //    {
        //        await base.DoEdit(updateAllFields);
        //        trans.Commit();
        //    }
        //}

        public override async Task DoDelete()
        {
            base.DoDelete();
        }
    }
}
