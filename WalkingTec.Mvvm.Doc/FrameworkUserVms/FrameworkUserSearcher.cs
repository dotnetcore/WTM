using System;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Doc.FrameworkUserVms
{
    public class FrameworkUserSearcher : BaseSearcher
    {
        [Display(Name = "账号")]
        public string ITCode { get; set; }

        [Display(Name = "名字")]
        public string Name { get; set; }

    }
}
