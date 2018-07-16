using System;
using System.Collections.Generic;
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
    }
}
