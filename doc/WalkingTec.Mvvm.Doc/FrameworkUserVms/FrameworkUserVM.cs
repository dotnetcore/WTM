using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Doc.FrameworkUserVms
{
    public class FrameworkUserVM : BaseCRUDVM<FrameworkUserBase>
    {
        public List<ComboSelectListItem> AllRoles { get; set; }
        [Display(Name = "角色")]
        public List<Guid> SelectedRolesIDs { get; set; }

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
            SelectedRolesIDs = Entity.UserRoles.Select(x => x.RoleId).ToList();
            AllRoles = new List<ComboSelectListItem>
            {
                new ComboSelectListItem{ Text = "管理员", Value="管理员"},
                new ComboSelectListItem{ Text = "普通用户", Value="普通用户"}
            };
            SelectedGroupIDs = Entity.UserGroups.Select(x => x.GroupId).ToList();
            AllGroups = new List<ComboSelectListItem>
            {
                new ComboSelectListItem{ Text = "北京组", Value="北京组"},
                new ComboSelectListItem{ Text = "上海组", Value="上海组"}
            };

        }

        public override void DoAdd()
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
            Entity.Password = Utils.GetMD5String(Entity.Password);
            base.DoAdd();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            var oldroles = DC.Set<FrameworkUserRole>().Where(x => x.UserId == Entity.ID).ToList();
            var oldgroups = DC.Set<FrameworkUserGroup>().Where(x => x.UserId == Entity.ID).ToList();
            foreach (var item in oldroles)
            {
                DC.DeleteEntity(item);
            }
            foreach (var item in oldgroups)
            {
                DC.DeleteEntity(item);
            }
            if (SelectedRolesIDs != null)
            {
                foreach (var item in SelectedRolesIDs)
                {
                    DC.Set<FrameworkUserRole>().Add(new FrameworkUserRole { UserId = Entity.ID, RoleId = item });
                }
            }
            if (SelectedGroupIDs != null)
            {
                foreach (var groupid in SelectedGroupIDs)
                {
                    Entity.UserGroups.Add(new FrameworkUserGroup { GroupId = groupid });
                }
            }
            base.DoEdit();
        }

        public void ChangePassword()
        {
            Entity.Password = Utils.GetMD5String(Entity.Password);
            DC.UpdateProperty(Entity, x => x.Password);
            DC.SaveChanges();
        }
    }
}
