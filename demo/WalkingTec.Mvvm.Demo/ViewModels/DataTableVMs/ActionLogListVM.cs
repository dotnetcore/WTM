using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.ViewModels.DataTableVMs
{
    public class ActionLogListVM : BasePagedListVM<CustomView, BaseSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            var actions = new List<GridAction>
            {
                this.MakeAction(null,null,"ScriptButton",null, GridActionParameterTypesEnum.NoId).SetOnClickScript("test"),
                this.MakeAction("ActionLog","Details","详情(Dialog)","Details dialog", GridActionParameterTypesEnum.SingleId,"_Admin").SetShowDialog(true).SetShowInRow(true).SetQueryString("a=1"),
                this.MakeAction("ActionLog","Details","详情(新窗口)","Details new window", GridActionParameterTypesEnum.SingleId,"_Admin").SetShowDialog(true).SetIsRedirect(true).SetShowInRow(true).SetQueryString("a=2"),
                this.MakeAction("ActionLog","Details","详情(本窗口)","Details new window", GridActionParameterTypesEnum.SingleId,"_Admin").SetShowDialog(false).SetIsRedirect(true).SetShowInRow(true).SetQueryString("a=3"),
                this.MakeStandardAction("DataTable", GridActionStandardTypesEnum.ExportExcel, "导出","")
            };
            return actions;
        }

        protected override IEnumerable<IGridColumn<CustomView>> InitGridHeader()
        {
            var header = new List<GridColumn<CustomView>>();
            var topheader = this.MakeGridHeaderParent("父表头");
            var secondheader = this.MakeGridHeaderParent("父表头2");

            secondheader.SetChildren(
                this.MakeGridHeader(x => x.test3, 120),
                this.MakeGridHeader(x => x.test4, 120)
            );

            topheader.SetChildren(
                this.MakeGridHeader(x => x.test1, 120),
                secondheader,
                this.MakeGridHeader(x => x.test2, 120)
            );
            header.Add(topheader);
            header.Add(this.MakeGridHeader(x => "弹出框").SetTitle("弹出框").SetFormat((a, b) =>{
                return UIService.MakeButton(ButtonTypesEnum.Button, $"/_admin/actionlog/details/{a.ID}", "弹出框", 500, null,title: "日志", rtype: RedirectTypesEnum.Layer, buttonClass:"layui-btn-warm");
            }));
            header.Add(this.MakeGridHeader(x => "新Tab").SetTitle("新Tab").SetFormat((a, b) => {
                return UIService.MakeButton(ButtonTypesEnum.Button, $"/_admin/actionlog/details/{a.ID}", "弹出框", 500, null, title: "日志", rtype: RedirectTypesEnum.NewTab);
            }));
            header.Add(this.MakeGridHeader(x => "新Window").SetTitle("新Window").SetFormat((a, b) => {
                return UIService.MakeButton(ButtonTypesEnum.Button, $"/_admin/actionlog/details/{a.ID}", "弹出框", 500, null, title: "日志", rtype: RedirectTypesEnum.NewWindow);
            }));
            header.Add(this.MakeGridHeader(x => "Self").SetTitle("Self").SetFormat((a, b) => {
                return UIService.MakeButton(ButtonTypesEnum.Button, $"/_admin/actionlog/details/{a.ID}", "弹出框", 500, null, title: "日志", currentdivid:this.ViewDivId, rtype: RedirectTypesEnum.Self);
            }));

            header.Add(this.MakeGridHeaderAction(width: 320));

            return header;
        }

        public override DbCommand GetSearchCommand()
        {
            string sql = string.Empty;
            switch (ConfigInfo.DbType)
            {
                case DBTypeEnum.MySql:
                case DBTypeEnum.PgSql:
                case DBTypeEnum.SQLite:
                    sql = string.Format("SELECT id, itcode as test1, logtype as test2 from actionlogs limit 10"); break;
                case DBTypeEnum.SqlServer:
                    sql = string.Format("SELECT top 10 id, itcode as test1, logtype as test2 from actionlogs"); break;
                case DBTypeEnum.Oracle:
                    sql = string.Format("SELECT id, itcode as test1, logtype as test2 from actionlogs fetch next 10 rows only"); break;
            }
            var cmd = DC.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;

            //var cmd = ((DbContext)DC).Database.GetDbConnection().CreateCommand();
            //cmd.CommandText = "SP_StudentInformationSummarySearch";
            //cmd.CommandType = System.Data.CommandType.StoredProcedure;
            return cmd;
        }
    }

    public class CustomView : TopBasePoco
    {
        public string test1 { get; set; }
        public ActionLogTypesEnum test2 { get; set; }
        public string test3 { get; set; }
        public string test4 { get; set; }
    }
}
