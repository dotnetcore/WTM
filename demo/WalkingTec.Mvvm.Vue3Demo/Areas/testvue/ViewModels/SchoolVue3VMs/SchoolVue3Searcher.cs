using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.ReactDemo.Models;


namespace WalkingTec.Mvvm.Vue3Demo.testvue.ViewModels.SchoolVue3VMs
{
    public partial class SchoolVue3Searcher : BaseSearcher
    {
        [Display(Name = "学校编码")]
        public String SchoolCode { get; set; }
        [Display(Name = "学校名称")]
        public String SchoolName { get; set; }
        [Display(Name = "学校类型")]
        public SchoolTypeEnum? SchoolType { get; set; }
        [Display(Name = "是学校")]
        public Boolean? IsSchool { get; set; }

        protected override void InitVM()
        {
        }

    }
}
