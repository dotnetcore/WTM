using System;
using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// Type 类型比较器
    /// </summary>
    public class TypeComparer : IEqualityComparer<Type>
    {
        public bool Equals(Type x, Type y) => x.AssemblyQualifiedName == y.AssemblyQualifiedName;

        public int GetHashCode(Type obj) => throw new NotImplementedException();
    }
}
