using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.ReactDemo.Models;


namespace WalkingTec.Mvvm.ReactDemo.ViewModels.SchoolVMs
{
    public partial class SchoolSearcher : BaseSearcher
    {
        [Display(Name = "学校编码")]
        public String SchoolCode { get; set; }
        [Display(Name = "学校名称")]
        public String SchoolName { get; set; }
        [Display(Name = "地点")]
        public Guid? PlaceId { get; set; }

        protected override async Task InitVM()
        {
        }

    }
}
