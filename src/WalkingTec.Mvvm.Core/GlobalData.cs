using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WalkingTec.Mvvm.Core.Support.Json;

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

        public Type CustomUserType { get; set; }
        /// <summary>
        /// 模块
        /// </summary>
        public List<SimpleModule> AllModule { get; set; }

        private Func<List<SimpleMenu>> MenuGetFunc;

        public List<SimpleMenu> AllMenus => MenuGetFunc?.Invoke();

        /// <summary>
        /// 设置菜单委托
        /// </summary>
        /// <param name="func"></param>
        public void SetMenuGetFunc(Func<List<SimpleMenu>> func) => MenuGetFunc = func;

        public List<Type> GetTypesAssignableFrom<T>()
        {
            var rv = new List<Type>();
            foreach (var ass in AllAssembly)
            {
                var types = new List<Type>();
                try
                {
                    types.AddRange(ass.GetExportedTypes());
                }
                catch { }

                rv.AddRange(types.Where(x => typeof(T).IsAssignableFrom(x) && x != typeof(T) && x.IsAbstract == false).ToList());
            }
            return rv;
        }

    }
}
