using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.ISOTypeVMs
{
    public partial class ISOTypeTemplateVM : BaseTemplateVM
    {
        [Display(Name = "ISO名称")]
        public ExcelPropety IsoName_Excel = ExcelPropety.CreateProperty<ISOType>(x => x.IsoName);
        [Display(Name = " 版本号")]
        public ExcelPropety ISOVerSion_Excel = ExcelPropety.CreateProperty<ISOType>(x => x.ISOVerSion);
        [Display(Name = " 附加信息")]
        public ExcelPropety Description_Excel = ExcelPropety.CreateProperty<ISOType>(x => x.Description);

	    protected override void InitVM()
        {
        }

    }

    public class ISOTypeImportVM : BaseImportVM<ISOTypeTemplateVM, ISOType>
    {

    }

}
