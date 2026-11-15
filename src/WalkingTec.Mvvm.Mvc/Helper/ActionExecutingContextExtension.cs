using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// ActionExecutingContext 扩展类，用于在 Action 执行前为控制器设置 WTM 上下文
    /// </summary>
    public static class ActionExecutingContextExtension
    {
        /// <summary>
        /// 为 Action 执行上下文中的控制器设置 WTM 上下文：
        /// 从请求服务容器获取 WTMContext，并绑定模型状态服务与会话服务
        /// </summary>
        /// <param name="self">Action 执行上下文</param>
        public static void SetWtmContext(this ActionExecutingContext self)
        {
            var controller = self.Controller as IBaseController;
            // 控制器未实现 IBaseController 接口时无需处理
            if (controller == null)
            {
                return;
            }
            // 控制器尚未持有 WTM 上下文时，从请求的服务容器中获取
            if (controller.Wtm == null)
            {
                controller.Wtm = self.HttpContext.RequestServices.GetRequiredService<WTMContext>();
            }
            // 将 MVC 的 ModelState 与 Session 包装为 WTM 统一的服务接口，异常时静默忽略
            try
            {
                controller.Wtm.MSD = new ModelStateServiceProvider(self.ModelState);
                controller.Wtm.Session = new SessionServiceProvider(self.HttpContext.Session);
            }
            catch { }
        }
    }
}
