using System.IO;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Core.Support.FileHandlers
{
    /// <summary>
    /// 文件处理器接口：框架附件存储的抽象契约，由本地磁盘、OSS、数据库等具体实现完成文件的上传、读取与删除
    /// </summary>
    public interface IWtmFileHandler
    {
        /// <summary>
        /// 上传文件到存储介质
        /// </summary>
        /// <param name="fileName">原始文件名</param>
        /// <param name="fileLength">文件长度（字节）</param>
        /// <param name="data">文件内容流</param>
        /// <param name="group">附件分组名，用于定位对应的存储配置，可为 null</param>
        /// <param name="subdir">存储子目录，可为 null</param>
        /// <param name="extra">附加信息，可为 null</param>
        /// <returns>元组：path 为文件存储路径，handlerInfo 为处理器附加信息（如 OSS 分组名）</returns>
        (string path,string handlerInfo) Upload(string fileName, long fileLength, Stream data, string group=null, string subdir=null, string extra=null);
        /// <summary>
        /// 读取指定附件的内容流
        /// </summary>
        /// <param name="file">要读取的附件对象</param>
        /// <returns>附件内容的流；无法获取时返回 null</returns>
        Stream GetFileData(IWtmFile file);

        /// <summary>
        /// 删除指定附件在存储介质上的文件
        /// </summary>
        /// <param name="file">要删除的附件对象</param>
        void DeleteFile(IWtmFile file);

    }
}
