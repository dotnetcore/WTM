using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.MyUserVMs
{
    public partial class MyUserVM : BaseCRUDVM<MyUser>
    {
        public List<ComboSelectListItem> AllUserRoless { get; set; }
        [Display(Name = "角色")]
        public List<Guid> SelectedUserRolesIDs { get; set; }
        public List<ComboSelectListItem> AllUserGroupss { get; set; }
        [Display(Name = "用户组")]
        public List<Guid> SelectedUserGroupsIDs { get; set; }

        public MyUserVM()
        {
            SetInclude(x => x.UserRoles);
            SetInclude(x => x.UserGroups);
        }

        public override void Validate()
        {
            base.Validate();
        }

        protected override void InitVM()
        {
            AllUserRoless = DC.Set<FrameworkRole>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, y => y.RoleName);
            SelectedUserRolesIDs = Entity.UserRoles?.Select(x => x.RoleId).ToList();
            AllUserGroupss = DC.Set<FrameworkGroup>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, y => y.GroupName);
            SelectedUserGroupsIDs = Entity.UserGroups?.Select(x => x.GroupId).ToList();
        }

        public override void DoAdd()
        {
            Entity.UserRoles = new List<FrameworkUserRole>();
            if (SelectedUserRolesIDs != null)
            {
                foreach (var id in SelectedUserRolesIDs)
                {
                    Entity.UserRoles.Add(new FrameworkUserRole { RoleId = id });
                }
            }

            Entity.UserGroups = new List<FrameworkUserGroup>();
            if (SelectedUserGroupsIDs != null)
            {
                foreach (var id in SelectedUserGroupsIDs)
                {
                    Entity.UserGroups.Add(new FrameworkUserGroup { GroupId = id });
                }
            }
           
            base.DoAdd();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            Entity.UserRoles = new List<FrameworkUserRole>();
            if(SelectedUserRolesIDs != null )
            {
                SelectedUserRolesIDs.ForEach(x => Entity.UserRoles.Add(new FrameworkUserRole { ID = Guid.NewGuid(), RoleId = x }));
            }

            Entity.UserGroups = new List<FrameworkUserGroup>();
            if(SelectedUserGroupsIDs != null )
            {
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
