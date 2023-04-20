// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms
{
    public class FrameworkUserBatchVM : BaseBatchVM<FrameworkUser, FrameworkUser_BatchEdit>
    {
        [JsonIgnore]
        public FrameworkRoleListVM RoleListVM { get; set; }

        public FrameworkUserBatchVM()
        {
            ListVM = new FrameworkUserListVM();
            LinkedVM = new FrameworkUser_BatchEdit();
        }

        protected override Task InitVM()
        {
            RoleListVM = Wtm.CreateVM<FrameworkRoleListVM>();
            return Task.CompletedTask;
        }

        public override async Task<bool> DoBatchEdit()
        {
            var entityList = await DC.Set<FrameworkUser>().AsNoTracking().CheckIDs(Ids.ToList()).ToListAsync();
            foreach (var entity in entityList)
            {
                List<Guid> todelete = new List<Guid>();
                todelete.AddRange(await DC.Set<FrameworkUserRole>().AsNoTracking().Where(x => x.UserCode == entity.ITCode).Select(x => x.ID).ToListAsync ());
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
            return await base.DoBatchEdit();
        }
    }

    public class FrameworkUser_BatchEdit : BaseVM
    {
        [Display(Name = "_Admin.Role")]
        public List<string> SelectedRolesCodes { get; set; }
        public List<ComboSelectListItem> AllRoles { get; set; }

        protected override async Task InitVM()
        {
            AllRoles = await DC.Set<FrameworkRole>().GetSelectListItems(Wtm, y => y.RoleName, y => y.RoleCode);
        }
    }

}
