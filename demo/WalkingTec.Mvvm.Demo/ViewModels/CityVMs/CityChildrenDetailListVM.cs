
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;

namespace WalkingTec.Mvvm.Demo.ViewModels.CityVMs
{
    public partial class CityChildrenDetailListVM : BasePagedListVM<City, CityDetailSearcher>
    {
        
        protected override Task<List<GridAction>> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("City", GridActionStandardTypesEnum.AddRow, "新建","", dialogWidth: 800),
                this.MakeStandardAction("City", GridActionStandardTypesEnum.RemoveRow, "删除","", dialogWidth: 800),
            };
        }
 
        protected override Task<IEnumerable<IGridColumn<City>>> InitGridHeader()
        {
            return new List<GridColumn<City>>{
                
                this.MakeGridHeader(x => x.Name).SetEditType(EditTypeEnum.TextBox),

                this.MakeGridHeaderAction(width: 200)
            };
        }

        
        public override Task<IOrderedQueryable<City>> GetSearchQuery()
        {
                
            var id = (Guid?)Searcher.ParentId.ConvertValue(typeof(Guid?));
            if (id == null)
                return Task.FromResult (new List<City> ().AsQueryable ().OrderBy (x => x.ID));
            var query = DC.Set<City>()
                .Where(x => id == x.ParentId)
                .OrderBy(x => x.ID);
            return Task.FromResult (query);
        }

    }

    public partial class CityDetailSearcher : BaseSearcher
    {
        
        [Display(Name = "_Model.City.Children")]
        public string ParentId { get; set; }
    }

}
