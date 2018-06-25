using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo.ViewModels.EmployeeVMs;

namespace WalkingTec.Mvvm.Demo.ViewModels.TestRoleVMs
{
    public class TestRoleVM : BaseCRUDVM<TestRole>
    {
        public EmployeeListVM ListVM { get; set; }
        public TestRoleVM()
        {
        }

        protected override void InitVM()
        {
            ListVM = new EmployeeListVM();
            ListVM.CopyContext(this);
            ListVM.DoInit();
            ListVM.DetailGridPrix = "Entity.Employees";
        }

    }
}
