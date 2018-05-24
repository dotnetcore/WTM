using System;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 标记Controller和Action的描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class ActionDescriptionAttribute : Attribute
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 新建一个描述
        /// </summary>
        /// <param name="desc">描述</param>
        public ActionDescriptionAttribute(string desc)
        {
            this.Description = desc;
        }
    }
}