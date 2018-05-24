using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Serialization;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core
{
    public class FrameworkUserBaseListVM : BasePagedListVM<FrameworkUserBase, FrameworkUserBaseSearcher>
    {
        public List<FrameworkRole> AllRoles { get; set; }
        protected override IEnumerable<IGridColumn<FrameworkUserBase>> InitGridHeader()
        {
            return new List<GridColumn<FrameworkUserBase>>{
                this.MakeGridHeader(x => x.ITCode, 120).SetEditType(),
                this.MakeGridHeader(x => x.Name, 120).SetEditType(),
                this.MakeGridHeader(x => x.UserRoles, 240).SetEditType().SetFormat((entity,val)=>{
                    return entity.UserRoles.Select(x=>x.Role.RoleName).ToList().ToSpratedString();
                })
            };
        }

        public override IOrderedQueryable<FrameworkUserBase> GetSearchQuery()
        {

            var query = DC.Set<FrameworkUserBase>().Include(x=>x.UserRoles).ThenInclude(r=>r.Role)
                .CheckContain(Searcher.ITCode, x=>x.ITCode)
                .CheckContain(Searcher.Name, x=>x.Name)
                .Select(x => new FrameworkUserBase
                {
                    ID = x.ID,
                    ITCode = x.ITCode,
                    Name = x.Name,
                    UserRoles = x.UserRoles,
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
        [XmlIgnore]
        public List<FrameworkUserRole> UserRoles { get; set; }
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
