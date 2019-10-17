using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleBatchVM : BaseBatchVM<FrameworkRole, BaseVM>
    {
        public FrameworkRoleBatchVM()
        {
            ListVM = new FrameworkRoleListVM();
        }

        protected override bool CheckIfCanDelete(object id, out string errorMessage)
        {
            Guid? checkid = Guid.Parse(id.ToString());
            var check = DC.Set<FrameworkUserRole>().Any(x => x.RoleId == checkid);
            if (check == true)
            {
                errorMessage = Program._localizer["CannotDelete", Program._localizer["Role"]];
                return false;
            }
            else
            {
                errorMessage = null;
                return true;
            }
        }
    }
}
