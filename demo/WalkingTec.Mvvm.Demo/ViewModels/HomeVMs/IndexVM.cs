using System.Collections.Generic;
using System.Linq;
using System.Web;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Demo.ViewModels.HomeVMs
{
    public class IndexVM : BaseVM
    {
        public List<FrameworkMenu> AllMenu { get; set; }

        public List<TreeSelectListItem> Menu
        {
            get
            {
                return GetUserMenu();
            }
        }

        public List<TreeSelectListItem> GetUserMenu()
        {
            if (ConfigInfo.IsQuickDebug == true)
            {
                return AllMenu.AsQueryable().GetTreeSelectListItems(null, null, x => x.PageName, null, x => x.IConId.ToString(), x => x.Url, SortByName: false); ;
            }
            else
            {
                var rv = AllMenu.Where(x =>x.ShowOnMenu == true).AsQueryable().GetTreeSelectListItems(null, null, x => x.PageName, null, x => x.IConId.ToString(), x => x.IsInside==true ? x.Url : "/_framework/outside?url="+x.Url, SortByName: false);
                RemoveUnAccessableMenu(rv, LoginUserInfo);
                RemoveEmptyMenu(rv);
                return rv;
            }
        }

        /// <summary>
        /// 移除没有权限访问的菜单
        /// </summary>
        /// <param name="menus">菜单列表</param>
        /// <param name="info">用户信息</param>
        private void RemoveUnAccessableMenu(List<TreeSelectListItem> menus, LoginUserInfo info)
        {
            if (menus == null)
            {
                return;
            }

            List<TreeSelectListItem> toRemove = new List<TreeSelectListItem>();
            //如果没有指定用户信息，则用当前用户的登录信息
            if (info == null)
            {
                info = LoginUserInfo;
            }
            //循环所有菜单项
            foreach (var menu in menus)
            {
                //判断是否有权限，如果没有，则添加到需要移除的列表中
                var url = menu.Url;
                if (!string.IsNullOrEmpty(url) && url.StartsWith("/_framework/outside?url="))
                {
                    url = url.Replace("/_framework/outside?url=", "");
                }
                if (!string.IsNullOrEmpty(url) && info.IsAccessable(HttpUtility.UrlDecode(url)) == false)
                {
                    toRemove.Add(menu);
                }
                //如果有权限，则递归调用本函数检查子菜单
                else
                {
                    RemoveUnAccessableMenu(menu.Children, info);
                }
            }
            //删除没有权限访问的菜单
            foreach (var remove in toRemove)
            {
                menus.Remove(remove);
            }
        }

        private void RemoveEmptyMenu(List<TreeSelectListItem> menus)
        {
            for(int i = 0; i < menus.Count; i++)
            {
                if ((menus[i].Children == null || menus[i].Children.Count == 0) && (menus[i].Url == null))
                {
                    menus.RemoveAt(i);
                    i--;
                }
            }

        }

    }
}
