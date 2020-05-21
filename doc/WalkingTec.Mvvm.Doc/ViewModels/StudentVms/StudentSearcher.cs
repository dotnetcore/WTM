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
        [Display(Name = "Account")]
        public string LoginName { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Sex")]
        public Models.SexEnum? Sex { get; set; }

        public Guid? DepId { get; set; }
    }
}
