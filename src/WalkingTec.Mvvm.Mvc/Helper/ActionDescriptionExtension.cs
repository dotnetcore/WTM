using System;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// ActionDescriptionAttribute 扩展类，提供获取 Action 描述信息（支持多语言本地化）的方法
    /// </summary>
    public static class ActionDescriptionExtension
    {
        /// <summary>
        /// 根据控制器实例获取 Action 的本地化描述信息
        /// </summary>
        /// <param name="self">Action 描述特性实例</param>
        /// <param name="controller">当前控制器实例</param>
        /// <returns>本地化后的描述文本</returns>
        public static string GetDescription(this ActionDescriptionAttribute self, IBaseController controller)
        {
            return self.GetDescription(controller.GetType());
        }

        /// <summary>
        /// 根据控制器类型获取 Action 的本地化描述信息
        /// </summary>
        /// <param name="self">Action 描述特性实例</param>
        /// <param name="controllertype">控制器类型</param>
        /// <returns>本地化后的描述文本；未设置描述时返回空字符串</returns>
        public static string GetDescription(this ActionDescriptionAttribute self, Type controllertype)
        {
            string rv = "";
            if (string.IsNullOrEmpty(self.Description) == false)
            {
                    // 通过全局本地化器根据描述键查找对应语言的文本
                    rv = Core.CoreProgram._localizer?[self.Description];
            }
            return rv;

        }

    }
}
