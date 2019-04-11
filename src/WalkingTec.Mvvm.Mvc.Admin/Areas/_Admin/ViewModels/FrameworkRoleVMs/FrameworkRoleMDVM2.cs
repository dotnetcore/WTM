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
                if (item.AllActions != null)
                {
                    var ids = item.AllActions.Select(x => Guid.Parse(x.Value));
                    item.Actions = ids.Where(x => allowedids.Contains(x)).ToList();
                }
            }
            Pages = data2;
        }

        public bool DoChange()
        {
            var all = FC.Where(x => x.Key.StartsWith("menu_")).ToList();
            List<Guid> AllowedMenuIds = all.Where(x => x.Value.ToString() == "1").Select(x=> Guid.Parse(x.Key.Replace("menu_",""))).ToList();
            var torem = AllowedMenuIds.Distinct();
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
