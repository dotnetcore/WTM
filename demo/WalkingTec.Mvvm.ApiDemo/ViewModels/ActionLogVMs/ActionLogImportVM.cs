using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core;


namespace WalkingTec.Mvvm.ApiDemo.ViewModels.ActionLogVMs
{
    public partial class ActionLogTemplateVM : BaseTemplateVM
    {
        [Display(Name = "模块")]
        public ExcelPropety ModuleName_Excel = ExcelPropety.CreateProperty<ActionLog>(x => x.ModuleName);
        [Display(Name = "动作")]
        public ExcelPropety ActionName_Excel = ExcelPropety.CreateProperty<ActionLog>(x => x.ActionName);
        [Display(Name = "ITCode")]
        public ExcelPropety ITCode_Excel = ExcelPropety.CreateProperty<ActionLog>(x => x.ITCode);
        [Display(Name = "Url")]
        public ExcelPropety ActionUrl_Excel = ExcelPropety.CreateProperty<ActionLog>(x => x.ActionUrl);
        [Display(Name = "操作时间")]
        public ExcelPropety ActionTime_Excel = ExcelPropety.CreateProperty<ActionLog>(x => x.ActionTime);
        [Display(Name = "时长")]
        public ExcelPropety Duration_Excel = ExcelPropety.CreateProperty<ActionLog>(x => x.Duration);
        [Display(Name = "备注")]
        public ExcelPropety Remark_Excel = ExcelPropety.CreateProperty<ActionLog>(x => x.Remark);
        [Display(Name = "IP")]
        public ExcelPropety IP_Excel = ExcelPropety.CreateProperty<ActionLog>(x => x.IP);
        [Display(Name = "类型")]
        public ExcelPropety LogType_Excel = ExcelPropety.CreateProperty<ActionLog>(x => x.LogType);

	    protected override void InitVM()
        {
        }

    }

    public class ActionLogImportVM : BaseImportVM<ActionLogTemplateVM, ActionLog>
    {

    }

}
