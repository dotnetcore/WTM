using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;


namespace WalkingTec.Mvvm.ApiDemo.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleListVM : BasePagedListVM<FrameworkRole_View, FrameworkRoleSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800),
                this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.Delete, "删除", "",dialogWidth: 800),
                this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<FrameworkRole_View>> InitGridHeader()
        {
            return new List<GridColumn<FrameworkRole_View>>{
                this.MakeGridHeader(x => x.RoleCode),
                this.MakeGridHeader(x => x.RoleName),
                this.MakeGridHeader(x => x.RoleRemark),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<FrameworkRole_View> GetSearchQuery()
        {
            var query = DC.Set<FrameworkRole>()
                .CheckContain(Searcher.RoleCode, x=>x.RoleCode)
                .CheckContain(Searcher.RoleName, x=>x.RoleName)
                .Select(x => new FrameworkRole_View
                {
				    ID = x.ID,
                    RoleCode = x.RoleCode,
                    RoleName = x.RoleName,
                    RoleRemark = x.RoleRemark,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class FrameworkRole_View : FrameworkRole{

    }
}
