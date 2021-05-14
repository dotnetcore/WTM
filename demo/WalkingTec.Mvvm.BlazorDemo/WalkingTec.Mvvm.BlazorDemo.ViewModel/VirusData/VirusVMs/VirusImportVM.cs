using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models.Virus;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.VirusData.VirusVMs
{
    public partial class VirusTemplateVM : BaseTemplateVM
    {
        [Display(Name = "病毒名称")]
        public ExcelPropety VirtusName_Excel = ExcelPropety.CreateProperty<Virus>(x => x.VirtusName);
        [Display(Name = "病毒代码")]
        public ExcelPropety VirtusCode_Excel = ExcelPropety.CreateProperty<Virus>(x => x.VirtusCode);
        [Display(Name = "病毒描述")]
        public ExcelPropety Remark_Excel = ExcelPropety.CreateProperty<Virus>(x => x.Remark);
        [Display(Name = "病毒种类")]
        public ExcelPropety VirtusType_Excel = ExcelPropety.CreateProperty<Virus>(x => x.VirtusType);

	    protected override void InitVM()
        {
        }

    }

    public class VirusImportVM : BaseImportVM<VirusTemplateVM, Virus>
    {

    }

}
