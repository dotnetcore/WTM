using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.EmployeeVMs
{
    public class EmployeeVM : BaseCRUDVM<Employee>
    {
        public List<ComboSelectListItem> AllCompanys { get; set; }

        public EmployeeVM()
        {
            SetInclude(x => x.Company);
        }

        protected override void InitVM()
        {
            AllCompanys = DC.Set<Company>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.Name);
        }

    }
}
