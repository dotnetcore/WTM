using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Doc.ViewModels.StudentVms;

namespace WalkingTec.Mvvm.Doc.FrameworkUserVms
{
    public class FrameworkAllVM : BaseVM
    {
        public StudentVm Vm { get; set; }
        public StudentListVm ListVm { get; set; }

        public FrameworkAllVM()
        {
            Vm = new StudentVm();
            ListVm = new StudentListVm();
        }

        protected override void InitVM()
        {
            Vm.CopyContext(this);
            ListVm.CopyContext(this);
        }
    }
}
