using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.StudentVMs
{
    public partial class StudentListVM : BasePagedListVM<Student_View, StudentSearcher>
    {
        protected override Task<List<GridAction>> InitGridAction()
        {
            return Task.FromResult (new List<GridAction>
            {
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.Create, Localizer["Sys.Create"],"", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.Edit, Localizer["Sys.Edit"], "", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.Delete, Localizer["Sys.Delete"], "", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.Details, Localizer["Sys.Details"], "", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.BatchEdit, Localizer["Sys.BatchEdit"], "", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.BatchDelete, Localizer["Sys.BatchDelete"], "", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.Import, Localizer["Sys.Import"], "", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.ExportExcel, Localizer["Sys.Export"], ""),
            });
        }


        protected override Task<IEnumerable<IGridColumn<Student_View>>> InitGridHeader()
        {
            return Task.FromResult<IEnumerable<IGridColumn<Student_View>>> (new List<GridColumn<Student_View>>{
                this.MakeGridHeader(x => x.ID),
                this.MakeGridHeader(x => x.Password),
                this.MakeGridHeader(x => x.Email),
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.Sex),
                this.MakeGridHeader(x => x.CellPhone),
                this.MakeGridHeader(x => x.Address),
                this.MakeGridHeader(x => x.ZipCode),
                this.MakeGridHeader(x => x.PhotoId).SetFormat(PhotoIdFormat),
                this.MakeGridHeader(x => x.IsValid),
                this.MakeGridHeader(x => x.EnRollDate),
                this.MakeGridHeader(x => x.MajorName_view),
                this.MakeGridHeaderAction(width: 200)
            });
        }
        private List<ColumnFormatInfo> PhotoIdFormat(Student_View entity, object val)
        {
            return new List<ColumnFormatInfo>
            {
                ColumnFormatInfo.MakeDownloadButton(ButtonTypesEnum.Button,entity.PhotoId),
                ColumnFormatInfo.MakeViewButton(ButtonTypesEnum.Button,entity.PhotoId,640,480),
            };
        }


        public override Task<IOrderedQueryable<Student_View>> GetSearchQuery()
        {
            var query = DC.Set<Student>()
                .CheckContain(Searcher.Password, x=>x.Password)
                .CheckEqual(Searcher.Sex, x=>x.Sex)
                .CheckContain(Searcher.ZipCode, x=>x.ZipCode)
                .CheckWhere(Searcher.SelectedStudentMajorIDs,x=>DC.Set<StudentMajor>().Where(y=>Searcher.SelectedStudentMajorIDs.Contains(y.MajorId)).Select(z=>z.StudentId).Contains(x.ID))
                .Select(x => new Student_View
                {
				    ID = x.ID,
                    Password = x.Password,
                    Email = x.Email,
                    Name = x.Name,
                    Sex = x.Sex,
                    CellPhone = x.CellPhone,
                    Address = x.Address,
                    ZipCode = x.ZipCode,
                    PhotoId = x.PhotoId,
                    IsValid = x.IsValid,
                    EnRollDate = x.EnRollDate,
                    MajorName_view = x.StudentMajor.Select(y=>y.Major.MajorName).ToSepratedString(null,","), 
                })
                .OrderBy(x => x.ID);
            return Task.FromResult (query);
        }

    }

    public class Student_View : Student{
        [Display(Name = "专业名称")]
        public String MajorName_view { get; set; }

    }
}
