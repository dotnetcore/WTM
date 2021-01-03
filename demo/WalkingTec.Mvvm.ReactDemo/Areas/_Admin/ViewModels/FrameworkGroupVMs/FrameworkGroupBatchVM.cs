using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkGroupVMs
{
    public class FrameworkGroupBatchVM : BaseBatchVM<FrameworkGroup, BaseVM>
    {
        public FrameworkGroupBatchVM()
        {
            ListVM = new FrameworkGroupListVM();
        }

        public override bool DoBatchDelete()
        {
            using (var trans = DC.BeginTransaction())
            {
                var userorleids = DC.Set<FrameworkUserGroup>().AsNoTracking().Where(y => DC.Set<FrameworkGroup>().CheckIDs(Ids.ToList(), null).Select(x => x.GroupCode).Contains(y.GroupCode)).Select(x => x.ID);
                foreach (var item in userorleids)
                {
                    FrameworkUserGroup f = new FrameworkUserGroup { ID = item };
                    DC.DeleteEntity(f);
                }
                DC.SaveChanges();
                base.DoBatchDelete();
                if (MSD.IsValid == true)
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
