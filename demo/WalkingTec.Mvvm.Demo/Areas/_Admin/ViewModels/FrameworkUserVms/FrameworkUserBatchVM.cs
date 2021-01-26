// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms
{
    public class FrameworkUserBatchVM : BaseBatchVM<FrameworkUser, FrameworkUser_BatchEdit>
    {
        public FrameworkUserBatchVM()
        {
            ListVM = new FrameworkUserListVM();
            LinkedVM = new FrameworkUser_BatchEdit();
        }

        public override bool DoBatchEdit()
        {
            var entityList = DC.Set<FrameworkUser>().AsNoTracking().CheckIDs(Ids.ToList()).ToList();
            foreach (var entity in entityList)
            {
                List<Guid> todelete = new List<Guid>();
                todelete.AddRange(DC.Set<FrameworkUserRole>().AsNoTracking().Where(x => x.UserCode == entity.ITCode).Select(x => x.ID));
                foreach (var item in todelete)
                {
                    DC.DeleteEntity(new FrameworkUserRole { ID = item });
                }

                if (LinkedVM.SelectedRolesCodes != null)
                {
                    foreach (var rolecode in LinkedVM.SelectedRolesCodes)
                    {
                        FrameworkUserRole r = new FrameworkUserRole
                        {
                            RoleCode = rolecode,
                            UserCode = entity.ITCode
                        };
                        DC.AddEntity(r);
                    }
                }
            }
            return base.DoBatchEdit();
        }
    }

    public class FrameworkUser_BatchEdit : BaseVM
    {
        [Display(Name = "_Admin.Role")]
        public List<string> SelectedRolesCodes { get; set; }
        public List<ComboSelectListItem> AllRoles { get; set; }

        protected override void InitVM()
        {
            AllRoles = DC.Set<FrameworkRole>().GetSelectListItems(Wtm, y => y.RoleName, y => y.RoleCode);
        }
    }

}
