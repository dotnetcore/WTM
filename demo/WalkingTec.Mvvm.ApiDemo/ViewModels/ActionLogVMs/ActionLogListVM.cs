using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;


namespace WalkingTec.Mvvm.ApiDemo.ViewModels.ActionLogVMs
{
    public partial class ActionLogListVM : BasePagedListVM<ActionLog_View, ActionLogSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("ActionLog", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("ActionLog", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800),
                this.MakeStandardAction("ActionLog", GridActionStandardTypesEnum.Delete, "删除", "",dialogWidth: 800),
                this.MakeStandardAction("ActionLog", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeStandardAction("ActionLog", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                this.MakeStandardAction("ActionLog", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                this.MakeStandardAction("ActionLog", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<ActionLog_View>> InitGridHeader()
        {
            return new List<GridColumn<ActionLog_View>>{
                this.MakeGridHeader(x => x.ModuleName),
                this.MakeGridHeader(x => x.ActionName),
                this.MakeGridHeader(x => x.ITCode),
                this.MakeGridHeader(x => x.ActionUrl),
                this.MakeGridHeader(x => x.ActionTime),
                this.MakeGridHeader(x => x.Duration),
                this.MakeGridHeader(x => x.Remark),
                this.MakeGridHeader(x => x.IP),
                this.MakeGridHeader(x => x.LogType),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<ActionLog_View> GetSearchQuery()
        {
            var query = DC.Set<ActionLog>()
                .CheckContain(Searcher.ITCode, x=>x.ITCode)
                .CheckContain(Searcher.ActionUrl, x=>x.ActionUrl)
                .CheckEqual(Searcher.ActionTime, x=>x.ActionTime)
                .CheckContain(Searcher.IP, x=>x.IP)
                .CheckEqual(Searcher.LogType, x=>x.LogType)
                .Select(x => new ActionLog_View
                {
				    ID = x.ID,
                    ModuleName = x.ModuleName,
                    ActionName = x.ActionName,
                    ITCode = x.ITCode,
                    ActionUrl = x.ActionUrl,
                    ActionTime = x.ActionTime,
                    Duration = x.Duration,
                    Remark = x.Remark,
                    IP = x.IP,
                    LogType = x.LogType,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class ActionLog_View : ActionLog{

    }
}
