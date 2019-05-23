using System;
using System.Collections.Generic;
using WalkingTec.Mvvm.Core.ConfigOptions;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// Configs
    /// </summary>
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

        private bool? _isQuickDebug;

        /// <summary>
        /// 是否启动调试模式
        /// </summary>
        public bool IsQuickDebug
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
                return _cookiePre ?? string.Empty;
            }
            set
            {
                _cookiePre = value;
            }
        }

        #endregion

        #region 文件存储方式

        private SaveFileModeEnum? _saveFileMode;

        /// <summary>
        /// 文件存储方式
        /// </summary>
        [Obsolete("该属性已过时，将在以后的版本中删除。推荐的替代方法是使用Configs.FileUploadOptions 配置上传设置。")]
        public SaveFileModeEnum? SaveFileMode
        {
            get
            {
                return _saveFileMode;
            }
            set
            {
                _saveFileMode = value;
            }
        }

        #endregion

        #region 上传文件路径
        private string _uploadDir;

        /// <summary>
        /// 上传文件路径
        /// </summary>
        [Obsolete("该属性已过时，将在以后的版本中删除。推荐的替代方法是使用Configs.FileUploadOptions 配置上传设置。")]
        public string UploadDir
        {
            get
            {
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
                    _rpp = DefaultConfigConsts.DEFAULT_RPP;
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

        #region Tab页显示方式

        private TabModeEnum? _tabMode;

        /// <summary>
        /// 数据库类型
        /// </summary>
        public TabModeEnum TabMode
        {
            get
            {
                if (_tabMode == null)
                {
                    _tabMode = TabModeEnum.Default;
                }
                return _tabMode.Value;
            }
            set
            {
                _tabMode = value;
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
        /// DFS配置
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

        #region 文件相关设置

        private FileUploadOptions _fileUploadOptions;

        /// <summary>
        /// 文件相关设置
        /// </summary>
        public FileUploadOptions FileUploadOptions
        {
            get
            {
                if (_fileUploadOptions == null)
                {
                    _fileUploadOptions = new FileUploadOptions()
                    {
                        UploadLimit = DefaultConfigConsts.DEFAULT_UPLOAD_LIMIT,
                        SaveFileMode = SaveFileModeEnum.Database,
                        UploadDir = DefaultConfigConsts.DEFAULT_UPLOAD_DIR
                    };
                }
                // TODO下个版本中删除 else里面的逻辑
                else
                {
                    if (!string.IsNullOrEmpty(_uploadDir))
                    {
                        _fileUploadOptions.UploadDir = _uploadDir;
                    }
                    if (_saveFileMode.HasValue)
                    {
                        _fileUploadOptions.SaveFileMode = _saveFileMode.Value;
                    }
                }
                return _fileUploadOptions;
            }
            set
            {
                _fileUploadOptions = value;
            }
        }

        #endregion

        #region 附件是否公开

        private bool? _isFilePublic;

        /// <summary>
        /// 是否启动调试模式
        /// </summary>
        public bool IsFilePublic
        {
            get
            {
                return _isFilePublic ?? false;
            }
            set
            {
                _isFilePublic = value;
            }
        }

        #endregion


    }
}
