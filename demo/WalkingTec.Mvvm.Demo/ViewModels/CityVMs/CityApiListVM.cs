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
    public partial class CityApiListVM : BasePagedListVM<CityApi_View, CityApiSearcher>
    {

        protected override IEnumerable<IGridColumn<CityApi_View>> InitGridHeader()
        {
            return new List<GridColumn<CityApi_View>>{
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.Test),
                this.MakeGridHeader(x => x.Name_view),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<CityApi_View> GetSearchQuery()
        {
            var query = DC.Set<City>()
                .CheckContain(Searcher.Name, x=>x.Name)
                .CheckContain(Searcher.Test, x=>x.Test)
                .CheckEqual(Searcher.ParentId, x=>x.ParentId)
                .Select(x => new CityApi_View
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

    public class CityApi_View : City{
        [Display(Name = "名称")]
        public String Name_view { get; set; }

    }
}
