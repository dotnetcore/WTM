using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.SoftFacInfoVMs
{
    public partial class SoftFacInfoTemplateVM : BaseTemplateVM
    {
        [Display(Name = "exe名称")]
        public ExcelPropety IsoName_Excel = ExcelPropety.CreateProperty<SoftFacInfo>(x => x.IsoName);
        [Display(Name = " 版本号")]
        public ExcelPropety EXEVerSion_Excel = ExcelPropety.CreateProperty<SoftFacInfo>(x => x.EXEVerSion);
        [Display(Name = " 附加信息")]
        public ExcelPropety Description_Excel = ExcelPropety.CreateProperty<SoftFacInfo>(x => x.Description);

	    protected override void InitVM()
        {
        }

    }

    public class SoftFacInfoImportVM : BaseImportVM<SoftFacInfoTemplateVM, SoftFacInfo>
    {

    }

}
