using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms
{
    public class FrameworkUserVM : BaseCRUDVM<FrameworkUserBase>
    {
        [JsonIgnore]
        public List<ComboSelectListItem> AllRoles { get; set; }
        [Display(Name = "角色")]
        public List<Guid> SelectedRolesIDs { get; set; }

        [JsonIgnore]
        public List<ComboSelectListItem> AllGroups { get; set; }
        [Display(Name = "用户组")]
        public List<Guid> SelectedGroupIDs { get; set; }


        public FrameworkUserVM()
        {
            SetInclude(x => x.UserRoles, x=>x.UserGroups);
        }

        /// <summary>
        /// 验证重复字段
        /// </summary>
        /// <returns></returns>
        public override DuplicatedInfo<FrameworkUserBase> SetDuplicatedCheck()
        {
            var rv = CreateFieldsInfo(SimpleField(x => x.ITCode));
            return rv;
        }

        protected override void InitVM()
        {
            if (ControllerName.Contains("WalkingTec.Mvvm.Mvc.Admin.Controllers"))
            {
                SelectedRolesIDs = Entity.UserRoles.Select(x => x.RoleId).ToList();
                AllRoles = DC.Set<FrameworkRole>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.RoleName);
                SelectedGroupIDs = Entity.UserGroups.Select(x => x.GroupId).ToList();
                AllGroups = DC.Set<FrameworkGroup>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.GroupName);
            }

        }

        protected override void ReInitVM()
        {
            if (ControllerName.Contains("WalkingTec.Mvvm.Mvc.Admin.Controllers"))
            {
                AllRoles = DC.Set<FrameworkRole>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.RoleName);
                AllGroups = DC.Set<FrameworkGroup>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.GroupName);
            }
        }

        public override void DoAdd()
        {
            if (ControllerName.Contains("WalkingTec.Mvvm.Mvc.Admin.Controllers"))
            {
                if (SelectedRolesIDs != null)
                {
                    foreach (var roleid in SelectedRolesIDs)
                    {
                        Entity.UserRoles.Add(new FrameworkUserRole { RoleId = roleid });
                    }
                }
                if (SelectedGroupIDs != null)
                {
                    foreach (var groupid in SelectedGroupIDs)
                    {
                        Entity.UserGroups.Add(new FrameworkUserGroup { GroupId = groupid });
                    }
                }
            }
            Entity.IsValid = true;
            Entity.Password = Utils.GetMD5String(Entity.Password);
            base.DoAdd();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            if (ControllerName.Contains("WalkingTec.Mvvm.Mvc.Admin.Controllers"))
            {
                if (SelectedRolesIDs == null || SelectedRolesIDs.Count == 0)
                {
                    FC.Add("Entity.SelectedRolesIDs.DONOTUSECLEAR", "true");
                }
                else
                {
                    Entity.UserRoles = new List<FrameworkUserRole>();
                    SelectedRolesIDs.ForEach(x => Entity.UserRoles.Add(new FrameworkUserRole { ID = Guid.NewGuid(), UserId = Entity.ID, RoleId = x }));
                }
                if (SelectedGroupIDs == null || SelectedGroupIDs.Count == 0)
                {
                    FC.Add("Entity.SelectedGroupIDs.DONOTUSECLEAR", "true");
                }
                else
                {
                    SelectedGroupIDs.ForEach(x => Entity.UserGroups.Add(new FrameworkUserGroup { ID = Guid.NewGuid(), UserId = Entity.ID, GroupId = x }));
                }
            }
            base.DoEdit(updateAllFields);
        }

        public override void DoDelete()
        {
            base.DoDelete();
        }

        public void ChangePassword()
        {
            Entity.Password = Utils.GetMD5String(Entity.Password);
            DC.UpdateProperty(Entity, x => x.Password);
            DC.SaveChanges();
        }
    }
}
