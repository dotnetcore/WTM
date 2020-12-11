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
        [JsonIgnore]
        public List<ComboSelectListItem> AllRoles { get; set; }

        [JsonIgnore]
        public List<ComboSelectListItem> AllGroups { get; set; }


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
                AllRoles = DC.Set<FrameworkRole>().GetSelectListItems(Wtm, y => y.RoleName);
                AllGroups = DC.Set<FrameworkGroup>().GetSelectListItems(Wtm, y => y.GroupName);
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
            await Wtm.RemoveUserCache(Entity.ID.ToString());
        }

        public override async Task DoDeleteAsync()
        {
            await base.DoDeleteAsync();
            await Wtm.RemoveUserCache(Entity.ID.ToString());
        }

        public void ChangePassword()
        {
            Entity.Password = Utils.GetMD5String(Entity.Password);
            DC.UpdateProperty(Entity, x => x.Password);
            DC.SaveChanges();
        }
    }
}
