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

        public override void Validate()
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

        public override void DoAdd()
        {
            base.DoAdd();
            Cache.Delete(nameof(GlobalData.AllGroups));
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            if (FC.ContainsKey("Entity.GroupCode"))
            {
                FC.Remove("Entity.GroupCode");
            }

            base.DoEdit(updateAllFields);
            Cache.Delete(nameof(GlobalData.AllGroups));
        }

        public override async Task DoDeleteAsync()
        {
            Cache.Delete(nameof(GlobalData.AllGroups));
            using (var tran = DC.BeginTransaction())
            {
                try
                {
                    await base.DoDeleteAsync();
                    var ur = DC.Set<FrameworkUserGroup>().Where(x => x.GroupCode == Entity.GroupCode);
                    DC.Set<FrameworkUserGroup>().RemoveRange(ur);
                    DC.SaveChanges();
                    tran.Commit();
                    await Wtm.RemoveUserCacheByGroup(Entity.GroupCode);
                }
                catch
                {
                    tran.Rollback();
                }
            }
        }
    }


}
