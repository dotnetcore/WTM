using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs
{
    public class FrameworkMenuListVM2 : BasePagedListVM<FrameworkMenu_ListView, FrameworkMenuSearcher>
    {
        /// <summary>
        /// 页面显示按钮方案，如果需要增加Action类型则将按钮添加到此类中
        /// </summary>
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
                        this.MakeGridHeader(x => x.ICon, 100),
                        this.MakeGridHeader(x => x.CustomICon, 100),
                        this.MakeGridHeader(x => x.Children, 100),
                        this.MakeGridHeader(x=>x.ParentID).SetHide(),
                        this.MakeGridHeaderAction(width: 290)
                    });
            return rv;
        }

        /// <summary>
        /// 查询结果
        /// </summary>
        public override IOrderedQueryable<FrameworkMenu_ListView> GetSearchQuery()
        {
            var data = DC.Set<FrameworkMenu>().ToList();
            var topdata = data.Where(x => x.ParentId == null).ToList().FlatTree(x => x.DisplayOrder).Where(x => x.IsInside == false || x.FolderOnly == true || string.IsNullOrEmpty(x.MethodName)).ToList();
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
                ParentID = x.ParentId,
                ICon = x.IConId,
                CustomICon = x.CustumIcon,
                HasChild = (x.Children != null && x.Children.Count() > 0) ? true : false
            }).OrderBy(x => x.ExtraOrder).ToList();

            //foreach (var item in data2)
            //{
            //    if (item.ParentID != null)
            //    {
            //        var p = data2.Where(x => x.ID == item.ParentID).SingleOrDefault();
            //        if (p != null)
            //        {
            //            if (p.Children == null)
            //            {
            //                p.Children = new List<FrameworkMenu_ListView>();
            //            }
            //            var t = p.Children.ToList();
            //            t.Add(new FrameworkMenu_ListView
            //                {
            //                    ID = item.ID,
            //                    PageName = item.PageName,
            //                    ModuleName = item.ModuleName,
            //                    ActionName = item.ActionName,
            //                    ShowOnMenu = item.ShowOnMenu,
            //                    FolderOnly = item.FolderOnly,
            //                    IsPublic = item.IsPublic,
            //                    DisplayOrder = item.DisplayOrder,
            //                    ExtraOrder = order++,
            //                    ParentID = item.ParentID,
            //                    ICon = item.ICon,
            //                    CustomICon = item.CustomICon,
            //                    HasChild = (item.Children != null && item.Children.Count() > 0) ? true : false
            //                }
            //            );
            //            p.Children = t;
            //        }
            //    }
            //}
            //var toremove = data2.Where(x => x.ParentID != null).ToList();
            //toremove.ForEach(x => data2.Remove(x));d
            return data2.AsQueryable() as IOrderedQueryable<FrameworkMenu_ListView>;
        }

    }

}
