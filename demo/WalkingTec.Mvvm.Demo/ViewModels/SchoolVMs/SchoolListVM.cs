using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.SchoolVMs
{
    public class SchoolListVM : BasePagedListVM<School_View, SchoolSearcher>
    {

        public SchoolListVM()
        {
            NeedPage = false;
        }


        public override string SetFullRowBgColor(object entity)
        {
            var t = entity as School_View;
            if (t.SchoolType == SchoolTypeEnum.PRI)
            {
                return "FF0000";
            }
            else
            {
                return base.SetFullRowBgColor(entity);
            }
        }

        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800).SetHideOnToolBar(false).SetPromptMessage("你确定要修改么？").SetButtonClass("layui-btn-normal"),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Delete, "删除","", dialogWidth: 800).SetPromptMessage("你确定要删除么？").SetButtonClass("layui-btn-danger"),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeAction("School","EditIndex","列表编辑","列表编辑", GridActionParameterTypesEnum.NoId,dialogWidth:800).SetShowDialog(false).SetIsRedirect
                (true),
                this.MakeAction("School","Create2","主子表新建","主子表新建", GridActionParameterTypesEnum.NoId,dialogWidth:800),
                this.MakeAction("School","Edit2","主子表修改","主子表修改", GridActionParameterTypesEnum.SingleId,dialogWidth:800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardAction("School", GridActionStandardTypesEnum.ExportExcel, "导出",""),
                this.MakeAction("School","Download","下载",null, GridActionParameterTypesEnum.SingleId).SetOnClickScript("download"),
                this.MakeActionsGroup("批量处理",new List<GridAction>(){
                      this.MakeStandardAction("School", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                      this.MakeStandardAction("School", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                 }),
                this.MakeActionsGroup("批量处理2",new List<GridAction>(){
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
                }).SetHeader("测试").SetDisableExport(),
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
                .DPWhere(LoginUserInfo.DataPrivileges,x=>x.ID)
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
