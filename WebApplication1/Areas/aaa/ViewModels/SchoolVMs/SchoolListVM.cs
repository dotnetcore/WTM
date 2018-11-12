using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Doc.Models;


namespace WebApplication1.aaa.ViewModels.SchoolVMs
{
    public class SchoolListVM : BasePagedListVM<School_View, SchoolSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Create, "新建","aaa", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Edit, "修改","aaa", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Delete, "删除", "aaa",dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Details, "详细","aaa", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.BatchEdit, "批量修改","aaa", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.BatchDelete, "批量删除","aaa", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Import, "导入","aaa", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<School_View>> InitGridHeader()
        {
            return new List<GridColumn<School_View>>{
                this.MakeGridHeader(x => x.SchoolCode),
                this.MakeGridHeader(x => x.SchoolName),
                this.MakeGridHeader(x => x.SchoolType),
                this.MakeGridHeader(x => x.Remark),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<School_View> GetSearchQuery()
        {
            var query = DC.Set<School>()
                .CheckContain(Searcher.SchoolCode, x=>x.SchoolCode)
                .CheckContain(Searcher.Remark, x=>x.Remark)
                .Select(x => new School_View
                {
				    ID = x.ID,
                    SchoolCode = x.SchoolCode,
                    SchoolName = x.SchoolName,
                    SchoolType = x.SchoolType,
                    Remark = x.Remark,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class School_View : School{

    }
}
