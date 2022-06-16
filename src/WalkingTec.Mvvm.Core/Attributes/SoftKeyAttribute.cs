using System;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 标记模型上的字段不能被修改
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SoftKeyAttribute : Attribute
    {
        public string PropertyName { get; set; }
        public SoftKeyAttribute(string proName)
        {
            this.PropertyName = proName;
        }

    }
}
