// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleMDVM : BaseCRUDVM<FrameworkRole>
    {
        public FrameworkMenuListVM ListVM { get; set; }

        public FrameworkRoleMDVM()
        {
            ListVM = new FrameworkMenuListVM();
        }
        protected override FrameworkRole GetById(object Id)
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                return Wtm.CallAPI<FrameworkRoleVM>("mainhost", $"/api/_frameworkrole/{Id}").Result.Data.Entity;
            }
            else
            {
                return base.GetById(Id);
            }
        }

        protected override void InitVM()
        {
            ListVM.CopyContext(this);
            ListVM.Searcher.RoleCode = Entity.RoleCode;
        }

        public async Task<bool> DoChangeAsync()
        {
            var all = FC.Where(x => x.Key.StartsWith("menu_")).ToList();
            List<Guid> AllowedMenuIds = all.Where(x => x.Value.ToString() == "1").Select(x => Guid.Parse(x.Key.Replace("menu_", ""))).ToList();
            var torem = AllowedMenuIds.Distinct();
            var oldIDs = DC.Set<FunctionPrivilege>().Where(x => x.RoleCode == Entity.RoleCode).Select(x => x.ID).ToList();

            foreach (var oldid in oldIDs)
            {
                FunctionPrivilege fp = new FunctionPrivilege { ID = oldid };
                DC.Set<FunctionPrivilege>().Attach(fp);
                DC.DeleteEntity(fp);
            }
            foreach (var menuid in AllowedMenuIds)
            {
                FunctionPrivilege fp = new FunctionPrivilege();
                fp.MenuItemId = menuid;
                fp.RoleCode = Entity.RoleCode;
                fp.TenantCode = LoginUserInfo.CurrentTenant;
                fp.Allowed = true;
                DC.Set<FunctionPrivilege>().Add(fp);
            }
            await DC.SaveChangesAsync();
            await Wtm.RemoveUserCacheByRole(Entity.RoleCode);
            return true;
        }

    }
}
