using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Demo.ViewModels.DepartmentVMs
{
    public partial class DepartmentBatchVM : BaseBatchVM<Department, Department_BatchEdit>
    {
        public DepartmentBatchVM()
        {
            ListVM = new DepartmentListVM();
            LinkedVM = new Department_BatchEdit();
        }
    }
    /// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class Department_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }
}
