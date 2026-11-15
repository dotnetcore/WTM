using System.IO;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Core.Support.FileHandlers
{
    /// <summary>
    /// 文件处理器抽象基类：提供上传、读取、删除的默认空实现，实际的存储逻辑（本地磁盘、OSS、数据库等）由派生类按需重写
    /// </summary>
    public abstract class WtmFileHandlerBase : IWtmFileHandler
    {
        /// <summary>
        /// 当前 WTM 上下文
        /// </summary>
        public WTMContext wtm;
        /// <summary>
        /// 初始化文件处理器基类
        /// </summary>
        /// <param name="wtm">当前 WTM 上下文</param>
        public WtmFileHandlerBase(WTMContext wtm)
        {
            this.wtm = wtm;
        }


        /// <summary>
        /// 删除指定附件在存储介质上的文件（默认空实现，由派生类重写）
        /// </summary>
        /// <param name="file">要删除的附件对象</param>
        public virtual void DeleteFile(IWtmFile file)
        {
        }

        /// <summary>
        /// 读取指定附件的内容流（默认返回 null，由派生类重写）
        /// </summary>
        /// <param name="file">要读取的附件对象</param>
        /// <returns>附件内容流；默认返回 null</returns>
        public virtual Stream GetFileData(IWtmFile file)
        {
            return null;
        }

        /// <summary>
        /// 上传文件到存储介质（默认返回空元组表示未实际存储，由派生类重写）
        /// </summary>
        /// <param name="fileName">原始文件名</param>
        /// <param name="fileLength">文件长度（字节）</param>
        /// <param name="data">文件内容流</param>
        /// <param name="group">附件分组名，可为 null</param>
        /// <param name="subdir">存储子目录，可为 null</param>
        /// <param name="extra">附加信息，可为 null</param>
        /// <returns>元组：path 为存储路径，handlerInfo 为处理器附加信息；默认均为空字符串</returns>
        public virtual (string path, string handlerInfo) Upload(string fileName, long fileLength, Stream data, string group=null, string subdir = null, string extra=null)
        {
            return ("", "");
        }

    }
}
