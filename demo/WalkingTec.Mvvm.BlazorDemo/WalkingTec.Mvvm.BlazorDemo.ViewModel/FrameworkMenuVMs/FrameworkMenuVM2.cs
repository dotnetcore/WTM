// WTM默认页面 Wtm buidin page
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs
{
    public class FrameworkMenuVM2 : BaseCRUDVM<FrameworkMenu>
    {

        [Display(Name = "_Admin.Action")]
        public List<string> SelectedActionIDs { get; set; }

        [Display(Name = "_Admin.Module")]
        public string SelectedModule { get; set; }

        [Display(Name = "_Admin.AllowedRole")]
        public List<string> SelectedRolesCodes { get; set; }


        public FrameworkMenuVM2()
        {
            SelectedRolesCodes = new List<string>();
        }

        protected override async Task InitVM()
        {
            SelectedRolesCodes.AddRange(await DC.Set<FunctionPrivilege>().Where(x => x.MenuItemId == Entity.ID && x.RoleCode != null && x.Allowed == true).Select(x => x.RoleCode).ToListAsync());
            SelectedRolesCodes = SelectedRolesCodes.Distinct().ToList();

            var data = DC.Set<FrameworkMenu>().ToList();
            var topMenu = data.Where(x => x.ParentId == null).ToList().FlatTree(x => x.DisplayOrder);
            var modules = Wtm.GlobaInfo.AllModule;

            if (Entity.Url != null && Entity.IsInside == true)
            {
                SelectedModule = modules.Where(x => x.IsApi == true && (x.FullName == Entity.ClassName)).FirstOrDefault()?.FullName;
                if (SelectedModule != null)
                {
                    var urls = modules.Where(x => x.FullName == SelectedModule && x.IsApi == true).SelectMany(x => x.Actions).Where(x => x.IgnorePrivillege == false).Select(x => x.Url).ToList();
                    SelectedActionIDs = await DC.Set<FrameworkMenu>().Where(x => urls.Contains(x.Url) && x.IsInside == true && x.FolderOnly == false).Select(x => x.MethodName).ToListAsync();
                }
                else
                {
                    SelectedModule = Entity.Url;
                }
            }
        }

        public override async Task Validate()
        {
            if (Entity.IsInside == true && Entity.FolderOnly == false)
            {
                if (string.IsNullOrEmpty(SelectedModule) == true)
                {
                    MSD.AddModelError("SelectedModule", Localizer["Validate.{0}required", Localizer["_Admin.Module"]]);
                }
                else
                {
                    var modules = Wtm.GlobaInfo.AllModule;
                    var test = await DC.Set<FrameworkMenu>().Where(x => x.Url != null && x.Url.ToLower() == this.Entity.Url.ToLower() && x.ID != Entity.ID).FirstOrDefaultAsync();
                    if (test != null)
                    {
                        MSD.AddModelError(" error", Localizer["_Admin.ModuleHasSet"]);
                    }
                }
            }
            await base.Validate();
        }

        public override async Task DoEdit(bool updateAllFields = false)
        {
            if (Entity.IsInside == false)
            {
                if (Entity.Url != null && Entity.Url != "" && Entity.Url.StartsWith("/") == false)
                {
                    if (Entity.Url.ToLower().StartsWith("http://") == false && Entity.Url.ToLower().StartsWith("https://") == false)
                    {
                        Entity.Url = "http://" + Entity.Url;
                    }
                }
                if (Entity.Url != null)
                {
                    Entity.Url = Entity.Url.TrimEnd('/');
                }
            }
            else
            {

                if (string.IsNullOrEmpty(SelectedModule) == false && Entity.FolderOnly == false)
                {
                    var modules = Wtm.GlobaInfo.AllModule;
                    var ndc = DC.ReCreate();
                    var actionsInDB = DC.Set<FrameworkMenu>().AsNoTracking().Where(x => x.ParentId == Entity.ID).ToList();
                    var mo = modules.Where(x => x.FullName == this.SelectedModule && x.IsApi == true).FirstOrDefault();
                    if (mo != null)
                    {
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
                                FrameworkMenu menu = new FrameworkMenu();
                                menu.FolderOnly = false;
                                menu.IsPublic = Entity.IsPublic;
                                menu.Parent = Entity;
                                menu.ShowOnMenu = false;
                                menu.DisplayOrder = order++;
                                menu.IsInside = true;
                                menu.Domain = Entity.Domain;
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
                        Entity.ModuleName = "";
                        Entity.ClassName = "";
                        Entity.MethodName = "";
                    }
                }

                else
                {
                    Entity.Children = null;
                    Entity.Url = null;
                }
            }
            if (FC.ContainsKey("Entity.Children") == false)
            {
                FC.Add("Entity.Children", 0);
                FC.Add("Entity.Children[0].IsPublic", 0);
                FC.Add("Entity.Children[0].PageName", 0);
                FC.Add("Entity.Children[0].ModuleName", 0);
                FC.Add("Entity.Children[0].ActionName", 0);
                FC.Add("Entity.Children[0].ClassName", 0);
                FC.Add("Entity.Children[0].MethodName", 0);
                FC.Add("Entity.Children[0].Url", 0);
            }
            await base.DoEdit(updateAllFields);
            List<Guid> guids = new List<Guid>();
            guids.Add(Entity.ID);
            if (Entity.Children != null)
            {
                guids.AddRange(Entity.Children?.Select(x => x.ID).ToList());
            }
            await AddPrivilege(guids);
        }

        public override async Task DoAdd()
        {
            if (Entity.IsInside == false)
            {
                if (Entity.Url != null && Entity.Url != "" && Entity.Url.StartsWith("/") == false)
                {
                    if (Entity.Url.ToLower().StartsWith("http://") == false && Entity.Url.ToLower().StartsWith("https://") == false)
                    {
                        Entity.Url = "http://" + Entity.Url;
                    }
                }
                if (Entity.Url != null)
                {
                    Entity.Url = Entity.Url.TrimEnd('/');
                }
            }
            else
            {

                if (string.IsNullOrEmpty(SelectedModule) == false && Entity.FolderOnly == false)
                {
                    var modules = Wtm.GlobaInfo.AllModule;

                    var mo = modules.Where(x => x.FullName == this.SelectedModule && x.IsApi == true).FirstOrDefault();
                    if (mo != null)
                    {
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
                                menu.IsPublic = Entity.IsPublic;
                                menu.Parent = Entity;
                                menu.ShowOnMenu = false;
                                menu.DisplayOrder = order++;
                                menu.IsInside = true;
                                menu.Domain = Entity.Domain;
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
                }

                else
                {
                    Entity.Children = null;
                    Entity.Url = null;
                }

            }
            await base.DoAdd();
            List<Guid> guids = new List<Guid>();
            guids.Add(Entity.ID);
            if (Entity.Children != null)
            {
                guids.AddRange(Entity.Children?.Select(x => x.ID).ToList());
            }
            await AddPrivilege(guids);
        }

        public async Task AddPrivilege(List<Guid> menuids)
        {
            var admin = await DC.Set<FrameworkRole>().Where(x => x.RoleCode == "001").SingleOrDefaultAsync();
            if (admin != null && SelectedRolesCodes.Contains(admin.RoleCode) == false)
            {
                SelectedRolesCodes.Add(admin.RoleCode);
            }
            var toremove = DC.Set<FunctionPrivilege>().Where(x => SelectedRolesCodes.Contains(x.RoleCode) && menuids.Contains(x.MenuItemId)).ToList();
            toremove.ForEach(x => DC.DeleteEntity(x));
            foreach (var menuid in menuids)
            {

                if (SelectedRolesCodes != null)
                {
                    foreach (var code in SelectedRolesCodes)
                    {
                        FunctionPrivilege fp = new FunctionPrivilege();
                        fp.MenuItemId = menuid;
                        fp.RoleCode = code;
                        fp.Allowed = true;
                        await DC.Set<FunctionPrivilege>().AddAsync(fp);
                    }
                }
            }
            await DC.SaveChangesAsync();
            await Wtm.RemoveUserCacheByRole(SelectedRolesCodes.ToArray());
        }


        public override async Task DoDelete()
        {
            try
            {
                DC.CascadeDelete(Entity);
                await DC.SaveChangesAsync();
            }
            catch
            { }
        }
    }
}
