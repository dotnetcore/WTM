using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs
{
    public class PagePrivilegeVM : BaseCRUDVM<FrameworkMenu>
    {
        public List<ComboSelectListItem> AllRoles { get; set; }
        public List<ComboSelectListItem> AllUsers { get; set; }
        public List<ComboSelectListItem> AllUsers2 { get; set; }
        public List<Guid> SelectedRolesIDs { get; set; }
        public List<Guid> SelectedUsersID { get; set; }
        public List<Guid> SelectedRolesID2 { get; set; }
        public List<Guid> SelectedUsersID2 { get; set; }

        public PagePrivilegeVM()
        {
        }


        protected override void InitVM()
        {
            AllRoles = DC.Set<FrameworkRole>().GetSelectListItems(LoginUserInfo.DataPrivileges, null,y => y.RoleName);
            //AllUsers = DC.Set<FrameworkUserBase>().GetSelectListItems(null, x => x.ITCode, LoginUserInfo.DataPrivileges);

            SelectedRolesIDs = new List<Guid>();
            SelectedRolesIDs.AddRange(DC.Set<FunctionPrivilege>().Where(x => x.MenuItemId == Entity.ID && x.RoleId != null && x.Allowed == true).Select(x => x.RoleId.Value).ToList());

            SelectedUsersID = new List<Guid>();
            SelectedUsersID.AddRange(DC.Set<FunctionPrivilege>().Where(x => x.MenuItemId == Entity.ID && x.UserId != null && x.Allowed == true).Select(x => x.UserId.Value).ToList());

            AllUsers = DC.Set<FrameworkUserBase>().Where(x => SelectedUsersID.Contains(x.ID)).GetSelectListItems(LoginUserInfo.DataPrivileges, null, x => x.ITCode);

            SelectedRolesID2 = new List<Guid>();
            SelectedRolesID2.AddRange(DC.Set<FunctionPrivilege>().Where(x => x.MenuItemId == Entity.ID && x.RoleId != null && x.Allowed == false).Select(x => x.RoleId.Value).ToList());

            SelectedUsersID2 = new List<Guid>();
            SelectedUsersID2.AddRange(DC.Set<FunctionPrivilege>().Where(x => x.MenuItemId == Entity.ID && x.UserId != null && x.Allowed == false).Select(x => x.UserId.Value).ToList());

            AllUsers2 = DC.Set<FrameworkUserBase>().Where(x => SelectedUsersID2.Contains(x.ID)).GetSelectListItems(LoginUserInfo.DataPrivileges, null, x => x.ITCode);

        }

        public List<ComboSelectListItem> GetUsers(string text)
        {
            var d = DC.Set<FrameworkUserBase>().Where(x => x.ITCode.ToLower().Contains(text.ToLower())).GetSelectListItems(LoginUserInfo.DataPrivileges, null, x => x.ITCode);
            return d;
        }

        protected override void ReInitVM()
        {
            AllRoles = DC.Set<FrameworkRole>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.RoleName);

            if (SelectedUsersID != null)
                AllUsers = DC.Set<FrameworkUserBase>().Where(x => SelectedUsersID.Contains(x.ID)).GetSelectListItems(LoginUserInfo.DataPrivileges,null, x => x.ITCode);
            if (SelectedUsersID2 != null)
                AllUsers2 = DC.Set<FrameworkUserBase>().Where(x => SelectedUsersID2.Contains(x.ID)).GetSelectListItems(LoginUserInfo.DataPrivileges, null, x => x.ITCode);
        }

        public override void DoAdd()
        {
            var oldIDs = DC.Set<FunctionPrivilege>().Where(x => x.MenuItemId == this.Entity.ID).Select(x => x.ID).ToList();
            foreach (var oldid in oldIDs)
            {
                FunctionPrivilege fp = new FunctionPrivilege { ID = oldid };
                DC.Set<FunctionPrivilege>().Attach(fp);
                DC.DeleteEntity(fp);
            }
            if (SelectedRolesIDs != null)
            {
                foreach (var id in SelectedRolesIDs)
                {
                    FunctionPrivilege fp = new FunctionPrivilege();
                    fp.MenuItemId = this.Entity.ID;
                    fp.RoleId = id;
                    fp.UserId = null;
                    fp.Allowed = true;
                    DC.Set<FunctionPrivilege>().Add(fp);
                }
            }
            if (SelectedRolesID2 != null)
            {
                foreach (var id in SelectedRolesID2)
                {
                    FunctionPrivilege fp = new FunctionPrivilege();
                    fp.MenuItemId = this.Entity.ID;
                    fp.RoleId = id;
                    fp.UserId = null;
                    fp.Allowed = false;
                    DC.Set<FunctionPrivilege>().Add(fp);
                }
            }
            if (SelectedUsersID != null)
            {
                foreach (var id in SelectedUsersID)
                {
                    FunctionPrivilege fp = new FunctionPrivilege();
                    fp.MenuItemId = this.Entity.ID;
                    fp.RoleId = null;
                    fp.UserId = id;
                    fp.Allowed = true;
                    DC.Set<FunctionPrivilege>().Add(fp);
                }
            }
            if (SelectedUsersID2 != null)
            {
                foreach (var id in SelectedUsersID2)
                {
                    FunctionPrivilege fp = new FunctionPrivilege();
                    fp.MenuItemId = this.Entity.ID;
                    fp.RoleId = null;
                    fp.UserId = id;
                    fp.Allowed = false;
                    DC.Set<FunctionPrivilege>().Add(fp);
                }
            }
            DC.SaveChanges();
        }
    }
}
