using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;


namespace WalkingTec.Mvvm.ApiDemo.ViewModels.FrameworkGroupVMs
{
    public class FrameworkGroupListVM : BasePagedListVM<FrameworkGroup_View, FrameworkGroupSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800),
                this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.Delete, "删除", "",dialogWidth: 800),
                this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<FrameworkGroup_View>> InitGridHeader()
        {
            return new List<GridColumn<FrameworkGroup_View>>{
                this.MakeGridHeader(x => x.GroupCode),
                this.MakeGridHeader(x => x.GroupName),
                this.MakeGridHeader(x => x.GroupRemark),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<FrameworkGroup_View> GetSearchQuery()
        {
            var query = DC.Set<FrameworkGroup>()
                .CheckContain(Searcher.GroupCode, x=>x.GroupCode)
                .CheckContain(Searcher.GroupName, x=>x.GroupName)
                .Select(x => new FrameworkGroup_View
                {
				    ID = x.ID,
                    GroupCode = x.GroupCode,
                    GroupName = x.GroupName,
                    GroupRemark = x.GroupRemark,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class FrameworkGroup_View : FrameworkGroup{

    }
}
