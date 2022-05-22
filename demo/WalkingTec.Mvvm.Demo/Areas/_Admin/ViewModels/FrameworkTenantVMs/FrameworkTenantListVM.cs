using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkTenantVMs
{
    public partial class FrameworkTenantListVM : BasePagedListVM<FrameworkTenant_View, FrameworkTenantSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("FrameworkTenant", GridActionStandardTypesEnum.Create, Localizer["Sys.Create"],"_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkTenant", GridActionStandardTypesEnum.Edit, Localizer["Sys.Edit"], "_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkTenant", GridActionStandardTypesEnum.Delete, Localizer["Sys.Delete"], "_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkTenant", GridActionStandardTypesEnum.Details, Localizer["Sys.Details"], "_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkTenant", GridActionStandardTypesEnum.BatchEdit, Localizer["Sys.BatchEdit"], "_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkTenant", GridActionStandardTypesEnum.BatchDelete, Localizer["Sys.BatchDelete"], "_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkTenant", GridActionStandardTypesEnum.Import, Localizer["Sys.Import"], "_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkTenant", GridActionStandardTypesEnum.ExportExcel, Localizer["Sys.Export"], "_Admin"),
            };
        }


        protected override IEnumerable<IGridColumn<FrameworkTenant_View>> InitGridHeader()
        {
            return new List<GridColumn<FrameworkTenant_View>>{
                this.MakeGridHeader(x => x.TCode),
                this.MakeGridHeader(x => x.TName),
                this.MakeGridHeader(x => x.TDb),
                this.MakeGridHeader(x => x.TDbType),
                this.MakeGridHeader(x => x.DbContext),
                this.MakeGridHeader(x => x.TDomain),
                this.MakeGridHeader(x => x.Enabled),
                this.MakeGridHeader(x => x.EnableSub),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<FrameworkTenant_View> GetSearchQuery()
        {
            var query = DC.Set<FrameworkTenant>()
                .CheckContain(Searcher.TCode, x=>x.TCode)
                .CheckContain(Searcher.TName, x=>x.TName)
                .CheckContain(Searcher.TDomain, x=>x.TDomain)
                .Select(x => new FrameworkTenant_View
                {
				    ID = x.ID,
                    TCode = x.TCode,
                    TName = x.TName,
                    TDb = x.TDb,
                    TDbType = x.TDbType,
                    DbContext = x.DbContext,
                    TDomain = x.TDomain,
                    Enabled = x.Enabled,
                    EnableSub = x.EnableSub
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class FrameworkTenant_View : FrameworkTenant{

    }
}
