using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.ApiDemo.Models;


namespace WalkingTec.Mvvm.ApiDemo.ViewModels.CityVMs
{
    public class CityBatchVM : BaseBatchVM<City, City_BatchEdit>
    {
        public CityBatchVM()
        {
            ListVM = new CityListVM();
            LinkedVM = new City_BatchEdit();
        }

        protected override bool CheckIfCanDelete(Guid id, out string errorMessage)
        {
            errorMessage = null;
			return true;
        }
    }

	/// <summary>
    /// 批量编辑字段类
    /// </summary>
    public class City_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}
