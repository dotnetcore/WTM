using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.LinkTest2VMs
{
    public partial class LinkTest2ListVM : BasePagedListVM<LinkTest2_View, LinkTest2Searcher>
    {
        protected override Task<List<GridAction>> InitGridAction()
        {
            return Task.FromResult (new List<GridAction>
            {
                this.MakeStandardAction("LinkTest2", GridActionStandardTypesEnum.Create, Localizer["Sys.Create"],"", dialogWidth: 800),
                this.MakeStandardAction("LinkTest2", GridActionStandardTypesEnum.Edit, Localizer["Sys.Edit"], "", dialogWidth: 800),
                this.MakeStandardAction("LinkTest2", GridActionStandardTypesEnum.Delete, Localizer["Sys.Delete"], "", dialogWidth: 800),
                this.MakeStandardAction("LinkTest2", GridActionStandardTypesEnum.Details, Localizer["Sys.Details"], "", dialogWidth: 800),
                this.MakeStandardAction("LinkTest2", GridActionStandardTypesEnum.BatchEdit, Localizer["Sys.BatchEdit"], "", dialogWidth: 800),
                this.MakeStandardAction("LinkTest2", GridActionStandardTypesEnum.BatchDelete, Localizer["Sys.BatchDelete"], "", dialogWidth: 800),
                this.MakeStandardAction("LinkTest2", GridActionStandardTypesEnum.Import, Localizer["Sys.Import"], "", dialogWidth: 800),
                this.MakeStandardAction("LinkTest2", GridActionStandardTypesEnum.ExportExcel, Localizer["Sys.Export"], ""),
            });
        }


        protected override Task<IEnumerable<IGridColumn<LinkTest2_View>>> InitGridHeader()
        {
            return Task.FromResult<IEnumerable<IGridColumn<LinkTest2_View>>> (new List<GridColumn<LinkTest2_View>>{
                this.MakeGridHeader(x => x.name),
                this.MakeGridHeader(x => x.Name_view),
                this.MakeGridHeaderAction(width: 200)
            });
        }

        public override Task<IOrderedQueryable<LinkTest2_View>> GetSearchQuery()
        {
            var query = DC.Set<LinkTest2>()
                .CheckContain(Searcher.name, x=>x.name)
                .CheckWhere(Searcher.SelectedLinkStudentIDs,x=>DC.Set<LinkTestStudent>().Where(y=>Searcher.SelectedLinkStudentIDs.Contains(y.StudentId)).Select(z=>z.LinkTestId).Contains(x.ID))
                .Select(x => new LinkTest2_View
                {
				    ID = x.ID,
                    name = x.name,
                    Name_view = x.LinkStudent.Select(y=>y.Student.Name).ToSepratedString(null,","), 
                })
                .OrderBy(x => x.ID);
            return Task.FromResult (query);
        }

    }

    public class LinkTest2_View : LinkTest2{
        [Display(Name = "姓名")]
        public String Name_view { get; set; }

    }
}
