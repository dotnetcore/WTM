using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Options;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Core.Support.FileHandlers
{
    public class WtmFileProvider
    {
        public string SaveMode { get; set; }
        private static Dictionary<string, ConstructorInfo> _handlers;
        private  static ConstructorInfo _defaultHandler;
        private WTMContext _wtm;
        public static Func<IWtmFileHandler, string> _subDirFunc;

        public WtmFileProvider(WTMContext wtm)
        {
            _wtm = wtm;
        }

        public static void Init(Configs config, GlobalData gd)
        {
            _handlers = new Dictionary<string, ConstructorInfo>();
            var types = gd.GetTypesAssignableFrom<IWtmFileHandler>();
            int count = 1;
            foreach (var item in types)
            {
                var cons = item.GetConstructor(new Type[] { typeof(Configs), typeof(IDataContext) });
                var nameattr = item.GetCustomAttribute<DisplayAttribute>();
                string name = "";
                if (nameattr == null)
                {
                    name = "FileHandler" + count;
                    count++;
                }
                else
                {
                    name = nameattr.Name;
                }
                name = name.ToLower();
                if (name == config.FileUploadOptions.SaveFileMode.ToString().ToLower())
                {
                    _defaultHandler = cons;
                }
                _handlers.Add(name, cons);
            }
            if (_defaultHandler == null && types.Count > 0)
            {
                _defaultHandler = types[0].GetConstructor(new Type[] { typeof(Configs), typeof(IDataContext) });
            }

        }

        public IWtmFileHandler CreateFileHandler(string saveMode = null, IDataContext dc = null)
        {
            ConstructorInfo ci = null;
            if(dc == null)
            {
                dc = _wtm.CreateDC();
            }
            if (string.IsNullOrEmpty(saveMode))
            {
                ci = _defaultHandler;
            }
            else
            {
                saveMode = saveMode.ToLower();
                if (_handlers.ContainsKey(saveMode))
                {
                    ci = _handlers[saveMode];
                }
            }
            if (ci == null)
            {
                return new WtmDataBaseFileHandler(_wtm.ConfigInfo, dc);
            }
            else
            {
                return ci.Invoke(new object[] { _wtm.ConfigInfo, dc }) as IWtmFileHandler;
            }
        }

        public  IWtmFile Upload(string fileName, long fileLength, Stream data, string group = null, string subdir = null, string extra = null, string saveMode = null, IDataContext dc =null)
        {
            if (dc == null)
            {
                dc = _wtm.CreateDC();
            }
            var fh = CreateFileHandler(saveMode, dc);
            if (fh is WtmDataBaseFileHandler lfh)
            {
                return lfh.UploadToDB(fileName, fileLength, data, group, subdir, extra);
            }
            else
            {
                var rv = fh.Upload(fileName, fileLength, data, group, subdir, extra);
                if (string.IsNullOrEmpty(rv.path) == false)
                {
                    FileAttachment file = new FileAttachment();
                    file.FileName = fileName;
                    file.Length = fileLength;
                    file.UploadTime = DateTime.Now;
                    file.SaveMode = string.IsNullOrEmpty(saveMode) == true ? _wtm.ConfigInfo.FileUploadOptions.SaveFileMode : saveMode;
                    file.ExtraInfo = extra;
                    var ext = string.Empty;
                    if (string.IsNullOrEmpty(fileName) == false)
                    {
                        var dotPos = fileName.LastIndexOf('.');
                        ext = fileName[(dotPos + 1)..];
                    }
                    file.FileExt = ext;
                    file.Path = rv.path;
                    file.HandlerInfo = rv.handlerInfo;
                    dc.AddEntity(file);
                    dc.SaveChanges();
                    return file;
                }
                else
                {
                    return null;
                }
            }
        }

        public IWtmFile GetFile(string id, bool withData = true, IDataContext dc = null)
        {
            IWtmFile rv;
            if (dc == null)
            {
                dc = _wtm.CreateDC();
            }
            rv = dc.Set<FileAttachment>().CheckID(id).Select(x => new FileAttachment
            {
                ID = x.ID,
                ExtraInfo = x.ExtraInfo,
                FileExt = x.FileExt,
                FileName = x.FileName,
                Length = x.Length,
                Path = x.Path,
                SaveMode = x.SaveMode,
                UploadTime = x.UploadTime
            }).FirstOrDefault();
            if (rv != null && withData == true)
            {
                var fh = CreateFileHandler(rv.SaveMode, dc);
                rv.DataStream = fh.GetFileData(rv);
            }
            return rv;

        }

        public void DeleteFile(string id, IDataContext dc = null)
        {
            FileAttachment file = null;
            if (dc == null)
            {
                dc = _wtm.CreateDC();
            }
            file = dc.Set<FileAttachment>().CheckID(id)
                .Select(x => new FileAttachment
                {
                    ID = x.ID,
                    ExtraInfo = x.ExtraInfo,
                    FileExt = x.FileExt,
                    FileName = x.FileName,
                    Path = x.Path,
                    SaveMode = x.SaveMode,
                    Length = x.Length,
                    UploadTime = x.UploadTime
                })
                .FirstOrDefault();
            if (file != null)
            {
                try
                {
                    dc.Set<FileAttachment>().Remove(file);
                    dc.SaveChanges();
                    var fh = CreateFileHandler(file.SaveMode, dc);
                    fh.DeleteFile(file);
                }
                catch { }
            }

        }


        public string GetFileName(string id, IDataContext dc = null)
        {
            string rv;
            if (dc == null)
            {
                dc = _wtm.CreateDC();
            }
            rv = dc.Set<FileAttachment>().CheckID(id).Select(x => x.FileName).FirstOrDefault();
            return rv;
        }

    }
}
