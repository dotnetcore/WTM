using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.ViewModels.DataTableVMs
{
    public class ActionLogListVM : BasePagedListVM<BasePoco,BaseSearcher>
    {
        public override DbCommand GetSearchCommand()
        {
            var cmd = ((DbContext)DC).Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "SP_ActionLogSearch";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            return cmd;
        }
    }
}
