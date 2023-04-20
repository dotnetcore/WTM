// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

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

        public override async Task DoAdd()
        {
            await base.DoAdd();
            Wtm.RemoveRoleCache((await Wtm.GetLoginUserInfo ()).CurrentTenant).Wait();
        }

        public override async Task DoEdit(bool updateAllFields = false)
        {
            await base.DoEdit(updateAllFields);
            Wtm.RemoveRoleCache((await Wtm.GetLoginUserInfo ()).CurrentTenant).Wait();
        }

        public override async Task DoDelete()
        {
            using (var tran = DC.BeginTransaction())
            {
                try
                {
                    await base.DoDelete();
                    var ur = DC.Set<FrameworkUserRole>().Where(x => x.RoleCode == Entity.RoleCode);
                    DC.Set<FrameworkUserRole>().RemoveRange(ur);
                    await DC.SaveChangesAsync();
                    tran.Commit();
                    await Wtm.RemoveUserCacheByRole(Entity.RoleCode);
                    await Wtm.RemoveRoleCache((await Wtm.GetLoginUserInfo ()).CurrentTenant);
                }
                catch
                {
                    tran.Rollback();
                }
            }
        }
    }
}
