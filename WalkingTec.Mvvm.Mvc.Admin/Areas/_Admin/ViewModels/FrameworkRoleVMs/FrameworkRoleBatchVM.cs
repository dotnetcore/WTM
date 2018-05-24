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

        protected override bool CheckIfCanDelete(Guid id, out string errorMessage)
        {
            var check = DC.Set<FrameworkUserRole>().Any(x => x.RoleId == id);
            if (check == true)
            {
                errorMessage = "角色已被使用，无法删除";
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
