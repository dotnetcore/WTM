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
    [Display(Name = "database")]
    public class WtmDataBaseFileHandler : WtmFileHandlerBase
    {
        private static string _modeName = "database";

        public WtmDataBaseFileHandler(Configs config, IDataContext dc) : base(config, dc)
        {
        }

        public override Stream GetFileData(IWtmFile file)
        {
            var rv = _dc.Set<FileAttachment>().CheckID(file.GetID()).FirstOrDefault();
            if (rv != null)
            {
                return new MemoryStream((rv as FileAttachment).FileData);
            }
            return null;
        }


        public  IWtmFile UploadToDB(string fileName, long fileLength, Stream data, string groupName = null, string subdir = null, string extra = null)
        {
            FileAttachment file = new FileAttachment();
            file.FileName = fileName;
            file.Length = fileLength;
            file.UploadTime = DateTime.Now;
            file.SaveMode = _modeName;
            file.ExtraInfo = extra;
            var ext = string.Empty;
            if (string.IsNullOrEmpty(fileName) == false)
            {
                var dotPos = fileName.LastIndexOf('.');
                ext = fileName.Substring(dotPos + 1);
            }
            file.FileExt = ext;
            using (var dataStream = new MemoryStream())
            {
                data.CopyTo(dataStream);
                file.FileData = dataStream.ToArray();
            }
                _dc.AddEntity(file);
                _dc.SaveChanges();
            return file;
        }
    }
}
