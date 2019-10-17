using Newtonsoft.Json;

namespace WalkingTec.Mvvm.Core.ConfigOptions
{
    public class UEditorOptions
    {
        #region 上传图片配置项
        /// <summary>
        /// 执行上传图片的action名称
        /// </summary>
        /// <value></value>
        [JsonProperty("imageActionName")]
        public string ImageActionName { get; set; } = "UploadForLayUIUEditor";

        /// <summary>
        /// 提交的图片表单名称
        /// </summary>
        /// <value></value>
        [JsonProperty("imageFieldName")]
        public string ImageFieldName { get; set; } = "FileID";

        /// <summary>
        /// 上传大小限制，单位B
        /// </summary>
        /// <value></value>
        [JsonProperty("imageMaxSize")]
        public int ImageMaxSize { get; set; } = 2048000;

        /// <summary>
        /// 上传图片格式显示
        /// </summary>
        /// <value></value>
        [JsonProperty("imageAllowFiles")]
        public string[] ImageAllowFiles { get; set; } = new string[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };

        /// <summary>
        /// 是否压缩图片,默认是true
        /// </summary>
        /// <value></value>
        [JsonProperty("imageCompressEnable")]
        public bool ImageCompressEnable { get; set; } = true;

        /// <summary>
        /// 图片压缩最长边限制
        /// </summary>
        /// <value></value>
        [JsonProperty("imageCompressBorder")]
        public int ImageCompressBorder { get; set; } = 1600;

        /// <summary>
        /// 插入的图片浮动方式
        /// </summary>
        /// <value></value>
        [JsonProperty("imageInsertAlign")]
        public string ImageInsertAlign { get; set; } = "none";

        /// <summary>
        /// 图片访问路径前缀 默认返回全路径
        /// </summary>
        /// <value></value>
        [JsonProperty("imageUrlPrefix")]
        public string ImageUrlPrefix { get; set; } = string.Empty;

        /* {filename} 会替换成原文件名,配置这项需要注意中文乱码问题 */
        /* {rand:6} 会替换成随机数,后面的数字是随机数的位数 */
        /* {time} 会替换成时间戳 */
        /* {yyyy} 会替换成四位年份 */
        /* {yy} 会替换成两位年份 */
        /* {mm} 会替换成两位月份 */
        /* {dd} 会替换成两位日期 */
        /* {hh} 会替换成两位小时 */
        /* {ii} 会替换成两位分钟 */
        /* {ss} 会替换成两位秒 */
        /* 非法字符 \ : * ? " < > | */
        /* 具请体看线上文档: fex.baidu.com/ueditor/#use-format_upload_filename */

        /// <summary>
        /// 上传保存路径,可以自定义保存路径和文件名格式
        /// </summary>
        /// <value></value>
        [JsonProperty("imagePathFormat")]
        public string ImagePathFormat { get; set; } = "upload/image/{yyyy}{mm}{dd}/{time}{rand:6}";

        #endregion

        #region 涂鸦图片上传配置项

        /// <summary>
        /// 执行上传涂鸦的action名称
        /// </summary>
        /// <value></value>
        [JsonProperty("scrawlActionName")]
        public string ScrawlActionName { get; set; } = "UploadForLayUIUEditor";

        /// <summary>
        /// 提交的图片表单名称
        /// </summary>
        /// <value></value>
        [JsonProperty("scrawlFieldName")]
        public string ScrawlFieldName { get; set; } = "FileID";

        /// <summary>
        /// 上传保存路径,可以自定义保存路径和文件名格式
        /// </summary>
        /// <value></value>
        [JsonProperty("scrawlPathFormat")]
        public string ScrawlPathFormat { get; set; } = "upload/image/{yyyy}{mm}{dd}/{time}{rand:6}";

        /// <summary>
        /// 上传大小限制，单位B
        /// </summary>
        /// <value></value>
        [JsonProperty("scrawlMaxSize")]
        public int ScrawlMaxSize { get; set; } = 2048000;

        /// <summary>
        /// 图片访问路径前缀
        /// </summary>
        /// <value></value>
        [JsonProperty("scrawlUrlPrefix")]
        public string ScrawlUrlPrefix { get; set; } = string.Empty;

        /// <summary>
        /// 插入的图片浮动方式
        /// </summary>
        /// <value></value>
        [JsonProperty("scrawlInsertAlign")]
        public string ScrawlInsertAlign { get; set; } = "none";
        #endregion

        #region 截图工具上传

