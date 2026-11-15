
namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// Cookie 认证配置，对应 appsettings.json 中的 CookieOptions 节，用于配置 Cookie 登录的颁发者、过期时间、滑动过期及登录、注销、无权限跳转路径等
    /// </summary>
    public class CookieOption
    {
        /// <summary>
        /// 颁发者地址，默认 http://localhost
        /// </summary>
        public string Issuer { get; set; } = "http://localhost";
        /// <summary>
        /// 接收者（受众）地址，默认 http://localhost
        /// </summary>
        public string Audience { get; set; } = "http://localhost";
        /// <summary>
        /// Cookie 过期时间，单位秒，默认 3600（1 小时）
        /// </summary>
        public int Expires { get; set; } = 3600;
        /// <summary>
        /// 是否启用滑动过期（用户访问时自动续期），默认 true
        /// </summary>
        public bool SlidingExpiration { get; set; } = true;
        /// <summary>
        /// 登录页面路径，未登录访问受保护资源时跳转到此地址，默认 /Login/Login
        /// </summary>
        public string LoginPath { get; set; } = "/Login/Login";
        /// <summary>
        /// 注销页面路径，默认 /Login/Logout
        /// </summary>
        public string LogoutPath { get; set; } = "/Login/Logout";
        /// <summary>
        /// 无权限（已登录但权限不足）时跳转的页面路径，默认 /Login/Login
        /// </summary>
        public string AccessDeniedPath { get; set; } = "/Login/Login";
        /// <summary>
        /// Cookie 绑定的域名；为空字符串时使用当前请求的域名
        /// </summary>
        public string Domain { get; set; } = "";
        /// <summary>
        /// 登录成功后回跳地址的查询参数名，默认 ReturnUrl
        /// </summary>
        public string ReturnUrlParameter { get; set; } = "ReturnUrl";
    }
}
