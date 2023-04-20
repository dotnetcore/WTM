// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public override async Task Validate()
        {
            if (string.IsNullOrEmpty(Entity.Manager) == false)
            {
                var user = DC.Set<FrameworkUser>().Where(x => x.ITCode == Entity.Manager).FirstOrDefault();
                if (user == null)
                {
                    MSD.AddModelError("Entity.Manager", Localizer["Sys.CannotFindUser", Entity.Manager]);
                }
            }
            base.Validate();
        }

        public override async Task DoAdd()
        {
            await base.DoAdd ();
            Wtm.RemoveGroupCache((await GetLoginUserInfo ()).CurrentTenant).Wait();
        }

        public override async Task DoEdit(bool updateAllFields = false)
        {
            await base.DoEdit (updateAllFields);
            Wtm.RemoveGroupCache((await GetLoginUserInfo ()).CurrentTenant).Wait();
        }

        public override async Task DoDelete()
        {
            using (var tran = DC.BeginTransaction())
            {
                try
                {
                    await base.DoDelete();
                    var ur = DC.Set<FrameworkUserGroup>().Where(x => x.GroupCode == Entity.GroupCode);
                    DC.Set<FrameworkUserGroup>().RemoveRange(ur);
                    await DC.SaveChangesAsync();
                    tran.Commit();
                    await Wtm.RemoveUserCacheByGroup(Entity.GroupCode);
                    await Wtm.RemoveGroupCache((await GetLoginUserInfo ()).CurrentTenant);
                }
                catch
                {
                    tran.Rollback();
                }
            }
        }
    }


}
