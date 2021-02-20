using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.SchoolVMs
{
    public class SchoolSearcher : BaseSearcher
    {
        [Display(Name = "学校编码")]
        public String SchoolCode { get; set; }
        [Display(Name = "学校名称")]
        public String SchoolName { get; set; }
        [Display(Name = "学校类型")]
        public SchoolTypeEnum? SchoolType { get; set; }

        public Guid? CityId { get; set; }
        protected override void InitVM()
        {
            var test = DC.Set<School>().ToList();
        }

    }
}
