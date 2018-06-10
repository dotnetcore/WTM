using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Doc.Models;

namespace WalkingTec.Mvvm.Doc.ViewModels.StudentVms
{
    public class StudentListVm : BasePagedListVM<Student,StudentSearcher>
    {

        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {

            };
        }

        protected override IEnumerable<IGridColumn<Student>> InitGridHeader()
        {
            return new List<GridColumn<Student>>{
                this.MakeGridHeader(x => x.LoginName),
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.Sex),
                this.MakeGridHeader(x => x.CellPhone),
                this.MakeGridHeader(x=>x.IsValid)
            };
        }

        public override IOrderedQueryable<Student> GetSearchQuery()
        {
            List<Student> data = new List<Student>
            {
                new Student{ LoginName = "zhangsan", Name="张三", Sex= Models.SexEnum.Male, CellPhone="13012213483", ExcelIndex = 0, IsValid = true},
                new Student{ LoginName = "lisi", Name="李四", Sex= Models.SexEnum.Male, CellPhone="13075829654", ExcelIndex = 1, IsValid = false},
                new Student{ LoginName = "wangwu", Name="王五", Sex= Models.SexEnum.Male, CellPhone="13098635100", ExcelIndex = 2, IsValid = true},
                new Student{ LoginName = "zhaoliu", Name="赵六", Sex= Models.SexEnum.Female, CellPhone="13035698123", ExcelIndex = 3, IsValid = false},
            };

            var query = data.AsQueryable()
                .OrderBy(x => x.ExcelIndex);
            return query;
        }

    }

}
