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
        public WTMContext wtm;
        public WtmFileHandlerBase(WTMContext wtm)
        {
            this.wtm = wtm;
        }


        public virtual void DeleteFile(IWtmFile file)
        {
        }

        public virtual Stream GetFileData(IWtmFile file)
        {
            return null;
        }

        public virtual (string path, string handlerInfo) Upload(string fileName, long fileLength, Stream data, string group=null, string subdir = null, string extra=null)
        {
            return ("", "");
        }

    }
}
