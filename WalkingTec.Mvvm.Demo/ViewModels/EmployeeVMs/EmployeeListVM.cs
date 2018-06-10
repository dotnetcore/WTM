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
    public class EmployeeListVM : BasePagedListVM<Employee_View, EmployeeSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("Employee", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("Employee", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800),
                this.MakeStandardAction("Employee", GridActionStandardTypesEnum.Delete, "删除", "",dialogWidth: 800),
                this.MakeStandardAction("Employee", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeStandardAction("Employee", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                this.MakeStandardAction("Employee", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                this.MakeStandardAction("Employee", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<Employee_View>> InitGridHeader()
        {
            return new List<GridColumn<Employee_View>>{
                this.MakeGridHeader(x => x.Name_view),
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.Age),
                this.MakeGridHeader(x => x.Sex),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<Employee_View> GetSearchQuery()
        {
            var query = DC.Set<Employee>()
                .CheckEqual(Searcher.CompanyID, x=>x.CompanyID)
                .CheckContain(Searcher.Name, x=>x.Name)
                .CheckEqual(Searcher.Sex, x=>x.Sex)
                .DPWhere(LoginUserInfo.DataPrivileges,x=>x.CompanyID)
                .Select(x => new Employee_View
                {
				    ID = x.ID,
                    Name_view = x.Company.Name,
                    Name = x.Name,
                    Age = x.Age,
                    Sex = x.Sex,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class Employee_View : Employee{
        [Display(Name = "公司名称")]
        public String Name_view { get; set; }

    }
}
