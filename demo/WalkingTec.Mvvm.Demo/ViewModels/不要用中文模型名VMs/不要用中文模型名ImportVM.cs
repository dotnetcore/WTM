using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.不要用中文模型名VMs
{
    public partial class 不要用中文模型名TemplateVM : BaseTemplateVM
    {
        public ExcelPropety 不要_Excel = ExcelPropety.CreateProperty<不要用中文模型名>(x => x.不要);
        public ExcelPropety 用_Excel = ExcelPropety.CreateProperty<不要用中文模型名>(x => x.用);
        public ExcelPropety 中文_Excel = ExcelPropety.CreateProperty<不要用中文模型名>(x => x.中文);
        public ExcelPropety 模型名_Excel = ExcelPropety.CreateProperty<不要用中文模型名>(x => x.模型名);

	    protected override void InitVM()
        {
        }

    }

    public class 不要用中文模型名ImportVM : BaseImportVM<不要用中文模型名TemplateVM, 不要用中文模型名>
    {

    }

}
