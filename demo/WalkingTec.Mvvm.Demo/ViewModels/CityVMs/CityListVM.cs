using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.CityVMs
{
    public partial class CityListVM : BasePagedListVM<City_View, CitySearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("City", GridActionStandardTypesEnum.Create, Localizer["Create"],"", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.Edit, Localizer["Edit"],"", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.Delete, Localizer["Delete"], "",dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.Details, Localizer["Details"],"", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.BatchEdit, Localizer["BatchEdit"],"", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.BatchDelete, Localizer["BatchDelete"],"", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.Import, Localizer["Import"],"", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.ExportExcel, Localizer["Export"],""),
            };
        }

        protected override IEnumerable<IGridColumn<City_View>> InitGridHeader()
        {
            var rv = new List<GridColumn<City_View>>();
            rv.Add(this.MakeGridHeader(x => x.Name));
            rv.Add(this.MakeGridHeader(x => x.Name_view));

            for(int i = 0;i < 5; i++)
            {
                string name = "Dynamic" + i;
                rv.Add(this.MakeGridHeader(x => name).SetTitle(name).SetFormat((a, b) => b));
            }

            rv.Add(this.MakeGridHeaderAction(width: 200));
            return rv;
        }

        public override IOrderedQueryable<City_View> GetSearchQuery()
        {
            var query = DC.Set<City>()
                .CheckEqual(Searcher.ParentId, x=>x.ParentId)
                .DPWhere(LoginUserInfo.DataPrivileges,x=>x.ID)
                .Select(x => new City_View
                {
				    ID = x.ID,
                    Name = x.Name,
                    Name_view = x.Parent.Name,
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
