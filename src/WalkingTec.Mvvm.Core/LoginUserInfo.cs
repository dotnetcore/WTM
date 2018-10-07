using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 用户登录信息，需要保存在Session中，所以使用Serializable标记
    /// </summary>
    public class LoginUserInfo 
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 登录用户
        /// </summary>
        public string ITCode { get; set; }

        public string Name { get; set; }

        public string Memo { get; set; }

        public List<FrameworkRole> Roles { get; set; }

        public List<FrameworkGroup> Groups { get; set; }

        public Dictionary<string,object> Attributes { get; set; }
        /// <summary>
        /// 用户的页面权限列表
        /// </summary>
        public List<FunctionPrivilege> FunctionPrivileges { get; set; }
        /// <summary>
        /// 用户的数据权限列表
        /// </summary>
        public List<DataPrivilege> DataPrivileges { get; set; }

        private GlobalData _globalData;
        private GlobalData GlobalData
        {
            get
            {
                if (_globalData == null)
                {
                    _globalData = GlobalServices.GetRequiredService<GlobalData>();
                }
                return _globalData;
            }
        }

        private Configs _configs;
        private Configs Configs
        {
            get
            {
                if (_configs == null)
                {
                    _configs = GlobalServices.GetRequiredService<Configs>();
                }
                return _configs;
            }
        }


        /// <summary>
        /// 判断某URL是否有权限访问
        /// </summary>
        /// <param name="url">url地址</param>
        /// <returns>true代表可以访问，false代表不能访问</returns>
        public bool IsAccessable(string url)
        {
            // 如果是调试 或者 url 为 null or 空字符串
            if (Configs.IsQuickDebug || string.IsNullOrEmpty(url))
            {
                return true;
            }
            //循环所有不限制访问的url，如果含有当前判断的url，则认为可以访问
            var publicActions = GlobalData.AllAccessUrls;
            foreach (var au in publicActions)
            {
                if (new Regex(au + "[/\\?]?", RegexOptions.IgnoreCase).IsMatch(url))
                {
                    return true;
                }
            }
            //如果没有任何页面权限，则直接返回false
            if (FunctionPrivileges == null)
            {
                return false;
            }


            url = Regex.Replace(url, "/do(batch.*)", "/$1", RegexOptions.IgnoreCase);

            //如果url以#开头，一般是javascript使用的临时地址，不需要判断，直接返回true
            url = url.Trim();

            if (url.StartsWith("#"))
            {
                return true;
            }
            var menus = GlobalData.AllMenus;
            var menu = Utils.FindMenu(url);
            //如果最终没有找到，说明系统菜单中并没有配置这个url，返回false
            if (menu == null)
            {
                return false;
            }
            //如果找到了，则继续验证其他权限
            else
            {
                return IsAccessable(menu, menus);
            }
        }

        /// <summary>
        /// 判断某菜单是否有权限访问
        /// </summary>
        /// <param name="menu">菜单项</param>
        /// <param name="menus">所有系统菜单</param>
        /// <returns>true代表可以访问，false代表不能访问</returns>
        public bool IsAccessable(FrameworkMenu menu, List<FrameworkMenu> menus)
        {
            //寻找当前菜单的页面权限
            var find = FunctionPrivileges.Where(x => x.MenuItemId == menu.ID).ToList();
            //如果能找到直接对应的页面权限
            if (find.Count > 0)
            {
                //检查是否有拒绝访问的设定，如果有则直接返回false
                var deny = find.Where(x => x.Allowed == false && x.UserId != null).FirstOrDefault();
                if (deny != null)
                {
                    return false;
                }
                else
                {
                    //检查是否有允许访问的设定，如果有则直接返回true
                    var allow = find.Where(x => x.Allowed == true && x.UserId != null).FirstOrDefault();
                    if (allow != null)
                    {
                        return true;
                    }
                    else
                    {
                        //如果用户没有指定任何页面权限，则检查用户所属角色中是否有拒绝访问的设定，如果有则返回false
                        var roleDeny = find.Where(x => x.Allowed == false && x.RoleId != null).FirstOrDefault();
                        if (roleDeny != null)
                        {
                            return false;
                        }
                        //如果没有则返回true，因为find里面有值，但前三种情况都没有，则肯定是允许角色访问是有的，所以不再做判断了
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            //如果没有直接对应的，且如果当前menu设定了继承属性，则递归寻找上层菜单是否可以访问
            else
            {
                if (menu.Parent == null)
                {
                    return false;
                }
                else
                {
                    return IsAccessable(menu.Parent, menus);
                }
            }
        }

    }
}
