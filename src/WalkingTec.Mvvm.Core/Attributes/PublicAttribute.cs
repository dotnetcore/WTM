using System;
using Microsoft.AspNetCore.Authorization;

namespace WalkingTec.Mvvm.Core
{
    //[Obsolete("已废弃，预计v3.0版本及v2.10版本开始将删除")]
    /// <summary>
    /// 标记Action返回的为公共页面，跳过权限验证，不需要登录即可访问
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class PublicAttribute : Attribute, IAllowAnonymous
    {
    }
}
