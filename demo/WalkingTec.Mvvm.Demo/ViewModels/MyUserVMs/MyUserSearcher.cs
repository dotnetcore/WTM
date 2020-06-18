using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.MyUserVMs
{
    public partial class MyUserSearcher : BaseSearcher
    {
        [Display(Name = "附加信息2")]
        public String Extra2 { get; set; }
        [Display(Name = "Email")]
        public String Email { get; set; }
        [Display(Name = "CellPhone")]
        public String CellPhone { get; set; }
        [Display(Name = "IsValid")]
        public Boolean? IsValid { get; set; }
        public List<ComboSelectListItem> AllUserRoless { get; set; }
        [Display(Name = "Role")]
        public List<Guid> SelectedUserRolesIDs { get; set; }
        public List<ComboSelectListItem> AllUserGroupss { get; set; }
        [Display(Name = "Group")]
        public List<Guid> SelectedUserGroupsIDs { get; set; }

        protected override void InitVM()
        {
            AllUserRoless = DC.Set<FrameworkRole>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, y => y.RoleName);
            AllUserGroupss = DC.Set<FrameworkGroup>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, y => y.GroupName);
        }

    }
}
