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
        private static string _modeName = "DataBase";

        public WtmDataBaseFileHandler(Configs config, string csName) : base(config, csName)
        {
        }

        public override IWtmFile GetFile(string id, bool withData = true)
        {
            IWtmFile rv;
            using (var dc = _config.CreateDC(_cs))
            {
                if (withData == true)
                {
                    rv = dc.Set<FileAttachment>().CheckID(id).Where(x => x.SaveMode == _modeName).FirstOrDefault();
                    rv.DataStream = new MemoryStream((rv as FileAttachment).FileData);
                }
                else
                {
                    rv = dc.Set<FileAttachment>().CheckID(id).Where(x => x.SaveMode == _modeName).Select(x=> new FileAttachment {
                        ID = x.ID,
                        IsTemprory = x.IsTemprory,
                        ExtraInfo = x.ExtraInfo,
                        FileExt = x.FileExt,
                        FileName = x.FileName,
                        Length = x.Length,
                        Path = x.Path,
                        SaveMode = x.SaveMode,
                        UploadTime = x.UploadTime
                    }).FirstOrDefault();

                }
            }
            return rv;

        }


        public override IWtmFile Upload(string fileName, long fileLength, Stream data, string groupName= null, string subdir = null, string extra = null)
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
            using (var dataStream = new MemoryStream())
            {
                data.CopyTo(dataStream);
                file.FileData = dataStream.ToArray();
            }
            using (var dc = _config.CreateDC(_cs))
            {
                dc.AddEntity(file);
                dc.SaveChanges();
            }
            return file;
        }
    }
}
