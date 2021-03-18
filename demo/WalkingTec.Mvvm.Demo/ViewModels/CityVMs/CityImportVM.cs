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
    public partial class CityTemplateVM : BaseTemplateVM
    {
        [Display(Name = "名称")]
        public ExcelPropety Name_Excel = ExcelPropety.CreateProperty<City>(x => x.Name);
        [Display(Name = "二级")]
        public ExcelPropety sub1 = ExcelPropety.CreateProperty<City>(x => x.Children[0].Name);

        protected override void InitVM()
        {
        }

    }

    public class CityImportVM : BaseImportVM<CityTemplateVM, City>
    {

    }

}
