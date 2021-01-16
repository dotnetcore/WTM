using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Core.Support.FileHandlers
{
    public interface IWtmFileHandler
    {
        (string path,string handlerInfo) Upload(string fileName, long fileLength, Stream data, string group=null, string subdir=null, string extra=null);
        Stream GetFileData(IWtmFile file);

        void DeleteFile(IWtmFile file);

    }
}
