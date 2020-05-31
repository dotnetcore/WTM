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
    public partial class CityBatchVM : BaseBatchVM<City, City_BatchEdit>
    {
        public CityBatchVM()
        {
            ListVM = new CityListVM();
            LinkedVM = new City_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class City_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}
