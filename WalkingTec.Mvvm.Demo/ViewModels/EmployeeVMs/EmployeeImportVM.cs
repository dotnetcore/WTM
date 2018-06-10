using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.EmployeeVMs
{
    public class EmployeeTemplateVM : BaseTemplateVM
    {
        [Display(Name = "所属公司")]
        public ExcelPropety CompanyID_Excel = ExcelPropety.CreateProperty<Employee>(x => x.CompanyID);
        [Display(Name = "员工姓名")]
        public ExcelPropety Name_Excel = ExcelPropety.CreateProperty<Employee>(x => x.Name);
        [Display(Name = "员工年龄")]
        public ExcelPropety Age_Excel = ExcelPropety.CreateProperty<Employee>(x => x.Age);
        [Display(Name = "员工性别")]
        public ExcelPropety Sex_Excel = ExcelPropety.CreateProperty<Employee>(x => x.Sex);

	    protected override void InitVM()
        {
            CompanyID_Excel.DataType = ColumnDataType.ComboBox;
            CompanyID_Excel.ListItems = DC.Set<Company>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.Name);
        }

    }

    public class EmployeeImportVM : BaseImportVM<EmployeeTemplateVM, Employee>
    {

    }

}
