using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using Aliyun.OSS;
using WalkingTec.Mvvm.Core.ConfigOptions;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Core.Support.FileHandlers
{

    [Display(Name = "oss")]
    public class WtmOssFileHandler : WtmFileHandlerBase
    {
        private static string _modeName = "oss";

        public WtmOssFileHandler(Configs config, string csName) : base(config, csName)
        {
        }

        public override IWtmFile GetFile(string id, bool withData)
        {
            IWtmFile rv;
            using (var dc = _config.CreateDC(_cs))
            {
                rv = dc.Set<FileAttachment>().CheckID(id).Where(x => x.SaveMode == _modeName).FirstOrDefault();
            }
            if(withData == true && rv != null)
            {
                var ossSettings = _config.FileUploadOptions.Settings.Where(x => x.Key.ToLower() == "oss").Select(x => x.Value).FirstOrDefault();
                FileHandlerOptions groupInfo = null;
                if (string.IsNullOrEmpty(rv.ExtraInfo))
                {
                    groupInfo = ossSettings?.FirstOrDefault();
                }
                else
                {
                    groupInfo = ossSettings?.Where(x => x.GroupName.ToLower() == rv.ExtraInfo.ToLower()).FirstOrDefault();
                    if (groupInfo == null)
                    {
                        groupInfo = ossSettings?.FirstOrDefault();
                    }
                }
                if (groupInfo == null)
                {
                    return rv;
                }

                OssClient client = new OssClient(groupInfo.ServerUrl, groupInfo.Key, groupInfo.Secret);
                rv.DataStream = new MemoryStream();
                client.GetObject(new GetObjectRequest(groupInfo.GroupLocation, rv.Path), rv.DataStream);
                rv.DataStream.Position = 0;
            }
            return rv;

        }


        public override IWtmFile Upload(string fileName, long fileLength, Stream data, string group = null, string subdir = null, string extra = null)
        {
            FileAttachment file = new FileAttachment();
            file.FileName = fileName;
            file.Length = fileLength;
            file.UploadTime = DateTime.Now;
            file.SaveMode = _modeName;
            file.ExtraInfo = extra;
            file.IsTemprory = true;
            var ext = string.Empty;
            if (string.IsNullOrEmpty(fileName) == false)
            {
                var dotPos = fileName.LastIndexOf('.');
                ext = fileName.Substring(dotPos + 1);
            }
            file.FileExt = ext;

            var ossSettings = _config.FileUploadOptions.Settings.Where(x=>x.Key.ToLower() == "oss").Select(x=>x.Value).FirstOrDefault();
            FileHandlerOptions groupInfo = null;
            if (string.IsNullOrEmpty(group))
            {
                groupInfo = ossSettings?.FirstOrDefault();
            }
            else
            {
                groupInfo = ossSettings?.Where(x => x.GroupName.ToLower() == group.ToLower()).FirstOrDefault();
                if(groupInfo == null)
                {
                    groupInfo = ossSettings?.FirstOrDefault();
                }
            }
            if (groupInfo == null)
            {
                return null;
            }


            string pathHeader = "";
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
            var fullPath = Path.Combine(pathHeader, $"{Guid.NewGuid().ToNoSplitString()}.{file.FileExt}");
            fullPath = fullPath.Replace("\\", "/");
            OssClient client = new OssClient(groupInfo.ServerUrl, groupInfo.Key, groupInfo.Secret);
            var result = client.PutObject(groupInfo.GroupLocation, fullPath, data);
            if (result.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                file.Path = fullPath;
                file.ExtraInfo = groupInfo.GroupName;
                using (var dc = _config.CreateDC(_cs))
                {
                    dc.AddEntity(file);
                    dc.SaveChanges();
                }
                return file;
            }
            else
            {
                return null;
            }
        }

        public override FileAttachment DeleteFile(string id)
        {
            var old = base.DeleteFile(id);
            var ossSettings = _config.FileUploadOptions.Settings.Where(x => x.Key.ToLower() == "oss").Select(x => x.Value).FirstOrDefault();
            FileHandlerOptions groupInfo = null;
            if (string.IsNullOrEmpty(old.ExtraInfo))
            {
                groupInfo = ossSettings?.FirstOrDefault();
            }
            else
            {
                groupInfo = ossSettings?.Where(x => x.GroupName.ToLower() == old.ExtraInfo.ToLower()).FirstOrDefault();
                if (groupInfo == null)
                {
                    groupInfo = ossSettings?.FirstOrDefault();
                }
            }
            if (groupInfo == null)
            {
                return old;
            }
            try
            {
                OssClient client = new OssClient(groupInfo.ServerUrl, groupInfo.Key, groupInfo.Secret);
                client.DeleteObject(groupInfo.GroupLocation, old.Path);
            }
            catch { }
            return old;
        }
    }

}
