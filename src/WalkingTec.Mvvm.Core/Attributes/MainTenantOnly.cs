using System;
using Microsoft.AspNetCore.Authorization;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 标记Action只能由主用户访问，租户用户不能访问
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class MainTenantOnlyAttribute : Attribute
    {
    }
}
