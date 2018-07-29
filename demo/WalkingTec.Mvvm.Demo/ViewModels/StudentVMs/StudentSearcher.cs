using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.StudentVMs
{
    public class StudentSearcher : BaseSearcher
    {
        [Display(Name = "账号")]
        public String LoginName { get; set; }
        [Display(Name = "姓名")]
        public String Name { get; set; }

        protected override void InitVM()
        {
        }

    }
}
