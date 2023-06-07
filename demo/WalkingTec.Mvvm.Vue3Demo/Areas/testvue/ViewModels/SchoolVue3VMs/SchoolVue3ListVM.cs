using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.ReactDemo.Models;


namespace WalkingTec.Mvvm.Vue3Demo.testvue.ViewModels.SchoolVue3VMs
{
    public partial class SchoolVue3ListVM : BasePagedListVM<SchoolVue3_View, SchoolVue3Searcher>
    {

        protected override IEnumerable<IGridColumn<SchoolVue3_View>> InitGridHeader()
        {
            return new List<GridColumn<SchoolVue3_View>>{
                this.MakeGridHeader(x => x.SchoolCode),
                this.MakeGridHeader(x => x.SchoolName),
                this.MakeGridHeader(x => x.SchoolType),
                this.MakeGridHeader(x => x.Remark),
                this.MakeGridHeader(x => x.Level),
                this.MakeGridHeader(x => x.Name_view),
                this.MakeGridHeader(x => x.IsSchool),
                this.MakeGridHeader(x => x.PhotoId).SetFormat(PhotoIdFormat),
                this.MakeGridHeader(x => x.FileId).SetFormat(FileIdFormat),
                this.MakeGridHeaderAction(width: 200)
            };
        }
        private List<ColumnFormatInfo> PhotoIdFormat(SchoolVue3_View entity, object val)
        {
            return new List<ColumnFormatInfo>
            {
                ColumnFormatInfo.MakeDownloadButton(ButtonTypesEnum.Button,entity.PhotoId),
                ColumnFormatInfo.MakeViewButton(ButtonTypesEnum.Button,entity.PhotoId,640,480),
            };
        }

        private List<ColumnFormatInfo> FileIdFormat(SchoolVue3_View entity, object val)
        {
            return new List<ColumnFormatInfo>
            {
                ColumnFormatInfo.MakeDownloadButton(ButtonTypesEnum.Button,entity.FileId),
                ColumnFormatInfo.MakeViewButton(ButtonTypesEnum.Button,entity.FileId,640,480),
            };
        }


        public override IOrderedQueryable<SchoolVue3_View> GetSearchQuery()
        {
            var query = DC.Set<SchoolVue3>()
                .CheckContain(Searcher.SchoolCode, x=>x.SchoolCode)
                .CheckContain(Searcher.SchoolName, x=>x.SchoolName)
                .CheckEqual(Searcher.Level, x=>x.Level)
                .Select(x => new SchoolVue3_View
                {
				    ID = x.ID,
                    SchoolCode = x.SchoolCode,
                    SchoolName = x.SchoolName,
                    SchoolType = x.SchoolType,
                    Remark = x.Remark,
                    Level = x.Level,
                    Name_view = x.Place.Name,
                    IsSchool = x.IsSchool,
                    PhotoId = x.PhotoId,
                    FileId = x.FileId,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class SchoolVue3_View : SchoolVue3{
        [Display(Name = "名称")]
        public String Name_view { get; set; }

    }
}
