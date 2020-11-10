using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms
{
    public class FrameworkUserVM : BaseCRUDVM<FrameworkUser>
    {
        [Display(Name = "Role")]
        public List<Guid> SelectedRolesIDs { get; set; }

        [Display(Name = "Group")]
        public List<Guid> SelectedGroupIDs { get; set; }


        public FrameworkUserVM()
        {
            SetInclude(x => x.UserRoles, x=>x.UserGroups);
        }

        /// <summary>
        /// 验证重复字段
        /// </summary>
        /// <returns></returns>
        public override DuplicatedInfo<FrameworkUser> SetDuplicatedCheck()
        {
            var rv = CreateFieldsInfo(SimpleField(x => x.ITCode));
            return rv;
        }

        protected override void InitVM()
        {

        }

        public override async Task DoAddAsync()
        {
            Entity.IsValid = true;
            Entity.Password = Utils.GetMD5String(Entity.Password);
            await base.DoAddAsync();
        }

        public override async Task DoEditAsync(bool updateAllFields = false)
        {
            await base.DoEditAsync(updateAllFields);
            await LoginUserInfo.RemoveUserCache(Entity.ID.ToString());
        }

        public override async Task DoDeleteAsync()
        {
            await base.DoDeleteAsync();
            await LoginUserInfo.RemoveUserCache(Entity.ID.ToString());
        }

        public void ChangePassword()
        {
            Entity.Password = Utils.GetMD5String(Entity.Password);
            DC.UpdateProperty(Entity, x => x.Password);
            DC.SaveChanges();
        }
    }
}
