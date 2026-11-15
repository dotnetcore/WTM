using System;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 标记Action在提交时仅验证表单内字段（Entity前缀）的验证错误，忽略其他字段的验证错误
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class ValidateFormItemOnlyAttribute : Attribute
    {
    }
}
