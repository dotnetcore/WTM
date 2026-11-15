using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;

namespace WalkingTec.Mvvm.Core.Support.FileHandlers
{
    /// <summary>
    /// 文件提供程序：附件存储的统一入口，负责注册并按存储模式创建文件处理器（本地/OSS/数据库），同时提供附件的上传、查询与删除
    /// </summary>
    public class WtmFileProvider
    {
        /// <summary>
        /// 存储模式名称（如 local、oss、database）
        /// </summary>
        public string SaveMode { get; set; }
        // 已注册处理器集合：键为处理器名称（小写），值为以 WTMContext 为参数的构造函数
        private static Dictionary<string, ConstructorInfo> _handlers;
        // 默认处理器的构造函数，对应配置中 FileUploadOptions.SaveFileMode 指定的存储模式
        private  static ConstructorInfo _defaultHandler;
        // 当前 WTM 上下文
        private WTMContext _wtm;
        /// <summary>
        /// 全局子目录生成委托：未显式指定子目录时，由该委托根据文件处理器动态生成存储子目录
        /// </summary>
        public static Func<IWtmFileHandler, string> _subDirFunc;

        /// <summary>
        /// 初始化文件提供程序
        /// </summary>
        /// <param name="wtm">当前 WTM 上下文</param>
        public WtmFileProvider(WTMContext wtm)
        {
            _wtm = wtm;
        }

        /// <summary>
        /// 初始化文件提供程序：扫描所有实现了 IWtmFileHandler 接口的类型并注册到处理器集合，同时确定默认处理器
        /// </summary>
        /// <param name="config">系统配置，用于读取默认的文件存储模式</param>
        /// <param name="gd">全局数据，用于检索各程序集中的处理器类型</param>
        public static void Init(Configs config, GlobalData gd)
        {
            _handlers = new Dictionary<string, ConstructorInfo>();
            // 扫描所有程序集中实现了 IWtmFileHandler 接口的类型
            var types = gd.GetTypesAssignableFrom<IWtmFileHandler>();
            int count = 1;
            foreach (var item in types)
            {
                // 处理器必须提供以 WTMContext 为唯一参数的构造函数
                var cons = item.GetConstructor(new Type[] { typeof(WTMContext)});
                var nameattr = item.GetCustomAttribute<DisplayAttribute>();
                // 处理器名称优先取 Display 特性中的 Name，未标记时按序号自动生成
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
                // 与配置的存储模式一致的处理器作为默认处理器
                if (name == config.FileUploadOptions.SaveFileMode.ToString().ToLower())
                {
                    _defaultHandler = cons;
                }
                _handlers.Add(name, cons);
            }
            // 配置的模式未匹配到任何处理器时，回退使用第一个注册的处理器
            if (_defaultHandler == null && types.Count > 0)
            {
                _defaultHandler = types[0].GetConstructor(new Type[] { typeof(WTMContext) });
            }

        }

        /// <summary>
        /// 按存储模式创建对应的文件处理器实例
        /// </summary>
        /// <param name="saveMode">存储模式名称；为 null 时使用默认处理器，未注册时回退到数据库处理器</param>
        /// <param name="dc">数据连接；不为 null 时替换当前上下文使用的数据连接</param>
        /// <returns>文件处理器实例</returns>
        public IWtmFileHandler CreateFileHandler(string saveMode = null, IDataContext dc = null)
        {
            ConstructorInfo ci = null;
            // 允许调用方指定其他数据连接，替换当前上下文的 DC
            if (dc != null)
            {
                _wtm.DC = dc;
            }
            // 未指定存储模式时使用默认处理器；否则按名称（不区分大小写）查找
            if (string.IsNullOrEmpty(saveMode))
            {
                ci = _defaultHandler;
            }
            else
            {
                saveMode = saveMode.ToLower();
                if (_handlers != null && _handlers.ContainsKey(saveMode))
                {
                    ci = _handlers[saveMode];
                }
            }
            // 未匹配到任何处理器时，回退到数据库处理器
            if (ci == null)
            {
                return new WtmDataBaseFileHandler(_wtm);
            }
            else
            {
                return ci.Invoke(new object[] { _wtm }) as IWtmFileHandler;
            }
        }

