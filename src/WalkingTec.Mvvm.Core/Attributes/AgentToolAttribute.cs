using System;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// Agent工具标记特性：
    /// 标记在动作上 [AgentTool(true)] 把非标准的自定义接口也注册为Agent可调用的工具；
    /// 标记 [AgentTool(false)] 把默认会被注册的接口（如标准查询/增删改）排除出工具列表；
    /// 也可标记在控制器类上作为该控制器全部动作的默认开关
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AgentToolAttribute : Attribute
    {
        /// <summary>
        /// 是否注册为Agent工具
        /// </summary>
        public bool Enable { get; }

        /// <summary>
        /// 自定义工具描述（可选，不填时取接口的ActionDescription或动作名）
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 构造Agent工具标记
        /// </summary>
        /// <param name="enable">true=注册为工具；false=排除出工具列表</param>
        public AgentToolAttribute(bool enable)
        {
            Enable = enable;
        }
    }
}
