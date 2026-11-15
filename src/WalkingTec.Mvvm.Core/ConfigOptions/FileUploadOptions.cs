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


        /// <summary>
        /// 文件处理设置：键为文件处理模式名（local / oss 等，与各文件处理器的 Display 名称对应），值为该模式下按分组配置的处理选项列表
        /// </summary>
        public Dictionary<string, List<FileHandlerOptions>> Settings { get; set; }

    }

    /// <summary>
    /// 单个文件处理选项，对应 Settings 列表中的一项；local 模式下配置保存目录，oss 模式下配置存储桶与访问密钥等
    /// </summary>
    public class FileHandlerOptions
    {
        /// <summary>
        /// 分组名称，与上传文件时指定的分组（group）匹配；未指定分组时使用列表中的第一项
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 分组位置：local 模式下为文件保存目录（默认 ./uploads），oss 模式下为存储桶（Bucket）名称
        /// </summary>
        public string GroupLocation { get; set; }
        /// <summary>
        /// 文件服务器地址：oss 模式下为对象服务的 Endpoint
        /// </summary>
        public string ServerUrl { get; set; }

        /// <summary>
        /// 访问密钥（如 OSS 的 AccessKeyId）
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 访问秘钥（如 OSS 的 AccessKeySecret）
        /// </summary>
        public string Secret { get; set; }
    }
}
