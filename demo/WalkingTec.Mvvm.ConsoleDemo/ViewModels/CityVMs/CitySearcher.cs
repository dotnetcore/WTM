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
        public List<ComboSelectListItem> AllParents { get; set; }
        [Display(Name = "父级")]
        public int? ParentId { get; set; }

        protected override async Task InitVM()
        {
            AllParents = await DC.Set<City>().GetSelectListItems(Wtm, y => y.Name);
        }

    }
}
