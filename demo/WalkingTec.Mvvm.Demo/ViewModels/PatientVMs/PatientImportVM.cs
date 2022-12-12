using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models.Virus;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.PatientVMs
{
    public partial class PatientTemplateVM : BaseTemplateVM
    {
        [Display(Name = "患者姓名")]
        public ExcelPropety PatientName_Excel = ExcelPropety.CreateProperty<Patient>(x => x.PatientName);
        [Display(Name = "身份证")]
        public ExcelPropety IdNumber_Excel = ExcelPropety.CreateProperty<Patient>(x => x.IdNumber);
        [Display(Name = "性别")]
        public ExcelPropety Gender_Excel = ExcelPropety.CreateProperty<Patient>(x => x.Gender);
        [Display(Name = "状态")]
        public ExcelPropety Status_Excel = ExcelPropety.CreateProperty<Patient>(x => x.Status);
        [Display(Name = "生日")]
        public ExcelPropety Birthday_Excel = ExcelPropety.CreateProperty<Patient>(x => x.Birthday);
        [Display(Name = "籍贯")]
        public ExcelPropety Location_Excel = ExcelPropety.CreateProperty<Patient>(x => x.LocationId);
        [Display(Name = "所属医院")]
        public ExcelPropety Hospital_Excel = ExcelPropety.CreateProperty<Patient>(x => x.HospitalId);

	    protected override void InitVM()
        {
            Location_Excel.DataType = ColumnDataType.ComboBox;
            Location_Excel.ListItems = DC.Set<City>().GetSelectListItems(Wtm, y => y.Name);
            Hospital_Excel.DataType = ColumnDataType.ComboBox;
            Hospital_Excel.ListItems = DC.Set<Hospital>().GetSelectListItems(Wtm, y => y.Name);
        }

    }

    public class PatientImportVM : BaseImportVM<PatientTemplateVM, Patient>
    {

    }

}
