using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Core.Support.FileHandlers
{
    public abstract class WtmFileHandlerBase : IWtmFileHandler
    {
        protected Configs _config;
        protected string _cs;

        public WtmFileHandlerBase(Configs config, string csName)
        {
            _config = config;
            _cs = csName;
            if (string.IsNullOrEmpty(_cs))
            {
                _cs = "default";
            }
        }


        public virtual void DeleteFile(string id)
        {
            FileAttachment del = new FileAttachment();
            del.SetID(id);
            using (var dc = _config.CreateDC(_cs))
            {
                dc.Set<FileAttachment>().Attach(del);
                dc.Set<FileAttachment>().Remove(del);
                dc.SaveChanges();
            }
        }

        public virtual IWtmFile GetFile(string id,bool widhData = true)
        {
            IWtmFile rv;
            using (var dc = _config.CreateDC(_cs))
            {
                rv = dc.Set<FileAttachment>().CheckID(id).Select(x => new FileAttachment
                {
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
            return rv;
        }

        public virtual string GetFileName(string id)
        {
            string rv;
            using (var dc = _config.CreateDC(_cs))
            {
                rv = dc.Set<FileAttachment>().CheckID(id).Select(x => x.FileName).FirstOrDefault();
            }
            return rv;
        }

        public virtual IWtmFile Upload(string fileName, long fileLength, Stream data, string subdir = null, string extra = null)
        {
            return null;
        }

        public void SetTemp(string id, bool isTemp)
        {
            using (var dc = _config.CreateDC(_cs))
            {
                FileAttachment upd = new FileAttachment();
                upd.SetID(id);
                upd.IsTemprory = isTemp;
                dc.UpdateProperty(upd, x => x.IsTemprory);
                dc.SaveChanges();
            }

        }
    }
}
