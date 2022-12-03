using System.Collections.Generic;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Demo.ViewModels.DepartmentVMs
{
    public partial class DepartmentVM : BaseCRUDVM<Department>
    {
        public List<ComboSelectListItem> AllParents { get; set; }

        public DepartmentVM()
        {
            SetInclude(x => x.Parent);
        }

        protected override void InitVM()
        {
            AllParents = DC.Set<Department>().GetSelectListItems(Wtm, y => y.DepName);
        }

        public override void DoAdd()
        {
            base.DoAdd();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            base.DoEdit(updateAllFields);
        }

        public override void DoDelete()
        {
            base.DoDelete();
        }

    }
}
