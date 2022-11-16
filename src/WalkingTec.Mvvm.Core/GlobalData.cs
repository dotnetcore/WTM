using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WalkingTec.Mvvm.Core.Extensions;
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
        public List<string> AllMainTenantOnlyUrls { get; set; }
        public Type CustomUserType { get; set; }

        public bool IsSpa { get; set; }
        private List<PropertyInfo> _customUserProperties;
       public List<PropertyInfo> CustomUserProperties
        {
            get {
                if(_customUserProperties == null)
                {
                    _customUserProperties = new List<PropertyInfo>();
                    if(CustomUserType != null)
                    {
                        _customUserProperties = CustomUserType.GetProperties( BindingFlags.Public| BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(x=>x.PropertyType.IsListOf<TopBasePoco>() == false && typeof(TopBasePoco).IsAssignableFrom(x.PropertyType) == false).ToList();
                    }
                }
                return _customUserProperties;
            }
        }
        /// <summary>
        /// 模块
        /// </summary>
        public List<SimpleModule> AllModule { get; set; }

        private Func<List<SimpleMenu>> MenuGetFunc;
        private Func<List<FrameworkTenant>> TenantGetFunc;

        public List<SimpleMenu> AllMenus => MenuGetFunc?.Invoke();
        public List<FrameworkTenant> AllTenant => TenantGetFunc?.Invoke();
        /// <summary>
        /// 设置菜单委托
        /// </summary>
        /// <param name="func"></param>
        public void SetMenuGetFunc(Func<List<SimpleMenu>> func) => MenuGetFunc = func;
        public void SetTenantGetFunc(Func<List<FrameworkTenant>> func) => TenantGetFunc = func;

        public List<Type> GetPocoTypesAssignableFrom<T>()
        {
            var rv = new List<Type>();
            var allType = Utils.GetAllModels();
            rv.AddRange(allType.Where(x => typeof(T).IsAssignableFrom(x) && x != typeof(T) && x.IsAbstract == false).ToList());
            return rv;
        }
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
