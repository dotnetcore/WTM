using System;
using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// Type 类型比较器
    /// </summary>
    public class TypeComparer : IEqualityComparer<Type>
    {
        /// <summary>
        /// 比较两个类型是否相同，依据程序集限定名判断
        /// </summary>
        /// <param name="x">第一个类型</param>
        /// <param name="y">第二个类型</param>
        /// <returns>程序集限定名相同返回 true，否则返回 false</returns>
        public bool Equals(Type x, Type y) => x.AssemblyQualifiedName == y.AssemblyQualifiedName;

        /// <summary>
        /// 获取类型的哈希码，本实现未实现该功能，调用会抛出异常
        /// </summary>
        /// <param name="obj">目标类型</param>
        /// <returns>未实现，调用时抛出 NotImplementedException</returns>
        public int GetHashCode(Type obj) => throw new NotImplementedException();
    }
}
