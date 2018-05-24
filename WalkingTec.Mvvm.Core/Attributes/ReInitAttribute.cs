using System;

namespace WalkingTec.Mvvm.Core
{
    public enum ReInitModes { FAILEDONLY, SUCCESSONLY, ALWAYS }

    /// <summary>
    /// 标记VM中的ReInit方法是在提交错误时触发，提交成功时触发，或是都触发。这是为了一些特殊逻辑的VM设计的
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ReInitAttribute : Attribute
    {
        /// <summary>
        /// 触发模式
        /// </summary>
        public ReInitModes ReInitMode { get; set; }

        /// <summary>
        /// 新建触发标记
        /// </summary>
        /// <param name="mode">触发模式</param>
        public ReInitAttribute(ReInitModes mode)
        {
            this.ReInitMode = mode;
        }
    }
}