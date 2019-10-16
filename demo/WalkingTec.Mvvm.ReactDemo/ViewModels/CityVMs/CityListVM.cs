using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.ReactDemo.Models;


namespace WalkingTec.Mvvm.ReactDemo.ViewModels.CityVMs
{
    public class CityListVM : BasePagedListVM<City_View, CitySearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("City", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.Delete, "删除", "",dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.ExportExcel, "导出","")
            };
        }

        protected override IEnumerable<IGridColumn<City_View>> InitGridHeader()
        {
            return new List<GridColumn<City_View>>{
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.Name_view),
                this.MakeGridHeader(x => x.Level),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<City_View> GetSearchQuery()
        {
            var query = DC.Set<City>()
                .Select(x => new City_View
                {
				    ID = x.ID,
                    Name = x.Name,
                    Name_view = x.Parent.Name,
                    Level = x.Level,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class City_View : City{
        [Display(Name = "名称")]
        public String Name_view { get; set; }

    }
}
