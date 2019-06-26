using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.ViewModels.DataTableVMs
{
    public class ActionLogListVM : BasePagedListVM<ActionLog,BaseSearcher>
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

        protected override IEnumerable<IGridColumn<ActionLog>> InitGridHeader()
        {
            var header = new List<GridColumn<ActionLog>>();

            header.Add(this.MakeGridHeader(x => x.LogType, 80).SetForeGroundFunc((entity) => {
                if (entity.LogType == ActionLogTypesEnum.Exception)
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
            header.Add(this.MakeGridHeader(x => x.Duration, 100).SetSort(true).SetForeGroundFunc((entity) => {
                if (entity.Duration <= 1)
                {
                    return "008000";
                }
                else if (entity.Duration <= 3)
                {
                    return "FFC90E";
                }
                else
                {
                    return "FF0000";
                }
            }).SetFormat((entity, v) => { return ((double)v).ToString("f2"); }));
            header.Add(this.MakeGridHeader(x => x.IP, 120));
            header.Add(this.MakeGridHeader(x => x.Remark));
            header.Add(this.MakeGridHeaderAction(width: 120));

            return header;
        }

        public override DbCommand GetSearchCommand()
        {
            var sql = string.Format("SELECT top 10 * from actionlogs");

            var cmd = DC.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;

            //var cmd = ((DbContext)DC).Database.GetDbConnection().CreateCommand();
            //cmd.CommandText = "SP_StudentInformationSummarySearch";
            //cmd.CommandType = System.Data.CommandType.StoredProcedure;
            return cmd;
        }
    }
}
