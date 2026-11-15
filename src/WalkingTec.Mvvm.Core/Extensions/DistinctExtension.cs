using System;
using System.Collections.Generic;
using System.Linq;

namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// Distinct 去重相关的扩展函数
    /// </summary>
    public static class DistinctExtensions
    {
        /// <summary>
        /// 根据指定键对序列进行去重
        /// </summary>
        /// <typeparam name="T">源数据类型</typeparam>
        /// <typeparam name="V">去重键的类型</typeparam>
        /// <param name="source">源数据序列</param>
        /// <param name="keySelector">从元素中选取去重键的函数</param>
        /// <returns>按指定键去重后的序列</returns>
        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector)
        {
            return source.Distinct(new CommonEqualityComparer<T, V>(keySelector));
        }
    }
}
