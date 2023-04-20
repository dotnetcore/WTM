using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.BasicData.CityVMs
{
    public partial class CitySearcher : BaseSearcher
    {
        [Display(Name = "名称")]
        public String Name { get; set; }
        [Display(Name = "test")]
        public String Test { get; set; }
        [Display(Name = "_Admin.Parent")]
        public int? ParentId { get; set; }

        protected override async Task InitVM()
        {
        }

    }
}
