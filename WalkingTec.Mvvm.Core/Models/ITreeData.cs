using System;
using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 树形结构model接口
    /// </summary>
    public interface ITreeData
    {
        /// <summary>
        /// 父节点Id
        /// </summary>
        Guid? ParentId { get; set; }
    }

    /// <summary>
    /// 泛型树形结构model接口
    /// </summary>
    /// <typeparam name="T">父节点类型，父节点应该和实现本接口的类是同一个类，否则没有意义</typeparam>
    public interface ITreeData<T> : ITreeData where T : TopBasePoco
    {
        /// <summary>
        /// 通过实现这个函数获得所有的子节点数据
        /// </summary>
        /// <returns>所有子节点数据</returns>
        List<T> Children { get; }

        /// <summary>
        /// 获取父节点
        /// </summary>
        T Parent { get; }
    }
}
