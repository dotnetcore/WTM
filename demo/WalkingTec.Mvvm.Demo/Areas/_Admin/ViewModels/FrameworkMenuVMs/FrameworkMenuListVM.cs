// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs
{
    public class FrameworkMenuListVM : BasePagedListVM<FrameworkMenu_ListView, FrameworkMenuSearcher>
    {
        public FrameworkMenuListVM()
        {
            this.NeedPage = false;
        }

        protected override IEnumerable<IGridColumn<FrameworkMenu_ListView>> InitGridHeader()
        {
            List<GridColumn<FrameworkMenu_ListView>> rv = new List<GridColumn<FrameworkMenu_ListView>>();
            switch (SearcherMode)
            {
                case ListVMSearchModeEnum.Batch:
                    rv.AddRange(new GridColumn<FrameworkMenu_ListView>[] {
                        this.MakeGridHeader(x => x.PageName),
                        this.MakeGridHeader(x => x.ModuleName, 200),
                        this.MakeGridHeader(x => x.ActionName, 150),
                    });
                    break;
                case ListVMSearchModeEnum.Custom1:
                    rv.AddRange(new GridColumn<FrameworkMenu_ListView>[] {
                        this.MakeGridHeader(x => x.PageName,300),
                    });
                    break;
                case ListVMSearchModeEnum.Custom2:
                    rv.AddRange(new GridColumn<FrameworkMenu_ListView>[] {
                        this.MakeGridHeader(x => x.PageName,200),
                         this.MakeGridHeader(x => x.ParentID).SetHeader(Localizer["Sys.Operation"]).SetFormat((item, cell) => GenerateCheckBox(item)).SetAlign(GridColumnAlignEnum.Left),
                   });
                    break;
                default:
                    rv.AddRange(new GridColumn<FrameworkMenu_ListView>[] {
                        this.MakeGridHeader(x => x.PageName,300),
                        this.MakeGridHeader(x => x.ModuleName, 150),
                        this.MakeGridHeader(x => x.ShowOnMenu),
                        this.MakeGridHeader(x => x.FolderOnly),
                        this.MakeGridHeader(x => x.IsPublic),
                        this.MakeGridHeader(x => x.DisplayOrder),
                        this.MakeGridHeader(x => x.Icon, 100).SetFormat(PhotoIdFormat),
                        this.MakeGridHeaderAction(width: 270)
                    });
                    break;
            }
            return rv;
        }

        private object GenerateCheckBox(FrameworkMenu_ListView item)
        {
            string rv = "";
            if (item.FolderOnly == false)
            {
                if (item.IsInside == true)
                {

                    var others = item.Children?.ToList();
                    rv += UIService.MakeCheckBox(item.Allowed, Localizer["Sys.MainPage"], "menu_" + item.ID, "1");
                    if (others != null)
                    {
                        foreach (var c in others)
                        {
                            string actionname = "";
                            if(c.ActionName != null)
                            {
                                    actionname = Localizer[c.ActionName];
                            }
                            rv += UIService.MakeCheckBox(c.Allowed, actionname, "menu_" + c.ID, "1");
                        }
                    }
                }
                else
                {
                    rv += UIService.MakeCheckBox(item.Allowed, Localizer["Sys.MainPage"], "menu_" + item.ID, "1");
                }
            }
            return rv;
        }


        protected override List<GridAction> InitGridAction()
        {
            if (SearcherMode == ListVMSearchModeEnum.Search)
            {
                return new List<GridAction>{
                this.MakeAction("FrameworkMenu", "Create",Localizer["Sys.Create"], Localizer["Sys.Create"],  GridActionParameterTypesEnum.SingleIdWithNull,"_Admin").SetIconCls("layui-icon layui-icon-add-1"),
                this.MakeStandardAction("FrameworkMenu", GridActionStandardTypesEnum.Edit, "", "_Admin"),
                this.MakeStandardAction("FrameworkMenu", GridActionStandardTypesEnum.Delete, "", "_Admin"),
                this.MakeStandardAction("FrameworkMenu", GridActionStandardTypesEnum.Details, "", "_Admin"),
                this.MakeAction( "FrameworkMenu", "UnsetPages", Localizer["_Admin.CheckPage"], Localizer["_Admin.UnsetPages"],GridActionParameterTypesEnum.NoId, "_Admin").SetIconCls("layui-icon layui-icon-ok"),
                this.MakeAction("FrameworkMenu", "RefreshMenu", Localizer["_Admin.RefreshMenu"], Localizer["_Admin.RefreshMenu"],  GridActionParameterTypesEnum.NoId,"_Admin").SetShowDialog(false).SetIconCls("layui-icon layui-icon-refresh"),
                };
            }
            else
            {
                return new List<GridAction>();
            }
        }


        private string PhotoIdFormat(FrameworkMenu_ListView entity, object val)
        {
            if (entity.Icon != null)
            {
                return $"<i class='{entity.Icon}'></i>";
            }
            else
            {
                return "";
            }
        }

        public override IOrderedQueryable<FrameworkMenu_ListView> GetSearchQuery()
        {

            var data = DC.Set<FrameworkMenu>().ToList();
            var topdata = data.Where(x => x.ParentId == null).ToList().FlatTree(x => x.DisplayOrder).Where(x => x.IsInside == false || x.FolderOnly == true || x.Url.EndsWith("/Index") || x.MethodName == null).ToList();
            foreach (var item in topdata)
            {
                if (item.PageName?.StartsWith("MenuKey.") == true)
                {
                        item.PageName =Localizer[item.PageName];
                }
                if (item.ModuleName?.StartsWith("MenuKey.") == true)
                {
                        item.ModuleName = Localizer[item.ModuleName];
                }

            }
            topdata.ForEach((x) => { int l = x.GetLevel(); for (int i = 0; i < l; i++) { x.PageName = "&nbsp;&nbsp;&nbsp;&nbsp;" + x.PageName; } });
            if (SearcherMode == ListVMSearchModeEnum.Custom2)
            {
                var pris = DC.Set<FunctionPrivilege>()
                                .Where(x => x.RoleCode == DC.Set<FrameworkRole>().CheckID(Searcher.RoleID,null).Select(x=>x.RoleCode).FirstOrDefault()).ToList();
                var allowed = pris.Where(x => x.Allowed == true).Select(x => x.MenuItemId).ToList();
                var denied = pris.Where(x => x.Allowed == false).Select(x => x.MenuItemId).ToList();
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
                    Children = x.Children?.Select(y => new FrameworkMenu_ListView
                    {
                        ID = y.ID,
                        Allowed = allowed.Contains(y.ID),
                        ActionName = y.ActionName
                    }),
                    ExtraOrder = order++,
                    ParentID = x.ParentId,
                    Parent = x.Parent,
                    IsInside = x.IsInside,
                    HasChild = (x.Children != null && x.Children.Count() > 0) ? true : false,
                    Allowed = allowed.Contains(x.ID),
                    Denied = denied.Contains(x.ID)
                }).OrderBy(x => x.ExtraOrder);
                return data2.AsQueryable() as IOrderedQueryable<FrameworkMenu_ListView>;
            }
            else
            {
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
                    Icon = x.Icon,
                    HasChild = (x.Children != null && x.Children.Count() > 0) ? true : false
                }).OrderBy(x => x.ExtraOrder);

                return data2.AsQueryable() as IOrderedQueryable<FrameworkMenu_ListView>;

            }
        }

    }

    public class FrameworkMenu_ListView : BasePoco
    {
        [Display(Name = "_Admin.PageName")]
        public string PageName { get; set; }

        [Display(Name = "Codegen.ModuleName")]
        public string ModuleName { get; set; }

        [Display(Name = "_Admin.ActionName")]
        public string ActionName { get; set; }

        [Display(Name = "_Admin.ShowOnMenu")]
        public bool? ShowOnMenu { get; set; }

        [Display(Name = "_Admin.FolderOnly")]
        public bool? FolderOnly { get; set; }

        [Display(Name = "_Admin.IsPublic")]
        public bool? IsPublic { get; set; }

        [Display(Name = "_Admin.DisplayOrder")]
        public int? DisplayOrder { get; set; }

        [Display(Name = "_Admin.Icon")]
        public string Icon { get; set; }

        public bool Allowed { get; set; }

        public bool Denied { get; set; }

        public bool HasChild { get; set; }

        public string IconClass { get; set; }

        public IEnumerable<FrameworkMenu_ListView> Children { get; set; }

        public FrameworkMenu Parent { get; set; }

        public Guid? ParentID { get; set; }

        public int ExtraOrder { get; set; }

        public bool? IsInside { get; set; }
    }
}
