using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core.ConfigOptions
{
    /// <summary>
    /// FileOptions
    /// </summary>
    public class FileUploadOptions
    {
        /// <summary>
        /// 文件保存位置
        /// </summary>
        public string SaveFileMode { get; set; }

        /// <summary>
        /// 上传文件限制 单位字节 默认 20 * 1024 * 1024 = 20971520 bytes
        /// </summary>
        public long UploadLimit { get; set; } = 20971520;


        public Dictionary<string, List<FileHandlerOptions>> Settings { get; set; }

    }

    public class FileHandlerOptions
    {
        public string GroupName { get; set; }
        public string GroupLocation { get; set; }
        public string ServerUrl { get; set; }

        public string Key { get; set; }
        public string Secret { get; set; }
    }
}
