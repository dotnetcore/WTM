using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.EmployeeVMs
{
    public class EmployeeListVM : BasePagedListVM<Employee, EmployeeSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("Employee", GridActionStandardTypesEnum.AddRow, "新建","", dialogWidth: 800),
                this.MakeStandardAction("Employee", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800),
                this.MakeStandardAction("Employee", GridActionStandardTypesEnum.Delete, "删除", "",dialogWidth: 800),
                this.MakeStandardAction("Employee", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeStandardAction("Employee", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                this.MakeStandardAction("Employee", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                this.MakeStandardAction("Employee", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<Employee>> InitGridHeader()
        {
            return new List<GridColumn<Employee>>{
               this.MakeGridHeader(x => x.Name).SetEditType(EditTypeEnum.TextBox),
                 this.MakeGridHeader(x => x.Sex).SetEditType(EditTypeEnum.ComboBox, typeof(SexEnum).ToListItems(pleaseSelect:true)),
                this.MakeGridHeader(x => x.Age).SetEditType(EditTypeEnum.TextBox),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<Employee> GetSearchQuery()
        {
            var query = DC.Set<Employee>()
                .CheckEqual(Searcher.CompanyID, x=>x.CompanyID)
                .CheckContain(Searcher.Name, x=>x.Name)
                .CheckEqual(Searcher.Sex, x=>x.Sex)
                .DPWhere(LoginUserInfo.DataPrivileges,x=>x.CompanyID)               
                .OrderBy(x => x.ID);
            return query;
        }

    }

}
