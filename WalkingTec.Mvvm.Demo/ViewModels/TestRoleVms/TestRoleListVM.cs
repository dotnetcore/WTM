using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.bbb;


namespace WalkingTec.Mvvm.Demo.ViewModels.TestRoleVms
{
    public class TestRoleListVM : BasePagedListVM<TestRole_View, TestRoleSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("TestRole", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("TestRole", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800),
                this.MakeStandardAction("TestRole", GridActionStandardTypesEnum.Delete, "删除", "",dialogWidth: 800),
                this.MakeStandardAction("TestRole", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeStandardAction("TestRole", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                this.MakeStandardAction("TestRole", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                this.MakeStandardAction("TestRole", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<TestRole_View>> InitGridHeader()
        {
            return new List<GridColumn<TestRole_View>>{
                this.MakeGridHeader(x => x.RoleCode),
                this.MakeGridHeader(x => x.RoleName),
                this.MakeGridHeader(x => x.RoleRemark),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<TestRole_View> GetSearchQuery()
        {
            var query = DC.Set<TestRole>()
                .CheckContain(Searcher.RoleCode, x=>x.RoleCode)
                .CheckContain(Searcher.RoleName, x=>x.RoleName)
                .Select(x => new TestRole_View
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

    public class TestRole_View : TestRole{

    }
}
