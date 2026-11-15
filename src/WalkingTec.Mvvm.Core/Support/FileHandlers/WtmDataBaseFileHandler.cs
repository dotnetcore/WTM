using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Core.Support.FileHandlers
{
    /// <summary>
    /// 数据库文件处理器：将附件内容以字节数组形式直接保存到数据库的附件表中，不依赖外部存储介质
    /// </summary>
    [Display(Name = "database")]
    public class WtmDataBaseFileHandler : WtmFileHandlerBase
    {
        // 存储模式名称
        private static string _modeName = "database";

        /// <summary>
        /// 初始化数据库文件处理器
        /// </summary>
        /// <param name="wtm">当前 WTM 上下文</param>
        public WtmDataBaseFileHandler(WTMContext wtm) : base(wtm)
        {
        }

        /// <summary>
        /// 从数据库中读取指定附件的内容
        /// </summary>
        /// <param name="file">要读取的附件对象</param>
        /// <returns>包含附件内容的内存流；上下文、附件 ID 或数据不存在时返回 null</returns>
        public override Stream GetFileData(IWtmFile file)
        {
            try
            {
                var dc = wtm?.DC;
                if (dc == null) return null;
                var id = file?.GetID();
                if (id == null) return null;
                var rv = dc.Set<FileAttachment>().CheckID(id).FirstOrDefault();
                if (rv != null && (rv as FileAttachment)?.FileData != null)
                {
                    return new MemoryStream((rv as FileAttachment).FileData);
                }
                return null;
            }
            // 捕获读取过程中的异常并输出调试信息，读取失败时返回 null 而不中断调用方
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetFileData inner: dc={wtm?.DC != null}, file={file != null}, id={file?.GetID()}, ex={ex}");
                return null;
            }
        }


        /// <summary>
        /// 将文件上传并直接保存到数据库的附件表中
        /// </summary>
        /// <param name="fileName">原始文件名</param>
        /// <param name="fileLength">文件长度（字节）</param>
        /// <param name="data">文件内容流</param>
        /// <param name="groupName">附件分组名，可为 null</param>
        /// <param name="subdir">存储子目录，可为 null（数据库模式不使用）</param>
        /// <param name="extra">附加信息，可为 null</param>
        /// <returns>已持久化到数据库的附件实体</returns>
        public  IWtmFile UploadToDB(string fileName, long fileLength, Stream data, string groupName = null, string subdir = null, string extra = null)
        {
            FileAttachment file = new FileAttachment();
            file.FileName = fileName;
            file.Length = fileLength;
            file.UploadTime = DateTime.Now;
            file.SaveMode = _modeName;
            file.ExtraInfo = extra;
            file.TenantCode = wtm.LoginUserInfo?.CurrentTenant;
            // 提取文件扩展名（不含点，取最后一个点之后的部分）
            var ext = string.Empty;
            if (string.IsNullOrEmpty(fileName) == false)
            {
                var dotPos = fileName.LastIndexOf('.');
                ext = fileName.Substring(dotPos + 1);
            }
            file.FileExt = ext;
            // 将文件流完整读入内存并转为字节数组后存入数据库
            using (var dataStream = new MemoryStream())
            {
                data.CopyTo(dataStream);
                file.FileData = dataStream.ToArray();
            }
            wtm.DC.AddEntity(file);
            wtm.DC.SaveChanges();
            return file;
        }
    }
}
