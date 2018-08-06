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
    public class ActionLogListVM : BasePagedListVM<BasePoco,BaseSearcher>
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

        public override DbCommand GetSearchCommand()
        {
            string formatColumn = "ITCode,Name";
            // 存储过程中的动态sql
            var sql = string.Format("SELECT {0} FROM studentinformationsummary LIMIT {1},{2}", formatColumn, 0, 20);

            var cmd = ((DbContext)DC).Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "SP_StudentInformationSummarySearch";
            cmd.Parameters.Add(new MySqlParameter() { ParameterName = "ColumnsParams", Value = sql });
            cmd.CommandType = CommandType.StoredProcedure;

            //var cmd = ((DbContext)DC).Database.GetDbConnection().CreateCommand();
            //cmd.CommandText = "SP_StudentInformationSummarySearch";
            //cmd.CommandType = System.Data.CommandType.StoredProcedure;
            return cmd;
        }
    }
}
