using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Core.Support.FileHandlers
{

    [Display(Name = "local")]
    public class WtmLocalFileHandler : WtmFileHandlerBase
    {

        public WtmLocalFileHandler(WTMContext wtm) : base(wtm)
        {
        }

        public override Stream GetFileData(IWtmFile file)
        {
            return File.OpenRead(GetFullPath(file.Path));          
        }


        public override (string path, string handlerInfo) Upload(string fileName, long fileLength, Stream data, string group = null, string subdir = null, string extra = null)
        {
            var localSettings = wtm.ConfigInfo.FileUploadOptions.Settings.Where(x => x.Key.ToLower() == "local").Select(x => x.Value).FirstOrDefault();

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
                groupdir = "./uploads";
            }
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
            if (!Directory.Exists(fulldir))
            {
                Directory.CreateDirectory(fulldir);
            }
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
