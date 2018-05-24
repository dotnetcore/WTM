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
        /// 数据库上下文
        /// </summary>
        public ConstructorInfo DataContextCI { get; set; }

        /// <summary>
        /// 可访问的url地址
        /// </summary>
        public List<string> AllAccessUrls { get; set; }

        /// <summary>
        /// 模块
        /// </summary>
        public List<FrameworkModule> AllModule { get; set; }

        /// <summary>
        /// 数据库模型
        /// </summary>
        public List<Type> AllModels { get; set; }

        private Func<List<FrameworkMenu>> MenuGetFunc;

        public List<FrameworkMenu> AllMenus => MenuGetFunc == null ? null : MenuGetFunc();

        /// <summary>
        /// 设置菜单委托
        /// </summary>
        /// <param name="func"></param>
        public void SetMenuGetFunc(Func<List<FrameworkMenu>> func) => MenuGetFunc = func;
    }
}
