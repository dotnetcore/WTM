using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.MyUserVMs
{
    public partial class MyUserListVM : BasePagedListVM<MyUser_View, MyUserSearcher>
    {
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
                this.MakeStandardAction("MyUser", GridActionStandardTypesEnum.ExportExcel, "导出",""),
            };
        }

        protected override IEnumerable<IGridColumn<MyUser_View>> InitGridHeader()
        {
            return new List<GridColumn<MyUser_View>>{
                this.MakeGridHeader(x => x.Extra1),
                this.MakeGridHeader(x => x.Extra2),
                this.MakeGridHeader(x => x.ITCode),
                this.MakeGridHeader(x => x.Password),
                this.MakeGridHeader(x => x.Email),
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.Sex),
                this.MakeGridHeader(x => x.CellPhone),
                this.MakeGridHeader(x => x.HomePhone),
                this.MakeGridHeader(x => x.Address),
                this.MakeGridHeader(x => x.ZipCode),
                this.MakeGridHeader(x => x.PhotoId).SetFormat(PhotoIdFormat),
                this.MakeGridHeader(x => x.IsValid),
                this.MakeGridHeader(x => x.RoleName_view),
                this.MakeGridHeader(x => x.GroupName_view),
                this.MakeGridHeader(x => "你111").SetFormat((a,b)=>"asdf"),
                this.MakeGridHeader(x=>"CanEdit").SetHide().SetFormat((e,v)=>{
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
                .CheckContain(Searcher.Extra2, x=>x.Extra2)
                .CheckContain(Searcher.Email, x=>x.Email)
                .CheckContain(Searcher.CellPhone, x=>x.CellPhone)
                .CheckEqual(Searcher.IsValid, x=>x.IsValid)
                .CheckWhere(Searcher.SelectedUserRolesIDs,x=>DC.Set<FrameworkUserRole>().Where(y=>Searcher.SelectedUserRolesIDs.Contains(y.RoleId)).Select(z=>z.UserId).Contains(x.ID))
                .CheckWhere(Searcher.SelectedUserGroupsIDs,x=>DC.Set<FrameworkUserGroup>().Where(y=>Searcher.SelectedUserGroupsIDs.Contains(y.GroupId)).Select(z=>z.UserId).Contains(x.ID))
                .Select(x => new MyUser_View
                {
				    ID = x.ID,
                    Extra1 = x.Extra1,
                    Extra2 = x.Extra2,
                    ITCode = x.ITCode,
                    Password = x.Password,
                    Email = x.Email,
                    Name = x.Name,
                    Sex = x.Sex,
                    CellPhone = x.CellPhone,
                    HomePhone = x.HomePhone,
                    Address = x.Address,
                    ZipCode = x.ZipCode,
                    PhotoId = x.PhotoId,
                    IsValid = x.IsValid,
                    RoleName_view = x.UserRoles.Select(y=>y.Role.RoleName).ToSpratedString(null,","), 
                    GroupName_view = x.UserGroups.Select(y=>y.Group.GroupName).ToSpratedString(null,","), 
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class MyUser_View : MyUser{
        [Display(Name = "RoleName")]
        public String RoleName_view { get; set; }
        [Display(Name = "GroupName")]
        public String GroupName_view { get; set; }

    }
}
