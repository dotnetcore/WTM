using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text.Json.Serialization;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FileAttachment
    /// </summary>
    [Table("FileAttachments")]
    public class FileAttachment : TopBasePoco, IWtmFile,ITenant, IDisposable
    {
        /// <summary>
        /// 上传文件的原始文件名
        /// </summary>
        [Display(Name = "_Admin.FileName")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public string FileName { get; set; }

        /// <summary>
        /// 文件扩展名（不含点号，如 jpg、pdf）
        /// </summary>
        [Display(Name = "_Admin.FileExt")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(10)]
        public string FileExt { get; set; }

        /// <summary>
        /// 文件存储路径，采用外部文件存储方式时使用
        /// </summary>
        [Display(Name = "_Admin.Path")]
        public string Path { get; set; }

        /// <summary>
        /// 文件长度（字节）
        /// </summary>
        [Display(Name = "_Admin.Length")]
        public long Length { get; set; }

        /// <summary>
        /// 文件上传时间
        /// </summary>
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 文件保存方式（如数据库、物理路径、远程存储等）
        /// </summary>
        public string SaveMode { get; set; }

        /// <summary>
        /// 文件的二进制内容，采用数据库存储方式时使用
        /// </summary>
        public byte[] FileData { get; set; }

        /// <summary>
        /// 附加信息（JSON），存储与文件相关的扩展数据，如所属业务的主键等
        /// </summary>
        public string ExtraInfo { get; set; }
        /// <summary>
        /// 处理器信息，记录文件关联的后台处理程序配置（如图片压缩等）
        /// </summary>
        public string HandlerInfo { get; set; }

        /// <summary>
        /// 租户编码，标识附件所属的租户
        /// </summary>
        [Display(Name = "_Admin.Tenant")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TenantCode { get; set; }

        /// <summary>
        /// 文件的流式数据，不映射到数据库，用于以流的方式读取文件内容
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public Stream DataStream { get; set; }

        /// <summary>
        /// 释放 DataStream 持有的流资源
        /// </summary>
        public void Dispose()
        {
            if(DataStream != null)
            {
                DataStream.Dispose();
            }
        }

        /// <summary>
        /// 获取文件附件ID的字符串形式（IWtmFile 接口显式实现）
        /// </summary>
        /// <returns>文件附件ID的字符串形式</returns>
        string IWtmFile.GetID()
        {
            return ID.ToString();
        }
    }
}
