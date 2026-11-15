using System;
using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 通用相等比较器，通过指定的键选择器函数提取比较键，并按该键比较两个对象是否相等
    /// </summary>
    /// <typeparam name="T">待比较的对象类型</typeparam>
    /// <typeparam name="V">比较键的类型</typeparam>
    public class CommonEqualityComparer<T, V> : IEqualityComparer<T>
    {
        //键选择器函数，用于从对象中提取比较键
        private Func<T, V> keySelector;

        /// <summary>
        /// 使用指定的键选择器构造通用相等比较器
        /// </summary>
        /// <param name="keySelector">从对象中提取比较键的函数</param>
        public CommonEqualityComparer(Func<T, V> keySelector)
        {
            this.keySelector = keySelector;
        }

        /// <summary>
        /// 比较两个对象的比较键是否相等
        /// </summary>
        /// <param name="x">第一个对象</param>
        /// <param name="y">第二个对象</param>
        /// <returns>比较键相等返回true，否则返回false</returns>
        public bool Equals(T x, T y)
        {
            return EqualityComparer<V>.Default.Equals(keySelector(x), keySelector(y));
        }

        /// <summary>
        /// 获取对象比较键的哈希码
        /// </summary>
        /// <param name="obj">待计算哈希码的对象</param>
        /// <returns>比较键的哈希码</returns>
        public int GetHashCode(T obj)
        {
            return EqualityComparer<V>.Default.GetHashCode(keySelector(obj));
        }
    }
}
