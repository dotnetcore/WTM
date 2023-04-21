// WTM默认页面 Wtm buidin page
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs
{
    public class FrameworkRoleListVM : BasePagedListVM<FrameworkRole, FrameworkRoleSearcher>
    {
        protected override async Task<List<GridAction>> InitGridAction()
        {
            if (ConfigInfo.HasMainHost && (await Wtm.GetLoginUserInfo ())?.CurrentTenant == null)
            {
                return new List<GridAction>
                {
                    this.MakeAction("FrameworkRole","PageFunction",Localizer["_Admin.PageFunction"],Localizer["_Admin.PageFunction"], GridActionParameterTypesEnum.SingleId,"_Admin",800).SetShowInRow(),
                };
            }
            else
            {
                return new List<GridAction>
                {
                    this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.Create, "","_Admin", dialogWidth: 800),
                    this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.Edit, "","_Admin", dialogWidth: 800),
                    this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.Delete, "", "_Admin",dialogWidth: 800),
                    this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.Details, "","_Admin", dialogWidth: 800),
                    this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.BatchDelete, "","_Admin", dialogWidth: 800),
                    this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.Import, "","_Admin", dialogWidth: 800),
                    this.MakeAction("FrameworkRole","PageFunction",Localizer["_Admin.PageFunction"],Localizer["_Admin.PageFunction"], GridActionParameterTypesEnum.SingleId,"_Admin",800).SetShowInRow(),
                    this.MakeStandardAction("FrameworkRole", GridActionStandardTypesEnum.ExportExcel, "","_Admin"),
                };
            }
        }

        protected override Task<IEnumerable<IGridColumn<FrameworkRole>>> InitGridHeader()
        {
            return Task.FromResult<IEnumerable<IGridColumn<FrameworkRole>>> (new List<GridColumn<FrameworkRole>>{
                this.MakeGridHeader(x => x.RoleCode, 120),
                this.MakeGridHeader(x => x.RoleName, 120),
                this.MakeGridHeader(x => x.RoleRemark),
                this.MakeGridHeaderAction(width: 300)
            });
        }

        public override Task<IOrderedQueryable<FrameworkRole>> GetSearchQuery()
        {
            var query = DC.Set<FrameworkRole>()
                .CheckContain(Searcher.RoleCode, x => x.RoleCode)
                .CheckContain(Searcher.RoleName, x => x.RoleName)
                .OrderBy(x => x.RoleCode);
            return Task.FromResult (query);
        }

    }
}
