using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleListVM : BasePagedListVM<FrameworkRole, FrameworkRoleSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.Create, "新建","_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.Edit, "修改","_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.Delete, "删除", "_Admin",dialogWidth: 800),
                this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.Details, "详细","_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.BatchDelete, "批量删除","_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.Import, "导入","_Admin", dialogWidth: 800),
                this.MakeAction("FrameworkRole","PageFunction","页面权限","页面权限", GridActionParameterTypesEnum.SingleId,"_Admin",800).SetShowInRow(),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<FrameworkRole>> InitGridHeader()
        {
            return new List<GridColumn<FrameworkRole>>{
                this.MakeGridHeader(x => x.RoleCode, 120),
                this.MakeGridHeader(x => x.RoleName, 120),
                this.MakeGridHeader(x => x.RoleRemark),
                this.MakeGridHeaderAction(width: 300)
            };
        }

        public override IOrderedQueryable<FrameworkRole> GetSearchQuery()
        {
            //var query = DC.Set<FrameworkRole>()
            //    .CheckWhere(Searcher.RoleCode, x => x.RoleCode.Contains(Searcher.RoleCode))
            //    .CheckWhere(Searcher.RoleName, x => x.RoleName.ToLower().Contains(Searcher.RoleName.ToLower()))
            //    .OrderBy(x => x.RoleCode);
            var query = DC.Set<FrameworkRole>()
                .CheckContain(Searcher.RoleCode,x=>x.RoleCode)
                .CheckContain(Searcher.RoleName,x=>x.RoleName)
                .OrderBy(x => x.RoleCode);
            return query;
        }

    }
}
