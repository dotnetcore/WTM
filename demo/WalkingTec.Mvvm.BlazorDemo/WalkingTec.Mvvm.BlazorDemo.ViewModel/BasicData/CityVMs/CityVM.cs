using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.BasicData.CityVMs
{
    public partial class CityVM : BaseCRUDVM<City>
    {

        public CityVM()
        {
            SetInclude(x => x.Parent);
            SetInclude(x => x.Children);
        }

        protected override async Task InitVM()
        {
        }
    }
}
