using System;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// Agent对话功能配置项，对应appsettings.json的Agent节
    /// </summary>
    public class AgentOption
    {
        /// <summary>
        /// Agent可注册为工具的接口范围：Query=仅查询接口（默认，安全）；All=查询+新增/修改/删除接口
        /// </summary>
        public string ToolScope { get; set; } = "Query";

        /// <summary>
        /// 是否开放全部增删改工具
        /// </summary>
        public bool AllowAllTools => string.Equals(ToolScope, "All", StringComparison.OrdinalIgnoreCase);
    }
}
