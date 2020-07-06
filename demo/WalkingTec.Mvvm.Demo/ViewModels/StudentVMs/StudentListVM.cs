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
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.Create, Localizer["Create"],"", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.Edit, Localizer["Edit"],"", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.Delete, Localizer["Delete"], "",dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.Details, Localizer["Details"],"", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.BatchEdit, Localizer["BatchEdit"],"", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.BatchDelete, Localizer["BatchDelete"],"", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.Import, Localizer["Import"],"", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.ExportExcel, Localizer["Export"],""),
            };
        }

        protected override IEnumerable<IGridColumn<Student_View>> InitGridHeader()
        {
            return new List<GridColumn<Student_View>>{
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
            };
        }
        private List<ColumnFormatInfo> PhotoIdFormat(Student_View entity, object val)
        {
            return new List<ColumnFormatInfo>
            {
                ColumnFormatInfo.MakeDownloadButton(ButtonTypesEnum.Button,entity.PhotoId),
                ColumnFormatInfo.MakeViewButton(ButtonTypesEnum.Button,entity.PhotoId,640,480),
            };
        }


        public override IOrderedQueryable<Student_View> GetSearchQuery()
        {
            var query = DC.Set<Student>()
                .CheckContain(Searcher.Name, x=>x.Name)
                .CheckEqual(Searcher.Sex, x=>x.Sex)
                .CheckContain(Searcher.ZipCode, x=>x.ZipCode)
                .CheckEqual(Searcher.IsValid, x=>x.IsValid)
                .CheckBetween(Searcher.EnRollDate?.GetStartTime(), Searcher.EnRollDate?.GetEndTime(), x => x.EnRollDate, includeMax: false)
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
                    MajorName_view = x.StudentMajor.Select(y=>y.Major.MajorName).ToSpratedString(null,","), 
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class Student_View : Student{
        [Display(Name = "专业名称")]
        public String MajorName_view { get; set; }

    }
}
