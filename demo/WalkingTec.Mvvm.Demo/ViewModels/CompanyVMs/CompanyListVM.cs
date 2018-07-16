using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.CompanyVMs
{
    public class CompanyListVM : BasePagedListVM<Company_View, CompanySearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("Company", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("Company", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800),
                this.MakeStandardAction("Company", GridActionStandardTypesEnum.Delete, "删除", "",dialogWidth: 800),
                this.MakeStandardAction("Company", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeStandardAction("Company", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                this.MakeStandardAction("Company", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                this.MakeStandardAction("Company", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<Company_View>> InitGridHeader()
        {
            return new List<GridColumn<Company_View>>{
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.Description),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<Company_View> GetSearchQuery()
        {
            var query = DC.Set<Company>()
                .CheckContain(Searcher.Name, x=>x.Name)
                .CheckContain(Searcher.Description, x=>x.Description)
                .Select(x => new Company_View
                {
				    ID = x.ID,
                    Name = x.Name,
                    Description = x.Description,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class Company_View : Company{

    }
}
