// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs
{
    public class FrameworkMenuListVM2 : BasePagedListVM<FrameworkMenu_ListView, BaseSearcher>
    {
        public FrameworkMenuListVM2()
        {
            this.NeedPage = false;
        }

        protected override IEnumerable<IGridColumn<FrameworkMenu_ListView>> InitGridHeader()
        {
            List<GridColumn<FrameworkMenu_ListView>> rv = new List<GridColumn<FrameworkMenu_ListView>>();
            rv.AddRange(new GridColumn<FrameworkMenu_ListView>[] {
                        this.MakeGridHeader(x => x.PageName, 300),
                        this.MakeGridHeader(x => x.ModuleName, 150),
                        this.MakeGridHeader(x => x.ShowOnMenu, 60),
                        this.MakeGridHeader(x => x.FolderOnly, 60),
                        this.MakeGridHeader(x => x.IsPublic, 60),
                        this.MakeGridHeader(x => x.DisplayOrder, 60),
                        this.MakeGridHeader(x => x.Icon, 100),
                        this.MakeGridHeader(x => x.Children, 100),
                        this.MakeGridHeader(x=>x.ParentId).SetHide(),
                        this.MakeGridHeaderAction(width: 290)
                    });
            return rv;
        }

        public override IOrderedQueryable<FrameworkMenu_ListView> GetSearchQuery()
        {
            var data = DC.Set<FrameworkMenu>().ToList();
            var topdata = data.Where(x => x.ParentId == null).ToList().FlatTree(x => x.DisplayOrder).Where(x => x.IsInside == false || x.FolderOnly == true || string.IsNullOrEmpty(x.MethodName)).ToList();
            foreach (var item in topdata)
            {
                if (item.PageName?.StartsWith("MenuKey.") == true)
                {
                    item.PageName = Localizer[item.PageName];
                }
                if (item.ModuleName?.StartsWith("MenuKey.") == true)
                {
                    item.ModuleName = Localizer[item.ModuleName];
                }
            }
            int order = 0;
            var data2 = topdata.Select(x => new FrameworkMenu_ListView
            {
                ID = x.ID,
                PageName = x.PageName,
                ModuleName = x.ModuleName,
                ActionName = x.ActionName,
                ShowOnMenu = x.ShowOnMenu,
                FolderOnly = x.FolderOnly,
                IsPublic = x.IsPublic,
                DisplayOrder = x.DisplayOrder,
                ExtraOrder = order++,
                ParentId = x.ParentId,
                Icon = x.Icon
            }).OrderBy(x => x.ExtraOrder).ToList();
            return data2.AsQueryable() as IOrderedQueryable<FrameworkMenu_ListView>;
        }

    }
    public class FrameworkMenu_ListView : FrameworkMenu
    {
        public bool Allowed { get; set; }

        public bool Denied { get; set; }

        public string IconClass { get; set; }

        public int ExtraOrder { get; set; }

        public new List<FrameworkMenu_ListView> Children { get; set; }
        public new bool HasChildren
        {
            get
            {
                return Children?.Any() == true;
            }
        }
    }

}
