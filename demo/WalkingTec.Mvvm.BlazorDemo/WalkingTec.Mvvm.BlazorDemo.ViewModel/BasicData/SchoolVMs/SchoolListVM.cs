using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.BasicData.SchoolVMs
{
    public partial class SchoolListVM : BasePagedListVM<School_View, SchoolSearcher>
    {

        protected override Task<IEnumerable<IGridColumn<School_View>>> InitGridHeader()
        {
            return Task.FromResult<IEnumerable<IGridColumn<School_View>>> (new List<GridColumn<School_View>>{
                this.MakeGridHeader(x => x.SchoolCode),
                this.MakeGridHeader(x => x.SchoolName),
                this.MakeGridHeader(x => x.SchoolType),
                this.MakeGridHeader(x => x.Remark),
                this.MakeGridHeader(x => x.PhotoId).SetFormat(PhotoIdFormat),
                this.MakeGridHeader(x => x.FileId).SetFormat(FileIdFormat),
                this.MakeGridHeaderAction(width: 200)
            });
        }
        private List<ColumnFormatInfo> PhotoIdFormat(School_View entity, object val)
        {
            return new List<ColumnFormatInfo>
            {
                ColumnFormatInfo.MakeDownloadButton(ButtonTypesEnum.Button,entity.PhotoId),
                ColumnFormatInfo.MakeViewButton(ButtonTypesEnum.Button,entity.PhotoId,640,480),
            };
        }

        private List<ColumnFormatInfo> FileIdFormat(School_View entity, object val)
        {
            return new List<ColumnFormatInfo>
            {
                ColumnFormatInfo.MakeDownloadButton(ButtonTypesEnum.Button,entity.FileId),
                ColumnFormatInfo.MakeViewButton(ButtonTypesEnum.Button,entity.FileId,640,480),
            };
        }


        public override Task<IOrderedQueryable<School_View>> GetSearchQuery()
        {
            var query = DC.Set<School>()
                .CheckContain(Searcher.SchoolCode, x=>x.SchoolCode)
                .CheckContain(Searcher.SchoolName, x=>x.SchoolName)
                .CheckEqual(Searcher.SchoolType, x=>x.SchoolType)
                .CheckContain(Searcher.Remark, x=>x.Remark)
                .Select(x => new School_View
                {
				    ID = x.ID,
                    SchoolCode = x.SchoolCode,
                    SchoolName = x.SchoolName,
                    SchoolType = x.SchoolType,
                    Remark = x.Remark,
                    PhotoId = x.PhotoId,
                    FileId = x.FileId,
                })
                .OrderBy(x => x.ID);
            return Task.FromResult (query);
        }

    }

    public class School_View : School{

    }
}
