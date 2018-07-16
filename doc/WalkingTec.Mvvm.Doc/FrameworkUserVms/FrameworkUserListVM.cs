using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Doc.FrameworkUserVms
{
    public class FrameworkUserListVM : BasePagedListVM<FrameworkUser_View, FrameworkUserSearcher>
    {

        public List<FrameworkRole> AllRoles { get; set; }
        public List<FrameworkGroup> AllGroups { get; set; }

        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("FrameworkUser", GridActionStandardTypesEnum.Create, "新建", "_Admin",dialogWidth: 800),
                this.MakeStandardAction("FrameworkUser", GridActionStandardTypesEnum.Edit, "修改", "_Admin",dialogWidth: 800),
                this.MakeAction("FrameworkUser","Password","修改密码","修改密码", GridActionParameterTypesEnum.SingleId,"_Admin",400).SetShowInRow(true),
                this.MakeStandardAction("FrameworkUser", GridActionStandardTypesEnum.Delete, "删除","_Admin",dialogWidth: 800),
                this.MakeStandardAction("FrameworkUser", GridActionStandardTypesEnum.Details, "详细", "_Admin",dialogWidth: 600),
                this.MakeStandardAction("FrameworkUser", GridActionStandardTypesEnum.BatchDelete, "批量删除","_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkUser", GridActionStandardTypesEnum.Import, "导入","_Admin", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<FrameworkUser_View>> InitGridHeader()
        {
            return new List<GridColumn<FrameworkUser_View>>{
                this.MakeGridHeader(x => x.ITCode),
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.IsValid).SetHeader("启用").SetWidth(80),
                this.MakeGridHeader(x => x.CellPhone),
                this.MakeGridHeader(x => x.HomePhone),
                this.MakeGridHeader(x => x.Roles).SetFormat(RoleFormat),
                this.MakeGridHeader(x => x.Groups).SetFormat(GroupFormat),
                this.MakeGridHeader(x=> x.PhotoId).SetFormat(PhotoIdFormat),                
                this.MakeGridHeaderAction(width: 300)
            };
        }

        private List<ColumnFormatInfo> PhotoIdFormat(FrameworkUser_View entity, object val)
        {
            return new List<ColumnFormatInfo>
            {
                ColumnFormatInfo.MakeDownloadButton(ButtonTypesEnum.Button,entity.PhotoId),
                ColumnFormatInfo.MakeViewButton(ButtonTypesEnum.Button,entity.PhotoId,640,480),
            };
        }

        private string RoleFormat(FrameworkUser_View entity, object val)
        {
            if (AllRoles == null)
            {
                return "";
            }
            var roleids = entity.UserRoles.Select(x => x.RoleId).ToList();
            return AllRoles.Where(x => roleids.Contains(x.ID)).Select(x => x.RoleName).ToList().ToSpratedString();
        }

        private string GroupFormat(FrameworkUser_View entity, object val)
        {
            if (AllGroups == null)
            {
                return "";
            }
            var groupids = entity.UserGroups.Select(x => x.GroupId).ToList();
            return AllGroups.Where(x => groupids.Contains(x.ID)).Select(x => x.GroupName).ToList().ToSpratedString();
        }

        public override IOrderedQueryable<FrameworkUser_View> GetSearchQuery()
        {
            AllRoles = DC.Set<FrameworkRole>().ToList();
            AllGroups = DC.Set<FrameworkGroup>().ToList();
            var query = DC.Set<FrameworkUserBase>().Include(x => x.UserRoles).Include(x=>x.UserGroups)
                .CheckContain(Searcher.ITCode,x=>x.ITCode)
                .CheckContain(Searcher.Name, x=>x.Name)
                .CheckEqual(Searcher.IsValid, x=>x.IsValid)
                .Select(x => new FrameworkUser_View
                {
                    ID = x.ID,
                    ITCode = x.ITCode,
                    Name = x.Name,
                    PhotoId = x.PhotoId,
                    CellPhone = x.CellPhone,
                    HomePhone = x.HomePhone,
                    IsValid = x.IsValid,
                    UserRoles = x.UserRoles,
                    UserGroups = x.UserGroups
                })
                .OrderBy(x => x.ITCode);
            return query;
        }

    }

    public class FrameworkUser_View : FrameworkUserBase
    {
        [Display(Name = "角色")]
        public string Roles { get; set; }

        [Display(Name = "用户组")]
        public string Groups { get; set; }
    }
}
