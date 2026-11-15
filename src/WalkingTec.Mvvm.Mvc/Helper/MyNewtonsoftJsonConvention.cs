using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using WalkingTec.Mvvm.Mvc.Binders;

namespace WalkingTec.Mvvm.Mvc.Helper
{
    /// <summary>
    /// 控制器模型约定：为 Elsa.Server.Api 命名空间下的控制器统一应用 Newtonsoft.Json 输出格式化器，
    /// 解决 Elsa 工作流 API 与 WTM 默认 System.Text.Json 序列化行为不一致的问题
    /// </summary>
    public class MyNewtonsoftJsonConvention : IControllerModelConvention
    {
        private readonly Assembly _sharedAssembly;

        /// <summary>
        /// 使用指定的共享程序集构造约定实例
        /// </summary>
        /// <param name="sharedAssembly">共享程序集</param>
        public MyNewtonsoftJsonConvention(Assembly sharedAssembly)
        {
            _sharedAssembly = sharedAssembly;
        }

        /// <summary>
        /// 应用约定：为符合条件的控制器挂载 Newtonsoft.Json 格式化器特性，
        /// 使其请求和响应均按 Newtonsoft.Json 方式处理
        /// </summary>
        /// <param name="controller">当前控制器模型</param>
        public void Apply(ControllerModel controller)
        {
            if (ShouldApplyConvention(controller))
            {
                var formatterAttribute = new NewtonsoftJsonFormatterAttribute();

                // The attribute itself also implements IControllerModelConvention so we have to call that one as well.
                // This way, the NewtonsoftJsonBodyModelBinder will be properly connected to the controller actions.
                formatterAttribute.Apply(controller);

                controller.Filters.Add(formatterAttribute);
            }
        }

        /// <summary>
        /// 判断是否需要对指定控制器应用本约定：
        /// 仅对 Elsa.Server.Api 前缀且未显式标记 NewtonsoftJsonFormatterAttribute 的控制器生效
        /// </summary>
        private bool ShouldApplyConvention(ControllerModel controller)
        {
            return controller.ControllerType.FullName.StartsWith("Elsa.Server.Api") &&
                !controller.Attributes.Any(x => x.GetType() == typeof(NewtonsoftJsonFormatterAttribute));
        }
    }
}
