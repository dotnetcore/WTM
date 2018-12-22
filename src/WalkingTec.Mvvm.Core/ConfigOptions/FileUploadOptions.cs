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
        public SaveFileModeEnum SaveFileMode { get; set; }

        /// <summary>
        /// 上传目录
        /// </summary>
        public string UploadDir { get; set; }

        /// <summary>
        /// 上传文件限制 单位字节 默认 20 * 1024 * 1024 = 20971520 bytes
        /// </summary>
        public int UploadLimit { get; set; }
    }
}
