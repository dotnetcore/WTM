using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core;


namespace WalkingTec.Mvvm.ApiDemo.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleTemplateVM : BaseTemplateVM
    {
        [Display(Name = "角色编号")]
        public ExcelPropety RoleCode_Excel = ExcelPropety.CreateProperty<FrameworkRole>(x => x.RoleCode);
        [Display(Name = "角色名称")]
        public ExcelPropety RoleName_Excel = ExcelPropety.CreateProperty<FrameworkRole>(x => x.RoleName);
        [Display(Name = "备注")]
        public ExcelPropety RoleRemark_Excel = ExcelPropety.CreateProperty<FrameworkRole>(x => x.RoleRemark);

	    protected override void InitVM()
        {
        }

    }

    public class FrameworkRoleImportVM : BaseImportVM<FrameworkRoleTemplateVM, FrameworkRole>
    {

    }

}
