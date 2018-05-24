using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Doc.FrameworkUserVms
{
    public class FrameworkAllVM : BaseVM
    {
        public FrameworkUserVM Vm { get; set; }
        public FrameworkUserListVM ListVm { get; set; }
    }
}
