using System;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 标记Controller或Action仅在快速调试模式下可访问；非调试模式访问时会被权限过滤器拒绝
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class DebugOnlyAttribute : Attribute
    {
    }
}
