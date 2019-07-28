using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.ActionLogVMs
{
    public class ActionLogListVM : BasePagedListVM<ActionLogDisplayModel, ActionLogSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            var actions = new List<GridAction>
            {
                this.MakeStandardAction("ActionLog", GridActionStandardTypesEnum.Details, "详细","_Admin", dialogWidth: 800).SetHideOnToolBar(true),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
            return actions;
        }

        protected override IEnumerable<IGridColumn<ActionLogDisplayModel>> InitGridHeader()
        {
            var header = new List<GridColumn<ActionLogDisplayModel>>();

            header.Add(this.MakeGridHeader(x => x.LogType, 80).SetForeGroundFunc((entity)=> {
                if(entity.LogType == ActionLogTypesEnum.Exception)
                {
                    return "FF0000";
                }
                else
                {
                    return "";
                }
            }));
            header.Add(this.MakeGridHeader(x => x.ModuleName, 120));
            header.Add(this.MakeGridHeader(x => x.ActionName, 120));
            header.Add(this.MakeGridHeader(x => x.ITCode, 120));
            header.Add(this.MakeGridHeader(x => x.ActionUrl, 200));
            header.Add(this.MakeGridHeader(x => x.ActionTime, 200).SetSort(true));
            header.Add(this.MakeGridHeader(x => x.Duration, 100).SetSort(true).SetForeGroundFunc((entity)=> {
                if(entity.Duration <= 1)
                {
                    return "008000";
                }
                else if(entity.Duration <= 3)
                {
                    return "FFC90E";
                }
                else
                {
                    return "FF0000";
                }
            }).SetFormat((entity,v)=> { return ((double)v).ToString("f2"); }));
            header.Add(this.MakeGridHeader(x => x.IP, 120));
            header.Add(this.MakeGridHeader(x => x.Remark));
            header.Add(this.MakeGridHeaderAction(width: 120));

            return header;
        }
        
        public override IOrderedQueryable<ActionLogDisplayModel> GetSearchQuery()
        {
            var query = DC.Set<ActionLog>()
                .CheckContain(Searcher.ITCode, x=>x.ITCode)
                .CheckContain(Searcher.ActionUrl, x=>x.ActionUrl)
                //.CheckEqual(Searcher.LogType, x=>x.LogType)
                .CheckContain(Searcher.LogType, x=>x.LogType)
                .CheckContain(Searcher.IP, x=>x.IP)
                .CheckBetween(Searcher.StartActionTime, Searcher.EndActionTime?.Date.AddDays(1), x=>x.ActionTime, includeMax:false)
                .Select(x=>new ActionLogDisplayModel()
                {
                    ID          = x.ID,
                    ModuleName  = x.ModuleName,
                    ITCode      = x.ITCode,
                    ActionTime  = x.ActionTime,
                    ActionName  = x.ActionName,
                    ActionUrl   = x.ActionUrl,
                    Duration    = x.Duration,
                    IP          = x.IP,
                    LogType     = x.LogType,
                    Remark      = x.Remark,
                    CreateBy    = x.CreateBy,
                    CreateTime  = x.CreateTime,
                    UpdateBy    = x.UpdateBy,
                    UpdateTime  = x.UpdateTime
                })
                .OrderByDescending(x=>x.ActionTime);

            return query;
        }
    }
}
