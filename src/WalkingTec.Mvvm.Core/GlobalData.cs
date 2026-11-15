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
        /// <summary>
        /// 只有主租户可以访问的 url 地址
        /// </summary>
        public List<string> AllMainTenantOnlyUrls { get; set; }
        /// <summary>
        /// 自定义用户类型
        /// </summary>
        public Type CustomUserType { get; set; }

        /// <summary>
        /// 是否为单页应用模式
        /// </summary>
        public bool IsSpa { get; set; }
        private List<PropertyInfo> _customUserProperties;
        /// <summary>
        /// 自定义用户类型的属性列表，只包含简单属性，过滤掉模型关联属性
        /// </summary>
       public List<PropertyInfo> CustomUserProperties
        {
            get {
                if(_customUserProperties == null)
                {
                    _customUserProperties = new List<PropertyInfo>();
                    if(CustomUserType != null)
                    {
                        // 只获取本类型声明的公开实例属性，排除一对多和一对一的模型关联属性
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

        /// <summary>
        /// 所有菜单，通过设置的委托延迟获取
        /// </summary>
        public List<SimpleMenu> AllMenus => MenuGetFunc?.Invoke();
        /// <summary>
        /// 所有租户，通过设置的委托延迟获取
        /// </summary>
        public List<FrameworkTenant> AllTenant => TenantGetFunc?.Invoke();
        /// <summary>
        /// 设置菜单委托
        /// </summary>
        /// <param name="func"></param>
        public void SetMenuGetFunc(Func<List<SimpleMenu>> func) => MenuGetFunc = func;
        /// <summary>
        /// 设置租户获取委托
        /// </summary>
        /// <param name="func">获取租户列表的委托</param>
        public void SetTenantGetFunc(Func<List<FrameworkTenant>> func) => TenantGetFunc = func;

        /// <summary>
        /// 在所有模型类型中查找继承自指定类型的具体类型
        /// </summary>
        /// <typeparam name="T">基类或接口类型</typeparam>
        /// <returns>符合条件的模型类型列表</returns>
        public List<Type> GetPocoTypesAssignableFrom<T>()
        {
            var rv = new List<Type>();
            var allType = Utils.GetAllModels();
            rv.AddRange(allType.Where(x => typeof(T).IsAssignableFrom(x) && x != typeof(T) && x.IsAbstract == false).ToList());
            return rv;
        }
        /// <summary>
        /// 在所有程序集中查找继承自指定类型的具体类型
        /// </summary>
        /// <typeparam name="T">基类或接口类型</typeparam>
        /// <returns>符合条件的类型列表</returns>
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
