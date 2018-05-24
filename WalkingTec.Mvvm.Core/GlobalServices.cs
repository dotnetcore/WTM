using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// GlobalServices
    /// </summary>
    public static class GlobalServices
    {
        private static IServiceProvider _serviceProvider = null;
        /// <summary>
        /// IocContainer
        /// </summary>
        public static IServiceProvider ServiceProvider => _serviceProvider ??
                throw new NullReferenceException("请先设置 DI Container 例 GlobalServices.SetServiceProvider(container)");

        /// <summary>
        /// SetServiceProvider
        /// </summary>
        /// <param name="container"></param>
        public static void SetServiceProvider(IServiceProvider container) { _serviceProvider = container; }

        #region GetServicce

        /// <summary>
        /// GetService
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>() where T : class => ServiceProvider.GetService<T>();

        /// <summary>
        /// GetService
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetService(Type type) => ServiceProvider.GetService(type);

        /// <summary>
        /// GetServices
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetServices<T>() where T : class => ServiceProvider.GetServices<T>();

        /// <summary>
        /// GetServices
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<object> GetServices(Type type) => ServiceProvider.GetServices(type);

        /// <summary>
        /// GetRequiredService
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetRequiredService<T>() where T : class => ServiceProvider.GetRequiredService<T>();

        /// <summary>
        /// GetRequiredService
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetRequiredService(Type type) => ServiceProvider.GetRequiredService(type);

        #endregion
    }
}
