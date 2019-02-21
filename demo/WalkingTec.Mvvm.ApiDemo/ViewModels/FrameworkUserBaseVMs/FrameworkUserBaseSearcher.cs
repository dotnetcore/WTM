using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core;


namespace WalkingTec.Mvvm.ApiDemo.ViewModels.FrameworkUserBaseVMs
{
    public class FrameworkUserBaseSearcher : BaseSearcher
    {
        [Display(Name = "账号")]
        public String ITCode { get; set; }
        [Display(Name = "姓名")]
        public String Name { get; set; }
        [Display(Name = "性别")]
        public SexEnum? Sex { get; set; }
        [Display(Name = "是否有效")]
        public Boolean? IsValid { get; set; }

        protected override void InitVM()
        {
        }

    }
}
