using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 跨域（CORS）配置，对应 appsettings.json 中的 CorsOptions 节
    /// </summary>
    public class Cors
    {
        /// <summary>
        /// 是否启用跨域，默认 false；为 true 时应用启动会启用 CORS 中间件（未配置 Policy 时使用允许所有来源的默认策略，已配置时使用第一条策略）
        /// </summary>
        public bool EnableAll { get; set; }
        /// <summary>
        /// 跨域策略列表；应用启动时按每项的名称和域名逐条注册 CORS 策略，未配置时框架注册允许所有来源的默认策略
        /// </summary>
        public List<CorsPolicy> Policy { get; set; }
    }

    /// <summary>
    /// 单条跨域策略配置（策略名称 + 允许的来源域名）
    /// </summary>
    public class CorsPolicy
    {
        /// <summary>
        /// 策略名称，应用通过 UseCors(名称) 启用对应策略
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 该策略允许的来源域名，多个域名用逗号分隔
        /// </summary>
        public string Domain { get; set; }
    }
}
