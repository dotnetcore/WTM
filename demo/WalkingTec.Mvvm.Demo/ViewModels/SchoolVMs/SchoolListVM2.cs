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
    public class SchoolListVM2 : BasePagedListVM<School_View, SchoolSearcher>
    {
        public SchoolListVM2()
        {
            DetailGridPrix = "EntityList";
        }

        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800).SetHideOnToolBar(false),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Delete, "删除","", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel),
            };
        }

        protected override IEnumerable<IGridColumn<School_View>> InitGridHeader()
        {
            return new List<GridColumn<School_View>>{
                this.MakeGridHeader(x => x.SchoolCode),
                this.MakeGridHeader(x => x.SchoolName).SetEditType(EditTypeEnum.TextBox),
                this.MakeGridHeader(x => x.SchoolType),
                this.MakeGridHeader(x => "test").SetFormat((a,b)=>{
                    return this.UIService.MakeScriptButton(ButtonTypesEnum.Button,"测试","alert('aaa');");
                }).SetHeader("测试"),
                this.MakeGridHeader(x => x.Remark),
                this.MakeGridHeaderAction(width: 500)
            };
        }

        public override IOrderedQueryable<School_View> GetSearchQuery()
        {
            var query = DC.Set<School>()
                .CheckContain(Searcher.SchoolCode, x => x.SchoolCode)
                .CheckContain(Searcher.SchoolName, x => x.SchoolName)
                .CheckEqual(Searcher.SchoolType, x => x.SchoolType)
                .Select(x => new School_View
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
