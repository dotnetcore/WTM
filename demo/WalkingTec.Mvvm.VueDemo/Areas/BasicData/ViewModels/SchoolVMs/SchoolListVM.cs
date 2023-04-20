using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.VueDemo.BasicData.ViewModels.SchoolVMs
{
    public partial class SchoolListVM : BasePagedListVM<School_View, SchoolSearcher>
    {

        protected override Task<IEnumerable<IGridColumn<School_View>>> InitGridHeader()
        {
            return Task.FromResult<IEnumerable<IGridColumn<School_View>>> (new List<GridColumn<School_View>>{
                this.MakeGridHeader(x => x.SchoolCode),
                this.MakeGridHeader(x => x.SchoolName),
                this.MakeGridHeader(x => x.FileId).SetFormat(FileIdFormat),
                this.MakeGridHeader(x => x.SchoolType),
                this.MakeGridHeader(x => x.Remark),
                this.MakeGridHeaderAction(width: 200)
            });
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
                .Select(x => new School_View
                {
				    ID = x.ID,
                    SchoolCode = x.SchoolCode,
                    SchoolName = x.SchoolName,
                    FileId = x.FileId,
                    SchoolType = x.SchoolType,
                    Remark = x.Remark,
                })
                .OrderBy(x => x.ID);
            return Task.FromResult (query);
        }

    }

    public class School_View : School{

    }
}
