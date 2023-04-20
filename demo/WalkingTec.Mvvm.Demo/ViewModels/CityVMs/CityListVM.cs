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
        protected override Task<List<GridAction>> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("City", GridActionStandardTypesEnum.Create, Localizer["Sys.Create"],"", dialogWidth: 800),
                this.MakeAction("City","CreateGroup","groupadd","groupadd", GridActionParameterTypesEnum.NoId).SetShowInRow(false),
               this.MakeStandardAction("City", GridActionStandardTypesEnum.Edit, Localizer["Sys.Edit"], "", dialogWidth: 800),
                this.MakeAction("City","EditGroup","groupadd","groupedit", GridActionParameterTypesEnum.SingleId).SetShowInRow(true).SetHideOnToolBar(),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.Delete, Localizer["Sys.Delete"], "", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.Details, Localizer["Sys.Details"], "", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.BatchEdit, Localizer["Sys.BatchEdit"], "", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.BatchDelete, Localizer["Sys.BatchDelete"], "", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.Import, Localizer["Sys.Import"], "", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.ExportExcel, Localizer["Sys.Export"], "")
            };
        }


        protected override Task<IEnumerable<IGridColumn<City_View>>> InitGridHeader()
        {
            return new List<GridColumn<City_View>>{
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.Test),
                this.MakeGridHeader(x => x.Name_view),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override async Task<IOrderedQueryable<City_View>> GetSearchQuery()
        {
            var query = (await DC.Set<City>()
                .CheckContain(Searcher.Name, x=>x.Name)
                .DPWhere(Wtm, x=>x.ID))
                //.DPWhere(Wtm, x=>x.ParentId)
                //.DPWhere(Wtm, x => x.Parent.Children[0].Children[0].ID)
                .Select(x => new City_View
                {
				    ID = x.ID,
                    Name = x.Name,
                    Test = x.Test,
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
