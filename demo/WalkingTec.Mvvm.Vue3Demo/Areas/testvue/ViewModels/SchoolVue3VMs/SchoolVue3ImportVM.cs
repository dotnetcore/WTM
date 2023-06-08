using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.ReactDemo.Models;


namespace WalkingTec.Mvvm.Vue3Demo.testvue.ViewModels.SchoolVue3VMs
{
    public partial class SchoolVue3TemplateVM : BaseTemplateVM
    {
        [Display(Name = "学校编码")]
        public ExcelPropety SchoolCode_Excel = ExcelPropety.CreateProperty<SchoolVue3>(x => x.SchoolCode);
        [Display(Name = "学校名称")]
        public ExcelPropety SchoolName_Excel = ExcelPropety.CreateProperty<SchoolVue3>(x => x.SchoolName);
        [Display(Name = "学校类型")]
        public ExcelPropety SchoolType_Excel = ExcelPropety.CreateProperty<SchoolVue3>(x => x.SchoolType);
        [Display(Name = "备注")]
        public ExcelPropety Remark_Excel = ExcelPropety.CreateProperty<SchoolVue3>(x => x.Remark);
        [Display(Name = "级别")]
        public ExcelPropety Level_Excel = ExcelPropety.CreateProperty<SchoolVue3>(x => x.Level);
        [Display(Name = "地点")]
        public ExcelPropety Place_Excel = ExcelPropety.CreateProperty<SchoolVue3>(x => x.PlaceId);
        [Display(Name = "是学校")]
        public ExcelPropety IsSchool_Excel = ExcelPropety.CreateProperty<SchoolVue3>(x => x.IsSchool);

	    protected override void InitVM()
        {
            Place_Excel.DataType = ColumnDataType.ComboBox;
            Place_Excel.ListItems = DC.Set<City>().GetSelectListItems(Wtm, y => y.Name);
        }

    }

    public class SchoolVue3ImportVM : BaseImportVM<SchoolVue3TemplateVM, SchoolVue3>
    {

    }

}
