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
    public class FrameworkMenuVM2 : BaseCRUDVM<FrameworkMenu>
    {

        [Display(Name = "Action")]
        public List<string> SelectedActionIDs { get; set; }

        [Display(Name = "Module")]
        public string SelectedModule { get; set; }

        [Display(Name = "AllowedRole")]
        public List<Guid> SelectedRolesIDs { get; set; }


        public FrameworkMenuVM2()
        {
            SelectedRolesIDs = new List<Guid>();
        }

        protected override void InitVM()
        {
            SelectedRolesIDs.AddRange(DC.Set<FunctionPrivilege>().Where(x => x.MenuItemId == Entity.ID && x.RoleId != null && x.Allowed == true).Select(x => x.RoleId.Value).ToList());

            var data = DC.Set<FrameworkMenu>().ToList();
            var topMenu = data.Where(x => x.ParentId == null).ToList().FlatTree(x => x.DisplayOrder);
            var pids = Entity.GetAllChildrenIDs(DC);
            var modules = GlobalServices.GetRequiredService<GlobalData>().AllModule;

            if (Entity.Url != null && Entity.IsInside == true)
            {
                SelectedModule = modules.Where(x => x.IsApi == true && x.FullName == Entity.ClassName).FirstOrDefault().FullName;
                var urls = modules.Where(x => x.FullName == SelectedModule && x.IsApi == true).SelectMany(x => x.Actions).Where(x => x.IgnorePrivillege == false).Select(x => x.Url).ToList();
                SelectedActionIDs = DC.Set<FrameworkMenu>().Where(x => urls.Contains(x.Url) && x.IsInside == true && x.FolderOnly == false).Select(x => x.MethodName).ToList();
            }
        }

        public override void Validate()
        {
            if (Entity.IsInside == true && Entity.FolderOnly == false)
            {
                var modules = GlobalServices.GetRequiredService<GlobalData>().AllModule;
                var test = DC.Set<FrameworkMenu>().Where(x => x.ClassName == this.SelectedModule && string.IsNullOrEmpty(x.MethodName) && x.ID != Entity.ID).FirstOrDefault();
                if (test != null)
                {
                    MSD.AddModelError(" error", Program._localizer["ModuleHasSet"]);
                }
            }
            base.Validate();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            List<Guid> guids = new List<Guid>();
            if (Entity.IsInside == false)
            {
                if (Entity.Url != null && Entity.Url != "")
                {
                    if (Entity.DomainId == null)
                    {
                        if (Entity.Url.ToLower().StartsWith("http://") == false && Entity.Url.ToLower().StartsWith("https://") == false && Entity.Url.StartsWith("@") == false)
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
                    MSD.AddModelError("SelectedModule", Program._localizer["SelectModule"]);
                    return;
                }

                if (string.IsNullOrEmpty(SelectedModule) == false && Entity.FolderOnly == false)
                {
                    var modules = GlobalServices.GetRequiredService<GlobalData>().AllModule;
                    var ndc = DC.ReCreate();
                    var actionsInDB = DC.Set<FrameworkMenu>().AsNoTracking().Where(x => x.ParentId == Entity.ID).ToList();
                    var mo = modules.Where(x => x.FullName == this.SelectedModule && x.IsApi == true).FirstOrDefault();
                    Entity.ModuleName = mo.ModuleName;
                    Entity.ClassName = mo.FullName;
                    Entity.MethodName = null;

                    var otherActions = mo.Actions;
                    int order = 1;
                    Entity.Children = new List<FrameworkMenu>();
                    foreach (var action in otherActions)
                    {
                        if (SelectedActionIDs != null && SelectedActionIDs.Contains(action.MethodName))
                        {
                            Guid aid = action.ID;
                            var adb = actionsInDB.Where(x => x.Url.ToLower() == action.Url.ToLower()).FirstOrDefault();
                            if (adb != null)
                            {
                                aid = adb.ID;
                            }
                            else
                            {
                                guids.Add(aid);
                            }
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
                            menu.PageName = action.ActionDes?.Description ?? action.ActionName;
                            menu.ModuleName = action.Module.ModuleName;
                            menu.ActionName = action.ActionDes?.Description ?? action.ActionName;
                            menu.Url = action.Url;
                            menu.ClassName = action.Module.FullName;
                            menu.MethodName = action.MethodName;
                            menu.ID = aid;
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
            if(FC.ContainsKey("Entity.Children") == false)
            {
                FC.Add("Entity.Children", 0);
            }
            base.DoEdit(updateAllFields);
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
                        if (Entity.Url.ToLower().StartsWith("http://") == false && Entity.Url.ToLower().StartsWith("https://") == false && Entity.Url.StartsWith("@") == false)
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
                    MSD.AddModelError("SelectedModule", Program._localizer["SelectModule"]);
                    return;
                }
                if (string.IsNullOrEmpty(SelectedModule) == false && Entity.FolderOnly == false)
                {
                    var modules = GlobalServices.GetRequiredService<GlobalData>().AllModule;

                    var mo = modules.Where(x => x.FullName == this.SelectedModule && x.IsApi == true).FirstOrDefault();
                    Entity.ModuleName = mo.ModuleName;
                    Entity.ClassName = mo.FullName;
                    Entity.MethodName = null;

                    var otherActions = mo.Actions;
                    int order = 1;
                    Entity.Children = new List<FrameworkMenu>();
                    foreach (var action in otherActions)
                    {
                        if (SelectedActionIDs != null && SelectedActionIDs.Contains(action.MethodName))
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
                            menu.PageName = action.ActionDes?.Description ?? action.ActionName;
                            menu.ModuleName = action.Module.ModuleName;
                            menu.ActionName = action.ActionDes?.Description ?? action.ActionName;
                            menu.Url = action.Url;
                            menu.ClassName = action.Module.FullName;
                            menu.MethodName = action.MethodName;
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
            //var oldIDs = DC.Set<FunctionPrivilege>().Where(x => menuids.Contains(x.MenuItemId)).Select(x => x.ID).ToList();
            var admin = DC.Set<FrameworkRole>().Where(x => x.RoleCode == "001").SingleOrDefault();
            //foreach (var oldid in oldIDs)
            //{
            //    try
            //    {
            //        FunctionPrivilege fp = new FunctionPrivilege { ID = oldid };
            //        DC.Set<FunctionPrivilege>().Attach(fp);
            //        DC.DeleteEntity(fp);
            //    }
            //    catch { }
            //}
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
