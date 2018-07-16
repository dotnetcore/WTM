using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;

namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// 字符串辅助类
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 根据名字获取Id形式
        /// </summary>
        /// <param name="fieldName">名字</param>
        /// <returns>将[].转换成_形式的Id</returns>
        public static string GetIdByName(this string fieldName)
        {
            return fieldName == null ? "" : fieldName.Replace(".", "_").Replace("[", "_").Replace("]", "_");
        }

        /// <summary>
        /// 格式化URL
        /// </summary>
        /// <param name="url">初始url</param>
        /// <returns>格式化后的url</returns>
        public static string CorrectUrl(this string url)
        {
            if (string.IsNullOrWhiteSpace(url) == true)
            {
                url = "";
            }
            else
            {
                url = url.ToLower();
                url = url.Trim('/', '\\');
                if (url.StartsWith("http://") == false && url.StartsWith("https://") == false)
                {
                    url = "http://" + url;
                }
            }
            return url;
        }
        /// <summary>
        /// 将数据列表转化为逗号分隔的字符串
        /// </summary>
        /// <typeparam name="T">源数据类</typeparam>
        /// <typeparam name="V">文本字段</typeparam>
        /// <param name="self">源数据List</param>
        /// <param name="textField">要拼接的文本字段</param>
        /// <param name="Format">转化文本字段的表达式</param>
        /// <param name="seperator">分隔符，默认为逗号</param>
        /// <returns>转化后的字符串</returns>
        public static string ToSpratedString<T, V>(this IEnumerable<T> self, Expression<Func<T, V>> textField, Func<V, string> Format = null, string seperator = ",")
        {
            string rv = "";
            if (self == null)
            {
                return rv;
            }
            //循环所有数据
            for (int i = 0; i < self.Count(); i++)
            {
                //获取文本字段的值
                V text = textField.Compile().Invoke(self.ElementAt(i));
                string str = "";
                //如果有转换函数，则调用获取转换后的字符串
                if (Format == null)
                {
                    if (text == null)
                    {
                        str = "";
                    }
                    else
                    {
                        str = text.ToString();
                    }
                }
                else
                {
                    str = Format.Invoke(text);
                }
                rv += str;
                //拼接分隔符
                if (i < self.Count() - 1)
                {
                    rv += seperator;
                }
            }
            //返回转化后的字符串
            return rv;
        }

        public static string ToSpratedString(this IEnumerable self, Func<object, string> Format = null, string seperator = ",")
        {
            string rv = "";
            if (self == null)
            {
                return rv;
            }
            foreach (var item in self)
            {
                if (Format == null)
                {
                    rv += item.ToString() + seperator;
                }
                else
                {
                    rv += Format.Invoke(item) + seperator;
                }
            }
            if (rv.Length > 0)
            {
                rv = rv.Substring(0, rv.Length - 1);
            }
            return rv;
        }

        public static string ToSpratedString(this NameValueCollection self, string seperator = ",")
        {
            string rv = "";
            if (self == null)
            {
                return rv;
            }
            foreach (var item in self)
            {
                rv += item.ToString() + "=" + self[item.ToString()] + seperator;
            }
            if (rv.Length > 0)
            {
                rv = rv.Substring(0, rv.Length - 1);
            }
            return rv;
        }

    }
}