        /// <summary>
        /// 执行上传截图的action名称
        /// </summary>
        /// <value></value>
        [JsonProperty("snapscreenActionName")]
        public string SnapscreenActionName { get; set; } = "UploadForLayUIUEditor";

        /// <summary>
        /// 上传保存路径,可以自定义保存路径和文件名格式
        /// </summary>
        /// <value></value>
        [JsonProperty("snapscreenPathFormat")]
        public string SnapscreenPathFormat { get; set; } = "upload/image/{yyyy}{mm}{dd}/{time}{rand:6}";

        /// <summary>
        /// 图片访问路径前缀
        /// </summary>
        /// <value></value>
        [JsonProperty("snapscreenUrlPrefix")]
        public string SnapscreenUrlPrefix { get; set; } = string.Empty;

        /// <summary>
        /// 插入的图片浮动方式
        /// </summary>
        /// <value></value>
        [JsonProperty("snapscreenInsertAlign")]
        public string SnapscreenInsertAlign { get; set; } = "none";
        #endregion

        #region 抓取远程图片配置

        [JsonProperty("catcherLocalDomain")]
        public string[] CatcherLocalDomain { get; set; } = new string[] { "127.0.0.1", "localhost", "img.baidu.com" };

        /// <summary>
        /// 执行抓取远程图片的action名称
        /// </summary>
        /// <value></value>
        [JsonProperty("catcherActionName")]
        public string CatcherActionName { get; set; } = "catchimage";

        /// <summary>
        /// 提交的图片列表表单名称
        /// </summary>
        /// <value></value>
        [JsonProperty("catcherFieldName")]
        public string CatcherFieldName { get; set; } = "source";

        /// <summary>
        /// 上传保存路径,可以自定义保存路径和文件名格式
        /// </summary>
        /// <value></value>
        [JsonProperty("catcherPathFormat")]
        public string CatcherPathFormat { get; set; } = "upload/image/{yyyy}{mm}{dd}/{time}{rand:6}";

        /// <summary>
        /// 图片访问路径前缀
        /// </summary>
        /// <value></value>
        [JsonProperty("catcherUrlPrefix")]
        public string CatcherUrlPrefix { get; set; } = string.Empty;

        /// <summary>
        /// 上传大小限制，单位B
        /// </summary>
        /// <value></value>
        [JsonProperty("catcherMaxSize")]
        public int CatcherMaxSize { get; set; } = 2048000;

        /// <summary>
        /// 抓取图片格式显示
        /// </summary>
        /// <value></value>
        [JsonProperty("catcherAllowFiles")]
        public string[] CatcherAllowFiles { get; set; } = new string[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };
        #endregion

        #region 上传视频配置

        /// <summary>
        /// 执行上传视频的action名称
        /// </summary>
        /// <value></value>
        [JsonProperty("videoActionName")]
        public string VideoActionName { get; set; } = "UploadForLayUIUEditor";

        /// <summary>
        /// 提交的视频表单名称
        /// </summary>
        /// <value></value>
        [JsonProperty("videoFieldName")]
        public string VideoFieldName { get; set; } = "FileID";

        /// <summary>
        /// 上传保存路径,可以自定义保存路径和文件名格式
        /// </summary>
        /// <value></value>
        [JsonProperty("videoPathFormat")]
        public string VideoPathFormat { get; set; } = "upload/video/{yyyy}{mm}{dd}/{time}{rand:6}";

        /// <summary>
        /// 视频访问路径前缀
        /// </summary>
        /// <value></value>
        [JsonProperty("videoUrlPrefix")]
        public string VideoUrlPrefix { get; set; } = string.Empty;

        /// <summary>
        /// 上传大小限制，单位B，默认100MB
        /// </summary>
        /// <value></value>
        [JsonProperty("videoMaxSize")]
        public int VideoMaxSize { get; set; } = 102400000;

        /// <summary>
        /// 上传视频格式显示
        /// </summary>
        /// <value></value>
        [JsonProperty("videoAllowFiles")]
        public string[] VideoAllowFiles { get; set; } = new string[] { ".flv", ".swf", ".mkv", ".avi", ".rm", ".rmvb", ".mpeg", ".mpg", ".ogg", ".ogv", ".mov", ".wmv", ".mp4", ".webm", ".mp3", ".wav", ".mid" };

        #endregion

        #region 上传文件配置

        /// <summary>
        /// controller里,执行上传视频的action名称
        /// </summary>
        /// <value></value>
        [JsonProperty("fileActionName")]
        public string FileActionName { get; set; } = "UploadForLayUIUEditor";

