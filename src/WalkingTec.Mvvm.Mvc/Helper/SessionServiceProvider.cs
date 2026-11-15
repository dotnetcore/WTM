using Microsoft.AspNetCore.Http;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// 基于原生 ISession 实现的会话服务提供程序，向框架暴露统一的键值存取接口
    /// </summary>
    public class SessionServiceProvider : ISessionService
    {
        private ISession _inner { get; set; }

        /// <summary>
        /// 使用指定的 Session 构造服务实例
        /// </summary>
        /// <param name="s">被包装的原生 Session 对象</param>
        public SessionServiceProvider(ISession s)
        {
            _inner = s;
        }
        /// <summary>
        /// 从会话中读取指定键的值并反序列化为对象
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="key">键名</param>
        /// <returns>读取到的值</returns>
        public T Get<T>(string key)
        {
            return _inner.Get<T>(key);
        }
        /// <summary>
        /// 将指定值序列化后写入会话
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="key">键名</param>
        /// <param name="val">要写入的值</param>
        public void Set<T>(string key, T val)
        {
            _inner.Set<T>(key, val);
        }

        /// <summary>
        /// 获取当前会话 ID
        /// </summary>
        public string SessionId
        {
            get
            {
                return _inner.Id;
            }
        }
    }
}
