using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models.Virus;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.HospitalVMs
{
    public partial class HospitalTemplateVM : BaseTemplateVM
    {
        [Display(Name = "医院名称")]
        public ExcelPropety Name_Excel = ExcelPropety.CreateProperty<Hospital>(x => x.Name);
        [Display(Name = "医院级别")]
        public ExcelPropety Level_Excel = ExcelPropety.CreateProperty<Hospital>(x => x.Level);
        [Display(Name = "医院地点")]
        public ExcelPropety Location_Excel = ExcelPropety.CreateProperty<Hospital>(x => x.LocationId);

	    protected override async Task InitVM()
        {
            Location_Excel.DataType = ColumnDataType.ComboBox;
            Location_Excel.ListItems = await DC.Set<City>().GetSelectListItems(Wtm, y => y.Name);
        }

    }

    public class HospitalImportVM : BaseImportVM<HospitalTemplateVM, Hospital>
    {

    }

}
