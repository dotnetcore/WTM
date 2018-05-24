using System;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 标记Controller或Action不受权限控制，只要登录任何人都可访问
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AllRightsAttribute : Attribute
    {
    }
}