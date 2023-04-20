using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.LinkTestVMs
{
    public partial class LinkTestListVM : BasePagedListVM<LinkTest_View, LinkTestSearcher>
    {
        protected override Task<List<GridAction>> InitGridAction()
        {
            return Task.FromResult (new List<GridAction>
            {
                this.MakeStandardAction("LinkTest", GridActionStandardTypesEnum.Create, Localizer["Sys.Create"],"", dialogWidth: 800),
                this.MakeStandardAction("LinkTest", GridActionStandardTypesEnum.Edit, Localizer["Sys.Edit"], "", dialogWidth: 800),
                this.MakeStandardAction("LinkTest", GridActionStandardTypesEnum.Delete, Localizer["Sys.Delete"], "", dialogWidth: 800),
                this.MakeStandardAction("LinkTest", GridActionStandardTypesEnum.Details, Localizer["Sys.Details"], "", dialogWidth: 800),
                this.MakeStandardAction("LinkTest", GridActionStandardTypesEnum.BatchEdit, Localizer["Sys.BatchEdit"], "", dialogWidth: 800),
                this.MakeStandardAction("LinkTest", GridActionStandardTypesEnum.BatchDelete, Localizer["Sys.BatchDelete"], "", dialogWidth: 800),
                this.MakeStandardAction("LinkTest", GridActionStandardTypesEnum.Import, Localizer["Sys.Import"], "", dialogWidth: 800),
                this.MakeStandardAction("LinkTest", GridActionStandardTypesEnum.ExportExcel, Localizer["Sys.Export"], ""),
            });
        }


        protected override Task<IEnumerable<IGridColumn<LinkTest_View>>> InitGridHeader()
        {
            return Task.FromResult<IEnumerable<IGridColumn<LinkTest_View>>> (new List<GridColumn<LinkTest_View>>{
                this.MakeGridHeader(x => x.name),
                this.MakeGridHeader(x => x.Name_view),
                this.MakeGridHeaderAction(width: 200)
            });
        }

        public override Task<IOrderedQueryable<LinkTest_View>> GetSearchQuery()
        {
            var query = DC.Set<LinkTest>()
                .CheckContain(Searcher.name, x=>x.name)
                .CheckEqual(Searcher.StudentId, x=>x.StudentId)
                .Select(x => new LinkTest_View
                {
				    ID = x.ID,
                    name = x.name,
                    Name_view = x.Student.Name,
                })
                .OrderBy(x => x.ID);
            return Task.FromResult (query);
        }

    }

    public class LinkTest_View : LinkTest{
        [Display(Name = "姓名")]
        public String Name_view { get; set; }

    }
}
