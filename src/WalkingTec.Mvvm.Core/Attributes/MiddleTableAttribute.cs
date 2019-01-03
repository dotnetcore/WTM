using System;
using System.Collections.Generic;
using System.Text;

namespace WalkingTec.Mvvm.Core.Attributes
{

    /// <summary>
    /// 标记某个Model是一个多对多的中间表
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MiddleTableAttribute : Attribute
    {
    }
}
