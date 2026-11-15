using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core.Support
{
    /// <summary>
    /// NuGet 包信息查询结果，对应 NuGet 服务接口返回的 JSON 结构
    /// </summary>
    public class NugetInfo
    {
        /// <summary>
        /// 包信息数据列表
        /// </summary>
        public List<NugetInfoData> data { get; set; }
    }

    /// <summary>
    /// NuGet 包版本信息数据
    /// </summary>
    public class NugetInfoData
    {
        /// <summary>
        /// 包的最新版本号
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// 包的历史版本列表
        /// </summary>
        public List<NugetVersionData> versions {get;set;}
    }

    /// <summary>
    /// NuGet 包单条版本号数据
    /// </summary>
    public class NugetVersionData
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string version { get; set; }
    }
}
