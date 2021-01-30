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
        [Display(Name = "test")]
        public String Test { get; set; }
        public List<ComboSelectListItem> AllParents { get; set; }
        [Display(Name = "_Admin.Parent")]
        public Guid? ParentId { get; set; }

        protected override void InitVM()
        {
            AllParents = DC.Set<City>().GetSelectListItems(Wtm, y => y.Name);
        }

    }
}
