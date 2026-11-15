using System;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 标记Controller或Action不记录操作日志，日志过滤器对标记的请求不再写入ActionLog
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class NoLogAttribute : Attribute
    {
    }
}
