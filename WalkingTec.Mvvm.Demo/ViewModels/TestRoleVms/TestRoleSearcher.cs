using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.bbb;


namespace WalkingTec.Mvvm.Demo.ViewModels.TestRoleVms
{
    public class TestRoleSearcher : BaseSearcher
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
