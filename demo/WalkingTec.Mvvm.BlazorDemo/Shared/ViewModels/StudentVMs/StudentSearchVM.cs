using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.BlazorDemo.ViewModels.StudentVMs
{
    public class StudentSearchVM : BaseVM
    {
        public StudentListVM ValidList { get; set; }
        public StudentListVM InValidList { get; set; }

        public StudentSearchVM()
        {
            ValidList = new StudentListVM();
            InValidList = new StudentListVM();
        }

        protected override void InitVM()
        {
            ValidList.CopyContext(this);
            InValidList.CopyContext(this);
        }
    }
}
