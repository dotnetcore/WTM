using System;
using System.Collections.Generic;
using System.Reflection;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 应用全局缓存
    /// </summary>
    public class GlobalData
    {

        #region Data Privilege

        /// <summary>
        /// Data Privilege
        /// </summary>
        public List<IDataPrivilege> DataPrivilegeSettings { get; set; }

        #endregion

        /// <summary>
        /// 程序集
        /// </summary>
        public List<Assembly> AllAssembly { get; set; }

        /// <summary>
        /// 可访问的url地址
        /// </summary>
        public List<string> AllAccessUrls { get; set; }


        /// <summary>
        /// 模块
        /// </summary>
        public List<FrameworkModule> AllModule { get; set; }

        private Func<List<FrameworkMenu>> MenuGetFunc;

        public List<FrameworkMenu> AllMenus => MenuGetFunc?.Invoke();

        /// <summary>
        /// 设置菜单委托
        /// </summary>
        /// <param name="func"></param>
        public void SetMenuGetFunc(Func<List<FrameworkMenu>> func) => MenuGetFunc = func;

    }
}
