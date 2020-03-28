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
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Create, "新建","BasicData", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Edit, "修改","BasicData", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Delete, "删除", "BasicData",dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Details, "详细","BasicData", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.BatchEdit, "批量修改","BasicData", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.BatchDelete, "批量删除","BasicData", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Import, "导入","BasicData", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.ExportExcel, "导出","BasicData"),
            };
        }

        protected override IEnumerable<IGridColumn<School_View>> InitGridHeader()
        {
            return new List<GridColumn<School_View>>{
                this.MakeGridHeader(x => x.SchoolCode),
                this.MakeGridHeader(x => x.SchoolName),
                this.MakeGridHeader(x => x.FileId).SetFormat(FileIdFormat),
                this.MakeGridHeader(x => x.SchoolType),
                this.MakeGridHeader(x => x.Remark),
                this.MakeGridHeaderAction(width: 200)
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


        public override IOrderedQueryable<School_View> GetSearchQuery()
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
                    FileId = x.FileId,
                    SchoolType = x.SchoolType,
                    Remark = x.Remark,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class School_View : School{

    }
}
