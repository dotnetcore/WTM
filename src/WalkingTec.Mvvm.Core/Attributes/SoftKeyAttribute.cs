using System;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 标记模型上的字段不能被修改
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SoftKeyAttribute : Attribute
    {
        /// <summary>
        /// 主表中作为软主键的字段名称，子表的软外键引用该字段的值以建立关联
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 新建软主键标记
        /// </summary>
        /// <param name="proName">主表中作为软主键的字段名称</param>
        public SoftKeyAttribute(string proName)
        {
            this.PropertyName = proName;
        }

    }
}
