using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Serialization;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms
{
    public class FrameworkUserBaseListVM : BasePagedListVM<FrameworkUserBase_ListView, FrameworkUserBaseSearcher>
    {
        public List<FrameworkRole> AllRoles { get; set; }
        protected override IEnumerable<IGridColumn<FrameworkUserBase_ListView>> InitGridHeader()
        {
            return new List<GridColumn<FrameworkUserBase_ListView>>{
                this.MakeGridHeader(x => x.ITCode, 120).SetEditType(),
                this.MakeGridHeader(x => x.Name, 120).SetEditType(),
                this.MakeGridHeader(x => x.RoleName_view),
                this.MakeGridHeader(x => x.GroupName_view),
            };
        }

        public override IOrderedQueryable<FrameworkUserBase_ListView> GetSearchQuery()
        {

            var query = DC.Set<FrameworkUserBase>().Include(x=>x.UserRoles).ThenInclude(r=>r.Role)
                .CheckContain(Searcher.ITCode, x=>x.ITCode)
                .CheckContain(Searcher.Name, x=>x.Name)
                .Select(x => new FrameworkUserBase_ListView
                {
                    ID = x.ID,
                    ITCode = x.ITCode,
                    Name = x.Name,
                    RoleName_view = DC.Set<FrameworkRole>().Where(y => x.UserRoles.Select(z => z.RoleId).Contains(y.ID)).Select(y => y.RoleName).ToSpratedString(null, ","),
                    GroupName_view = DC.Set<FrameworkGroup>().Where(y => x.UserGroups.Select(z => z.GroupId).Contains(y.ID)).Select(y => y.GroupName).ToSpratedString(null, ","),
                })
                .OrderBy(x => x.ITCode);
            return query;
        }

    }

    public class FrameworkUserBase_ListView : BasePoco
    {
        [Display(Name = "账号")]
        public string ITCode { get; set; }

        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Display(Name = "角色")]
        public string RoleName_view { get; set; }

        [Display(Name = "用户组")]
        public string GroupName_view { get; set; }
    }

    public class FrameworkUserBaseSearcher : BaseSearcher
    {
        [Display(Name = "账号")]
        public string ITCode { get; set; }

        [Display(Name = "名字")]
        public string Name { get; set; }

        [Display(Name = "类型")]
        public List<Guid> Roles { get; set; }

        public List<ComboSelectListItem> AllRoles { get; set; }

        protected override void InitVM()
        {
            AllRoles = DC.Set<FrameworkRole>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, x => x.RoleName, x => x.ID.ToString(), true, true);
        }
    }

}
