using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs
{
    public class FrameworkMenuVM : BaseCRUDVM<FrameworkMenu>
    {
        public List<ComboSelectListItem> AllParents { get; set; }
        public List<ComboSelectListItem> AllModules { get; set; }
        public List<ComboSelectListItem> AllActions { get; set; }

        [Display(Name = "允许角色")]
        public List<Guid> SelectedRolesIDs { get; set; }
        [Display(Name = "允许用户")]
        public List<Guid> SelectedUsersID { get; set; }
        [Display(Name = "拒绝角色")]
        public List<Guid> SelectedRolesID2 { get; set; }
        [Display(Name = "拒绝用户")]
        public List<Guid> SelectedUsersID2 { get; set; }

        public FrameworkUserBaseListVM UserListVM { get; set; }
        public FrameworkRoleListVM RoleListVM { get; set; }

        public FrameworkMenuVM()
        {
            UserListVM = new FrameworkUserBaseListVM();
            RoleListVM = new FrameworkRoleListVM();
            AllActions = new List<ComboSelectListItem>();
            AllModules = new List<ComboSelectListItem>();

            SelectedRolesIDs = new List<Guid>();
            SelectedUsersID = new List<Guid>();
            SelectedRolesID2 = new List<Guid>();
            SelectedUsersID2 = new List<Guid>();
        }

        protected override void InitVM()
        {
            SelectedRolesIDs.AddRange(DC.Set<FunctionPrivilege>().Where(x => x.MenuItemId == Entity.ID && x.RoleId != null && x.Allowed == true).Select(x => x.RoleId.Value).ToList());
            SelectedUsersID.AddRange(DC.Set<FunctionPrivilege>().Where(x => x.MenuItemId == Entity.ID && x.UserId != null && x.Allowed == true).Select(x => x.UserId.Value).ToList());
            SelectedRolesID2.AddRange(DC.Set<FunctionPrivilege>().Where(x => x.MenuItemId == Entity.ID && x.RoleId != null && x.Allowed == false).Select(x => x.RoleId.Value).ToList());
            SelectedUsersID2.AddRange(DC.Set<FunctionPrivilege>().Where(x => x.MenuItemId == Entity.ID && x.UserId != null && x.Allowed == false).Select(x => x.UserId.Value).ToList());

            var data = DC.Set<FrameworkMenu>().ToList();
            var topMenu = data.Where(x => x.ParentId == null).ToList().FlatTree(x=>x.DisplayOrder);
            var pids = Entity.GetAllChildrenIDs(DC);
            AllParents = topMenu.Where(x => x.ID != Entity.ID && !pids.Contains(x.ID)).ToList().ToListItems(y => y.PageName, x => x.ID);
            foreach (var p in AllParents)
            {
                Guid temp = Guid.Parse(p.Value);
                var m = topMenu.Where(x => x.ID == temp).SingleOrDefault();
                if (m != null && m.ActionId != null)
                {
                    p.Text = p.Text + "(" + m.ModuleName + ")";
                }
            }
            AllModules = DC.Set<FrameworkModule>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.ModuleName);
            if (Entity.ModuleId != null)
            {
                AllActions = DC.Set<FrameworkAction>().GetSelectListItems(LoginUserInfo.DataPrivileges, x => x.ModuleId == Entity.ModuleId, y => y.ActionName);
            }
        }

        public override void DoAdd()
        {
            if (Entity.IsInside == true)
            {
                var domain = DC.Set<FrameworkDomain>().Where(x => x.ID == Entity.DomainId).FirstOrDefault();
                FrameworkAction act = null;
                if (domain == null)
                {
                    act = DC.Set<FrameworkAction>().Include(x => x.Module.Area).Where(x => x.ID == Entity.ActionId).FirstOrDefault();
                }
                else
                {
                    act = APIHelper.CallAPI<FrameworkAction>(domain.Address + "/WebApi/Pub/GetAction?id=" + Entity.ActionId + "&domainid=" + Entity.DomainId + "&eventts=" + DateTime.Now.Ticks).Result;
                }
                if (act != null)
                {
                    Entity.Url = "/" + act.Module.ClassName + "/" + act.MethodName;
                    if (act.Module.Area != null)
                    {
                        Entity.Url = "/" + act.Module.Area.Prefix + Entity.Url;
                    }
                    Entity.ModuleName = act.Module.ModuleName;
                    Entity.ActionName = act.ActionName;
                }
            }
            else
            {
                Entity.ActionId = null;
                Entity.ModuleId = null;
                if (Entity.Url != null && Entity.Url != "")
                {
                    if (Entity.DomainId == null)
                    {
                        if (Entity.Url.ToLower().StartsWith("http://") == false)
                        {
                            Entity.Url = "http://" + Entity.Url;
                        }
                    }
                    else
                    {
                        if (Entity.Url.StartsWith("/") == false)
                        {
                            Entity.Url = "/" + Entity.Url;
                        }
                    }
                }
            }
            var role = DC.Set<FrameworkRole>().Where(x => x.RoleCode == "001").SingleOrDefault();
            if (role != null && SelectedRolesIDs.Contains(role.ID) == false && SelectedRolesID2.Contains(role.ID) == false)
            {
                Entity.Privileges.Add(new FunctionPrivilege { RoleId = role.ID, Allowed = true });
            }
            base.DoAdd();
            AddPrivilege(Entity.ID);
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            if (Entity.IsInside == true)
            {
                var domain = DC.Set<FrameworkDomain>().Where(x => x.ID == Entity.DomainId).FirstOrDefault();
                FrameworkAction act = null;
                if (domain == null)
                {
                    act = DC.Set<FrameworkAction>().Include(x => x.Module.Area).Where(x => x.ID == Entity.ActionId).FirstOrDefault();
                }
                else
                {
                    act = APIHelper.CallAPI<FrameworkAction>(domain.Address + "/WebApi/Pub/GetAction?id=" + Entity.ActionId + "&domainid=" + Entity.DomainId + "&eventts=" + DateTime.Now.Ticks).Result;
                }
                if (act != null)
                {
                    Entity.Url = "/" + act.Module.ClassName + "/" + act.MethodName;
                    if (act.Module.Area != null)
                    {
                        Entity.Url = "/" + act.Module.Area.Prefix + Entity.Url;
                    }
                    Entity.ModuleName = act.Module.ModuleName;
                    Entity.ActionName = act.ActionName;
                }
            }
            else
            {
                Entity.ActionId = null;
                Entity.ModuleId = null;
                if (Entity.Url != null && Entity.Url != "")
                {
                    if (Entity.DomainId == null)
                    {
                        if (Entity.Url.ToLower().StartsWith("http://") == false)
                        {
                            Entity.Url = "http://" + Entity.Url;
                        }
                    }
                    else
                    {
                        if (Entity.Url.StartsWith("/") == false)
                        {
                            Entity.Url = "/" + Entity.Url;
                        }
                    }
                }
            }
            base.DoEdit(true);
            AddPrivilege(Entity.ID);
        }

        public void DoBatchAdd()
        {
            var role = DC.Set<FrameworkRole>().Where(x => x.RoleCode == "001").SingleOrDefault();
            if (role != null && SelectedRolesIDs.Contains(role.ID) == false && SelectedRolesID2.Contains(role.ID) == false)
            {
                Entity.Privileges.Add(new FunctionPrivilege { RoleId = role.ID, Allowed = true });
            }
            if (Entity.ModuleId != null && Entity.ActionId != null)
            {
                var domain = DC.Set<FrameworkDomain>().Where(x => x.ID == Entity.DomainId).FirstOrDefault();
                List<FrameworkAction> otherActions = null;
                FrameworkAction act = null;
                if (domain == null)
                {
                    act = DC.Set<FrameworkAction>().Include(x => x.Module.Area).Where(x => x.ID == Entity.ActionId).FirstOrDefault();
                    otherActions = DC.Set<FrameworkAction>().Include(x => x.Module.Area).Where(x => x.ModuleId == this.Entity.ModuleId && x.ID != Entity.ActionId).ToList();
                }
                else
                {
                    otherActions = APIHelper.CallAPI<List<FrameworkAction>>(domain.Address + "/WebApi/Pub/GetOtherActions?id=" + Entity.ActionId + "&moduleid=" + Entity.ModuleId + "&eventts=" + DateTime.Now.Ticks).Result;
                    act = APIHelper.CallAPI<FrameworkAction>(domain.Address + "/WebApi/Pub/GetAction?id=" + Entity.ActionId + "&domainid=" + Entity.DomainId + "&eventts=" + DateTime.Now.Ticks).Result;
                }
                if (act != null)
                {
                    Entity.Url = "/" + act.Module.ClassName + "/" + act.MethodName;
                    if (act.Module.Area != null)
                    {
                        Entity.Url = "/" + act.Module.Area.Prefix + Entity.Url;
                    }
                    Entity.ModuleName = act.Module.ModuleName;
                    Entity.ActionName = act.ActionName;
                }
                int order = 1;
                foreach (var action in otherActions)
                {
                    FrameworkMenu menu = new FrameworkMenu();
                    menu.ActionId = action.ID;
                    menu.ModuleId = action.ModuleId;
                    menu.FolderOnly = false;
                    menu.IsInherit = true;
                    menu.IsPublic = false;
                    menu.Parent = Entity;
                    menu.ShowOnMenu = false;
                    menu.DisplayOrder = order++;
                    menu.Privileges = new List<FunctionPrivilege>();
                    menu.CreateBy = LoginUserInfo.ITCode;
                    menu.CreateTime = DateTime.Now;
                    menu.IsInside = true;
                    menu.DomainId = Entity.DomainId;
                    if (role != null)
                    {
                        menu.Privileges.Add(new FunctionPrivilege { RoleId = role.ID, Allowed = true });
                    }
                    menu.PageName = action.ActionName;
                    menu.ModuleName = action.Module.ModuleName;
                    menu.ActionName = action.ActionName;
                    menu.Url = "/" + action.Module.ClassName + "/" + action.MethodName;
                    if (action.Module.Area != null)
                    {
                        menu.Url = "/" + action.Module.Area.Prefix + menu.Url;
                    }

                    Entity.Children.Add(menu);
                }
            }
            base.DoAdd();
            AddPrivilege(Entity.ID);
        }

        public void AddPrivilege(Guid menuid)
        {
            var oldIDs = DC.Set<FunctionPrivilege>().Where(x => x.MenuItemId == menuid).Select(x => x.ID).ToList();
            foreach (var oldid in oldIDs)
            {
                try
                {
                    FunctionPrivilege fp = new FunctionPrivilege { ID = oldid };
                    DC.Set<FunctionPrivilege>().Attach(fp);
                    DC.DeleteEntity(fp);
                }
                catch { }
            }
            if (SelectedRolesIDs != null)
            {
                foreach (var id in SelectedRolesIDs)
                {
                    FunctionPrivilege fp = new FunctionPrivilege();
                    fp.MenuItemId = menuid;
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
                    fp.MenuItemId = menuid;
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
                    fp.MenuItemId = menuid;
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
                    fp.MenuItemId = menuid;
                    fp.RoleId = null;
                    fp.UserId = id;
                    fp.Allowed = false;
                    DC.Set<FunctionPrivilege>().Add(fp);
                }
            }
            DC.SaveChanges();
        }


        public override void DoDelete()
        {
            try
            {
                //级联删除所有子集
                DC.CascadeDelete(Entity);
                DC.SaveChanges();
            }
            catch
            { }
        }
    }
}
