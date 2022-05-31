// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                         this.MakeGridHeader(x => x.ParentId).SetHeader(Localizer["Sys.Operation"]).SetFormat((item, cell) => GenerateCheckBox(item)).SetAlign(GridColumnAlignEnum.Left),
                   });
                    break;
                default:
                    rv.AddRange(new GridColumn<FrameworkMenu_ListView>[] {
                        this.MakeGridHeader(x => x.PageName,300),
                        this.MakeGridHeader(x => x.ModuleName, 150),
                        this.MakeGridHeader(x => x.ShowOnMenu),
                        this.MakeGridHeader(x => x.FolderOnly),
                        this.MakeGridHeader(x => x.IsPublic),
                       this.MakeGridHeader(x => x.TenantAllowed),
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
            StringBuilder rv = new StringBuilder();
            if (item.FolderOnly == false)
            {
                rv.Append($"<input do-not-use-selectall hidden lay-skin='primary' type='checkbox' title='{Localizer["Sys.All"]}'>");
                if (item.IsInside == true)
                {
                    var others = item.Children?.ToList();
                    rv.Append(UIService.MakeCheckBox(item.Allowed, Localizer["Sys.MainPage"], "menu_" + item.ID, "1"));
                    if (others != null)
                    {
                        foreach (var c in others)
                        {
                            string actionname = "";
                            if (c.ActionName != null)
                            {
                                actionname = Localizer[c.ActionName];
                            }
                            rv.Append(UIService.MakeCheckBox(c.Allowed, actionname, "menu_" + c.ID, "1"));
                        }
                    }
                }
                else
                {
                    rv.Append(UIService.MakeCheckBox(item.Allowed, Localizer["Sys.MainPage"], "menu_" + item.ID, "1"));
                }
            }

            return rv.ToString();
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
            List<FrameworkMenu> data = new List<FrameworkMenu>();
            using (var maindc = Wtm.CreateDC(false, "default"))
            {
                data = maindc.Set<FrameworkMenu>().ToList();
            }
            var topdata = data.Where(x => x.ParentId == null).ToList().FlatTree(x => x.DisplayOrder).Where(x => x.IsInside == false || x.FolderOnly == true || x.Url.EndsWith("/Index") || string.IsNullOrEmpty(x.MethodName)).ToList();
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
           if(Wtm.ConfigInfo.EnableTenant == true && LoginUserInfo.CurrentTenant != null)
            {
                var ct = Wtm.GlobaInfo.AllTenant.Where(x => x.TCode == LoginUserInfo.CurrentTenant).FirstOrDefault();
                for(int i = 0; i < topdata.Count; i++)
                {
                    if (topdata[i].TenantAllowed == false || (topdata[i].Url != null && ct.EnableSub == false && topdata[i].Url.ToLower().Contains("frameworktenant")))
                    {
                        topdata.RemoveAt(i);
                        i--;
                        continue;
                    }
                    var hostonly = Wtm.GlobaInfo.AllMainTenantOnlyUrls;
                    foreach (var au in hostonly)
                    {
                        if (topdata[i].Url != null && new Regex("^" + au + "[/\\?]?", RegexOptions.IgnoreCase).IsMatch(topdata[i].Url))
                        {
                            topdata.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                }
            }
            topdata.ForEach((x) => { int l = x.GetLevel(); for (int i = 0; i < l; i++) { x.PageName = "&nbsp;&nbsp;&nbsp;&nbsp;" + x.PageName; } });
            if (SearcherMode == ListVMSearchModeEnum.Custom2)
            {
                var pris = DC.Set<FunctionPrivilege>()
                                .Where(x => x.RoleCode == Searcher.RoleCode).ToList();
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
                    }).ToList(),
                    ExtraOrder = order++,
                    ParentId = x.ParentId,
                    IsInside = x.IsInside,
                    TenantAllowed = x.TenantAllowed,
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
                    TenantAllowed = x.TenantAllowed,
                    ParentId = x.ParentId,
                    Icon = x.Icon
                }).OrderBy(x => x.ExtraOrder);

                return data2.AsQueryable() as IOrderedQueryable<FrameworkMenu_ListView>;

            }
        }

    }

}
