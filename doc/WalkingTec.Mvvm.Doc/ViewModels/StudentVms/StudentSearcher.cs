using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Doc.ViewModels.StudentVms
{
    public class StudentSearcher : BaseSearcher
    {
        [Display(Name = "账号")]
        public string LoginName { get; set; }

        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Display(Name = "性别")]
        public Models.SexEnum? Sex { get; set; }

    }
}
