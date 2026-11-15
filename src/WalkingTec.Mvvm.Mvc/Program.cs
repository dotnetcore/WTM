using Microsoft.Extensions.Localization;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// Mvc 层的程序基础类，承载框架 Mvc 层全局共享的静态对象
    /// </summary>
    public class MvcProgram
    {
        /// <summary>
        /// Mvc 层使用的多语言资源访问器，引用 Core 层初始化的全局多语言对象
        /// </summary>
        public static IStringLocalizer _localizer = Core.CoreProgram._localizer;

    }
}
