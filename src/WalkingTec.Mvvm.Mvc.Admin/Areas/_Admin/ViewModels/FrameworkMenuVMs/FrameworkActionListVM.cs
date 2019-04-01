using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs
{
    public class FrameworkActionListVM : BasePagedListVM<FrameworkAction_ListView, BaseSearcher>
    {

        public FrameworkActionListVM()
        {
            NeedPage = false;
        }

        protected override List<GridAction> InitGridAction()
        {
            var actions = new List<GridAction>
            {
            };
            return actions;
        }

        protected override IEnumerable<IGridColumn<FrameworkAction_ListView>> InitGridHeader()
        {
            var header = new List<GridColumn<FrameworkAction_ListView>>();

            header.Add(this.MakeGridHeader(x => x.ModuleName, 150));
            header.Add(this.MakeGridHeader(x => x.ActionName, 150));
            header.Add(this.MakeGridHeader(x => x.ClassName, 150));
            header.Add(this.MakeGridHeader(x => x.MethodName, 150));

            return header;
        }

        /// <summary>
        /// 查询结果
        /// </summary>
        public override IOrderedQueryable<FrameworkAction_ListView> GetSearchQuery()
        {
            var newdc = DC as FrameworkContext;
            List<FrameworkAction_ListView> actions = new List<FrameworkAction_ListView>();
            if (ControllerName == "WalkingTec.Mvvm.Mvc._FrameworkController")
            {
                 actions = GlobalServices.GetRequiredService<GlobalData>().AllModule.SelectMany(x=>x.Actions)
                    .Where(x => x.Module.IsApi == false &&  newdc.BaseFrameworkMenus.Where(y => y.IsInside == true && y.FolderOnly == false).Select(y => y.Url).Distinct().Contains(x.Url) == false)
                    .Select(x => new FrameworkAction_ListView
                    {
                        ID = x.ID,
                        ModuleID = x.ModuleId,
                        ModuleName = x.Module.ModuleName,
                        ActionName = x.ActionName,
                        ClassName = x.Module.ClassName,
                        MethodName = x.MethodName,
                        AreaName = x.Module.Area?.AreaName
                    }).ToList();
            }
            else
            {
                actions = GlobalServices.GetRequiredService<GlobalData>().AllModule.SelectMany(x => x.Actions)
                   .Where(x => x.Module.IsApi == true && newdc.BaseFrameworkMenus.Where(y => y.IsInside == true && y.FolderOnly == false).Select(y => y.Url).Distinct().Contains(x.Url) == false)
                   .Select(x => new FrameworkAction_ListView
                    {
                        ID = x.ID,
                        ModuleID = x.ModuleId,
                        ModuleName = x.Module.ModuleName,
                        ActionName = x.ActionName,
                        ClassName = x.Module.ClassName,
                        MethodName = x.MethodName,
                        AreaName = x.Module.Area?.AreaName
                    }).ToList();

            }

            var modules = GlobalServices.GetRequiredService<GlobalData>().AllModule;
            List<FrameworkAction_ListView> toremove = new List<FrameworkAction_ListView>();
            foreach (var item in actions)
            {
                var m = modules.Where(x => x.ClassName == item.ClassName && x.Area?.AreaName == item.AreaName).FirstOrDefault();
                var a = m?.Actions.Where(x => x.MethodName == item.MethodName).FirstOrDefault();
                if(m?.IgnorePrivillege == true || a?.IgnorePrivillege == true)
                {
                    toremove.Add(item);
                }
            }
            toremove.ForEach(x => actions.Remove(x));
            return actions.AsQueryable().OrderBy(x=>x.AreaName).ThenBy(x=>x.ModuleName).ThenBy(x=>x.MethodName);
        }

    }

    public class FrameworkAction_ListView : BasePoco
    {
        public Guid? ModuleID { get; set; }

        [Display(Name = "模块名称")]
        public string ModuleName { get; set; }
        [Display(Name = "动作名称")]
        public string ActionName { get; set; }
        [Display(Name = "类名")]
        public string ClassName { get; set; }
        [Display(Name = "方法名")]
        public string MethodName { get; set; }

        public string AreaName { get; set; }

    }
}
