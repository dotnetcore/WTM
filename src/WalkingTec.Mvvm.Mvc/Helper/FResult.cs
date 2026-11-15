using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// LayUI 前端脚本结果封装，继承自 ContentResult，
    /// 通过 StringBuilder 累积待输出到前端执行的 JavaScript 脚本
    /// </summary>
    public class FResult : ContentResult
    {
        /// <summary>
        /// 脚本构造器，累积最终输出到前端的 JavaScript 脚本内容
        /// </summary>
        public StringBuilder ContentBuilder { get; set; }
        /// <summary>
        /// 关联的控制器实例，用于获取当前窗口标识等信息
        /// </summary>
        public BaseController Controller { get; set; }

        /// <summary>
        /// 无参构造函数，初始化为空脚本内容
        /// </summary>
        public FResult() : this(string.Empty) { }

        /// <summary>
        /// 构造函数，以指定内容初始化脚本构造器
        /// </summary>
        /// <param name="content">初始脚本内容</param>
        public FResult(string content)
        {
            ContentBuilder = new StringBuilder(content);
        }

        /// <summary>
        /// 执行结果输出，将累积的脚本内容写入 Content 后响应给前端
        /// </summary>
        /// <param name="context">Action 执行上下文</param>
        /// <returns>异步执行任务</returns>
        public override Task ExecuteResultAsync(ActionContext context)
        {
            // 将构造器中累积的脚本赋值给 Content，交由基类输出
            Content = ContentBuilder.ToString();
            return base.ExecuteResultAsync(context);
        }
    }
}