        /// <summary>
        /// 提交的文件表单名称
        /// </summary>
        /// <value></value>
        [JsonProperty("fileFieldName")]
        public string FileFieldName { get; set; } = "FileID";

        /// <summary>
        /// 上传保存路径,可以自定义保存路径和文件名格式
        /// </summary>
        /// <value></value>
        [JsonProperty("filePathFormat")]
        public string FilePathFormat { get; set; } = "upload/file/{yyyy}{mm}{dd}/{time}{rand:6}";

        /// <summary>
        /// 文件访问路径前缀
        /// </summary>
        /// <value></value>
        [JsonProperty("fileUrlPrefix")]
        public string FileUrlPrefix { get; set; } = string.Empty;

        /// <summary>
        /// 上传大小限制，单位B，默认50MB
        /// </summary>
        /// <value></value>
        [JsonProperty("fileMaxSize")]
        public int FileMaxSize { get; set; } = 51200000;

        /// <summary>
        /// 上传文件格式显示
        /// </summary>
        /// <value></value>
        [JsonProperty("fileAllowFiles")]
        public string[] FileAllowFiles { get; set; } = new string[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".flv", ".swf", ".mkv", ".avi", ".rm", ".rmvb", ".mpeg", ".mpg", ".ogg", ".ogv", ".mov", ".wmv", ".mp4", ".webm", ".mp3", ".wav", ".mid", ".rar", ".zip", ".tar", ".gz", ".7z", ".bz2", ".cab", ".iso", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf", ".txt", ".md", ".xml" };

        #endregion

        #region 列出指定目录下的图片

        /// <summary>
        /// 执行图片管理的action名称
        /// </summary>
        /// <value></value>
        [JsonProperty("imageManagerActionName")]
        public string ImageManagerActionName { get; set; } = "listimage";

        /// <summary>
        /// 指定要列出图片的目录
        /// </summary>
        /// <value></value>
        [JsonProperty("imageManagerListPath")]
        public string ImageManagerListPath { get; set; } = string.Empty;

        /// <summary>
        /// 每次列出文件数量
        /// </summary>
        /// <value></value>
        [JsonProperty("imageManagerListSize")]
        public int ImageManagerListSize { get; set; } = 20;

        /// <summary>
        /// 图片访问路径前缀
        /// </summary>
        /// <value></value>
        [JsonProperty("imageManagerUrlPrefix")]
        public string ImageManagerUrlPrefix { get; set; } = string.Empty;

        /// <summary>
        /// 插入的图片浮动方式
        /// </summary>
        /// <value></value>
        [JsonProperty("imageManagerInsertAlign")]
        public string ImageManagerInsertAlign { get; set; } = "none";

        /// <summary>
        /// 列出的文件类型
        /// </summary>
        /// <value></value>
        [JsonProperty("imageManagerAllowFiles")]
        public string[] ImageManagerAllowFiles { get; set; } = new string[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };
        #endregion

        #region 列出指定目录下的文件

        /// <summary>
        /// 执行文件管理的action名称
        /// </summary>
        /// <value></value>
        [JsonProperty("fileManagerActionName")]
        public string FileManagerActionName { get; set; } = "listfile";

        /// <summary>
        /// 指定要列出文件的目录
        /// </summary>
        /// <value></value>
        [JsonProperty("fileManagerListPath")]
        public string FileManagerListPath { get; set; } = "upload/file";

        /// <summary>
        /// 文件访问路径前缀
        /// </summary>
        /// <value></value>
        [JsonProperty("fileManagerUrlPrefix")]
        public string FileManagerUrlPrefix { get; set; } = "/ueditor/net/";

        /// <summary>
        /// 每次列出文件数量
        /// </summary>
        /// <value></value>
        [JsonProperty("fileManagerListSize")]
        public int FileManagerListSize { get; set; } = 20;

        /// <summary>
        /// 列出的文件类型
        /// </summary>
        /// <value></value>
        [JsonProperty("fileManagerAllowFiles")]
        public string[] FileManagerAllowFiles { get; set; } = new string[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".flv", ".swf", ".mkv", ".avi", ".rm", ".rmvb", ".mpeg", ".mpg", ".ogg", ".ogv", ".mov", ".wmv", ".mp4", ".webm", ".mp3", ".wav", ".mid", ".rar", ".zip", ".tar", ".gz", ".7z", ".bz2", ".cab", ".iso", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf", ".txt", ".md", ".xml" };
        #endregion
    }
}
