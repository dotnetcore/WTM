namespace WalkingTec.Mvvm.Core.ConfigOptions
{
    /// <summary>
    /// 域名配置，对应 appsettings.json 中 Domains 节下的一条配置；用于多域名部署时区分系统对外的访问地址和微服务间的内网调用地址，其中键为 mainhost 的节点保留为主域名
    /// </summary>
    public class Domain
    {
        /// <summary>
        /// 域名标识（Domains 字典的键，配置绑定时自动回填为小写）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 对外（外网）访问地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 对内（内网）访问地址，微服务内部互相调用时使用
        /// </summary>
        public string InnerAddress { get; set; }

        /// <summary>
        /// 入口地址，访问该域名时跳转到的目标 URL
        /// </summary>
        public string EntryUrl { get; set; }

        /// <summary>
        /// 外网完整地址；获取时若 Address 未携带 http:// 或 https:// 前缀则自动补充 http://
        /// </summary>
        public string Url
        {
            get
            {
                // 地址未以 http:// 或 https:// 开头时，自动补充 http:// 前缀
                var rv = Address;
                if (string.IsNullOrEmpty(rv) == false && rv.ToLower().StartsWith("http://") == false && rv.ToLower().StartsWith("https://") == false)
                {
                    rv = "http://" + rv;
                }
                return rv;
            }
        }

        /// <summary>
        /// 内网完整地址；获取时若 InnerAddress 未携带 http:// 或 https:// 前缀则自动补充 http://
        /// </summary>
        public string InnerUrl
        {
            get
            {
                // 地址未以 http:// 或 https:// 开头时，自动补充 http:// 前缀
                var rv = InnerAddress;
                if (string.IsNullOrEmpty(rv) == false && rv.ToLower().StartsWith("http://") == false && rv.ToLower().StartsWith("https://") == false)
                {
                    rv = "http://" + rv;
                }
                return rv;
            }
        }

    }
}
