using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.VueDemo.ViewModels.StudentVMs
{
    public partial class StudentSearcher : BaseSearcher
    {
        [Display(Name = "姓名")]
        public String Name { get; set; }
        [Display(Name = "性别")]
        public GenderEnum? Sex { get; set; }
        [Display(Name = "邮编")]
        public String ZipCode { get; set; }
        [Display(Name = "专业")]
        public List<Guid> SelectedStudentMajorIDs { get; set; }

        protected override async Task InitVM()
        {
        }

    }
}
