// WTM默认页面 Wtm buidin page
using System;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms
{
    public class FrameworkUserSearcher : BaseSearcher
    {
        [Display(Name = "_Admin.Account")]
        public string ITCode { get; set; }

        [Display(Name = "_Admin.Name")]
        public string Name { get; set; }
        [Display(Name = "_Admin.IsValid")]
        public bool? IsValid { get; set; }

    }
}
