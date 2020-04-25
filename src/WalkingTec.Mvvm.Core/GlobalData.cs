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
        /// <summary>
        /// 程序集
        /// </summary>
        public List<Assembly> AllAssembly { get; set; }

        /// <summary>
        /// 可访问的url地址
        /// </summary>
        public List<string> AllAccessUrls { get; set; }


        private Func<List<FrameworkModule>> ModuleGetFunc;
        private List<FrameworkModule> _allModules;
        /// <summary>
        /// 模块
        /// </summary>
        public List<FrameworkModule> AllModule
        {
            get
            {
                if(_allModules == null)
                {
                    _allModules = ModuleGetFunc?.Invoke();
                }
                return _allModules;
            }
        }

        /// <summary>
        /// 数据库模型
        /// </summary>
        public List<Type> AllModels { get; set; }

        private Func<List<FrameworkMenu>> MenuGetFunc;

        public List<FrameworkMenu> AllMenus => MenuGetFunc?.Invoke();

        /// <summary>
        /// 设置菜单委托
        /// </summary>
        /// <param name="func"></param>
        public void SetMenuGetFunc(Func<List<FrameworkMenu>> func) => MenuGetFunc = func;
        public void SetModuleGetFunc(Func<List<FrameworkModule>> func) => ModuleGetFunc = func;

    }
}
