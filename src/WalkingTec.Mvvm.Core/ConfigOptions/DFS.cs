using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    // 分布式文件系统（FastDFS）客户端配置，对应 appsettings.json 中文件上传相关配置里的 DFS 节。
    // FastDFS 由 Tracker（追踪服务器）和 Storage（存储服务器）组成：客户端先访问 Tracker 查询可用的 Storage 地址，再向 Storage 上传或下载文件。
    // 下列参数分别控制与 Storage、Tracker 之间的最大连接数、连接超时（毫秒）与连接存活时间（毫秒）。
    /// <summary>
    /// DFS
    /// </summary>
    public class DFS
    {
        /// <summary>
        /// StorageMaxConnection
        /// </summary>
        public int? StorageMaxConnection { get; set; }

        /// <summary>
        /// TrackerMaxConnection
        /// </summary>
        public int? TrackerMaxConnection { get; set; }

        /// <summary>
        /// ConnectionTimeout
        /// </summary>
        public int? ConnectionTimeout { get; set; }

        /// <summary>
        /// ConnectionLifeTime
        /// </summary>
        public int? ConnectionLifeTime { get; set; }

        /// <summary>
        /// Trackers
        /// </summary>
        public List<DFSTracker> Trackers { get; set; }
    }
}
