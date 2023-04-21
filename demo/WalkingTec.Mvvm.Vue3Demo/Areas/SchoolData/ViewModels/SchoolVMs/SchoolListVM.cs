using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.ReactDemo.Models;


namespace WalkingTec.Mvvm.ReactDemo.ViewModels.SchoolVMs
{
    public partial class SchoolListVM : BasePagedListVM<School_View, SchoolSearcher>
    {

        protected override Task<IEnumerable<IGridColumn<School_View>>> InitGridHeader()
        {
            return Task.FromResult<IEnumerable<IGridColumn<School_View>>> (new List<GridColumn<School_View>>{
                this.MakeGridHeader(x => x.SchoolCode),
                this.MakeGridHeader(x => x.SchoolName),
                this.MakeGridHeader(x => x.SchoolType),
                this.MakeGridHeader(x => x.Remark),
                this.MakeGridHeader(x => x.Level),
                this.MakeGridHeader(x => x.Name_view),
                this.MakeGridHeaderAction(width: 200)
            });
        }

        public override Task<IOrderedQueryable<School_View>> GetSearchQuery()
        {
            var query = DC.Set<School>()
                .CheckContain(Searcher.SchoolCode, x=>x.SchoolCode)
                .CheckContain(Searcher.SchoolName, x=>x.SchoolName)
                .CheckEqual(Searcher.PlaceId, x=>x.PlaceId)
                .Select(x => new School_View
                {
				    ID = x.ID,
                    SchoolCode = x.SchoolCode,
                    SchoolName = x.SchoolName,
                    SchoolType = x.SchoolType,
                    Remark = x.Remark,
                    Level = x.Level,
                    Name_view = x.Place.Name,
                })
                .OrderBy(x => x.ID);
            return Task.FromResult (query);
        }

    }

    public class School_View : School{
        [Display(Name = "名称")]
        public String Name_view { get; set; }

    }
}
