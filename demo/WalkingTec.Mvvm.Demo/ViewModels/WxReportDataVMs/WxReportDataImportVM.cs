using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.WxReportDataVMs
{
    public partial class WxReportDataTemplateVM : BaseTemplateVM
    {
        [Display(Name = "姓名")]
        public ExcelPropety ToWxUser_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.ToWxUser);
        [Display(Name = "姓名")]
        public ExcelPropety FrameworkUser_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.FrameworkUserId);
        [Display(Name = "报表日期")]
        public ExcelPropety Date_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.Date);
        [Display(Name = "报表类型")]
        public ExcelPropety Type_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.Type);
        [Display(Name = "数据类别")]
        public ExcelPropety DataType_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.DataType);
        [Display(Name = "土方1:米")]
        public ExcelPropety TuFang1_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.TuFang1);
        [Display(Name = "距离1:KM")]
        public ExcelPropety KM1_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.KM1);
        [Display(Name = "土方2:米")]
        public ExcelPropety TuFang2_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.TuFang2);
        [Display(Name = "距离2:KM")]
        public ExcelPropety KM2_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.KM2);
        [Display(Name = "土方3:米")]
        public ExcelPropety TuFang3_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.TuFang3);
        [Display(Name = "距离3:KM")]
        public ExcelPropety KM3_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.KM3);
        [Display(Name = "土方4:米")]
        public ExcelPropety TuFang4_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.TuFang4);
        [Display(Name = "距离4:KM")]
        public ExcelPropety KM4_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.KM4);
        [Display(Name = "土方5:米")]
        public ExcelPropety TuFang5_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.TuFang5);
        [Display(Name = "距离5:KM")]
        public ExcelPropety KM5_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.KM5);
        [Display(Name = "土方6:米")]
        public ExcelPropety TuFang6_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.TuFang6);
        [Display(Name = "距离6:KM")]
        public ExcelPropety KM6_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.KM6);
        [Display(Name = "淤泥1:米")]
        public ExcelPropety YnNi1_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.YnNi1);
        [Display(Name = "淤泥1:KM")]
        public ExcelPropety YnNi1KM_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.YnNi1KM);
        [Display(Name = "淤泥2:米")]
        public ExcelPropety YnNi2_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.YnNi2);
        [Display(Name = "淤泥2:KM")]
        public ExcelPropety YnNi2KM_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.YnNi2KM);
        [Display(Name = "淤泥3:米")]
        public ExcelPropety YnNi3_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.YnNi3);
        [Display(Name = "淤泥3:KM")]
        public ExcelPropety YnNi3KM_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.YnNi3KM);
        [Display(Name = "淤泥4:米")]
        public ExcelPropety YnNi4_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.YnNi4);
        [Display(Name = "淤泥1:KM")]
        public ExcelPropety YnNi4KM_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.YnNi4KM);
        [Display(Name = "加油1:升")]
        public ExcelPropety JiaYou1_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.JiaYou1);
        [Display(Name = "加油2:升")]
        public ExcelPropety JiaYou2_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.JiaYou2);
        [Display(Name = "就餐天数")]
        public ExcelPropety CanFeiRen_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.CanFeiRen);
        [Display(Name = "借支金额")]
        public ExcelPropety JieZhiMoney_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.JieZhiMoney);
        [Display(Name = "扩展字段")]
        public ExcelPropety Extend01_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.Extend01);
        [Display(Name = "扩展字段")]
        public ExcelPropety Extend02_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.Extend02);
        [Display(Name = "扩展字段")]
        public ExcelPropety Extend03_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.Extend03);
        [Display(Name = "扩展字段")]
        public ExcelPropety Extend04_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.Extend04);
        [Display(Name = "扩展字段")]
        public ExcelPropety Extend05_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.Extend05);
        [Display(Name = "扩展字段")]
        public ExcelPropety Extend07_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.Extend07);
        [Display(Name = "扩展字段")]
        public ExcelPropety Extend08_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.Extend08);
        [Display(Name = "扩展字段")]
        public ExcelPropety Extend09_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.Extend09);
        [Display(Name = "扩展字段")]
        public ExcelPropety Extend10_Excel = ExcelPropety.CreateProperty<WxReportData>(x => x.Extend10);

	    protected override async Task InitVM()
        {
            FrameworkUser_Excel.DataType = ColumnDataType.ComboBox;
            FrameworkUser_Excel.ListItems = await DC.Set<FrameworkUser>().GetSelectListItems(Wtm, y => y.Name);
        }

    }

    public class WxReportDataImportVM : BaseImportVM<WxReportDataTemplateVM, WxReportData>
    {

    }

}
