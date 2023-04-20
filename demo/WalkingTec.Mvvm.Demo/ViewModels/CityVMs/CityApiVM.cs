using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.CityVMs
{
    public partial class CityApiVM : BaseCRUDVM<City>
    {

        public CityApiVM()
        {
            SetInclude(x => x.Parent);
        }

        protected override Task InitVM()
        {
            return Task.CompletedTask;
        }
    }
}
