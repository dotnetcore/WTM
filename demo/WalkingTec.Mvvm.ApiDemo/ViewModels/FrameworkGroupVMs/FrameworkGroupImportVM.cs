using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core;


namespace WalkingTec.Mvvm.ApiDemo.ViewModels.FrameworkGroupVMs
{
    public class FrameworkGroupTemplateVM : BaseTemplateVM
    {
        [Display(Name = "用户组编码")]
        public ExcelPropety GroupCode_Excel = ExcelPropety.CreateProperty<FrameworkGroup>(x => x.GroupCode);
        [Display(Name = "用户组名称")]
        public ExcelPropety GroupName_Excel = ExcelPropety.CreateProperty<FrameworkGroup>(x => x.GroupName);
        [Display(Name = "备注")]
        public ExcelPropety GroupRemark_Excel = ExcelPropety.CreateProperty<FrameworkGroup>(x => x.GroupRemark);

	    protected override void InitVM()
        {
        }

    }

    public class FrameworkGroupImportVM : BaseImportVM<FrameworkGroupTemplateVM, FrameworkGroup>
    {

    }

}
