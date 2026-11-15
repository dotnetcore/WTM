using System.Collections.Generic;
using System.Text.Json;

namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// 字典相关的扩展函数
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// 根据键获取字典中的值并转换为目标类型，键不存在时返回类型默认值
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="self">源字典</param>
        /// <param name="key">要获取的键</param>
        /// <returns>转换后的值，键不存在时返回类型默认值</returns>
        public static T GetValue<T>(this Dictionary<string,object> self, string key)
        {
            if (self.ContainsKey(key))
            {
                object v = self[key];
                //如果值是 JsonElement（来自 Json 配置的反序列化），先转成字符串再进行类型转换
                if(v is JsonElement j)
                {
                    v = j.ToString();
                }
                var cv = v.ConvertValue(typeof(T));
                if (cv == null)
                {
                    return default(T);
                }
                else
                {
                    return (T)cv;
                }
            }
            else
            {
                return default(T);
            }
        }
            
    }
}
