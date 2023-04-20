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
    public partial class CityApiBatchVM : BaseBatchVM<City, CityApi_BatchEdit>
    {
        public CityApiBatchVM()
        {
            ListVM = new CityApiListVM();
            LinkedVM = new CityApi_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class CityApi_BatchEdit : BaseVM
    {
        [Display(Name = "名称")]
        public String Name { get; set; }
        [Display(Name = "test")]
        public String Test { get; set; }
        [Display(Name = "_Admin.Parent")]
        public Guid? ParentId { get; set; }

        protected override async Task InitVM()
        {
        }

    }

}
