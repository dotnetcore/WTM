using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.MajorVMs
{
    public partial class MajorApiListVM : BasePagedListVM<MajorApi_View, MajorApiSearcher>
    {

        protected override Task<IEnumerable<IGridColumn<MajorApi_View>>> InitGridHeader()
        {
            return Task.FromResult<IEnumerable<IGridColumn<MajorApi_View>>> (new List<GridColumn<MajorApi_View>>{
                this.MakeGridHeader(x => x.MajorCode),
                this.MakeGridHeader(x => x.MajorName),
                this.MakeGridHeader(x => x.MajorType),
                this.MakeGridHeader(x => x.Remark),
                this.MakeGridHeader(x => x.SchoolName_view),
                this.MakeGridHeader(x => x.Name_view),
                this.MakeGridHeaderAction(width: 200)
            });
        }

        public override Task<IOrderedQueryable<MajorApi_View>> GetSearchQuery()
        {
            var query = DC.Set<Major>()
                .CheckEqual(Searcher.MajorType, x=>x.MajorType)
                .CheckContain(Searcher.Remark, x=>x.Remark)
                .CheckEqual(Searcher.SchoolId, x=>x.SchoolId)
                .CheckWhere(Searcher.SelectedStudentMajorsIDs,x=>DC.Set<StudentMajor>().Where(y=>Searcher.SelectedStudentMajorsIDs.Contains(y.StudentId)).Select(z=>z.MajorId).Contains(x.ID))
                .Select(x => new MajorApi_View
                {
				    ID = x.ID,
                    MajorCode = x.MajorCode,
                    MajorName = x.MajorName,
                    MajorType = x.MajorType,
                    Remark = x.Remark,
                    SchoolName_view = x.School.SchoolName,
                    Name_view = x.StudentMajors.Select(y=>y.Student.Name).ToSepratedString(null,","), 
                })
                .OrderBy(x => x.ID);
            return Task.FromResult (query);
        }

    }

    public class MajorApi_View : Major{
        [Display(Name = "学校名称")]
        public String SchoolName_view { get; set; }
        [Display(Name = "姓名")]
        public String Name_view { get; set; }

    }
}
