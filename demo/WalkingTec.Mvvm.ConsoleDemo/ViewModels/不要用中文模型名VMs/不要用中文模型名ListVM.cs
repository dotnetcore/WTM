using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.不要用中文模型名VMs
{
    public partial class 不要用中文模型名ListVM : BasePagedListVM<不要用中文模型名_View, 不要用中文模型名Searcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("不要用中文模型名", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("不要用中文模型名", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800),
                this.MakeStandardAction("不要用中文模型名", GridActionStandardTypesEnum.Delete, "删除", "",dialogWidth: 800),
                this.MakeStandardAction("不要用中文模型名", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeStandardAction("不要用中文模型名", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                this.MakeStandardAction("不要用中文模型名", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                this.MakeStandardAction("不要用中文模型名", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardAction("不要用中文模型名", GridActionStandardTypesEnum.ExportExcel, "导出",""),
            };
        }

        protected override IEnumerable<IGridColumn<不要用中文模型名_View>> InitGridHeader()
        {
            return new List<GridColumn<不要用中文模型名_View>>{
                this.MakeGridHeader(x => x.不要),
                this.MakeGridHeader(x => x.用),
                this.MakeGridHeader(x => x.中文),
                this.MakeGridHeader(x => x.模型名),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<不要用中文模型名_View> GetSearchQuery()
        {
            var query = DC.Set<不要用中文模型名>()
                .CheckContain(Searcher.不要, x=>x.不要)
                .CheckEqual(Searcher.用, x=>x.用)
                .CheckEqual(Searcher.中文, x=>x.中文)
                .CheckEqual(Searcher.模型名, x=>x.模型名)
                .Select(x => new 不要用中文模型名_View
                {
				    ID = x.ID,
                    不要 = x.不要,
                    用 = x.用,
                    中文 = x.中文,
                    模型名 = x.模型名,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class 不要用中文模型名_View : 不要用中文模型名{

    }
}
