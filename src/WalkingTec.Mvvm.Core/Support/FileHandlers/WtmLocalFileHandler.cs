using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Core.Support.FileHandlers
{

    /// <summary>
    /// 本地磁盘文件处理器：将附件保存到服务器本地文件系统，存储目录可在上传配置中按分组指定
    /// </summary>
    [Display(Name = "local")]
    public class WtmLocalFileHandler : WtmFileHandlerBase
    {

        /// <summary>
        /// 初始化本地文件处理器
        /// </summary>
        /// <param name="wtm">当前 WTM 上下文</param>
        public WtmLocalFileHandler(WTMContext wtm) : base(wtm)
        {
        }

        /// <summary>
        /// 读取本地存储的附件文件
        /// </summary>
        /// <param name="file">要读取的附件对象</param>
        /// <returns>以只读方式打开的文件流</returns>
        public override Stream GetFileData(IWtmFile file)
        {
            return File.OpenRead(GetFullPath(file.Path));          
        }


        /// <summary>
        /// 将文件上传到本地磁盘
        /// </summary>
        /// <param name="fileName">原始文件名</param>
        /// <param name="fileLength">文件长度（字节）</param>
        /// <param name="data">文件内容流</param>
        /// <param name="group">附件分组名，用于定位分组对应的存储目录，可为 null</param>
        /// <param name="subdir">存储子目录，可为 null</param>
        /// <param name="extra">附加信息，可为 null</param>
        /// <returns>元组：path 为文件的存储路径（相对于站点根目录），handlerInfo 为空字符串</returns>
        public override (string path, string handlerInfo) Upload(string fileName, long fileLength, Stream data, string group = null, string subdir = null, string extra = null)
        {
            // 读取 local 模式的上传配置
            var localSettings = wtm.ConfigInfo.FileUploadOptions.Settings.Where(x => x.Key.ToLower() == "local").Select(x => x.Value).FirstOrDefault();

            // 根据分组名定位分组对应的存储目录；未指定分组时使用第一个配置
            var groupdir = "";
            if (string.IsNullOrEmpty(group))
            {
                groupdir = localSettings?.FirstOrDefault().GroupLocation;
            }
            else {
               groupdir = localSettings?.Where(x => x.GroupName.ToLower() == group.ToLower()).FirstOrDefault().GroupLocation;
            }
            if (string.IsNullOrEmpty(groupdir))
            {
                // 未配置任何目录时，回退到默认目录 ./uploads
                groupdir = "./uploads";
            }
            // 拼接子目录：优先使用显式传入的 subdir，否则调用全局委托 _subDirFunc 动态生成
            string pathHeader = groupdir;
            if (string.IsNullOrEmpty(subdir) == false)
            {
                pathHeader = Path.Combine(pathHeader, subdir);
            }
            else
            {
                var sub = WtmFileProvider._subDirFunc?.Invoke(this);
                if(string.IsNullOrEmpty(sub)== false)
                {
                    pathHeader = Path.Combine(pathHeader, sub);
                }
            }
            string fulldir = GetFullPath(pathHeader);
            // 目录不存在时自动创建
            if (!Directory.Exists(fulldir))
            {
                Directory.CreateDirectory(fulldir);
            }
            // 提取文件扩展名（不含点），并使用不带连字符的 GUID 生成新文件名，避免同名覆盖
            var ext = string.Empty;
            if (string.IsNullOrEmpty(fileName) == false)
            {
                var dotPos = fileName.LastIndexOf('.');
                ext = fileName.Substring(dotPos + 1);
            }
            var filename = $"{Guid.NewGuid().ToNoSplitString()}.{ext}";
            var fullPath = Path.Combine(fulldir, filename);
            using (var fileStream = File.Create(fullPath))
            {
                data.CopyTo(fileStream);
            }
            data.Dispose();
            return (Path.Combine(pathHeader, filename),"");
        }

        /// <summary>
        /// 删除本地磁盘上的附件文件（忽略删除过程中的异常）
        /// </summary>
        /// <param name="file">要删除的附件对象</param>
        public override void DeleteFile(IWtmFile file)
        {
            if (string.IsNullOrEmpty(file?.Path) == false)
            {
                try
                {
                    File.Delete(GetFullPath(file?.Path));
                }
                catch { }
            }
        }

        /// <summary>
        /// 将相对路径转换为基于站点根目录的绝对路径
        /// </summary>
        /// <param name="path">原始路径；以点开头的相对路径会拼接到 HostRoot 之下</param>
        /// <returns>规范化后的绝对路径</returns>
        private string GetFullPath(string path)
        {
            string rv = "";
            if (path.StartsWith("."))
            {
                rv = Path.Combine(wtm.ConfigInfo.HostRoot, path);
            }
            else
            {
                rv = path;
            }
            rv = Path.GetFullPath(rv);
            return rv ;
        }
    }

}
