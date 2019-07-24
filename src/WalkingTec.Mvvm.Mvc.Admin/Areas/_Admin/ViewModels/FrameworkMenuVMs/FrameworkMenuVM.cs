using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs
{
    public class FrameworkMenuVM : BaseCRUDVM<FrameworkMenu>
    {
        [JsonIgnore]
        public List<ComboSelectListItem> AllParents { get; set; }
        [JsonIgnore]
        public List<ComboSelectListItem> AllModules { get; set; }
        [JsonIgnore]
        public List<ComboSelectListItem> AllActions { get; set; }

        [Display(Name = "动作")]
        public List<string> SelectedActionIDs { get; set; }

        [Display(Name = "模块")]
        public string SelectedModule { get; set; }

        [Display(Name = "允许角色")]
        public List<Guid> SelectedRolesIDs { get; set; }

        [JsonIgnore]
        public FrameworkUserBaseListVM UserListVM { get; set; }
        [JsonIgnore]
        public FrameworkRoleListVM RoleListVM { get; set; }

        public FrameworkMenuVM()
        {
            UserListVM = new FrameworkUserBaseListVM();
            RoleListVM = new FrameworkRoleListVM();
            AllActions = new List<ComboSelectListItem>();
            AllModules = new List<ComboSelectListItem>();

            SelectedRolesIDs = new List<Guid>();
        }

        protected override void InitVM()
        {
            SelectedRolesIDs.AddRange(DC.Set<FunctionPrivilege>().Where(x => x.MenuItemId == Entity.ID && x.RoleId != null && x.Allowed == true).Select(x => x.RoleId.Value).ToList());

            var data = DC.Set<FrameworkMenu>().ToList();
            var topMenu = data.Where(x => x.ParentId == null).ToList().FlatTree(x => x.DisplayOrder);
            var pids = Entity.GetAllChildrenIDs(DC);
            AllParents = topMenu.Where(x => x.ID != Entity.ID && !pids.Contains(x.ID) && x.FolderOnly == true).ToList().ToListItems(y => y.PageName, x => x.ID);

            var modules = GlobalServices.GetRequiredService<GlobalData>().AllModule;

            var m = modules.Where(x => x.NameSpace != "WalkingTec.Mvvm.Admin.Api").ToList();
            List<FrameworkModule> toremove = new List<FrameworkModule>();
            foreach (var item in m)
            {
                var f = modules.Where(x => x.ClassName == item.ClassName && x.Area?.AreaName == item.Area?.AreaName).FirstOrDefault();
                if (f?.IgnorePrivillege == true)
                {
                    toremove.Add(item);
                }
            }
            toremove.ForEach(x => m.Remove(x));
            AllModules = m.ToListItems(y => y.ModuleName, y => y.FullName);
            if (string.IsNullOrEmpty(Entity.Url) == false && Entity.IsInside == true)
            {
                SelectedModule = modules.Where(x => x.IsApi == false && (x.ClassName == Entity.ClassName || x.FullName == Entity.ClassName)).SelectMany(x => x.Actions).FirstOrDefault().Module.FullName;
                var mm = modules.Where(x => x.FullName == SelectedModule).SelectMany(x => x.Actions).Where(x => x.MethodName != "Index" && x.IgnorePrivillege == false).ToList();
                AllActions = mm.ToListItems(y => y.ActionName, y => y.Url);
                SelectedActionIDs = DC.Set<FrameworkMenu>().Where(x => AllActions.Select(y => y.Value).Contains(x.Url) && x.IsInside == true && x.FolderOnly == false).Select(x => x.Url).ToList();
            }
        }

        protected override void ReInitVM()
        {
            var data = DC.Set<FrameworkMenu>().ToList();
            var topMenu = data.Where(x => x.ParentId == null).ToList().FlatTree(x => x.DisplayOrder);
            var pids = Entity.GetAllChildrenIDs(DC);
            AllParents = topMenu.Where(x => x.ID != Entity.ID && !pids.Contains(x.ID) && x.FolderOnly == true).ToList().ToListItems(y => y.PageName, x => x.ID);
            var modules = GlobalServices.GetRequiredService<GlobalData>().AllModule;

            var m = modules.Where(x => x.NameSpace != "WalkingTec.Mvvm.Admin.Api").ToList();
            List<FrameworkModule> toremove = new List<FrameworkModule>();
            foreach (var item in m)
            {
                var f = modules.Where(x => x.ClassName == item.ClassName && x.Area?.AreaName == item.Area?.AreaName).FirstOrDefault();
                if (f?.IgnorePrivillege == true)
                {
                    toremove.Add(item);
                }
            }
            toremove.ForEach(x => m.Remove(x));
            AllModules = m.ToListItems(y => y.ModuleName, y => y.FullName);
            if (string.IsNullOrEmpty(SelectedModule) == false)
            {
                var mm = modules.Where(x => x.FullName == SelectedModule).SelectMany(x => x.Actions).Where(x => x.MethodName != "Index" && x.IgnorePrivillege == false).ToList();
                AllActions = mm.ToListItems(y => y.ActionName, y => y.Url);
            }

        }

        public override void Validate()
        {
            if (Entity.IsInside == true && Entity.FolderOnly == false)
            {
                var modules = GlobalServices.GetRequiredService<GlobalData>().AllModule;
                var mainAction = modules.Where(x => x.FullName == this.SelectedModule).SelectMany(x => x.Actions).Where(x => x.MethodName == "Index").SingleOrDefault();
                if (mainAction != null)
                {
                    var test = DC.Set<FrameworkMenu>().Where(x => x.Url == mainAction.Url && x.ID != Entity.ID).FirstOrDefault();
                    if (test != null)
                    {
                        MSD.AddModelError(" error", "该模块已经配置过了");
                    }
                }
            }
            base.Validate();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            if (Entity.IsInside == false)
            {
                if (Entity.Url != null && Entity.Url != "")
                {
                    if (Entity.DomainId == null)
                    {
                        if (Entity.Url.ToLower().StartsWith("http://") == false && Entity.Url.ToLower().StartsWith("https://") == false)
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
            else
            {

                if (string.IsNullOrEmpty(SelectedModule) == true && Entity.FolderOnly == false)
                {
                    MSD.AddModelError("SelectedModule", "请选择一个模块");
                    return;
                }
                if (string.IsNullOrEmpty(SelectedModule) == false && Entity.FolderOnly == false)
                {
                    var modules = GlobalServices.GetRequiredService<GlobalData>().AllModule;
                    List<FrameworkAction> otherActions = null;
                    var mainAction = modules.Where(x => x.FullName == this.SelectedModule).SelectMany(x => x.Actions).Where(x => x.MethodName == "Index").SingleOrDefault();
                    if (mainAction == null && Entity.ShowOnMenu == true)
                    {
                        MSD.AddModelError("Entity.ModuleId", "模块中没有找到Index页面");
                        return;
                    }
                    if (mainAction == null && Entity.ShowOnMenu == false)
                    {
                        var model = modules.Where(x => x.FullName == this.SelectedModule).FirstOrDefault();
                        mainAction = new FrameworkAction();
                        mainAction.Module = model;
                        mainAction.MethodName = "Index";
                    }
                    var ndc = DC.ReCreate();
                    var oldIDs = ndc.Set<FrameworkMenu>().Where(x => x.ParentId == Entity.ID).Select(x => x.ID).ToList();
                    foreach (var oldid in oldIDs)
                    {
                        try
                        {
                            FrameworkMenu fp = new FrameworkMenu { ID = oldid };
                            ndc.Set<FrameworkMenu>().Attach(fp);
                            ndc.DeleteEntity(fp);
                        }
                        catch { }
                    }
                    ndc.SaveChanges();

                    Entity.Url = mainAction.Url;
                    Entity.ModuleName = mainAction.Module.ModuleName;
                    Entity.ClassName = mainAction.Module.ClassName;
                    Entity.MethodName = "Index";

                    otherActions = modules.Where(x => x.FullName == this.SelectedModule).SelectMany(x => x.Actions).Where(x => x.MethodName != "Index").ToList();
                    int order = 1;
                    foreach (var action in otherActions)
                    {
                        if (SelectedActionIDs != null && SelectedActionIDs.Contains(action.Url))
                        {
                            FrameworkMenu menu = new FrameworkMenu();
                            menu.FolderOnly = false;
                            menu.IsPublic = false;
                            menu.Parent = Entity;
                            menu.ShowOnMenu = false;
                            menu.DisplayOrder = order++;
                            menu.Privileges = new List<FunctionPrivilege>();
                            menu.CreateBy = LoginUserInfo.ITCode;
                            menu.CreateTime = DateTime.Now;
                            menu.IsInside = true;
                            menu.DomainId = Entity.DomainId;
                            menu.PageName = action.ActionName;
                            menu.ModuleName = action.Module.ModuleName;
                            menu.ActionName = action.ActionName;
                            menu.Url = action.Url;
                            Entity.Children.Add(menu);
                        }
                    }
                }

                else
                {
                    Entity.Children = null;
                    Entity.Url = null;
                }
            }
            base.DoEdit();
            List<Guid> guids = new List<Guid>();
            guids.Add(Entity.ID);
            if (Entity.Children != null)
            {
                guids.AddRange(Entity.Children?.Select(x => x.ID).ToList());
            }
            AddPrivilege(guids);
        }

        public override void DoAdd()
        {
            if (Entity.IsInside == false)
            {
                if (Entity.Url != null && Entity.Url != "")
                {
                    if (Entity.DomainId == null)
                    {
                        if (Entity.Url.ToLower().StartsWith("http://") == false && Entity.Url.ToLower().StartsWith("https://") == false)
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
            else
            {

                if (string.IsNullOrEmpty(SelectedModule) == true && Entity.FolderOnly == false)
                {
                    MSD.AddModelError("SelectedModule", "请选择一个模块");
                    return;
                }
                if (string.IsNullOrEmpty(SelectedModule) == false && Entity.FolderOnly == false)
                {
                    var modules = GlobalServices.GetRequiredService<GlobalData>().AllModule;
                    List<FrameworkAction> otherActions = null;
                    var mainAction = modules.Where(x => x.FullName == this.SelectedModule).SelectMany(x => x.Actions).Where(x => x.MethodName == "Index").SingleOrDefault();
                    if (mainAction == null && Entity.ShowOnMenu == true)
                    {
                        MSD.AddModelError("Entity.ModuleId", "模块中没有找到Index页面");
                        return;
                    }
                    if(mainAction == null && Entity.ShowOnMenu == false)
                    {
                        var model = modules.Where(x => x.FullName == this.SelectedModule).FirstOrDefault();
                        mainAction = new FrameworkAction();
                        mainAction.Module = model;
                        mainAction.MethodName = "Index";
                    }
                    Entity.Url = mainAction.Url;
                    Entity.ModuleName = mainAction.Module.ModuleName;
                    Entity.ClassName = mainAction.Module.ClassName;
                    Entity.MethodName = "Index";

                    otherActions = modules.Where(x => x.FullName == this.SelectedModule).SelectMany(x => x.Actions).Where(x => x.MethodName != "Index").ToList();
                    int order = 1;
                    foreach (var action in otherActions)
                    {
                        if (SelectedActionIDs.Contains(action.Url))
                        {
                            FrameworkMenu menu = new FrameworkMenu();
                            menu.FolderOnly = false;
                            menu.IsPublic = false;
                            menu.Parent = Entity;
                            menu.ShowOnMenu = false;
                            menu.DisplayOrder = order++;
                            menu.Privileges = new List<FunctionPrivilege>();
                            menu.CreateBy = LoginUserInfo.ITCode;
                            menu.CreateTime = DateTime.Now;
                            menu.IsInside = true;
                            menu.DomainId = Entity.DomainId;
                            menu.PageName = action.ActionName;
                            menu.ModuleName = action.Module.ModuleName;
                            menu.ActionName = action.ActionName;
                            menu.Url = action.Url;
                            Entity.Children.Add(menu);
                        }
                    }
                }

                else
                {
                    Entity.Children = null;
                    Entity.Url = null;
                }

            }
            base.DoAdd();
            List<Guid> guids = new List<Guid>();
            guids.Add(Entity.ID);
            if (Entity.Children != null)
            {
                guids.AddRange(Entity.Children?.Select(x => x.ID).ToList());
            }
            AddPrivilege(guids);
        }

        public void AddPrivilege(List<Guid> menuids)
        {
            var oldIDs = DC.Set<FunctionPrivilege>().Where(x => menuids.Contains(x.MenuItemId)).Select(x => x.ID).ToList();
            var admin = DC.Set<FrameworkRole>().Where(x => x.RoleCode == "001").SingleOrDefault();
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
            if (admin != null && SelectedRolesIDs.Contains(admin.ID) == false)
            {
                SelectedRolesIDs.Add(admin.ID);
            }
            foreach (var menuid in menuids)
            {

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
