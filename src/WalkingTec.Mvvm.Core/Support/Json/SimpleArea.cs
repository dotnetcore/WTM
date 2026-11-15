using System;
using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core.Support.Json
{
    /// <summary>
    /// 区域(Area)的精简传输类，保存 MVC 区域的基础信息及其包含的模块列表，
    /// 用于登录用户信息缓存和 API 输出
    /// </summary>
    public class SimpleArea
    {
        /// <summary>
        /// 主键 ID
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 区域路由前缀
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 区域下包含的模块列表
        /// </summary>
        public List<SimpleModule> Modules { get; set; }

    }
}
