using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Aliyun.OSS;
using WalkingTec.Mvvm.Core.ConfigOptions;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Core.Support.FileHandlers
{

    /// <summary>
    /// 阿里云 OSS 文件处理器：将附件上传到对象存储服务，支持按分组配置多个存储位置
    /// </summary>
    [Display(Name = "oss")]
    public class WtmOssFileHandler : WtmFileHandlerBase
    {
        //private static string _modeName = "oss";

        /// <summary>
        /// 初始化 OSS 文件处理器
        /// </summary>
        /// <param name="wtm">当前 WTM 上下文</param>
        public WtmOssFileHandler(WTMContext wtm) : base(wtm)
        {
        }

        /// <summary>
        /// 从阿里云 OSS 下载指定附件的内容
        /// </summary>
        /// <param name="file">要读取的附件对象，其 HandlerInfo 记录了上传时使用的分组名</param>
        /// <returns>包含附件内容的内存流；未找到可用的分组配置时返回 null</returns>
        public override Stream GetFileData(IWtmFile file)
        {
            var ossSettings = wtm.ConfigInfo.FileUploadOptions.Settings.Where(x => x.Key.ToLower() == "oss").Select(x => x.Value).FirstOrDefault();
            // 按附件记录的 HandlerInfo 定位分组配置；未记录或匹配不到时回退到第一个配置
            FileHandlerOptions groupInfo = null;
            if (string.IsNullOrEmpty(file.HandlerInfo))
            {
                groupInfo = ossSettings?.FirstOrDefault();
            }
            else
            {
                groupInfo = ossSettings?.Where(x => x.GroupName.ToLower() == file.HandlerInfo.ToLower()).FirstOrDefault();
                if (groupInfo == null)
                {
                    groupInfo = ossSettings?.FirstOrDefault();
                }
            }
            if (groupInfo == null)
            {
                return null;
            }

            OssClient client = new OssClient(groupInfo.ServerUrl, groupInfo.Key, groupInfo.Secret);
            var rv = new MemoryStream();
            client.GetObject(new GetObjectRequest(groupInfo.GroupLocation, file.Path), rv);
            // 重置流位置，保证调用方能从头读取
            rv.Position = 0;
            return rv;

        }


        /// <summary>
        /// 将文件上传到阿里云 OSS
        /// </summary>
        /// <param name="fileName">原始文件名</param>
        /// <param name="fileLength">文件长度（字节）</param>
        /// <param name="data">文件内容流</param>
        /// <param name="group">附件分组名，用于定位分组对应的 OSS 配置，可为 null</param>
        /// <param name="subdir">存储子目录，可为 null</param>
        /// <param name="extra">附加信息，可为 null</param>
        /// <returns>元组：path 为 OSS 对象路径，handlerInfo 为实际使用的分组名；未找到配置或上传失败时均为 null</returns>
        public override (string path, string handlerInfo) Upload(string fileName, long fileLength, Stream data, string group = null, string subdir = null, string extra = null)
        {
            // 提取文件扩展名（不含点，取最后一个点之后的部分）
            var ext = string.Empty;
            if (string.IsNullOrEmpty(fileName) == false)
            {
                var dotPos = fileName.LastIndexOf('.');
                ext = fileName.Substring(dotPos + 1);
            }

            var ossSettings = wtm.ConfigInfo.FileUploadOptions.Settings.Where(x => x.Key.ToLower() == "oss").Select(x => x.Value).FirstOrDefault();
            // 按分组名定位 OSS 分组配置；未指定或匹配不到时回退到第一个配置
            FileHandlerOptions groupInfo = null;
            if (string.IsNullOrEmpty(group))
            {
                groupInfo = ossSettings?.FirstOrDefault();
            }
            else
            {
                groupInfo = ossSettings?.Where(x => x.GroupName.ToLower() == group.ToLower()).FirstOrDefault();
                if (groupInfo == null)
                {
                    groupInfo = ossSettings?.FirstOrDefault();
                }
            }
            if (groupInfo == null)
            {
                return (null,null);
            }


            // 计算对象存储路径：优先使用显式传入的 subdir，否则调用全局委托 _subDirFunc 动态生成
            string pathHeader = "";
            if (string.IsNullOrEmpty(subdir) == false)
            {
                pathHeader = Path.Combine(pathHeader, subdir);
            }
            else
            {
                var sub = WtmFileProvider._subDirFunc?.Invoke(this);
                if (string.IsNullOrEmpty(sub) == false)
                {
                    pathHeader = Path.Combine(pathHeader, sub);
                }
            }
            // 以不带连字符的 GUID 生成对象名，并将路径分隔符统一为正斜杠以符合 OSS 路径规范
            var fullPath = Path.Combine(pathHeader, $"{Guid.NewGuid().ToNoSplitString()}.{ext}");
            fullPath = fullPath.Replace("\\", "/");
            // 图片类型统一设置 ContentType，便于浏览器直接预览
            var imagetypes = new string[]{ "jpg","jpeg","bmp","tif","gif","png"};
            ObjectMetadata md = new ObjectMetadata();
            ext = ext.ToLower();
            if (imagetypes.Contains(ext))
            {
                md.ContentType = "image/jpg";
            }
            OssClient client = new OssClient(groupInfo.ServerUrl, groupInfo.Key, groupInfo.Secret);
            var result = client.PutObject(groupInfo.GroupLocation, fullPath, data,md);
            // 根据 HTTP 状态码判断上传是否成功
            if (result.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return (fullPath, groupInfo.GroupName);
            }
            else
            {
                return (null, null);
            }
        }

        /// <summary>
        /// 删除 OSS 上存储的附件对象（忽略删除过程中的异常）
        /// </summary>
        /// <param name="file">要删除的附件对象，其 HandlerInfo 记录了上传时使用的分组名</param>
        public override void DeleteFile(IWtmFile file)
        {
            var ossSettings = wtm.ConfigInfo.FileUploadOptions.Settings.Where(x => x.Key.ToLower() == "oss").Select(x => x.Value).FirstOrDefault();
            // 按附件的 HandlerInfo 定位分组配置；未记录或匹配不到时回退到第一个配置
            FileHandlerOptions groupInfo = null;
            if (string.IsNullOrEmpty(file.HandlerInfo))
            {
                groupInfo = ossSettings?.FirstOrDefault();
            }
            else
            {
                groupInfo = ossSettings?.Where(x => x.GroupName.ToLower() == file.HandlerInfo.ToLower()).FirstOrDefault();
                if (groupInfo == null)
                {
                    groupInfo = ossSettings?.FirstOrDefault();
                }
            }
            if (groupInfo == null)
            {
                return;
            }
            try
            {
                OssClient client = new OssClient(groupInfo.ServerUrl, groupInfo.Key, groupInfo.Secret);
                client.DeleteObject(groupInfo.GroupLocation, file.Path);
            }
            catch { }
            return;
        }
    }

}
