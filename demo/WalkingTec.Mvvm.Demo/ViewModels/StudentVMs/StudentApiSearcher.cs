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
    public partial class StudentApiSearcher : BaseSearcher
    {
        [Display(Name = "账号")]
        public String ID { get; set; }
        [Display(Name = "邮箱")]
        public String Email { get; set; }
        [Display(Name = "日期")]
        public DateRange EnRollDate { get; set; }

        protected override async Task InitVM()
        {
        }

    }
}
