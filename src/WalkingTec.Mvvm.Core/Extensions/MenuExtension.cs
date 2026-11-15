using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// 菜单相关的扩展函数，用于将系统菜单数据转换为前端 LayUI 菜单结构
    /// </summary>
    public static class MenuExtension
    {
        /// <summary>
        /// 将系统菜单列表转换为 LayUI 菜单结构，并根据配置决定是否进行权限过滤
        /// </summary>
        /// <param name="allmenus">所有菜单的简化信息列表</param>
        /// <param name="wtm">WTM 上下文</param>
        /// <returns>转换后的 LayUI 菜单列表</returns>
        public static List<LayUIMenu> ToLayuiMenu(this List<SimpleMenu> allmenus, WTMContext wtm)
        {
            var resultMenus = new List<LayUIMenu>();
            //快速调试模式下显示全部菜单，并跳过权限过滤
            if (wtm.ConfigInfo.IsQuickDebug == true)
            {
                GenerateMenuTree(allmenus, resultMenus, true);
                RemoveEmptyMenu(resultMenus);
                LocalizeMenu(resultMenus);
            }
            else
            {
                //正常模式下只显示配置为显示的菜单，并移除当前用户无权限访问的菜单
                GenerateMenuTree(allmenus.Where(x => x.ShowOnMenu == true).ToList(), resultMenus);
                RemoveUnAccessableMenu(resultMenus, wtm);
                RemoveEmptyMenu(resultMenus);
                LocalizeMenu(resultMenus);
            }
            return resultMenus;
        }

        /// <summary>
        /// 将简化菜单列表组装为最多三层的 LayUI 菜单树
        /// </summary>
        /// <param name="menus">所有菜单的简化信息列表</param>
        /// <param name="resultMenus">输出的顶层菜单列表，子菜单挂载到各菜单的 Children 属性上</param>
        /// <param name="quickDebug">是否为快速调试模式，该模式下为没有图标的菜单生成默认图标</param>
        private static void GenerateMenuTree(List<SimpleMenu> menus, List<LayUIMenu> resultMenus, bool quickDebug = false)
        {
            //先取出父节点为空的顶层菜单并按显示顺序排序
            resultMenus.AddRange(menus.Where(x => x.ParentId == null).Select(x => new LayUIMenu()
            {
                Id = x.ID,
                Title = x.PageName,
                Url = x.Url,
                Order = x.DisplayOrder,
                Icon = quickDebug && string.IsNullOrEmpty(x.Icon) ? $"_wtmicon _wtmicon-{(string.IsNullOrEmpty(x.Url) ? "folder" : "file")}" : x.Icon
            })
            .OrderBy(x => x.Order)
            .ToList());

            //为每个顶层菜单查找并挂载二级子菜单，再为二级菜单挂载三级子菜单
            foreach (var menu in resultMenus)
            {
                var temp = menus.Where(x => x.ParentId == menu.Id).Select(x => new LayUIMenu()
                {
                    Id = x.ID,
                    Title = x.PageName,
                    Url = x.Url,
                    Order = x.DisplayOrder,
                    Icon = quickDebug && string.IsNullOrEmpty(x.Icon) ? $"_wtmicon _wtmicon-{(string.IsNullOrEmpty(x.Url) ? "folder" : "file")}" : x.Icon
                })
                .OrderBy(x => x.Order)
                .ToList();
                if (temp.Count() > 0)
                {
                    menu.Children = temp;
                    foreach (var item in menu.Children)
                    {
                        item.Children = menus.Where(x => x.ParentId == item.Id).Select(x => new LayUIMenu()
                        {
                            Title = x.PageName,
                            Url = x.Url,
                            Order = x.DisplayOrder,
                            Icon = quickDebug && string.IsNullOrEmpty(x.Icon) ? $"_wtmicon _wtmicon-{(string.IsNullOrEmpty(x.Url) ? "folder" : "file")}" : x.Icon
                        })
                        .OrderBy(x => x.Order)
                        .ToList();

                        if (item.Children.Count() == 0)
                            item.Children = null;
                    }
                }
            }
        }

        /// <summary>
        /// 递归移除当前用户无权限访问的菜单
        /// </summary>
        /// <param name="menus">待处理的菜单列表</param>
        /// <param name="wtm">WTM 上下文</param>
        private static void RemoveUnAccessableMenu(List<LayUIMenu> menus, WTMContext wtm)
        {
            if (menus == null)
            {
                return;
            }

            List<LayUIMenu> toRemove = new List<LayUIMenu>();
            //如果没有指定用户信息，则用当前用户的登录信息
            var info = wtm.LoginUserInfo;
            //循环所有菜单项
            foreach (var menu in menus)
            {
                //判断是否有权限，如果没有，则添加到需要移除的列表中
                var url = menu.Url;
                if (!string.IsNullOrEmpty(url) && url.StartsWith("/_framework/outside?url="))
                {
                    url = url.Replace("/_framework/outside?url=", "");
                }
                if (!string.IsNullOrEmpty(url) && wtm.IsAccessable(url) == false)
                {
                    toRemove.Add(menu);
                }
                //如果有权限，则递归调用本函数检查子菜单
                else
                {
                    RemoveUnAccessableMenu(menu.Children, wtm);
                }
            }
            //删除没有权限访问的菜单
            foreach (var remove in toRemove)
            {
                menus.Remove(remove);
            }
        }

        /// <summary>
        /// RemoveEmptyMenu
        /// </summary>
        /// <param name="menus"></param>
        private static void RemoveEmptyMenu(List<LayUIMenu> menus)
        {
            if (menus == null)
            {
                return;
            }
            List<LayUIMenu> toRemove = new List<LayUIMenu>();
            //循环所有菜单项
            foreach (var menu in menus)
            {
                RemoveEmptyMenu(menu.Children);
                if ((menu.Children == null || menu.Children.Count == 0) && (string.IsNullOrEmpty(menu.Url)))
                {
                    toRemove.Add(menu);
                }
            }
            foreach (var remove in toRemove)
            {
                menus.Remove(remove);
            }
        }

        /// <summary>
        /// 递归对菜单标题做多语言本地化翻译
        /// </summary>
        /// <param name="menus">待处理的菜单列表</param>
        private static void LocalizeMenu(List<LayUIMenu> menus)
        {
            if (menus == null)
            {
                return;
            }
            //循环所有菜单项
            foreach (var menu in menus)
            {
                LocalizeMenu(menu.Children);
                menu.Title = Core.CoreProgram._localizer?[menu.Title];
            }
        }

    }
}
