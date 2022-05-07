// WTM默认页面 Wtm buidin page
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkGroupVMs
{
    public class FrameworkGroupListVM : BasePagedListVM<FrameworkGroup, FrameworkGroupSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            if (ConfigInfo.HasMainHost)
            {
                return new List<GridAction>
                {
                    this.MakeAction("FrameworkGroup","DataFunction",Localizer["_Admin.DataPrivilege"],Localizer["_Admin.DataPrivilege"], GridActionParameterTypesEnum.SingleId,"_Admin",800,null,null,x=>x.GroupCode).SetShowInRow(),
                };
            }
            else
            {
                return new List<GridAction>
                {
                    this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.Create, "","_Admin", dialogWidth: 800),
                    this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.Edit, "","_Admin", dialogWidth: 800),
                    this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.Delete, "", "_Admin",dialogWidth: 800),
                    this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.BatchDelete, "","_Admin", dialogWidth: 800),
                    this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.Import, "","_Admin", dialogWidth: 800),
                    this.MakeAction("FrameworkGroup","DataFunction",Localizer["_Admin.DataPrivilege"],Localizer["_Admin.DataPrivilege"], GridActionParameterTypesEnum.SingleId,"_Admin",800,null,null,x=>x.GroupCode).SetShowInRow(),
                    this.MakeStandardAction("FrameworkGroup", GridActionStandardTypesEnum.ExportExcel, "","_Admin"),
                };
            }
        }

        protected override IEnumerable<IGridColumn<FrameworkGroup>> InitGridHeader()
        {
            return new List<GridColumn<FrameworkGroup>>{
                this.MakeGridHeader(x => x.GroupCode, 120),
                this.MakeGridHeader(x => x.GroupName, 120),
                this.MakeGridHeader(x => x.GroupRemark),
                this.MakeGridHeaderAction(width: 300)
            };
        }

        public override IOrderedQueryable<FrameworkGroup> GetSearchQuery()
        {
            if (ConfigInfo.HasMainHost)
            {
                var rv = Wtm.CallAPI<ApiListModel<FrameworkGroup>>("mainhost", "/api/_frameworkgroup/search", HttpMethodEnum.POST, this.Searcher).Result;
                if (rv?.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    EntityList = rv.Data.Data;
                    Searcher.PageCount = rv.Data.PageCount;
                    Searcher.Count = rv.Data.Count;
                }
                return null;
            }
            else
            {
                var query = DC.Set<FrameworkGroup>()
                    .CheckContain(Searcher.GroupCode, x => x.GroupCode)
                    .CheckContain(Searcher.GroupName, x => x.GroupName)
                    .OrderBy(x => x.GroupCode);
                return query;
            }
        }
    }
}
