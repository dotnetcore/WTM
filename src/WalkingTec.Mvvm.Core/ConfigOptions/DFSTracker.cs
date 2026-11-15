namespace WalkingTec.Mvvm.Core
{
    // FastDFS 追踪服务器（Tracker Server）地址配置：Tracker 负责调度，向客户端返回可用的存储（Storage）服务器地址，可配置多台实现负载均衡
    /// <summary>
    /// DFSTracker
    /// </summary>
    public class DFSTracker
    {
        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; }
    }
}
