using System;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 标记Action返回的为公共页面，跳过权限验证，不需要登录即可访问
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class PublicAttribute : Attribute
    {
    }
}