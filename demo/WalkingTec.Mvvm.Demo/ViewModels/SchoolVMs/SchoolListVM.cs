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
    public class SchoolListVM : BasePagedListVM<School_View, SchoolSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800).SetHideOnToolBar(false),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Delete, "删除","", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeAction("School","Create2","主子表新建","主子表新建", GridActionParameterTypesEnum.NoId,dialogWidth:800),
                this.MakeAction("School","Edit2","主子表修改","主子表修改", GridActionParameterTypesEnum.SingleId,dialogWidth:800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel),
                this.MakeActionsGroup("批量处理",new List<GridAction>(){
                      this.MakeStandardAction("School", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                      this.MakeStandardAction("School", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                 })
            };
        }

        protected override IEnumerable<IGridColumn<School_View>> InitGridHeader()
        {
            return new List<GridColumn<School_View>>{
                this.MakeGridHeader(x => x.SchoolCode),
                this.MakeGridHeader(x => x.SchoolName),
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

    public class School_View : School
    {

    }
}
