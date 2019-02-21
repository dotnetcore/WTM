using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core;


namespace WalkingTec.Mvvm.ApiDemo.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleSearcher : BaseSearcher
    {
        [Display(Name = "角色编号")]
        public String RoleCode { get; set; }
        [Display(Name = "角色名称")]
        public String RoleName { get; set; }

        protected override void InitVM()
        {
        }

    }
}
