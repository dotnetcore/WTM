using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleVM : BaseCRUDVM<FrameworkRole>
    {
        public override DuplicatedInfo<FrameworkRole> SetDuplicatedCheck()
        {
            var rv = CreateFieldsInfo(SimpleField(x => x.RoleName));
            rv.AddGroup(SimpleField(x => x.RoleCode));
            return rv;
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            if (FC.ContainsKey("Entity.RoleCode"))
            {
                FC.Remove("Entity.RoleCode");
            }
            base.DoEdit(updateAllFields);
        }
    }
}
