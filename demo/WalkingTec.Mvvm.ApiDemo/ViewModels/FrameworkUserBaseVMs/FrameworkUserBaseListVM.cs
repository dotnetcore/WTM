using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;


namespace WalkingTec.Mvvm.ApiDemo.ViewModels.FrameworkUserBaseVMs
{
    public class FrameworkUserBaseListVM : BasePagedListVM<FrameworkUserBase_View, FrameworkUserBaseSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("FrameworkUserBase", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("FrameworkUserBase", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800),
                this.MakeStandardAction("FrameworkUserBase", GridActionStandardTypesEnum.Delete, "删除", "",dialogWidth: 800),
                this.MakeStandardAction("FrameworkUserBase", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeStandardAction("FrameworkUserBase", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                this.MakeStandardAction("FrameworkUserBase", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                this.MakeStandardAction("FrameworkUserBase", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<FrameworkUserBase_View>> InitGridHeader()
        {
            return new List<GridColumn<FrameworkUserBase_View>>{
                this.MakeGridHeader(x => x.ITCode),
                this.MakeGridHeader(x => x.Password),
                this.MakeGridHeader(x => x.Email),
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.Sex),
                this.MakeGridHeader(x => x.CellPhone),
                this.MakeGridHeader(x => x.PhotoId).SetFormat(PhotoIdFormat),
                this.MakeGridHeader(x => x.IsValid),
                this.MakeGridHeader(x => x.RoleName_view),
                this.MakeGridHeader(x => x.GroupName_view),
                this.MakeGridHeaderAction(width: 200)
            };
        }
        private List<ColumnFormatInfo> PhotoIdFormat(FrameworkUserBase_View entity, object val)
        {
            return new List<ColumnFormatInfo>
            {
                ColumnFormatInfo.MakeDownloadButton(ButtonTypesEnum.Button,entity.PhotoId),
                ColumnFormatInfo.MakeViewButton(ButtonTypesEnum.Button,entity.PhotoId,640,480),
            };
        }

        public override IOrderedQueryable<FrameworkUserBase_View> GetSearchQuery()
        {
            var query = DC.Set<FrameworkUserBase>()
                .CheckContain(Searcher.ITCode, x=>x.ITCode)
                .CheckContain(Searcher.Name, x=>x.Name)
                .CheckEqual(Searcher.Sex, x=>x.Sex)
                .CheckEqual(Searcher.IsValid, x=>x.IsValid)
                .Select(x => new FrameworkUserBase_View
                {
				    ID = x.ID,
                    ITCode = x.ITCode,
                    Password = x.Password,
                    Email = x.Email,
                    Name = x.Name,
                    Sex = x.Sex,
                    CellPhone = x.CellPhone,
                    PhotoId = x.PhotoId,
                    IsValid = x.IsValid,
                    RoleName_view = DC.Set<FrameworkRole>().Where(y => x.UserRoles.Select(z => z.RoleId).Contains(y.ID)).Select(y => y.RoleName).ToSpratedString(null,","),
                    GroupName_view = DC.Set<FrameworkGroup>().Where(y => x.UserGroups.Select(z => z.GroupId).Contains(y.ID)).Select(y => y.GroupName).ToSpratedString(null,","),
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class FrameworkUserBase_View : FrameworkUserBase{
        [Display(Name = "角色名称")]
        public String RoleName_view { get; set; }
        [Display(Name = "用户组名称")]
        public String GroupName_view { get; set; }

    }
}
