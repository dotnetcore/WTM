using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.BasicData.CityVMs
{
    public partial class CityListVM : BasePagedListVM<City_View, CitySearcher>
    {

        protected override Task<IEnumerable<IGridColumn<City_View>>> InitGridHeader()
        {
            return Task.FromResult<IEnumerable<IGridColumn<City_View>>> (new List<GridColumn<City_View>>{
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.Test),
                this.MakeGridHeader(x => x.Name_view),
                this.MakeGridHeaderAction(width: 200)
            });
        }

        public override Task<IOrderedQueryable<City_View>> GetSearchQuery()
        {
            var query = DC.Set<City>()
                .CheckContain(Searcher.Name, x=>x.Name)
                .CheckContain(Searcher.Test, x=>x.Test)
                .CheckEqual(Searcher.ParentId, x=>x.ParentId)
                .Select(x => new City_View
                {
				    ID = x.ID,
                    Name = x.Name,
                    Test = x.Test,
                    Name_view = x.Parent.Name,
                })
                .OrderBy(x => x.ID);
            return Task.FromResult (query);
        }

    }

    public class City_View : City{
        [Display(Name = "名称")]
        public String Name_view { get; set; }

    }
}
