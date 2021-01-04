using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleBatchVM : BaseBatchVM<FrameworkRole, BaseVM>
    {
        public FrameworkRoleBatchVM()
        {
            ListVM = new FrameworkRoleListVM();
        }


        public override bool DoBatchDelete()
        {
            using(var trans = DC.BeginTransaction())
            {
                var userorleids = DC.Set<FrameworkUserRole>().AsNoTracking().Where(y => DC.Set<FrameworkRole>().CheckIDs(Ids.ToList(),null).Select(x => x.RoleCode).Contains(y.RoleCode)).Select(x => x.ID);
                foreach (var item in userorleids)
                {
                    FrameworkUserRole f = new FrameworkUserRole { ID = item };
                    DC.DeleteEntity(f);
                }
                DC.SaveChanges();
                base.DoBatchDelete();
                if(MSD.IsValid == true)
                {
                    trans.Commit();
                }
                else
                {
                    trans.Rollback();
                }
            }

            return base.DoBatchDelete();
        }
    }
}
