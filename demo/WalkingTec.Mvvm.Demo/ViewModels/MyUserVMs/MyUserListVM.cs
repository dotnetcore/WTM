using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkRoleVMs;

namespace WalkingTec.Mvvm.Demo.ViewModels.MyUserVMs
{
    public class MyUserListVM : BasePagedListVM<MyUser_View, MyUserSearcher>
    {
        public FrameworkRoleListVM RoleList = new FrameworkRoleListVM();

        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("MyUser", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("MyUser", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800).SetBindVisiableColName("CanEdit"),
                this.MakeStandardAction("MyUser", GridActionStandardTypesEnum.Delete, "删除", "",dialogWidth: 800),
                this.MakeStandardAction("MyUser", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeStandardAction("MyUser", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                this.MakeStandardAction("MyUser", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                this.MakeStandardAction("MyUser", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<MyUser_View>> InitGridHeader()
        {
            return new List<GridColumn<MyUser_View>>{
                this.MakeGridHeader(x => x.Extra2),
                this.MakeGridHeader(x => x.ITCode),
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.Sex),
                this.MakeGridHeader(x => x.ZipCode),
                this.MakeGridHeader(x => x.PhotoId).SetFormat(PhotoIdFormat),
                this.MakeGridHeader(x => x.IsValid),
                this.MakeGridHeader(x => x.RoleName_view),
                this.MakeGridHeader(x => x.GroupName_view),
                this.MakeGridHeader(x=>x.CanEdit).SetHide().SetFormat((e,v)=>{
                    if (e.Sex == SexEnum.Male){
                        return "true";
                    }
                    else {
                        return "false";
                    }
                }),
                this.MakeGridHeaderAction(width: 200)
            };
        }
        private List<ColumnFormatInfo> PhotoIdFormat(MyUser_View entity, object val)
        {
            return new List<ColumnFormatInfo>
            {
                ColumnFormatInfo.MakeDownloadButton(ButtonTypesEnum.Button,entity.PhotoId),
                ColumnFormatInfo.MakeViewButton(ButtonTypesEnum.Button,entity.PhotoId,640,480),
            };
        }

        public override IOrderedQueryable<MyUser_View> GetSearchQuery()
        {
            var query = DC.Set<MyUser>()
                .CheckContain(Searcher.ITCode, x => x.ITCode)
                .CheckContain(Searcher.Name, x => x.Name)
                .CheckEqual(Searcher.Sex, x => x.Sex)
                .CheckEqual(Searcher.IsValid, x => x.IsValid)
                .CheckWhere(Searcher.SelectedUserRolesIDs, x => DC.Set<FrameworkUserRole>().Where(y => Searcher.SelectedUserRolesIDs.Contains(y.RoleId)).Select(z => z.UserId).Contains(x.ID))
                .Select(x => new MyUser_View
                {
                    ID = x.ID,
                    Extra2 = x.Extra2,
                    ITCode = x.ITCode,
                    Name = x.Name,
                    Sex = x.Sex,
                    ZipCode = x.ZipCode,
                    PhotoId = x.PhotoId,
                    IsValid = x.IsValid,
                    RoleName_view = DC.Set<FrameworkRole>().Where(y => x.UserRoles.Select(z => z.RoleId).Contains(y.ID)).Select(y => y.RoleName).ToSpratedString(null, ","),
                    GroupName_view = DC.Set<FrameworkGroup>().Where(y => x.UserGroups.Select(z => z.GroupId).Contains(y.ID)).Select(y => y.GroupName).ToSpratedString(null, ","),
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class MyUser_View : MyUser
    {
        [Display(Name = "角色名称")]
        public String RoleName_view { get; set; }
        [Display(Name = "用户组名称")]
        public String GroupName_view { get; set; }

        public string CanEdit { get; set; }

    }
}
