using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.MyUserVMs
{
    public partial class MyUserVM : BaseCRUDVM<MyUser>
    {
        public List<ComboSelectListItem> AllUserRoless { get; set; }
        public List<ComboSelectListItem> AllUserGroupss { get; set; }

        public MyUserVM()
        {
            SetInclude(x => x.UserRoles);
            SetInclude(x => x.UserGroups);
        }

        public override void Validate()
        {
            base.Validate();
        }

        protected override void InitVM()
        {
            AllUserRoless = DC.Set<FrameworkRole>().GetSelectListItems(Wtm, y => y.RoleName);
            AllUserGroupss = DC.Set<FrameworkGroup>().GetSelectListItems(Wtm, y => y.GroupName);
        }

    }
}
