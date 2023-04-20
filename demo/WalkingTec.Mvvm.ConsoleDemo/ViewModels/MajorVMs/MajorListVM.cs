using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.MajorVMs
{
    public class MajorListVM : BasePagedListVM<Major_View, MajorSearcher>
    {
        protected override Task<List<GridAction>> InitGridAction()
        {
            return Task.FromResult (new List<GridAction>
            {
                this.MakeStandardAction("Major", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("Major", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800),
                this.MakeStandardAction("Major", GridActionStandardTypesEnum.Delete, "删除", "",dialogWidth: 800),
                this.MakeStandardAction("Major", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeStandardAction("Major", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                this.MakeStandardAction("Major", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                this.MakeStandardAction("Major", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardAction("Major", GridActionStandardTypesEnum.ExportExcel, "导出","")
            });
        }

        protected override Task<IEnumerable<IGridColumn<Major_View>>> InitGridHeader()
        {
            return Task.FromResult<IEnumerable<IGridColumn<Major_View>>> (new List<GridColumn<Major_View>>{
                this.MakeGridHeader(x => x.MajorCode).SetSort(),
                this.MakeGridHeader(x => x.MajorName),
                this.MakeGridHeader(x => x.MajorType),
                this.MakeGridHeader(x => x.Remark),
                this.MakeGridHeader(x => x.SchoolName_view),
                this.MakeGridHeaderAction(width: 200)
            });
        }

        public override async Task<IOrderedQueryable<Major_View>> GetSearchQuery()
        {
            var query = (await (await DC.Set<Major>()
                .CheckContain(Searcher.MajorCode, x => x.MajorCode)
                .CheckContain(Searcher.MajorName, x => x.MajorName)
                .CheckEqual(Searcher.SchoolId, x => x.SchoolId)
                .DPWhere(Wtm, x => x.SchoolId))
                .DPWhere(Wtm, x=>x.ID))
                .Select(x => new Major_View
                {
                    ID = x.ID,
                    MajorCode = x.MajorCode,
                    MajorName = x.MajorName,
                    MajorType = x.MajorType,
                    Remark = x.Remark,
                    SchoolName_view = x.School.SchoolName,
                })
                .OrderBy(x => x.MajorCode);
            return query;
        }

    }

    public class Major_View : Major
    {
        [Display(Name = "学校名称")]
        public String SchoolName_view { get; set; }

    }
}
