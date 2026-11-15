using System;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 标记模型上的字段不能被修改
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SoftFKAttribute : Attribute
    {
        /// <summary>
        /// 子表中作为软外键的字段名称，框架保存时将主表的软主键值写入子表的该字段
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 新建软外键标记
        /// </summary>
        /// <param name="proName">子表中作为软外键的字段名称</param>
        public SoftFKAttribute(string proName)
        {
            this.PropertyName = proName;
        }

    }
}
