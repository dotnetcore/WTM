using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.EmployeeVMs
{
    public class EmployeeSearcher : BaseSearcher
    {
        public List<ComboSelectListItem> AllCompanys { get; set; }
        [Display(Name = "所属公司")]
        public Guid CompanyID { get; set; }
        [Display(Name = "员工姓名")]
        public String Name { get; set; }
        [Display(Name = "员工性别")]
        public SexEnum Sex { get; set; }

        protected override void InitVM()
        {
            AllCompanys = DC.Set<Company>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.Name);
        }

    }
}
