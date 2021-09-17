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

        public static bool IsNumber(this Type self)
        {
            Type checktype = self;
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

        public static Dictionary<string, string> GetRandomValues(this Type self)
        {
            Dictionary<string, string> rv = new Dictionary<string, string>();
            string pat = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            var pros = self.GetAllProperties();
            List<string> skipFields = new List<string>()
            {
               nameof(TopBasePoco.BatchError),
               nameof(TopBasePoco.Checked),
               nameof(TopBasePoco.ExcelIndex),
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
                if (notmapped == null &&
                    pro.PropertyType.IsList() == false &&
                    pro.PropertyType.IsSubclassOf(typeof(TopBasePoco)) == false &&
                    skipFields.Contains(key) == false
                    )
                {
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
                var required = pro.GetCustomAttributes<RequiredAttribute>() != null;
                if (notmapped == null &&
                    pro.PropertyType.IsList() == false &&
                    pro.PropertyType.IsSubclassOf(typeof(TopBasePoco)) == false &&
                    skipFields.Contains(key) == false
                    )
                {
                    if (pro.PropertyType.IsNumber())
                    {
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


        public static PropertyInfo GetSingleProperty(this Type self, string name)
        {
            if (_propertyCache.ContainsKey(self.FullName) == false)
            {
                var properties = self.GetProperties().ToList();
                _propertyCache = _propertyCache.Add(self.FullName, properties);
                return properties.Where(x => x.Name == name).FirstOrDefault();
            }
            else
            {
                return _propertyCache[self.FullName].Where(x => x.Name == name).FirstOrDefault();
            }
        }

        public static PropertyInfo GetSingleProperty(this Type self, Func<PropertyInfo,bool> where)
        {
            if (_propertyCache.ContainsKey(self.FullName) == false)
            {
                var properties = self.GetProperties().ToList();
                _propertyCache = _propertyCache.Add(self.FullName, properties);
                return properties.Where(where).FirstOrDefault();
            }
            else
            {
                return _propertyCache[self.FullName].Where(where).FirstOrDefault();
            }
        }

        public static List<PropertyInfo> GetAllProperties(this Type self)
        {
            if (_propertyCache.ContainsKey(self.FullName) == false)
            {
                var properties = self.GetProperties().ToList();
                _propertyCache = _propertyCache.Add(self.FullName, properties);
                return properties;
            }
            else
            {
                return _propertyCache[self.FullName];
            }
        }

    }
}
