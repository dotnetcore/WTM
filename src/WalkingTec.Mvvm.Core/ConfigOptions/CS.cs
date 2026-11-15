using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 数据库连接配置，对应 appsettings.json 中 Connections 节下的一条连接项；框架据此创建对应的 DataContext 实例
    /// </summary>
    public class CS
    {
        /// <summary>
        /// 连接名称，代码中通过此名称引用该连接（如默认连接 default）
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 数据库类型（SqlServer / MySql / PgSql / Memory / SQLite / Oracle / DaMeng）；未指定时默认使用 SqlServer
        /// </summary>
        public DBTypeEnum? DbType { get; set; }
        /// <summary>
        /// 数据库版本号，连接 Oracle 等对版本敏感的数据库时需要指定
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 该连接使用的 DataContext 类名；为空时默认使用 DataContext
        /// </summary>
        public string DbContext { get; set; }

        /// <summary>
        /// 缓存的 DataContext 构造函数，首次创建后复用，避免每次创建 DataContext 都进行反射查找
        /// </summary>
        public ConstructorInfo DcConstructor;
        private static List<ConstructorInfo> _cis;

        /// <summary>
        /// 所有程序集中可用的 DataContext 构造函数列表（构造参数为 CS 对象），静态缓存，供按类名查找构造函数使用
        /// </summary>
        public static List<ConstructorInfo> Cis
        {
            get
            {
                if (_cis == null)
                {
                    // 加载所有程序集，查找所有继承自 DbContext 的自定义 DataContext 类型（排除框架内置的 DbContext、FrameworkContext、EmptyContext）
                    var AllAssembly = Utils.GetAllAssembly();
                     _cis = new List<ConstructorInfo>();
                    if (AllAssembly != null)
                    {
                        foreach (var ass in AllAssembly)
                        {
                            try
                            {
                                var t = ass.GetExportedTypes().Where(x => typeof(DbContext).IsAssignableFrom(x) && x.Name != "DbContext" && x.Name != "FrameworkContext" && x.Name != "EmptyContext").ToList();
                                foreach (var st in t)
                                {
                                    var ci = st.GetConstructor(new Type[] { typeof(CS) });
                                    if (ci != null)
                                    {
                                        _cis.Add(ci);
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
                return _cis;
            }
        }

        private static List<ConstructorInfo> _cisFull;

        /// <summary>
        /// 所有程序集中可用的 DataContext 构造函数列表（构造参数为连接字符串 + 数据库类型），静态缓存，供直接指定连接串创建 DataContext 使用
        /// </summary>
        public static List<ConstructorInfo> CisFull
        {
            get
            {
                if (_cisFull == null)
                {
                    // 与 Cis 类似，但查找的是构造参数为 (string, DBTypeEnum) 的构造函数
                    var AllAssembly = Utils.GetAllAssembly();
                    _cisFull = new List<ConstructorInfo>();
                    if (AllAssembly != null)
                    {
                        foreach (var ass in AllAssembly)
                        {
                            try
                            {
                                var t = ass.GetExportedTypes().Where(x => typeof(DbContext).IsAssignableFrom(x) && x.Name != "DbContext" && x.Name != "FrameworkContext" && x.Name != "EmptyContext").ToList();
                                foreach (var st in t)
                                {
                                    var ci = st.GetConstructor(new Type[] { typeof(string), typeof(DBTypeEnum) });
                                    if (ci != null)
                                    {
                                        _cisFull.Add(ci);
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }
                return _cisFull;
            }
        }

        /// <summary>
        /// 创建该连接对应的 DataContext 实例
        /// </summary>
        public IDataContext CreateDC()
        {
            // 首次调用时按 DbContext 指定的类名在 Cis 中查找构造函数并缓存；未指定或找不到同名类时退回列表中的第一个
            if (DcConstructor == null)
            {
                string dcname = DbContext;
                if (string.IsNullOrEmpty(dcname))
                {
                    dcname = "DataContext";
                }
                DcConstructor = Cis.Where(x => x.DeclaringType.Name.ToLower() == dcname.ToLower()).FirstOrDefault();
                if (DcConstructor == null)
                {
                    DcConstructor = Cis.FirstOrDefault();
                }
            }
            return (IDataContext)DcConstructor?.Invoke(new object[] { this });
        }
    }
}
