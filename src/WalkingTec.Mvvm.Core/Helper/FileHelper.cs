using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.FDFS;

namespace WalkingTec.Mvvm.Core
{
    public class FileHelper
    {
        /// <summary>
        /// 通过FileAttachmentVM获取其文件流
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        public static byte[] GetFileByteForDownLoadByVM(FileAttachmentVM vm, Configs con)
        {
            byte[] data = null;
            SaveFileModeEnum saveMode = vm.Entity.SaveFileMode == null ? con.FileUploadOptions.SaveFileMode : vm.Entity.SaveFileMode.Value;
            data = GetBytes(saveMode, vm.Entity, vm.DC);
            return data;
        }

        /// <summary>
        /// 通过FileId获取其文件流
        /// </summary>
        /// <param name="fileid"></param>
        /// <param name="dc"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        public static byte[] GetFileByteForDownLoadById(Guid fileid, IDataContext dc, Configs con)
        {
            byte[] data = null;
            FileAttachment fi = dc.Set<FileAttachment>().Where(x => x.ID == fileid).FirstOrDefault();
            if (fi != null)
            {
                SaveFileModeEnum saveMode = fi.SaveFileMode == null ? con.FileUploadOptions.SaveFileMode : fi.SaveFileMode.Value;
                data = GetBytes(saveMode, fi, dc);
            }
            return data;
        }
        /// <summary>
        /// 通过FileAttachment实体获取其文件流
        /// </summary>
        /// <param name="fi">FileAttachment</param>
        /// <param name="con">con</param>
        /// <returns>byte[]</returns>
        public static byte[] GetFileByteForDownLoadByEntity(FileAttachment fi, Configs con)
        {

            byte[] data = null;
            if (fi != null)
            {
                SaveFileModeEnum saveMode = fi.SaveFileMode == null ? con.FileUploadOptions.SaveFileMode : fi.SaveFileMode.Value;
                data = GetBytes(saveMode, fi, null);
            }
            return data;
        }

        /// <summary>
        /// 上传文件并返回FileAttachmentVM(后台代码使用)
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="FileData"></param>
        /// <param name="con"></param>
        /// <param name="FileName"></param>
        /// <param name="savePlace"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static FileAttachmentVM GetFileByteForUpload(FileAttachmentVM vm, Stream FileData, Configs con, string FileName = null, SaveFileModeEnum? savePlace = null, string groupName = null)
        {
            savePlace = savePlace == null ? con.FileUploadOptions.SaveFileMode : savePlace;
            var ext = string.Empty;
            if (string.IsNullOrEmpty(FileName) == false)
            {
                var dotPos = FileName.LastIndexOf('.');
                ext = FileName.Substring(dotPos + 1);
            }
            vm.Entity.FileName = FileName;
            vm.Entity.FileExt = ext;

            if (savePlace == SaveFileModeEnum.Database)
            {
                using (var dataStream = new MemoryStream())
                {
                    FileData.CopyTo(dataStream);
                    vm.Entity.FileData = dataStream.ToArray();
                }
            }
            else if (savePlace == SaveFileModeEnum.Local)
            {
                string pathHeader = con.FileUploadOptions.UploadDir;
                if (!Directory.Exists(pathHeader))
                {
                    Directory.CreateDirectory(pathHeader);
                }
                var fullPath = Path.Combine(con.FileUploadOptions.UploadDir,$"{Guid.NewGuid().ToNoSplitString()}.{vm.Entity.FileExt}");
                using (var fileStream = File.Create(fullPath))
                {
                    FileData.CopyTo(fileStream);
                    vm.Entity.Path = fullPath;
                    vm.Entity.FileData = null;
                }
            }
            else if (savePlace == SaveFileModeEnum.DFS)
            {
                using (var dataStream = new MemoryStream())
                {
                    StorageNode node = null;
                    FileData.CopyTo(dataStream);

                    if (!string.IsNullOrEmpty(groupName))
                    {
                        node = FDFSClient.GetStorageNode(groupName);
                    }
                    else
                    {
                        node = FDFSClient.GetStorageNode();
                    }

                    if (node != null)
                    {
                        vm.Entity.Path = "/" + FDFSClient.UploadFile(node, dataStream.ToArray(), vm.Entity.FileExt);
                        vm.Entity.GroupName = node.GroupName;
                    }
                    vm.Entity.FileData = null;

                }
                FileData.Dispose();
            }
            return vm;
        }

        /// <summary>
        /// 下载文件 HSSFWorkbook
        /// </summary>
        /// <param name="hssfworkbook"></param>
        /// <param name="fa"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        public static HSSFWorkbook GetHSSWorkbook(HSSFWorkbook hssfworkbook, FileAttachment fa, Configs con)
        {
            var saveMode = fa.SaveFileMode == null ? con.FileUploadOptions.SaveFileMode : fa.SaveFileMode;
            if (saveMode == SaveFileModeEnum.Database)
            {
                using (MemoryStream ms = new MemoryStream(fa.FileData))
                {
                    hssfworkbook = new HSSFWorkbook(ms);
                }
            }
            if (saveMode == SaveFileModeEnum.Local)
            {
                using (FileStream file = new FileStream(fa.Path, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
            }
            if (saveMode == SaveFileModeEnum.DFS)
            {
                using (MemoryStream ms = new MemoryStream(FDFSClient.DownloadFile(fa.GroupName, fa.Path.TrimStart('/'))))
                {
                    hssfworkbook = new HSSFWorkbook(ms);
                }
            }
            return hssfworkbook;
        }

        /// <summary>
        /// 获取附件字节数组
        /// </summary>
        /// <param name="saveMode"></param>
        /// <param name="fa"></param>
        /// <param name="DC"></param>
        /// <returns></returns>
        private static byte[] GetBytes(SaveFileModeEnum saveMode, FileAttachment fa, IDataContext DC)
        {
            byte[] data = null;
            switch (saveMode)
            {
                case SaveFileModeEnum.Local:
                    if (!string.IsNullOrEmpty(fa.Path) && File.Exists(fa.Path))
                    {
                        data = File.ReadAllBytes(fa.Path);
                    }
                    break;
                case SaveFileModeEnum.Database:
                    data = DC.Set<FileAttachment>().Where(x => x.ID == fa.ID).Select(x => x.FileData).FirstOrDefault();
                    break;
                case SaveFileModeEnum.DFS:
                    try
                    {
                        data = FDFSClient.DownloadFile(fa.GroupName, fa.Path.TrimStart('/'));
                    }
                    catch (FDFSException)
                    {
                    }
                    break;
            }
            return data;
        }
    }
}
