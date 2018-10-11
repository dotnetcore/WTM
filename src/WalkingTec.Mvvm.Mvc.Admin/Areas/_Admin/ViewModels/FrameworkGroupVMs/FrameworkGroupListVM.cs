using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkGroupVMs
{
    public class FrameworkGroupListVM : BasePagedListVM<FrameworkGroup, FrameworkGroupSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.Create, "新建","_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.Edit, "修改","_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.Delete, "删除", "_Admin",dialogWidth: 800),
                this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.BatchDelete, "批量删除","_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.Import, "导入","_Admin", dialogWidth: 800),
                this.MakeAction("FrameworkGroup","DataFunction","数据权限","数据权限", GridActionParameterTypesEnum.SingleId,"_Admin",800).SetShowInRow(),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<FrameworkGroup>> InitGridHeader()
        {
            return new List<GridColumn<FrameworkGroup>>{
                this.MakeGridHeader(x => x.GroupCode, 120),
                this.MakeGridHeader(x => x.GroupName, 120),
                this.MakeGridHeader(x => x.GroupRemark),
                this.MakeGridHeaderAction(width: 300)
            };
        }

        public override IOrderedQueryable<FrameworkGroup> GetSearchQuery()
        {
            var query = DC.Set<FrameworkGroup>()
                .CheckContain(Searcher.GroupCode, x=>x.GroupCode)
                .CheckContain(Searcher.GroupName, x=>x.GroupName)
                .OrderBy(x => x.GroupCode);
            return query;
        }

    }
}
