using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleListVM : BasePagedListVM<FrameworkRole, FrameworkRoleSearcher>
    {

        protected override IEnumerable<IGridColumn<FrameworkRole>> InitGridHeader()
        {
            return new List<GridColumn<FrameworkRole>>{
                this.MakeGridHeader(x => x.RoleCode),
                this.MakeGridHeader(x => x.RoleName),
                this.MakeGridHeader(x => x.RoleRemark),
                this.MakeGridHeaderAction()
            };
        }

        public override IOrderedQueryable<FrameworkRole> GetSearchQuery()
        {
            var query = DC.Set<FrameworkRole>()
                .CheckContain(Searcher.RoleCode,x=>x.RoleCode)
                .CheckContain(Searcher.RoleName,x=>x.RoleName)
                .OrderBy(x => x.RoleCode);
            return query;
        }

    }
}
