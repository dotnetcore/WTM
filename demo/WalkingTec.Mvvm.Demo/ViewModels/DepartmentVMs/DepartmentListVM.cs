using System.ComponentModel.DataAnnotations;
using System;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Models;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Demo.ViewModels.DepartmentVMs
{
    public partial class DepartmentListVM : BasePagedListVM<Department_View, DepartmentSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("Department", GridActionStandardTypesEnum.Create, Localizer["Sys.Create"],dialogWidth: 800),
                this.MakeStandardAction("Department", GridActionStandardTypesEnum.Edit, Localizer["Sys.Edit"],  dialogWidth: 800),
                this.MakeStandardAction("Department", GridActionStandardTypesEnum.Delete, Localizer["Sys.Delete"],  dialogWidth: 800),
                this.MakeStandardAction("Department", GridActionStandardTypesEnum.Details, Localizer["Sys.Details"], dialogWidth: 800),
                this.MakeStandardAction("Department", GridActionStandardTypesEnum.BatchEdit, Localizer["Sys.BatchEdit"], dialogWidth: 800),
                this.MakeStandardAction("Department", GridActionStandardTypesEnum.BatchDelete, Localizer["Sys.BatchDelete"], dialogWidth: 800),
                this.MakeStandardAction("Department", GridActionStandardTypesEnum.Import, Localizer["Sys.Import"], dialogWidth: 800),
                this.MakeStandardAction("Department", GridActionStandardTypesEnum.ExportExcel, Localizer["Sys.Export"]),
            };
        }

        protected override IEnumerable<IGridColumn<Department_View>> InitGridHeader()
        {
            return new List<GridColumn<Department_View>>{
                this.MakeGridHeader(x => x.DepName),
                this.MakeGridHeader(x => x.DepName_view),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<Department_View> GetSearchQuery()
        {
            var query = DC.Set<Department>()
                .CheckContain(Searcher.DepName, x => x.DepName)
                .Select(x => new Department_View
                {
                    ID = x.ID,
                    DepName = x.DepName,
                    DepName_view = x.Parent.DepName,
                })
                .OrderBy(x => x.ID);
            return query;
        }
    }

    public class Department_View : Department
    {
        [Display(Name = "部门名称")]
        public String DepName_view { get; set; }

    }
}
