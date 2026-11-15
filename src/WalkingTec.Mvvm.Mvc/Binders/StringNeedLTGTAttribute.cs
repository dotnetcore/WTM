using System;

namespace WalkingTec.Mvvm.Mvc.Binders
{
    // 标记在 Action 方法上：该 Action 接收的字符串参数将保留客户端提交的尖括号字符，不做 HTML 编码
    /// <summary>
    /// StringNeedLTGT
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class StringNeedLTGTAttribute : Attribute
    {
    }
}
