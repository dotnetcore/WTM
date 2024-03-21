using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.BlazorDemo.Model;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.WorkFlowDemoVMs
{
    public partial class WorkFlowDemoListVM : BasePagedListVM<WorkFlowDemo_View, WorkFlowDemoSearcher>
    {

        protected override IEnumerable<IGridColumn<WorkFlowDemo_View>> InitGridHeader()
        {
            return new List<GridColumn<WorkFlowDemo_View>>{
                this.MakeGridHeader(x => x.Content),
                this.MakeGridHeader(x => "CanApprove").SetHide().SetFormat((a, b) =>
                {
                    return a.CanApprove;
                }),
                this.MakeGridHeader(x => "Tag").SetHide().SetFormat((a, b) =>
                {
                    return a.Tag;
                }),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<WorkFlowDemo_View> GetSearchQuery()
        {
            var query = DC.Set<WorkFlowDemo>()
                .CheckContain(Searcher.Content, x=>x.Content)
                .Select(x => new WorkFlowDemo_View
                {
				    ID = x.ID,
                    Content = x.Content,
                })
                .OrderBy(x => x.ID);
            return query;
        }

        public override void AfterDoSearcher()
        {
            if (Wtm.LoginUserInfo != null)
            {
                var ids = GetMyApproves();
                foreach (var item in EntityList)
                {
                    var m = ids.Find(x => x.ModelID == item.GetID().ToString());
                    if (m != null)
                    {
                        item.Tag = m.Tag;
                        item.CanApprove = "true";
                    }
                }
            }
        }

    }

    public class WorkFlowDemo_View : WorkFlowDemo{
        public string CanApprove { get; set; } = "false";

        public string Tag { get; set; }
    }
}
