using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Demo.ViewModels.DepartmentVMs
{

    public partial class DepartmentTemplateVM : BaseTemplateVM
    {
        [Display(Name = "部门名称")]
        public ExcelPropety DepName_Excel = ExcelPropety.CreateProperty<Department>(x => x.DepName);
        [Display(Name = "_Admin.Parent")]
        public ExcelPropety Parent_Excel = ExcelPropety.CreateProperty<Department>(x => x.ParentId);

        protected override void InitVM()
        {
            Parent_Excel.DataType = ColumnDataType.ComboBox;
            Parent_Excel.ListItems = DC.Set<Department>().GetSelectListItems(Wtm, y => y.DepName);
        }

    }

    public class DepartmentImportVM : BaseImportVM<DepartmentTemplateVM, Department>
    {
    }
}
