using System;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms
{
    public class FrameworkUserSearcher : BaseSearcher
    {
        [Display(Name = "Account")]
        public string ITCode { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

    }
}
