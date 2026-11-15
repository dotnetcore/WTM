//
// DistributedCacheExtensions.cs
//
// Author:
//       Michael,Vito
//
// Copyright (c) 2019 WTM
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;


namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// 分布式缓存扩展函数，封装基于 IDistributedCache 的对象存取操作，并自动为缓存键添加实例名前缀
    /// </summary>
    public static class DistributedCacheExtensions
    {
        /// <summary>
        /// CachingKey 分割符
        /// </summary>
        private const string SPLIT_CHAR = ":";

        //缓存实例名前缀，懒加载初始化
        private static string _instanceName;
        //缓存键前缀，默认取入口程序集名称，用于隔离不同应用的缓存数据
        private static string InstanceName
        {
            get
            {
                if (_instanceName == null)
                {
                    _instanceName = Assembly.GetEntryAssembly().GetName().Name + SPLIT_CHAR;
                }
                return _instanceName;
            }
        }

        /// <summary>
        /// 设置缓存实例名前缀，后续所有缓存键都将以此开头
        /// </summary>
        /// <param name="cache">分布式缓存实例</param>
        /// <param name="instanceName">实例名前缀</param>
        public static void SetInstanceName(
            this IDistributedCache cache,
            string instanceName)
        {
            _instanceName = instanceName + SPLIT_CHAR;
        }

        #region Get

        /// <summary>
        /// 从缓存中获取指定键的对象并反序列化为目标类型
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="cache">分布式缓存实例</param>
        /// <param name="key">缓存键，不区分大小写</param>
        /// <returns>反序列化后的对象，键不存在时返回类型默认值</returns>
        public static T Get<T>(
            this IDistributedCache cache,
            string key)
        {
            var value = cache.GetString(InstanceName + key.ToLower());
            if (value == null)
                return default;
            else
                return JsonSerializer.Deserialize<T>(value,Core.CoreProgram.DefaultJsonOption);
        }

        /// <summary>
        /// 从缓存中异步获取指定键的对象并反序列化为目标类型
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="cache">分布式缓存实例</param>
        /// <param name="key">缓存键，不区分大小写</param>
        /// <param name="token">取消令牌</param>
        /// <returns>反序列化后的对象，键不存在时返回类型默认值</returns>
        public static async Task<T> GetAsync<T>(
            this IDistributedCache cache,
            string key,
            CancellationToken token = default)
        {
            var value = await cache.GetStringAsync(InstanceName + key.ToLower(), token);
            if (value == null)
                return default;
            else
                return JsonSerializer.Deserialize<T>(value, Core.CoreProgram.DefaultJsonOption);
        }

        /// <summary>
        /// 尝试从缓存中获取指定键的对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="cache">分布式缓存实例</param>
        /// <param name="key">缓存键，不区分大小写</param>
        /// <param name="outValue">获取成功时返回反序列化后的对象，失败时返回类型默认值</param>
        /// <returns>键存在返回 true，否则返回 false</returns>
        public static bool TryGetValue<T>(
            this IDistributedCache cache,
            string key,
            out T outValue)
        {
            var value = cache.GetString(InstanceName + key.ToLower());
            if (value == null)
            {
                outValue = default;
                return false;
            }
            else
            {
                outValue = JsonSerializer.Deserialize<T>(value, Core.CoreProgram.DefaultJsonOption);
                return true;
            }
        }

        #endregion

        #region Set

        /// <summary>
        /// 将对象序列化后写入缓存
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="cache">分布式缓存实例</param>
        /// <param name="key">缓存键，不区分大小写</param>
        /// <param name="value">要缓存的对象</param>
        /// <param name="options">缓存过期等配置，为空且缓存登录用户信息时默认缓存 7 天</param>
        public static void Add<T>(
            this IDistributedCache cache,
            string key,
            T value,
            DistributedCacheEntryOptions options = null)
        {
            //未指定过期配置且缓存的是登录用户信息时，默认设置 7 天绝对过期
            if(options == null && typeof(T) == typeof(LoginUserInfo) ){
                options = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = new System.TimeSpan(7, 0, 0, 0)
                };
            }
            if (options == null)
                cache.Set(InstanceName + key.ToLower(), Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, CoreProgram.DefaultJsonOption)));
            else
                cache.Set(InstanceName + key.ToLower(), Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, CoreProgram.DefaultJsonOption)), options);
        }

        /// <summary>
        /// 将对象序列化后异步写入缓存
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="cache">分布式缓存实例</param>
        /// <param name="key">缓存键，不区分大小写</param>
        /// <param name="value">要缓存的对象</param>
        /// <param name="options">缓存过期等配置，可为空</param>
        /// <param name="token">取消令牌</param>
        public static async Task AddAsync<T>(
            this IDistributedCache cache,
            string key,
            T value,
            DistributedCacheEntryOptions options = null,
            CancellationToken token = default)
        {
            if (options == null)
                await cache.SetAsync(InstanceName + key.ToLower(), Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, CoreProgram.DefaultJsonOption)), token);
            else
                await cache.SetAsync(InstanceName + key.ToLower(), Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, CoreProgram.DefaultJsonOption)), options, token);
        }

        #endregion


        #region Delete

        /// <summary>
        /// 删除指定键的缓存
        /// </summary>
        /// <param name="cache">分布式缓存实例</param>
        /// <param name="key">缓存键，不区分大小写</param>
        public static void Delete(
            this IDistributedCache cache,
            string key)
        {
            cache.Remove(InstanceName + key.ToLower());
        }

        /// <summary>
        /// 异步删除指定键的缓存
        /// </summary>
        /// <param name="cache">分布式缓存实例</param>
        /// <param name="key">缓存键，不区分大小写</param>
        /// <param name="token">取消令牌</param>
        public static async Task DeleteAsync(
            this IDistributedCache cache,
            string key,
            CancellationToken token = default)
        {
            await cache.RemoveAsync(InstanceName + key.ToLower(), token);
        }

        #endregion
    }
}
