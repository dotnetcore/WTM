using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs;
using Newtonsoft.Json;

namespace WalkingTec.Mvvm.Demo.ViewModels.MyUserVMs
{
    public class MyUserVM : BaseCRUDVM<MyUser>
    {
        public List<ComboSelectListItem> AllUserRoless { get; set; }
        [Display(Name = "角色")]
        public List<Guid> SelectedUserRolesIDs { get; set; }
        public List<ComboSelectListItem> AllUserGroupss { get; set; }
        [Display(Name = "用户组")]
        public List<Guid> SelectedUserGroupsIDs { get; set; }
        [JsonIgnore]
        public FrameworkRoleListVM RoleListVM { get; set; }

        public MyUserVM()
        {
            SetInclude(x => x.UserRoles);
            SetInclude(x => x.UserGroups);
            RoleListVM = new FrameworkRoleListVM();
        }

        protected override void InitVM()
        {
            AllUserRoless = DC.Set<FrameworkRole>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.RoleName);
            SelectedUserRolesIDs = Entity.UserRoles.Select(x => x.RoleId).ToList();
            AllUserGroupss = DC.Set<FrameworkGroup>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.GroupName);
            SelectedUserGroupsIDs = Entity.UserGroups.Select(x => x.GroupId).ToList();
        }

        public override void DoAdd()
        {
            if (SelectedUserRolesIDs != null)
            {
                foreach (var id in SelectedUserRolesIDs)
                {
                    Entity.UserRoles.Add(new FrameworkUserRole { RoleId = id });
                }
            }

            if (SelectedUserGroupsIDs != null)
            {
                foreach (var id in SelectedUserGroupsIDs)
                {
                    Entity.UserGroups.Add(new FrameworkUserGroup { GroupId = id });
                }
            }
            Entity.IsValid = true;
            Entity.Password = Utils.GetMD5String(Entity.Password);

            base.DoAdd();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            if(SelectedUserRolesIDs == null || SelectedUserRolesIDs.Count == 0)
            {
                FC.Add("Entity.SelectedUserRolesIDs.DONOTUSECLEAR", "true");
            }
            else
            {
                Entity.UserRoles = new List<FrameworkUserRole>();
                SelectedUserRolesIDs.ForEach(x => Entity.UserRoles.Add(new FrameworkUserRole { ID = Guid.NewGuid(), RoleId = x }));
            }

            if(SelectedUserGroupsIDs == null || SelectedUserGroupsIDs.Count == 0)
            {
                FC.Add("Entity.SelectedUserGroupsIDs.DONOTUSECLEAR", "true");
            }
            else
            {
                Entity.UserGroups = new List<FrameworkUserGroup>();
                SelectedUserGroupsIDs.ForEach(x => Entity.UserGroups.Add(new FrameworkUserGroup { ID = Guid.NewGuid(), GroupId = x }));
            }

            base.DoEdit(updateAllFields);
        }

        public override void DoDelete()
        {
            base.DoDelete();
        }
    }
}
