using System;
using System.IO;

namespace WalkingTec.Mvvm.Core.Models
{
    /// <summary>
    /// WTM 文件接口，定义上传文件的公共属性和行为，
    /// FileAttachment 等文件实体均实现该接口
    /// </summary>
    public interface IWtmFile
    {
        /// <summary>
        /// 文件保存路径或定位信息（具体含义与保存模式相关）
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        string FileName { get; set; }
        /// <summary>
        /// 文件扩展名
        /// </summary>
        string FileExt { get; set; }
        /// <summary>
        /// 文件长度（字节数）
        /// </summary>
        long Length { get; set; }

        /// <summary>
        /// 文件上传时间
        /// </summary>
        DateTime UploadTime { get; set; }

        /// <summary>
        /// 保存模式，标识文件存储的位置或方式（如本地磁盘、数据库等）
        /// </summary>
        string SaveMode { get; set; }
        /// <summary>
        /// 附加信息，存储与文件相关的扩展数据
        /// </summary>
        string ExtraInfo { get; set; }
        /// <summary>
        /// 处理器信息，记录文件上传处理插件的相关配置
        /// </summary>
        string HandlerInfo { get; set; }

        /// <summary>
        /// 文件数据流，用于读取文件内容
        /// </summary>
        Stream DataStream { get; set; }

        /// <summary>
        /// 获取文件实体的主键 ID
        /// </summary>
        /// <returns>文件主键 ID 字符串</returns>
        string GetID();
    }
}
