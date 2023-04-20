using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.CityVMs
{
    public partial class CitySearcher : BaseSearcher
    {
        [Display(Name = "名称")]
        public String Name { get; set; }
        public Guid selectedparent { get; set; }
        public List<TreeSelectListItem> Items { get; set; }
        protected override async Task InitVM()
        {
            Items = DC.Set<City>().GetTreeSelectListItems(Wtm, x => x.Name);
        }

    }
}
