// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkGroupVMs
{
    public class FrameworkGroupVM : BaseCRUDVM<FrameworkGroup>
    {
        public override DuplicatedInfo<FrameworkGroup> SetDuplicatedCheck()
        {
            var rv = CreateFieldsInfo(SimpleField(x => x.GroupName));
            rv.AddGroup(SimpleField(x => x.GroupCode));
            return rv;
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            if (FC.ContainsKey("Entity.GroupCode"))
            {
                FC.Remove("Entity.GroupCode");
            }

            base.DoEdit(updateAllFields);
        }

        public override async Task DoDeleteAsync()
        {
            using (var tran = DC.BeginTransaction())
            {
                try
                {
                    await base.DoDeleteAsync();
                    var ur = DC.Set<FrameworkUserGroup>().Where(x => x.GroupCode == Entity.GroupCode);
                    DC.Set<FrameworkUserGroup>().RemoveRange(ur);
                    DC.SaveChanges();
                    tran.Commit();
                    await Wtm.RemoveUserCache(ur.Select(x => x.UserCode).ToArray());
                }
                catch
                {
                    tran.Rollback();
                }
            }
        }

    }
}
