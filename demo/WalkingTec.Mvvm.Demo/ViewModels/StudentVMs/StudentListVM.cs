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
    public class StudentListVM : BasePagedListVM<Student_View, StudentSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.Delete, "删除", "",dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                this.MakeStandardAction("Student", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<Student_View>> InitGridHeader()
        {
            return new List<GridColumn<Student_View>>{
                this.MakeGridHeader(x => x.LoginName),
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
                .CheckContain(Searcher.LoginName, x=>x.LoginName)
                .CheckContain(Searcher.Name, x=>x.Name)
                .CheckEqual(Searcher.IsValid, x=>x.IsValid)
                .Select(x => new Student_View
                {
				    ID = x.ID,
                    LoginName = x.LoginName,
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
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class Student_View : Student{

    }
}
