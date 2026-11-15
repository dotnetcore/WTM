using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        public List<CS> Connections
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

        private Dictionary<string, Domain> _domains;

        /// <summary>
        /// ConnectionStrings
        /// </summary>
        public Dictionary<string, Domain> Domains
        {
            get
            {
                if (_domains == null)
                {
                    _domains = new Dictionary<string, Domain>();
                }
                return _domains;
            }
            set
            {
                // 将配置中的域名键统一转为小写后重建字典，并把键回填到 Domain.Name，保证域名查找不区分大小写
                _domains = new Dictionary<string, Domain>();
                foreach (var domain in value)
                {
                    _domains.Add(domain.Key.ToLower(), domain.Value);
                }
                foreach (var item in _domains)
                {
                    if (item.Value != null)
                    {
                        item.Value.Name = item.Key;
                    }
                }
            }
        }

        private bool? _hasMainHost;

        /// <summary>
        /// 是否配置了主域名（Domains 中存在键为 mainhost 的节点）；未配置时默认返回 false
        /// </summary>
        public bool HasMainHost
        {
            get
            {
                // 首次访问时从 Domains 中查找 mainhost 节点的地址并缓存判断结果
                if (_hasMainHost == null)
                {
                    _mainHost = Domains?.Where(x => x.Key.ToLower() == "mainhost").Select(x => x.Value.Address).FirstOrDefault();
                    _hasMainHost = !string.IsNullOrEmpty(_mainHost);
                }
                return _hasMainHost == true;
            }
        }

        private string _mainHost;

        /// <summary>
        /// 主域名地址（Domains 中 mainhost 节点的 Address）；获取时自动去除末尾的 / 或 \ 分隔符，未配置时返回 null
        /// </summary>
        public string MainHost
        {
            get
            {
                // 每次获取时重新查找 mainhost 节点的地址，并去掉末尾的路径分隔符，避免拼接 URL 时出现双斜杠
                _mainHost = Domains?.Where(x => x.Key.ToLower() == "mainhost").Select(x => x.Value.Address).FirstOrDefault();
                if(_mainHost != null)
                {
                    _mainHost = _mainHost.Trim();
                    if(_mainHost.EndsWith('/') || _mainHost.EndsWith('\\'))
                    {
                        _mainHost = _mainHost[0..^1];
                    }
                }
                return _mainHost;
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

        #region Tenant

        private bool? _enableTenant;

        /// <summary>
        /// Is debug mode
        /// </summary>
        public bool EnableTenant
        {
            get
            {
                return _enableTenant ?? false;
            }
            set
            {
                _enableTenant = value;
            }
        }

        #endregion


        #region Workflow

        private bool? _enableWorkflow;

        /// <summary>
        /// Enable Elsa workflow
        /// </summary>
        public bool EnableWorkflow
        {
            get
            {
                return _enableWorkflow ?? false;
            }
            set
            {
                _enableWorkflow = value;
            }
        }

        #endregion


        /// <summary>
        /// 统一错误处理页面路径，默认 /_Framework/Error
        /// </summary>
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

        #region BlazorMode

        private BlazorModeEnum? _blazorMode;

        /// <summary>
        /// TabMode
        /// </summary>
        public BlazorModeEnum BlazorMode
        {
            get
            {
                if (_blazorMode == null)
                {
                    _blazorMode = BlazorModeEnum.Server;
                }
                return _blazorMode.Value;
            }
            set
            {
                _blazorMode = value;
            }
        }
        #endregion


        #region Custom settings

        private Dictionary<string, string> _appSettings;

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

        #region FileOptions

        private FileUploadOptions _fileUploadOptions;

        /// <summary>
        /// FileOptions
        /// </summary>
        public FileUploadOptions FileUploadOptions
        {
            get
            {
                // 未在配置文件中设置时提供默认值：上传限制 20MB、保存模式 database（文件存入数据库）
                if (_fileUploadOptions == null)
                {
                    _fileUploadOptions = new FileUploadOptions()
                    {
                        UploadLimit = DefaultConfigConsts.DEFAULT_UPLOAD_LIMIT,
                        SaveFileMode = "database",
                        Settings = new Dictionary<string, List<FileHandlerOptions>>()
                    };
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
        public UIOptions UIOptions
        {
            get
            {
                // 未在配置文件中设置时，为表格、下拉框、日期控件、搜索面板等子项逐个提供默认值
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

        private List<CultureInfo> _supportLanguages;

        /// <summary>
        /// 支持的语言列表（运行时生成）；由 Languages 属性按逗号分隔后逐个转换为 CultureInfo，默认仅包含 zh（中文）
        /// </summary>
        public List<CultureInfo> SupportLanguages
        {
            get
            {
                // 首次访问时将 Languages 按逗号分隔，并逐个转换为 CultureInfo 后缓存
                if (_supportLanguages == null)
                {
                    _supportLanguages = new List<CultureInfo>();
                    var lans = Languages.Split(",");
                    foreach (var lan in lans)
                    {
                        _supportLanguages.Add(new CultureInfo(lan));
                    }

                }
                return _supportLanguages;
            }
        }

        #endregion

        /// <summary>
        /// 站点根地址（含协议与端口，如 http://localhost:5000），供框架拼接完整 URL 使用，默认为空字符串
        /// </summary>
        public string HostRoot { get; set; } = "";


        #region CookieOption configs

        private CookieOption _cookieOption;

        /// <summary>
        ///  Cors configs
        /// </summary>
        public CookieOption CookieOptions
        {
            get
            {
                if (_cookieOption == null)
                {
                    _cookieOption = new CookieOption();
                }
                return _cookieOption;
            }
            set
            {
                _cookieOption = value;
            }
        }

        #endregion

        #region JwtOption configs

        private JwtOption _jwtOption;

        /// <summary>
        ///  Cors configs
        /// </summary>
        public JwtOption JwtOptions
        {
            get
            {
                if (_jwtOption == null)
                {
                    _jwtOption = new JwtOption();
                }
                return _jwtOption;
            }
            set
            {
                _jwtOption = value;
            }
        }

        #endregion

        #region LLM

        private LlmOption _llm;

        /// <summary>
        /// LLM configuration — OpenAI-compatible chat API
        /// </summary>
        public LlmOption Llm
        {
            get
            {
                if (_llm == null) _llm = new LlmOption();
                return _llm;
            }
            set { _llm = value; }
        }

        #endregion

        /// <summary>
        /// Embedding config
        /// </summary>
        public EmbeddingOption EmbeddingOptions { get; set; } = new EmbeddingOption();

        /// <summary>
        /// AI代码功能配置
        /// </summary>
        public AiCodeOptions AiCode { get; set; } = new AiCodeOptions();

        /// <summary>
        /// Agent对话功能配置
        /// </summary>
        public AgentOption Agent { get; set; } = new AgentOption();

    }
}
