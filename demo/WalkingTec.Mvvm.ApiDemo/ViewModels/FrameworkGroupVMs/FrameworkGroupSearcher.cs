using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core;


namespace WalkingTec.Mvvm.ApiDemo.ViewModels.FrameworkGroupVMs
{
    public class FrameworkGroupSearcher : BaseSearcher
    {
        [Display(Name = "用户组编码")]
        public String GroupCode { get; set; }
        [Display(Name = "用户组名称")]
        public String GroupName { get; set; }

        protected override void InitVM()
        {
        }

    }
}
