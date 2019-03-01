using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.ApiDemo;


namespace WalkingTec.Mvvm.ApiDemo.ViewModels.TestModelVMs
{
    public class TestModelListVM : BasePagedListVM<TestModel_View, TestModelSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("TestModel", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("TestModel", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800),
                this.MakeStandardAction("TestModel", GridActionStandardTypesEnum.Delete, "删除", "",dialogWidth: 800),
                this.MakeStandardAction("TestModel", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeStandardAction("TestModel", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                this.MakeStandardAction("TestModel", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                this.MakeStandardAction("TestModel", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<TestModel_View>> InitGridHeader()
        {
            return new List<GridColumn<TestModel_View>>{
                this.MakeGridHeader(x => x.Teset),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<TestModel_View> GetSearchQuery()
        {
            var query = DC.Set<TestModel>()
                .CheckEqual(Searcher.Teset, x=>x.Teset)
                .Select(x => new TestModel_View
                {
				    ID = x.ID,
                    Teset = x.Teset,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class TestModel_View : TestModel{

    }
}
