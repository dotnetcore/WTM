using System;
using Microsoft.Extensions.Localization;

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
        /// 页面所属类的全名，用于在菜单和权限体系中定位该Action所属的模块
        /// </summary>
        public string ClassFullName { get; set; }
        /// <summary>
        /// 标识该Action是否为页面，生成菜单时框架据此筛选出可作为菜单入口的Action
        /// </summary>
        public bool IsPage { get; set; }
        /// <summary>
        /// 多语言本地化实例，用于将描述文本翻译为当前语言
        /// </summary>
        public IStringLocalizer _localizer { get; set; }
        /// <summary>
        /// 新建一个描述
        /// </summary>
        public ActionDescriptionAttribute(string desc)
        {
            this.Description = desc;
        }

        /// <summary>
        /// 新建一个描述，并指定页面所属类的全名
        /// </summary>
        /// <param name="desc">描述文本</param>
        /// <param name="classFullName">页面所属类的全名</param>
        public ActionDescriptionAttribute(string desc, string classFullName)
        {
            this.Description = desc;
            this.ClassFullName = classFullName;
        }

        /// <summary>
        /// 新建一个描述，并指定页面所属类的全名以及是否为页面
        /// </summary>
        /// <param name="desc">描述文本</param>
        /// <param name="classFullName">页面所属类的全名</param>
        /// <param name="ispage">是否为页面</param>
        public ActionDescriptionAttribute(string desc, string classFullName, bool ispage)
        {
            this.Description = desc;
            this.ClassFullName = classFullName;
            this.IsPage = ispage;
        }

        /// <summary>
        /// 设置多语言本地化实例，供后续读取描述时进行多语言翻译
        /// </summary>
        /// <param name="controllertype">控制器类型（当前实现未使用该参数，直接使用全局本地化实例）</param>
        public void SetLoccalizer(Type controllertype)
        {
            _localizer = Core.CoreProgram._localizer;
        }
    }
}