        /// <summary>
        /// 上传文件：根据存储模式选择对应的文件处理器完成存储，并在数据库中生成附件记录
        /// </summary>
        /// <param name="fileName">原始文件名</param>
        /// <param name="fileLength">文件长度（字节）</param>
        /// <param name="data">文件内容流</param>
        /// <param name="group">附件分组名，可为 null</param>
        /// <param name="subdir">存储子目录，可为 null</param>
        /// <param name="extra">附加信息，可为 null</param>
        /// <param name="saveMode">存储模式（local/oss/database）；为 null 时使用系统配置的默认模式</param>
        /// <param name="dc">数据连接；为 null 时自动创建</param>
        /// <returns>上传成功返回附件对象，失败返回 null</returns>
        public  IWtmFile Upload(string fileName, long fileLength, Stream data, string group = null, string subdir = null, string extra = null, string saveMode = null, IDataContext dc =null)
        {
            if (dc == null)
            {
                dc = _wtm.CreateDC();
            }
            var fh = CreateFileHandler(saveMode, dc);
            if(fileName == null)
            {
                fileName = "unknown";
            }
            // 清理文件名中的尖括号和空格，避免存储路径异常与注入风险
            fileName = fileName.Replace("<", "").Replace(">","").Replace(" ", "");
            // 数据库处理器直接写入附件表；其他处理器先存储文件，成功后再补录附件记录
            if (fh is WtmDataBaseFileHandler lfh)
            {
                return lfh.UploadToDB(fileName, fileLength, data, group, subdir, extra);
            }
            else
            {
                var rv = fh.Upload(fileName, fileLength, data, group, subdir, extra);
                if (string.IsNullOrEmpty(rv.path) == false)
                {
                    // 文件存储成功后，组装附件记录并写入数据库
                    FileAttachment file = new FileAttachment();
                    file.FileName = fileName;
                    file.Length = fileLength;
                    file.UploadTime = DateTime.Now;
                    // 附件记录中保存实际使用的存储模式
                    file.SaveMode = string.IsNullOrEmpty(saveMode) == true ? _wtm.ConfigInfo.FileUploadOptions.SaveFileMode : saveMode;
                    file.ExtraInfo = extra;
                    // 提取文件扩展名（不含点）
                    var ext = string.Empty;
                    if (string.IsNullOrEmpty(fileName) == false)
                    {
                        var dotPos = fileName.LastIndexOf('.');
                        ext = fileName[(dotPos + 1)..];
                    }
                    file.FileExt = ext;
                    file.Path = rv.path;
                    file.HandlerInfo = rv.handlerInfo;
                    file.TenantCode = _wtm.LoginUserInfo?.CurrentTenant;
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

        /// <summary>
        /// 按 ID 查询附件，并可通过对应的文件处理器加载文件内容
        /// </summary>
        /// <param name="id">附件 ID</param>
        /// <param name="withData">是否加载文件内容，默认 true</param>
        /// <param name="dc">数据连接；为 null 时自动创建</param>
        /// <returns>附件对象；未找到或读取内容失败时返回 null</returns>
        public IWtmFile GetFile(string id, bool withData = true, IDataContext dc = null)
        {
            IWtmFile rv;
            if (dc == null)
            {
                dc = _wtm.CreateDC();
            }
            // 先只查询附件的元数据（不包含文件内容）
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
            // 需要内容时，再通过附件对应存储模式的处理器加载文件流
            if (rv != null && withData == true)
            {
                try
                {
                    var fh = CreateFileHandler(rv.SaveMode, dc);
                    rv.DataStream = fh.GetFileData(rv);
                }
                catch (Exception ex)
                {
                    // 记录异常但保持 rv=null，调用方通过 ErrorListVM 可查看
                    System.Diagnostics.Debug.WriteLine($"GetFileData failed: {ex.GetType().Name}: {ex.Message}");
                    rv = null;
                }
            }
            return rv;

        }

        /// <summary>
        /// 按 ID 删除附件：先删除数据库中的附件记录，再调用对应处理器删除实际存储的文件
        /// </summary>
        /// <param name="id">附件 ID</param>
        /// <param name="dc">数据连接；为 null 时自动创建</param>
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
                // 先删除数据库记录，再通过对应处理器清理实际存储的文件，删除失败时静默忽略
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


        /// <summary>
        /// 按 ID 查询附件的原始文件名
        /// </summary>
        /// <param name="id">附件 ID</param>
        /// <param name="dc">数据连接；为 null 时自动创建</param>
        /// <returns>清理特殊字符后的文件名；未找到时返回 unknown</returns>
        public string GetFileName(string id, IDataContext dc = null)
        {
            string rv;
            if (dc == null)
            {
                dc = _wtm.CreateDC();
            }
            rv = dc.Set<FileAttachment>().CheckID(id).Select(x => x.FileName).FirstOrDefault();
            // 未找到对应附件时使用 unknown 作为默认文件名
            if(rv == null)
            {
                rv = "unknown";
            }
            // 清理文件名中的尖括号和空格
            rv = rv.Replace("<", "").Replace(">", "").Replace(" ", "");
            return rv;
        }

    }
}
