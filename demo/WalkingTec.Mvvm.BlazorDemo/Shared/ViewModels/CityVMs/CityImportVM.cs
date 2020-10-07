using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.BlazorDemo.Models;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModels.CityVMs
{
    public partial class CityTemplateVM : BaseTemplateVM
    {
        [Display(Name = "名称")]
        public ExcelPropety Name_Excel = ExcelPropety.CreateProperty<City>(x => x.Name);
        [Display(Name = "父级")]
        public ExcelPropety Parent_Excel = ExcelPropety.CreateProperty<City>(x => x.ParentId);

	    protected override void InitVM()
        {
            Parent_Excel.DataType = ColumnDataType.ComboBox;
            Parent_Excel.ListItems = DC.Set<City>().GetSelectListItems(WtmContext, null, y => y.Name);
        }

    }

    public class CityImportVM : BaseImportVM<CityTemplateVM, City>
    {

    }

}
