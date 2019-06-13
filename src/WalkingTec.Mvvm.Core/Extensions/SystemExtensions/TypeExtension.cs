using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// Type的扩展函数
    /// </summary>
    public static class TypeExtension
    {
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
            return self.IsGeneric(typeof(List<>));
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

        public static Dictionary<string,string> GetRandomValues(this Type self)
        {
            Dictionary<string, string> rv = new Dictionary<string, string>();
            string pat = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            var pros = self.GetProperties();
            foreach (var pro in pros)
            {
                string key = pro.Name;
                string val = "";
                var notmapped = pro.GetCustomAttribute<NotMappedAttribute>();
                if (pro.IsPropertyRequired() && notmapped == null && 
                    pro.PropertyType.IsBoolOrNullableBool() == false && 
                    pro.PropertyType.IsEnumOrNullableEnum() == false &&
                    pro.PropertyType.IsList() == false &&
                    pro.PropertyType.IsSubclassOf(typeof(TopBasePoco)) == false)
                {
                    if (pro.PropertyType.IsNumber())
                    {
                        var range = pro.GetCustomAttribute<RangeAttribute>();
                        int start = 0;
                        int end = 100;
                        if (range != null)
                        {
                            start = (int)Math.Truncate(double.Parse(range.Minimum.ToString()));
                            end = (int)Math.Truncate(double.Parse(range.Maximum.ToString()));
                        }
                        Random r = new Random();
                        val = r.Next(start,end).ToString();
                    }
                    else if (pro.PropertyType == typeof(string))
                    {
                        var length = pro.GetCustomAttribute<StringLengthAttribute>();
                        var l = new Random().Next(3,10);
                        if (length != null && l > length.MaximumLength)
                        {
                            l = length.MaximumLength;
                        }
                        Random r = new Random();                        
                        for (int i = 0; i < l; i++)
                        {
                            int index = r.Next(pat.Length);
                            val += pat[index];
                        }
                        val = "\"" + val + "\"";
                    }
                    else if(pro.PropertyType == typeof(Guid) || pro.PropertyType == typeof(Guid?))
                    {
                        if (pros.Where(x => x.Name.ToLower() + "id" == key.ToLower()).Any())
                        {
                            val = "$fk$";
                        }
                    }
                    if (val != "")
                    {
                        rv.Add(key, val);
                    }
                }
            }
            return rv;
        }
    }
}
