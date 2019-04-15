using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleMDVM2 : BaseCRUDVM<FrameworkRole>
    {
        public List<Page_View> Pages { get; set; }

        public FrameworkRoleMDVM2()
        {

        }

        protected override void InitVM()
        {
            var allowedids = DC.Set<FunctionPrivilege>()
.Where(x => x.RoleId == Entity.ID && x.Allowed == true).Select(x => x.MenuItemId)
.ToList();
            var data = DC.Set<FrameworkMenu>().ToList();
            var topdata = data.Where(x => x.ParentId == null).ToList().FlatTree(x => x.DisplayOrder).Where(x => x.IsInside == false || x.FolderOnly == true || x.MethodName == null).ToList();
            int order = 0;
            var data2 = topdata.Select(x => new Page_View
            {
                ID = x.ID,
                Name = x.PageName,
                AllActions = x.FolderOnly == true ? null : x.Children.ToListItems(y => y.ActionName, y => y.ID, null),
                ParentID = x.ParentId,
                Level = x.GetLevel(),
                ExtraOrder = order++
            }).OrderBy(x => x.ExtraOrder).ToList();

            foreach (var item in data2)
            {
                if (item.AllActions == null)
                {
                    item.AllActions = new List<ComboSelectListItem>();
                }
                item.AllActions.Insert(0, new ComboSelectListItem { Text = "主页面", Value = item.ID.ToString() });
                var ids = item.AllActions.Select(x => Guid.Parse(x.Value));
                item.Actions = ids.Where(x => allowedids.Contains(x)).ToList();
            }
            Pages = data2;
        }

        public bool DoChange()
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

            var oldIDs = DC.Set<FunctionPrivilege>().Where(x => x.RoleId == Entity.ID).Select(x => x.ID).ToList();
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
                fp.RoleId = Entity.ID;
                fp.UserId = null;
                fp.Allowed = true;
                DC.Set<FunctionPrivilege>().Add(fp);
            }
            DC.SaveChanges();
            return true;
        }

    }

    public class Page_View : TopBasePoco
    {
        public string Name { get; set; }
        public List<Guid> Actions { get; set; }
        public List<ComboSelectListItem> AllActions { get; set; }

        public List<Page_View> Children { get; set; }

        [JsonIgnore]
        public int ExtraOrder { get; set; }

        [JsonIgnore]
        public Guid? ParentID { get; set; }

        [JsonIgnore]
        public Page_View Parent { get; set; }

        public int Level { get; set; }

    }

}
