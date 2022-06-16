using System;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 标记模型上的字段不能被修改
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SoftFKAttribute : Attribute
    {
        public string PropertyName { get; set; }
        public SoftFKAttribute(string proName)
        {
            this.PropertyName = proName;
        }

    }
}
