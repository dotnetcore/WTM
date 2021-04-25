using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.SchoolVMs
{
    public class SchoolListVM2 : BasePagedListVM<School, SchoolSearcher>
    {
        public SchoolListVM2()
        {
            DetailGridPrix = "EntityList";
        }

        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("School", GridActionStandardTypesEnum.AddRow, "新建","", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.RemoveRow, "删除","", dialogWidth: 800),
            };
        }

        protected override IEnumerable<IGridColumn<School>> InitGridHeader()
        {
            return new List<GridColumn<School>>{
                this.MakeGridHeader(x => x.SchoolCode).SetEditType(EditTypeEnum.TextBox),
                this.MakeGridHeader(x => x.SchoolName).SetEditType(EditTypeEnum.TextBox),
                this.MakeGridHeader(x => x.SchoolType).SetEditType(EditTypeEnum.ComboBox,typeof(SchoolTypeEnum).ToListItems(null,true)),
                this.MakeGridHeader(x => "test").SetFormat((a,b)=>{
                    return this.UIService.MakeScriptButton(ButtonTypesEnum.Button,"测试","alert('aaa');");
                }).SetHeader("测试"),
                this.MakeGridHeader(x => x.Remark).SetEditType(EditTypeEnum.TextBox),
                this.MakeGridHeaderAction(width: 500)
            };
        }

        public override IOrderedQueryable<School> GetSearchQuery()
        {
            var query = DC.Set<School>()
                .CheckContain(Searcher.SchoolCode, x => x.SchoolCode)
                .CheckContain(Searcher.SchoolName, x => x.SchoolName)
                .CheckEqual(Searcher.SchoolType, x => x.SchoolType)
                .Select(x => new School
                {
                    ID = x.ID,
                    SchoolCode = x.SchoolCode,
                    SchoolName = x.SchoolName,
                    SchoolType = x.SchoolType,
                    Remark = x.Remark,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }
}
