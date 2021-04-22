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

        public StudentVM()
        {
            SetInclude(x => x.StudentMajor);
            MajorList = new MajorListVM();
        }

        protected override void InitVM()
        {
            AllStudentMajors = DC.Set<Major>().GetSelectListItems(Wtm, y => y.MajorName);
        }

        public override void DoDelete()
        {
            base.DoDelete();
        }
    }
}
