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
        #region ConnectionStrings

        private List<CS> _connectStrings;

        /// <summary>
        /// ConnectionStrings
        /// </summary>
        public List<CS> ConnectionStrings
        {
            get
            {
                if (_connectStrings == null)
                {
                    _connectStrings = new List<CS>();
                }
                return _connectStrings;
            }
            set
            {
                _connectStrings = value;
            }
        }

        #endregion

        #region Domains

        private Dictionary<string,FrameworkDomain> _domains;

        /// <summary>
        /// ConnectionStrings
        /// </summary>
        public Dictionary<string, FrameworkDomain> Domains
        {
            get
            {
                if (_domains == null)
                {
                    _domains = new Dictionary<string, FrameworkDomain>();
                }
                return _domains;
            }
            set
            {
                _domains = value;
                foreach (var item in _domains)
                {
                    if(item.Value != null)
                    {
                        item.Value.Name = item.Key;
                    }
                }
            }
        }

        #endregion


        #region QuickDebug

        private bool? _isQuickDebug;

        /// <summary>
        /// Is debug mode
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

        public string ErrorHandler { get; set; } = "/_Framework/Error";

        #region Cookie prefix

        private string _cookiePre;

        /// <summary>
        /// Cookie prefix
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

        #region File attachment save mode

        private SaveFileModeEnum? _saveFileMode;

        /// <summary>
        /// File attachment save mode
        /// </summary>
        [Obsolete("use Configs.FileUploadOptions instead")]
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

        #region File attachment upload path
        private string _uploadDir;

        /// <summary>
        /// File attachment upload path
        /// </summary>
        [Obsolete("use Configs.FileUploadOptions instead")]
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

        #region Auto sync db

        private bool? _syncdb;

        /// <summary>
        /// Auto sync db(not supportted)
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

        #region Database type

        private DBTypeEnum? _dbtype;

        /// <summary>
        /// Database type
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

        public bool IsOldSqlServer { get; set; }

        #endregion

        #region PageMode

        private PageModeEnum? _pageMode;

        /// <summary>
        /// PageMode
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

        #region TabMode

        private TabModeEnum? _tabMode;

        /// <summary>
        /// TabMode
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

        #region EncryptKey

        private string _encryptKey;

        /// <summary>
        /// EncryptKey
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

        #region Custom settings

        private Dictionary<string,string> _appSettings;

        /// <summary>
        /// Custom settings
        /// </summary>
        public Dictionary<string, string> AppSettings
        {
            get
            {
                if (_appSettings == null)
                {
                    _appSettings = new Dictionary<string, string>();
                }
                return _appSettings;
            }
            set
            {
                _appSettings = value;
            }
        }

        #endregion

        #region Data Privilege

        private List<IDataPrivilege> _dataPrivilegeSettings;

        /// <summary>
        /// Data Privilege
        /// </summary>
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

        #region DFS Config

        private DFS _dfsServer;

        /// <summary>
        /// DFS Config
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

        #region FileOptions

        private FileUploadOptions _fileUploadOptions;

        /// <summary>
        /// FileOptions
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

        #region UIOptions

        private UIOptions _uiOptions;

        /// <summary>
        /// UIOptions
        /// </summary>
        public UIOptions UiOptions
        {
            get
            {
                if (_uiOptions == null)
                {
                    _uiOptions = new UIOptions();
                    if (_uiOptions.DataTable == null)
                        _uiOptions.DataTable = new UIOptions.DataTableOptions
                        {
                            RPP = DefaultConfigConsts.DEFAULT_RPP
                        };

                    if (_uiOptions.ComboBox == null)
                        _uiOptions.ComboBox = new UIOptions.ComboBoxOptions
                        {
                            DefaultEnableSearch = DefaultConfigConsts.DEFAULT_COMBOBOX_DEFAULT_ENABLE_SEARCH
                        };

                    if (_uiOptions.DateTime == null)
                        _uiOptions.DateTime = new UIOptions.DateTimeOptions
                        {
                            DefaultReadonly = DefaultConfigConsts.DEFAULT_DATETIME_DEFAULT_READONLY
                        };

                    if (_uiOptions.SearchPanel == null)
                        _uiOptions.SearchPanel = new UIOptions.SearchPanelOptions
                        {
                            DefaultExpand = DefaultConfigConsts.DEFAULT_SEARCHPANEL_DEFAULT_EXPAND
                        };
                }
                return _uiOptions;
            }
            set
            {
                _uiOptions = value;
            }
        }

        #endregion

        #region Is FileAttachment public

        private bool? _isFilePublic;

        /// <summary>
        /// Is FileAttachment public
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

        #region UEditorOptions

        private UEditorOptions _ueditorOptions;

        /// <summary>
        /// UEditor配置
        /// </summary>
        /// <value></value>
        public UEditorOptions UEditorOptions
        {
            get
            {
                if (_ueditorOptions == null)
                {
                    _ueditorOptions = new UEditorOptions();
                }
                return _ueditorOptions;
            }
            set
            {
                _ueditorOptions = value;
            }
        }
        #endregion

        #region Cors configs

        private Cors _cors;

        /// <summary>
        ///  Cors configs
        /// </summary>
        public Cors CorsOptions
        {
            get
            {
                if (_cors == null)
                {
                    _cors = new Cors();
                    _cors.Policy = new List<CorsPolicy>();
                }
                return _cors;
            }
            set
            {
                _cors = value;
            }
        }

        #endregion

        #region Support Languages

        private string _languages;

        /// <summary>
        /// Support Languages
        /// </summary>
        public string Languages
        {
            get
            {
                if (string.IsNullOrEmpty((_languages)))
                {
                    _languages = "zh";
                }
                return _languages;
            }
            set
            {
                _languages = value;
            }
        }

        #endregion
    }
}
