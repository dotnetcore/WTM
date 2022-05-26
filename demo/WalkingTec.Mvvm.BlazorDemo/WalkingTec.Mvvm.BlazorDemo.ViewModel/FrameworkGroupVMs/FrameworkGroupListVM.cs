// WTM默认页面 Wtm buidin page
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkGroupVMs
{
    public class FrameworkGroupListVM : BasePagedListVM<FrameworkGroup_View, FrameworkGroupSearcher>
    {
        public FrameworkGroupListVM()
        {
            NeedPage = false;
        }

        protected override List<GridAction> InitGridAction()
        {
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
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

        protected override IEnumerable<IGridColumn<FrameworkGroup_View>> InitGridHeader()
        {
            return new List<GridColumn<FrameworkGroup_View>>{
                this.MakeGridHeader(x => x.GroupName, 220),
                this.MakeGridHeader(x => x.GroupCode, 120),
                this.MakeGridHeader(x => x.ManagerName,220).SetFormat((a,b)=>{
                    string rv = "";
                    if(string.IsNullOrEmpty(a.ManagerName) == false)
                    {
                        rv += a.ManagerName;
                    }
                    if(string.IsNullOrEmpty(a.Manager) == false)
                    {
                        rv += $"({a.Manager})";
                    }
                    return rv;
                }),
              this.MakeGridHeader(x => x.GroupRemark),
                 this.MakeGridHeader(x => x.ParentId).SetHide(),
             this.MakeGridHeaderAction(width: 300)
            };
        }

        public override IOrderedQueryable<FrameworkGroup_View> GetSearchQuery()
        {
            return  DC.Set<FrameworkGroup>()
                .CheckContain(Searcher.GroupCode, x => x.GroupCode)
                .CheckContain(Searcher.GroupName, x => x.GroupName)
                 .GroupJoin(DC.Set<FrameworkUser>(), ok => ok.Manager, ik => ik.ITCode, (group, user) => new { user = user, group = group })
                 .SelectMany(x => x.user.DefaultIfEmpty(), (a, b) => new FrameworkGroup_View
                 {
                     ID = a.group.ID,
                     ParentId = a.group.ParentId,
                     GroupCode = a.group.GroupCode,
                     GroupName = a.group.GroupName,
                     Manager = a.group.Manager,
                     ManagerName = b.Name,
                 })
                .OrderBy(x => x.GroupCode);
        }
        public override void AfterDoSearcher()
        {
            var topdata = EntityList.MakeTree(x => x.GroupCode).FlatTree(x => x.GroupCode);
            if (ControllerName.Contains("/api") == false)
            {
                topdata.ForEach((x) => { int l = x.GetLevel(); for (int i = 0; i < l; i++) { x.GroupName = "&nbsp;&nbsp;&nbsp;&nbsp;" + x.GroupName; } });
            }
            EntityList = topdata;

        }
    }
    public class FrameworkGroup_View : TreePoco<FrameworkGroup_View>
    {
        [Display(Name = "_Admin.GroupCode")]
        public string GroupCode { get; set; }

        [Display(Name = "_Admin.GroupName")]
        public string GroupName { get; set; }

        [Display(Name = "_Admin.Remark")]
        public string GroupRemark { get; set; }

        [Display(Name = "_Admin.GroupManager")]
        public string Manager { get; set; }

        [Display(Name = "_Admin.Tenant")]
        public string TenantCode { get; set; }

        [Display(Name = "_Admin.GroupManager")]
        public string ManagerName { get; set; }

        public bool HasChild { get => HasChildren; }

    }

}
