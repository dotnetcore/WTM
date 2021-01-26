// WTM默认页面 Wtm buidin page
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms
{
    public class FrameworkUserListVM : BasePagedListVM<FrameworkUser_View, FrameworkUserSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("FrameworkUser", GridActionStandardTypesEnum.Create, "", "_Admin",dialogWidth: 800),
                this.MakeStandardAction("FrameworkUser", GridActionStandardTypesEnum.Edit, "", "_Admin",dialogWidth: 800),
                this.MakeAction("FrameworkUser","Password",Localizer?["Login.ChangePassword"],Localizer?["Login.ChangePassword"], GridActionParameterTypesEnum.SingleId,"_Admin",400).SetShowInRow(true),
                this.MakeStandardAction("FrameworkUser", GridActionStandardTypesEnum.Delete, "","_Admin",dialogWidth: 800),
                this.MakeStandardAction("FrameworkUser", GridActionStandardTypesEnum.Details, "", "_Admin",dialogWidth: 600),
                 this.MakeStandardAction("FrameworkUser", GridActionStandardTypesEnum.BatchEdit, "","_Admin", dialogWidth: 800),
               this.MakeStandardAction("FrameworkUser", GridActionStandardTypesEnum.BatchDelete, "","_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkUser", GridActionStandardTypesEnum.Import, "","_Admin", dialogWidth: 800),
                this.MakeStandardAction("FrameworkUser", GridActionStandardTypesEnum.ExportExcel, "","_Admin"),
            };
        }

        protected override IEnumerable<IGridColumn<FrameworkUser_View>> InitGridHeader()
        {
            return new List<GridColumn<FrameworkUser_View>>{
                this.MakeGridHeader(x => x.ITCode),
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.Gender,80),
                this.MakeGridHeader(x => x.CellPhone,120),
                this.MakeGridHeader(x => x.RoleName_view),
                this.MakeGridHeader(x => x.GroupName_view),
                this.MakeGridHeader(x=> x.PhotoId,170).SetFormat(PhotoIdFormat),
                this.MakeGridHeader(x => x.IsValid).SetHeader(Localizer["Sys.Enable"]).SetWidth(80),
                this.MakeGridHeaderAction(width: 280)
            };
        }

        private List<ColumnFormatInfo> PhotoIdFormat(FrameworkUser_View entity, object val)
        {
            return new List<ColumnFormatInfo>
            {
                ColumnFormatInfo.MakeDownloadButton(ButtonTypesEnum.Button,entity.PhotoId),
                ColumnFormatInfo.MakeViewButton(ButtonTypesEnum.Button,entity.PhotoId),
            };
        }

        public override IOrderedQueryable<FrameworkUser_View> GetSearchQuery()
        {
            var query = DC.Set<FrameworkUser>()
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
                    IsValid = x.IsValid,
                    RoleName_view = DC.Set<FrameworkUserRole>().Where(y => y.UserCode == x.ITCode)
                        .Join(DC.Set<FrameworkRole>(), ur => ur.RoleCode, role => role.RoleCode, (ur, role) => role.RoleName).ToSepratedString(null, ","),
                    GroupName_view = DC.Set<FrameworkUserGroup>().Where(y => y.UserCode == x.ITCode)
                        .Join(DC.Set<FrameworkGroup>(), ug => ug.GroupCode, group => group.GroupCode, (ug, group) => group.GroupName).ToSepratedString(null, ","),
                   Gender = x.Gender
                })
                .OrderBy(x => x.ITCode);
            return query;
        }

    }

    public class FrameworkUser_View : FrameworkUser
    {
        [Display(Name = "_Admin.Role")]
        public string RoleName_view { get; set; }

        [Display(Name = "_Admin.Group")]
        public string GroupName_view { get; set; }
    }
}
