using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.MajorVMs
{
    public partial class MajorApiSearcher : BaseSearcher
    {
        [Display(Name = "专业类别")]
        public MajorTypeEnum? MajorType { get; set; }
        [Display(Name = "备注")]
        public String Remark { get; set; }
        [Display(Name = "所属学校")]
        public int? SchoolId { get; set; }
        [Display(Name = "学生")]
        public List<string> SelectedStudentMajorsIDs { get; set; }

        protected override async Task InitVM()
        {
        }

    }
}
