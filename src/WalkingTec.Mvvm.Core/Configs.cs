using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    
    public class KV
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class DFS
    {
        public int? StorageMaxConnection { get; set; }
        public int? TrackerMaxConnection { get; set; }
        public int? ConnectionTimeout { get; set; }
        public int? ConnectionLifeTime { get; set; }
        public List<DFSTracker> Trackers { get; set; }
    }

    public class DFSTracker
    {
        public string IP { get; set; }
        public int Port { get; set; }
    }

    public class Configs
    {
        #region 数据库连接字符串
        private List<KV> _connectStrings;
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public List<KV> ConnectionStrings
        {
            get
            {

                if (_connectStrings == null)
                {
                    _connectStrings = new List<KV>();
                }
                return _connectStrings;
            }
            set
            {
                _connectStrings = value;
            }
        }
        #endregion

        #region 是否是调试模式
        private  bool? _isQuickDebug;
        /// <summary>
        /// 是否启动调试模式
        /// </summary>
        public  bool IsQuickDebug
        {
            get
            {
                return _isQuickDebug ?? false;
            }
            set
            {
                _isQuickDebug = value;
            }
        }

        #endregion

        #region Cookie前缀
        private string _cookiePre;
        /// <summary>
        /// 代理，用于系统访问外部网站
        /// </summary>
        public string CookiePre
        {
            get
            {
                return _cookiePre ?? "";
            }
            set
            {
                _cookiePre = value;
            }
        }
        #endregion

        #region 文件存储方式
        private  SaveFileModeEnum? _saveFileMode;
        /// <summary>
        /// 文件存储方式
        /// </summary>
        public  SaveFileModeEnum SaveFileMode
        {
            get
            {
                if (_saveFileMode == null)
                {
                    _saveFileMode = SaveFileModeEnum.Database;
                }
                return _saveFileMode.Value;
            }
            set
            {
                _saveFileMode = value;
            }
        }
        #endregion

        #region 上传文件路径
        private  string _uploadDir;
        /// <summary>
        /// 上传文件路径
        /// </summary>
        public  string UploadDir
        {
            get
            {
                if (_uploadDir == null)
                {
                    _uploadDir ="c:\\upload";
                    if (!string.IsNullOrEmpty(_uploadDir))
                    {
                        System.IO.Directory.CreateDirectory(_uploadDir);
                    }
                }
                return _uploadDir;
            }
            set
            {
                _uploadDir = value;
            }
        }
        #endregion

        #region 是否启用日志
        private bool? _enableLog;
        /// <summary>
        /// 是否启动调试模式
        /// </summary>
        public bool EnableLog
        {
            get
            {
                return _enableLog ?? false;
            }
            set
            {
                _enableLog = value;
            }
        }

        #endregion

        #region 是否在log中只记录一场
        private bool? _logExceptionOnly;
        /// <summary>
        /// 是否启动调试模式
        /// </summary>
        public bool LogExceptionOnly
        {
            get
            {
                return _logExceptionOnly ?? false;
            }
            set
            {
                _logExceptionOnly = value;
            }
        }

        #endregion

        #region 默认列表行数
        private int? _rpp;
        /// <summary>
        /// 默认列表行数
        /// </summary>
        public int RPP
        {
            get
            {
                if (_rpp == null)
                {
                    _rpp = 20;
                }
                return _rpp.Value;
            }
            set
            {
                _rpp = value;
            }
        }
        #endregion

        #region 自动更新数据库
        private bool? _syncdb;
        /// <summary>
        /// 是否自动更新数据库
        /// </summary>
        public bool SyncDB
        {
            get
            {
                return _syncdb ?? false;
            }
            set
            {
                _syncdb = value;
            }
        }

        #endregion

        #region 数据库类型
        private DBTypeEnum? _dbtype;
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DBTypeEnum DbType
        {
            get
            {
                if (_dbtype == null)
                {
                    _dbtype = DBTypeEnum.SqlServer;
                }
                return _dbtype.Value;
            }
            set
            {
                _dbtype = value;
            }
        }
        #endregion

        #region 页面显示方式
        private PageModeEnum? _pageMode;
        /// <summary>
        /// 数据库类型
        /// </summary>
        public PageModeEnum PageMode
        {
            get
            {
                if (_pageMode == null)
                {
                    _pageMode = PageModeEnum.Single;
                }
                return _pageMode.Value;
            }
            set
            {
                _pageMode = value;
            }
        }
        #endregion

        #region 加密密钥
        private string _encryptKey;
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string EncryptKey
        {
            get
            {
                if (string.IsNullOrEmpty(_encryptKey))
                {
                    _encryptKey = string.Empty;
                }
                return _encryptKey;
            }
            set
            {
                _encryptKey = value;
            }
        }
        #endregion

        #region 自定义应用配置
        private List<KV> _appSettings;
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public List<KV> AppSettings
        {
            get
            {

                if (_appSettings == null)
                {
                    _appSettings = new List<KV>();
                }
                return _appSettings;
            }
            set
            {
                _appSettings = value;
            }
        }

        #endregion

        #region 数据权限配置
        private List<IDataPrivilege> _dataPrivilegeSettings;

        public List<IDataPrivilege> DataPrivilegeSettings
        {
            get
            {

                if (_dataPrivilegeSettings == null)
                {
                    _dataPrivilegeSettings = new List<IDataPrivilege>();
                }
                return _dataPrivilegeSettings;
            }
            set
            {
                _dataPrivilegeSettings = value;
            }
        }
        #endregion

        #region DFS配置
        private DFS _dfsServer;
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public DFS DFSServer
        {
            get
            {

                if (_dfsServer == null)
                {
                    _dfsServer = new DFS();
                }
                return _dfsServer;
            }
            set
            {
                _dfsServer = value;
            }
        }

        #endregion

    }
}
