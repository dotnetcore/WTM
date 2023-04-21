using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.ReactDemo.Models;


namespace WalkingTec.Mvvm.ReactDemo.ViewModels.SchoolVMs
{
    public partial class SchoolTemplateVM : BaseTemplateVM
    {
        [Display(Name = "学校编码")]
        public ExcelPropety SchoolCode_Excel = ExcelPropety.CreateProperty<School>(x => x.SchoolCode);
        [Display(Name = "学校名称")]
        public ExcelPropety SchoolName_Excel = ExcelPropety.CreateProperty<School>(x => x.SchoolName);
        [Display(Name = "学校类型")]
        public ExcelPropety SchoolType_Excel = ExcelPropety.CreateProperty<School>(x => x.SchoolType);
        [Display(Name = "备注")]
        public ExcelPropety Remark_Excel = ExcelPropety.CreateProperty<School>(x => x.Remark);
        [Display(Name = "级别")]
        public ExcelPropety Level_Excel = ExcelPropety.CreateProperty<School>(x => x.Level);
        [Display(Name = "地点")]
        public ExcelPropety Place_Excel = ExcelPropety.CreateProperty<School>(x => x.PlaceId);

	    protected override async Task InitVM()
        {
            Place_Excel.DataType = ColumnDataType.ComboBox;
            Place_Excel.ListItems = await DC.Set<City>().GetSelectListItems(Wtm, y => y.Name);
        }

    }

    public class SchoolImportVM : BaseImportVM<SchoolTemplateVM, School>
    {

    }

}
