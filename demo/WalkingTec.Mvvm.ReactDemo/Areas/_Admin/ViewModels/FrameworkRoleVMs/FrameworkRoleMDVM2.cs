// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleMDVM2 : BaseCRUDVM<FrameworkRole>
    {
        public List<Page_View> Pages { get; set; }

        public FrameworkRoleMDVM2()
        {

        }

        protected override async Task<FrameworkRole> GetById(object Id)
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return Wtm.CallAPI<FrameworkRoleVM>("mainhost", $"/api/_frameworkrole/{Id}").Result.Data.Entity;
            }
            else
            {
                return await base.GetById(Id);
            }
        }

        protected override async Task InitVM()
        {
            var allowedids = DC.Set<FunctionPrivilege>()
                                        .Where(x => x.RoleCode == Entity.RoleCode && x.Allowed == true).Select(x => x.MenuItemId)
                                        .ToList();
            List<FrameworkMenu> data = new List<FrameworkMenu>();
            using (var maindc = Wtm.CreateDC(false, "default"))
            {
                data = await maindc.Set<FrameworkMenu>().ToListAsync();
            }
            var topdata = data.Where(x => x.ParentId == null).ToList().FlatTree(x => x.DisplayOrder).Where(x => x.IsInside == false || x.FolderOnly == true || string.IsNullOrEmpty(x.MethodName)).ToList();

            if (Wtm.ConfigInfo.EnableTenant == true && (await GetLoginUserInfo ()).CurrentTenant != null)
            {
                var _current_tenant = (await GetLoginUserInfo ()).CurrentTenant;
                var ct = Wtm.GlobaInfo.AllTenant.Where(x => x.TCode == _current_tenant).FirstOrDefault();
                for (int i = 0; i < topdata.Count; i++)
                {
                    if (topdata[i].TenantAllowed == false || (topdata[i].Url != null && ct.EnableSub == false && topdata[i].Url.ToLower().Contains("frameworktenant")))
                    {
                        topdata.RemoveAt(i);
                        i--;
                        continue;
                    }
                    var hostonly = Wtm.GlobaInfo.AllMainTenantOnlyUrls;
                    foreach (var au in hostonly)
                    {
                        if (topdata[i].Url != null && new Regex("^" + au + "[/\\?]?", RegexOptions.IgnoreCase).IsMatch(topdata[i].Url))
                        {
                            topdata.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                }
            }


            int order = 0;
            var data2 = topdata.Select(x => new Page_View
            {
                ID = x.ID,
                Name = x.PageName,
                AllActions = x.FolderOnly == true ? null : x.Children.ToListItems(y => y.ActionName, y => y.ID, null),
                ParentID = x.ParentId,
                Level = x.GetLevel(),
                IsFolder = x.FolderOnly,
                ExtraOrder = order++
            }).OrderBy(x => x.ExtraOrder).ToList();

            foreach (var item in data2)
            {
                if (item.Name?.StartsWith("MenuKey.") == true)
                {
                    item.Name = Localizer[item.Name];
                }
                if (item.AllActions == null)
                {
                    item.AllActions = new List<ComboSelectListItem>();
                }
                foreach (var act in item.AllActions)
                {
                    act.Text = Localizer[act.Text];
                }
                item.AllActions.Insert(0, new ComboSelectListItem { Text = Localizer["Sys.MainPage"], Value = item.ID.ToString() });
                var ids = item.AllActions.Select(x => Guid.Parse(x.Value.ToString()));
                item.Actions = ids.Where(x => allowedids.Contains(x)).ToList();
            }
            Pages = data2;
        }

        public async Task<bool> DoChangeAsync()
        {
            List<Guid> AllowedMenuIds = new List<Guid>();
            var torem = AllowedMenuIds.Distinct();

            foreach (var page in Pages)
            {
                if (page.Actions != null)
                {
                    foreach (var action in page.Actions)
                    {
                        if (AllowedMenuIds.Contains(action) == false)
                        {
                            AllowedMenuIds.Add(action);
                        }
                    }
                }
            }

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
                fp.TenantCode = (await GetLoginUserInfo ()).CurrentTenant;
                fp.Allowed = true;
                DC.Set<FunctionPrivilege>().Add(fp);
            }
            await DC.SaveChangesAsync();
            await Wtm.RemoveUserCacheByRole(Entity.RoleCode);
            return true;
        }

    }

    public class Page_View : TopBasePoco
    {
        [Display(Name = "_Admin.PageName")]
        public string Name { get; set; }
        [Display(Name = "_Admin.PageFunction")]
        public List<Guid> Actions { get; set; }
        [Display(Name = "_Admin.PageFunction")]
        public List<ComboSelectListItem> AllActions { get; set; }

        public List<Page_View> Children { get; set; }

        public bool IsFolder { get; set; }
        [JsonIgnore]
        public int ExtraOrder { get; set; }

        public Guid? ParentID { get; set; }

        [JsonIgnore]
        public Page_View Parent { get; set; }

        public int Level { get; set; }

    }

}
