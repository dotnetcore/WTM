using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Fare;

namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// Type的扩展函数
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// 类型属性信息缓存，键为类型全名，值为该类型的属性列表，用于减少重复反射
        /// </summary>
        public static ImmutableDictionary<string, List<PropertyInfo>> _propertyCache { get; set; } = new Dictionary<string, List<PropertyInfo>>().ToImmutableDictionary();
        /// <summary>
        /// 判断是否是泛型
        /// </summary>
        /// <param name="self">Type类</param>
        /// <param name="innerType">泛型类型</param>
        /// <returns>判断结果</returns>
        public static bool IsGeneric(this Type self, Type innerType)
        {
            if (self.GetTypeInfo().IsGenericType && self.GetGenericTypeDefinition() == innerType)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断是否为Nullable<>类型
        /// </summary>
        /// <param name="self">Type类</param>
        /// <returns>判断结果</returns>
        public static bool IsNullable(this Type self)
        {
            return self.IsGeneric(typeof(Nullable<>));
        }

        /// <summary>
        /// 判断是否为List<>类型
        /// </summary>
        /// <param name="self">Type类</param>
        /// <returns>判断结果</returns>
        public static bool IsList(this Type self)
        {
            return self.IsGeneric(typeof(List<>)) || self.IsGeneric(typeof(IEnumerable<>));
        }

        /// <summary>
        /// 判断是否为List<>类型
        /// </summary>
        /// <param name="self">Type类</param>
        /// <returns>判断结果</returns>
        public static bool IsListOf<T>(this Type self)
        {
            if (self.IsGeneric(typeof(List<>)) && typeof(T).IsAssignableFrom(self.GenericTypeArguments[0]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #region 判断是否为枚举

        /// <summary>
        /// 判断是否为枚举
        /// </summary>
        /// <param name="self">Type类</param>
        /// <returns>判断结果</returns>
        public static bool IsEnum(this Type self)
        {
            return self.GetTypeInfo().IsEnum;
        }

        /// <summary>
        /// 判断是否为枚举或者可空枚举
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsEnumOrNullableEnum(this Type self)
        {
            if (self == null)
            {
                return false;
            }
            if (self.IsEnum)
            {
                return true;
            }
            else
            {
                if (self.IsGenericType && self.GetGenericTypeDefinition() == typeof(Nullable<>) && self.GetGenericArguments()[0].IsEnum)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        /// <summary>
        /// 判断是否为值类型
        /// </summary>
        /// <param name="self">Type类</param>
        /// <returns>判断结果</returns>
        public static bool IsPrimitive(this Type self)
        {
            return self.GetTypeInfo().IsPrimitive || self == typeof(decimal);
        }

        /// <summary>
        /// 判断是否为常用数字类型（含可空数字类型）
        /// </summary>
        /// <param name="self">Type 类</param>
        /// <returns>判断结果</returns>
        public static bool IsNumber(this Type self)
        {
            Type checktype = self;
            //如果是可空类型，先取出其中的实际类型再判断
            if (self.IsNullable())
            {
                checktype = self.GetGenericArguments()[0];
            }
            if (checktype == typeof(int) || checktype == typeof(short) || checktype == typeof(long) || checktype == typeof(float) || checktype == typeof(decimal) || checktype == typeof(double))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #region 判断是否是Bool

        /// <summary>
        /// 判断是否为 bool 类型
        /// </summary>
        /// <param name="self">Type 类</param>
        /// <returns>判断结果</returns>
        public static bool IsBool(this Type self)
        {
            return self == typeof(bool);
        }

        /// <summary>
        /// 判断是否是 bool or bool?类型
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsBoolOrNullableBool(this Type self)
        {
            if (self == null)
            {
                return false;
            }
            if (self == typeof(bool) || self == typeof(bool?))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// 根据类型中各属性的特性（StringLength、Range 等）生成随机属性值字典，用于代码生成器构造模拟数据
        /// </summary>
        /// <param name="self">目标类型</param>
        /// <returns>属性名与随机值字符串的字典，值为可直接拼入代码的表达式文本</returns>
        public static Dictionary<string, string> GetRandomValues(this Type self)
        {
            Dictionary<string, string> rv = new Dictionary<string, string>();
            string pat = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            var pros = self.GetAllProperties();
            //框架内置字段及审计字段不参与随机值生成
            List<string> skipFields = new List<string>()
            {
               nameof(TopBasePoco.BatchError),
               nameof(TopBasePoco.Checked),
               nameof(TopBasePoco.ExcelIndex),
               nameof(ITenant.TenantCode)
            };
            if (typeof(IBasePoco).IsAssignableFrom(self))
            {
                skipFields.AddRange(
                    new string[]{
               nameof(IBasePoco.CreateBy),
               nameof(IBasePoco.CreateTime),
               nameof(IBasePoco.UpdateBy),
               nameof(IBasePoco.UpdateTime) }
                    );
            }
            if (typeof(IPersistPoco).IsAssignableFrom(self))
            {
                skipFields.Add(nameof(IPersistPoco.IsValid));
            }
            foreach (var pro in pros)
            {
                string key = pro.Name;
                string val = "";
                var notmapped = pro.GetCustomAttribute<NotMappedAttribute>();
                //跳过 NotMapped、集合类型、关联实体类及排除字段，只为简单属性生成随机值
                if (notmapped == null &&
                    pro.PropertyType.IsList() == false &&
                    pro.PropertyType.IsSubclassOf(typeof(TopBasePoco)) == false &&
                    skipFields.Contains(key) == false
                    )
                {
                    //数字类型：按 Range 特性指定的范围生成随机数，未指定时默认 0 到 100
                    if (pro.PropertyType.IsNumber())
                    {
                        var range = pro.GetCustomAttribute<RangeAttribute>();
                        int start = 0;
                        int end = 100;
                        if (range != null)
                        {
                            try
                            {
                                start = (int)Math.Truncate(double.Parse(range.Minimum.ToString()));
                                end = (int)Math.Truncate(double.Parse(range.Maximum.ToString()));
                            }
                            catch { }
                        }
                        Random r = new Random();
                        val = r.Next(start, end).ToString();
                    }
                    else if (pro.PropertyType.IsBoolOrNullableBool())
                    {
                        List<string> boolvalues = new List<string> { "true", "false" };
                        if (pro.PropertyType.IsNullable())
                        {
                            boolvalues.Add("null");
                        }
                        Random r = new Random();
                        var index = r.Next(0, boolvalues.Count);
                        val = boolvalues[index];
                    }
                    else if (pro.PropertyType.IsEnumOrNullableEnum())
                    {
                        List<string> enumvalues = new List<string>();
                        Type enumtype = null;
                        if (pro.PropertyType.IsNullable())
                        {
                            enumtype = pro.PropertyType.GenericTypeArguments[0];
                            enumvalues.Add("null");
                        }
                        else
                        {
                            enumtype = pro.PropertyType;
                        }
                        var vs = Enum.GetValues(enumtype);
                        Random r = new Random();
                        var index = r.Next(0, vs.Length);
                        val = enumtype.FullName+"."+ vs.GetValue(index).ToString();
                    }
                    else if (pro.PropertyType == typeof(string))
                    {
                        var length = pro.GetCustomAttribute<StringLengthAttribute>();
                        var min = 1;
                        var max = 20;
                        var l = 0;
                        if (length != null)
                        {
                            if (length.MaximumLength > 0)
                            {
                                max = length.MaximumLength;
                            }
                            if(length.MinimumLength > 0)
                            {
                                min = length.MinimumLength;
                            }
                        }
                        if(min == max)
                        {
                            l = max;
                        }
                        else if(min < max)
                        {
                            l = new Random().Next(min, max);
                        }
                        Random r = new Random();
                        for (int i = 0; i < l; i++)
                        {
                            int index = r.Next(pat.Length);
                            val += pat[index];
                        }
                        val = "\"" + val + "\"";
                    }
                    else if(pro.PropertyType == typeof(DateTime) || pro.PropertyType == typeof(DateTime?))
                    {
                        Random r = new Random();
                        val = DateTime.Now.AddDays(r.Next(-500, 500)).ToString("yyyy-MM-dd HH:mm:ss");
                        val = $"DateTime.Parse(\"{val}\")";
                    }
                    //如果当前属性名恰好是其他属性名加 id（即外键字段），则用占位符标记
                    if (pros.Where(x => x.Name.ToLower() + "id" == key.ToLower()).Any())
                    {
                        val = "$fk$";
                    }
                    if (val != "")
                    {
                        if (rv.ContainsKey(key) == false)
                        {
                            rv.Add(key, val);
                        }
                    }
                }
            }
            return rv;
        }

        /// <summary>
        /// 根据类型中各属性的特性生成用于测试数据的随机属性值字典，字符串属性支持按正则特性生成，枚举随机取值
        /// </summary>
        /// <param name="self">目标类型</param>
        /// <returns>属性名与随机值字符串的字典</returns>
        public static Dictionary<string, string> GetRandomValuesForTestData(this Type self)
        {
            Dictionary<string, string> rv = new Dictionary<string, string>();
            string pat = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            var pros = self.GetAllProperties();
            List<string> skipFields = new List<string>()
            {
               nameof(TopBasePoco.BatchError),
               nameof(TopBasePoco.Checked),
               nameof(TopBasePoco.ExcelIndex),
               nameof(ITenant.TenantCode)
            };
            if (typeof(IBasePoco).IsAssignableFrom(self))
            {
                skipFields.AddRange(
                    new string[]{
               nameof(IBasePoco.CreateBy),
               nameof(IBasePoco.CreateTime),
               nameof(IBasePoco.UpdateBy),
               nameof(IBasePoco.UpdateTime) }
                    );
            }
            if (typeof(IPersistPoco).IsAssignableFrom(self))
            {
                skipFields.Add(nameof(IPersistPoco.IsValid));
            }
            foreach (var pro in pros)
            {
                string key = pro.Name;
                string val = "";
                var notmapped = pro.GetCustomAttribute<NotMappedAttribute>();
                //读取 Required 特性，用于决定可空字段是否随机生成空值
                var required = pro.GetCustomAttributes<RequiredAttribute>() != null;
                if (notmapped == null &&
                    pro.PropertyType.IsList() == false &&
                    pro.PropertyType.IsSubclassOf(typeof(TopBasePoco)) == false &&
                    skipFields.Contains(key) == false
                    )
                {
                    if (pro.PropertyType.IsNumber())
                    {
                        //主键不生成随机值，由数据库或框架生成
                        if (pro.Name == "ID")
                        {
                            val = "";
                        }
                        else
                        {
                            var range = pro.GetCustomAttribute<RangeAttribute>();
                            int start = 0;
                            int end = 100;
                            if (range != null)
                            {
                                try
                                {
                                    start = (int)Math.Truncate(double.Parse(range.Minimum.ToString()));
                                    end = (int)Math.Truncate(double.Parse(range.Maximum.ToString()));
                                }
                                catch { }
                            }
                            Random r = new Random();
                            val = r.Next(start, end).ToString();
                        }
                    }
                    else if (pro.PropertyType.IsBoolOrNullableBool())
                    {
                        List<string> boolvalues = new List<string> { "true", "false" };
                        if (pro.PropertyType.IsNullable())
                        {
                            if (required == false)
                            {
                                boolvalues.Add("");
                            }
                        }
                        Random r = new Random();
                        var index = r.Next(0, boolvalues.Count);
                        val = boolvalues[index];
                    }
                    else if (pro.PropertyType.IsEnumOrNullableEnum())
                    {
                        List<string> enumvalues = new List<string>();
                        Type enumtype = null;
                        if (pro.PropertyType.IsNullable())
                        {
                            enumtype = pro.PropertyType.GenericTypeArguments[0];
                            if (required == false)
                            {
                                enumvalues.Add("");
                            }
                        }
                        else
                        {
                            enumtype = pro.PropertyType;
                        }
                        var vs = Enum.GetValues(enumtype);
                        foreach (var item in vs)
                        {
                            enumvalues.Add((int)item + "");
                        }
                        Random r = new Random();
                        var index = r.Next(0, enumvalues.Count);
                        val = enumvalues[index];
                    }
                    else if (pro.PropertyType == typeof(string))
                    {

                        //如果设置了正则特性，使用 Xeger 按正则表达式生成随机字符串
                        var reg = pro.GetCustomAttribute<RegularExpressionAttribute>();
                        var length = pro.GetCustomAttribute<StringLengthAttribute>();

                        if (reg != null) {
                            Xeger x = new Xeger(reg.Pattern);
                            val = x.Generate();
                            if(length != null)
                            {
                                if(length.MaximumLength > 0 && val.Length > length.MaximumLength)
                                {
                                    val = val.Substring(0, length.MaximumLength-1);
                                }
                            }
                        }
                        else
                        {
                            var min = 1;
                            var max = 20;
                            var l = 0;
                            if (length != null)
                            {
                                if (length.MaximumLength > 0)
                                {
                                    max = length.MaximumLength;
                                }
                                if (length.MinimumLength > 0)
                                {
                                    min = length.MinimumLength;
                                }
                            }
                            if (min == max)
                            {
                                l = max;
                            }
                            else if (min < max)
                            {
                                l = new Random().Next(min, max);
                            }
                            Random r = new Random();
                            for (int i = 0; i < l; i++)
                            {
                                int index = r.Next(pat.Length);
                                val += pat[index];
                            }
                        }
                        
                        val = "\"" + val + "\"";
                    }
                    else if (pro.PropertyType == typeof(DateTime) || pro.PropertyType == typeof(DateTime?))
                    {
                        Random r = new Random();
                        val = DateTime.Now.AddDays(r.Next(-500, 500)).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (pros.Where(x => x.Name.ToLower() + "id" == key.ToLower()).Any())
                    {
                        val = "$fk$";
                    }
                    if (val != "")
                    {
                        if (rv.ContainsKey(key) == false)
                        {
                            rv.Add(key, val);
                        }
                    }
                }
            }
            return rv;
        }


        /// <summary>
        /// 按名称获取类型的单个属性信息，结果会缓存以减少重复反射
        /// </summary>
        /// <param name="self">Type 类</param>
        /// <param name="name">属性名</param>
        /// <returns>属性信息，未找到时返回 null</returns>
        public static PropertyInfo GetSingleProperty(this Type self, string name)
        {
            //缓存中没有该类型的属性列表时，先反射获取并写入缓存
            if (_propertyCache.ContainsKey(self.FullName) == false)
            {
                var properties = self.GetProperties().ToList();
                try
                {
                    _propertyCache = _propertyCache.Add(self.FullName, properties);
                }
                catch { }
                return properties.Where(x => x.Name == name).FirstOrDefault();
            }
            else
            {
                return _propertyCache[self.FullName].Where(x => x.Name == name).FirstOrDefault();
            }
        }

        /// <summary>
        /// 按条件获取类型的单个属性信息，结果会缓存以减少重复反射
        /// </summary>
        /// <param name="self">Type 类</param>
        /// <param name="where">筛选属性的条件</param>
        /// <returns>符合条件的第一个属性信息，未找到时返回 null</returns>
        public static PropertyInfo GetSingleProperty(this Type self, Func<PropertyInfo,bool> where)
        {
            if (_propertyCache.ContainsKey(self.FullName) == false)
            {
                var properties = self.GetProperties().ToList();
                try
                {
                    _propertyCache = _propertyCache.Add(self.FullName, properties);
                }
                catch { }
                return properties.Where(where).FirstOrDefault();
            }
            else
            {
                return _propertyCache[self.FullName].Where(where).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取类型的所有属性信息，结果会缓存以减少重复反射
        /// </summary>
        /// <param name="self">Type 类</param>
        /// <returns>类型的属性信息列表</returns>
        public static List<PropertyInfo> GetAllProperties(this Type self)
        {
            if (_propertyCache.ContainsKey(self.FullName) == false)
            {
                var properties = self.GetProperties().ToList();
                try
                {
                    _propertyCache = _propertyCache.Add(self.FullName, properties);
                }
                catch
                {
                    if (_propertyCache.ContainsKey(self.FullName) == true)
                    {
                        return _propertyCache[self.FullName];
                    }
                }
                return properties;
            }
            else
            {
                return _propertyCache[self.FullName];
            }
        }

        /// <summary>
        /// 沿基类链向上查找第一个属于系统模型（工作流 Poco）的类型
        /// </summary>
        /// <param name="self">Type 类</param>
        /// <returns>匹配的模型类型，遍历到 object 仍未找到时返回 null</returns>
        public static Type GetParentWorkflowPoco(this Type self)
        {
            if(self == typeof(object))
            {
                return null;
            }
            var ms = Utils.GetAllModels();
            if (ms.Contains(self))
            {
                return self;
            }
            else
            {
                //当前类型不是系统模型时，继续向上查找其基类
                return self.BaseType.GetParentWorkflowPoco();
            }
        }
    }
}
