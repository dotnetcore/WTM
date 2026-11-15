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
        public static string ToSepratedString<T, V>(this IEnumerable<T> self, Expression<Func<T, V>> textField, Func<V, string> Format = null, string seperator = ",")
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

        /// <summary>
        /// 将非泛型集合转化为分隔符分隔的字符串
        /// </summary>
        /// <param name="self">源数据集合</param>
        /// <param name="Format">转化每个元素的委托函数，可为空</param>
        /// <param name="seperator">分隔符，默认为逗号</param>
        /// <returns>转化后的字符串</returns>
        public static string ToSepratedString(this IEnumerable self, Func<object, string> Format = null, string seperator = ",")
        {
            string rv = "";
            if (self == null)
            {
                return rv;
            }
            //循环所有数据，转换后为空的项不参与拼接
            foreach (var item in self)
            {
                var s = "";
                if (Format == null)
                {
                    s = item?.ToString()??"";
                }
                else
                {
                    s = Format.Invoke(item);
                }
                if(string.IsNullOrEmpty(s) == false)
                {
                    rv += s + seperator;
                }
            }
            if (rv.Length > 0)
            {
                rv = rv.Substring(0, rv.Length - 1);
            }
            return rv;
        }

        /// <summary>
        /// 将 NameValueCollection 转化为 键=值 形式、并以分隔符连接的字符串
        /// </summary>
        /// <param name="self">源数据集合</param>
        /// <param name="seperator">分隔符，默认为逗号</param>
        /// <returns>转化后的字符串</returns>
        public static string ToSepratedString(this NameValueCollection self, string seperator = ",")
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

        /// <summary>
        /// 将查询参数追加到 Url 末尾，Url 已包含查询串时用连接符拼接，否则以问号开头拼接
        /// </summary>
        /// <param name="self">原始 Url</param>
        /// <param name="query">要追加的查询参数</param>
        /// <returns>追加查询参数后的 Url</returns>
        public static string AppendQuery(this string self,string query)
        {
            if(self == null)
            {
                return null;
            }
            if (self.Contains("?"))
            {
                self += "&" + query;
            }
            else
            {
                self += "?" + query;
            }
            return self;
        }

        /// <summary>
        /// 将字典中的键值对转换为查询参数并追加到 Url 末尾
        /// </summary>
        /// <param name="self">原始 Url</param>
        /// <param name="data">键值对集合</param>
        /// <returns>追加查询参数后的 Url</returns>
        public static string AppendQuery(this string self, IDictionary data)
        {
            if (self == null)
            {
                return null;
            }
            string query = "";
            //将键值对拼接为查询参数字符串
            foreach (IDictionaryEnumerator item in data)
            {
                query += item.Key + "=" + item.Value + "&";
            }

            if (self.Contains("?"))
            {
                self += "&" + query;
            }
            else
            {
                self += "?" + query;
            }
            return self;
        }

        /// <summary>
        /// 将键值对列表转换为查询参数并追加到 Url 末尾
        /// </summary>
        /// <param name="self">原始 Url</param>
        /// <param name="data">键值对列表</param>
        /// <returns>追加查询参数后的 Url</returns>
        public static string AppendQuery(this string self, List<KeyValuePair<string, string>> data)
        {
            if (self == null)
            {
                return null;
            }
            string query = "";
            foreach (var item in data)
            {
                query += item.Key + "=" + item.Value + "&";
            }

            if (self.Contains("?"))
            {
                self += "&" + query;
            }
            else
            {
                self += "?" + query;
            }
            return self;
        }

        /// <summary>
        /// 将集合转换为查询参数形式的字符串，集合中所有元素使用同一个参数名
        /// </summary>
        /// <param name="self">源数据集合</param>
        /// <param name="name">参数名，默认为 id</param>
        /// <returns>查询参数形式的字符串</returns>
        public static string ToQueryString(this IEnumerable self, string name=null)
        {
            if(self == null)
            {
                return "";
            }
            //未指定参数名时默认使用 id
            if (string.IsNullOrEmpty(name))
            {
                name = "id";
            }
            string rv = "";
            foreach (var item in self)
            {
                rv += $"{name}={item?.ToString()}&";
            }
            if(rv.Length > 0)
            {
                rv = rv[0..^1];
            }
            return rv;
        }

        /// <summary>
        /// 移除字符串中的换行、制表等特殊控制字符
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <returns>移除特殊字符后的字符串</returns>
        public static string RemoveSpecialChar(this string self)
        {
            if(self == null)
            {
                return "";
            }
            return self.Replace(Environment.NewLine, "").Replace("\t", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\v", string.Empty).Replace("\b", string.Empty);
        }
    }
}
