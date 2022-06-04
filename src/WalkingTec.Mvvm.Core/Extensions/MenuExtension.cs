using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Core.Extensions
{
    public static class MenuExtension
    {
        public static List<LayUIMenu> ToLayuiMenu(this List<SimpleMenu> allmenus, WTMContext wtm)
        {
            var resultMenus = new List<LayUIMenu>();
            if (wtm.ConfigInfo.IsQuickDebug == true)
            {
                GenerateMenuTree(allmenus, resultMenus, true);
                RemoveEmptyMenu(resultMenus);
                LocalizeMenu(resultMenus);
            }
            else
            {
                GenerateMenuTree(allmenus.Where(x => x.ShowOnMenu == true).ToList(), resultMenus);
                RemoveUnAccessableMenu(resultMenus, wtm);
                RemoveEmptyMenu(resultMenus);
                LocalizeMenu(resultMenus);
            }
            return resultMenus;
        }

        private static void GenerateMenuTree(List<SimpleMenu> menus, List<LayUIMenu> resultMenus, bool quickDebug = false)
        {
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
