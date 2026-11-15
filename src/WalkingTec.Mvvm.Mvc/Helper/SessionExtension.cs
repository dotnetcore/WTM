using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// ISession 扩展类，提供基于 JSON 序列化的对象存储与读取方法
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// 将对象序列化为 JSON 字符串并写入 Session，随后同步提交会话
        /// </summary>
        /// <typeparam name="T">要存储的对象类型</typeparam>
        /// <param name="session">Session 对象</param>
        /// <param name="key">键名</param>
        /// <param name="value">要存储的值</param>
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
            session.CommitAsync().Wait();
        }

        /// <summary>
        /// 从 Session 中读取指定键的 JSON 字符串并反序列化为对象
        /// </summary>
        /// <typeparam name="T">要读取的对象类型</typeparam>
        /// <param name="session">Session 对象</param>
        /// <param name="key">键名</param>
        /// <returns>反序列化后的对象，键不存在时返回类型默认值</returns>
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) :
                                  JsonSerializer.Deserialize<T>(value);
        }
    }
}
