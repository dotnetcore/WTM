using System.Collections.Generic;

namespace WalkingTec.Mvvm.Mvc.UEditor
{
    /// <summary>
    /// UEditor 编辑器 config.json 配置文件的反序列化模型，
    /// 包含图片、涂鸦、截图、图片抓取、视频、附件上传及在线管理等各功能模块的服务器端配置项
    /// </summary>
    internal class UEditorConfigJson
    {
        /// <summary>
        /// 执行图片上传的服务器端接口(Action)名称
        /// </summary>
        public string imageActionName { get; set; }
        /// <summary>
        /// 提交图片上传的表单字段名称
        /// </summary>
        public string imageFieldName { get; set; }
        /// <summary>
        /// 允许上传的图片最大体积(字节)
        /// </summary>
        public string imageMaxSize { get; set; }
        /// <summary>
        /// 允许上传的图片扩展名列表
        /// </summary>
        public List<string> imageAllowFiles { get; set; }
        /// <summary>
        /// 是否开启图片压缩("true"/"false")
        /// </summary>
        public string imageCompressEnable { get; set; }
        /// <summary>
        /// 图片压缩的边界尺寸，超过该尺寸的图片才进行压缩
        /// </summary>
        public string imageCompressBorder { get; set; }
        /// <summary>
        /// 插入编辑器的图片的对齐方式
        /// </summary>
        public string imageInsertAlign { get; set; }
        /// <summary>
        /// 图片访问地址的前缀
        /// </summary>
        public string imageUrlPrefix { get; set; }
        /// <summary>
        /// 上传图片的保存路径及文件名格式规则
        /// </summary>
        public string imagePathFormat { get; set; }
        /// <summary>
        /// 执行涂鸦上传的服务器端接口名称
        /// </summary>
        public string scrawlActionName { get; set; }
        /// <summary>
        /// 提交涂鸦数据的表单字段名称
        /// </summary>
        public string scrawlFieldName { get; set; }
        /// <summary>
        /// 上传涂鸦图片的保存路径格式
        /// </summary>
        public string scrawlPathFormat { get; set; }
        /// <summary>
        /// 允许上传的涂鸦最大体积(字节)
        /// </summary>
        public string scrawlMaxSize { get; set; }
        /// <summary>
        /// 涂鸦图片访问地址的前缀
        /// </summary>
        public string scrawlUrlPrefix { get; set; }
        /// <summary>
        /// 插入编辑器的涂鸦图片的对齐方式
        /// </summary>
        public string scrawlInsertAlign { get; set; }
        /// <summary>
        /// 执行截图上传的服务器端接口名称
        /// </summary>
        public string snapscreenActionName { get; set; }
        /// <summary>
        /// 上传截图的保存路径格式
        /// </summary>
        public string snapscreenPathFormat { get; set; }
        /// <summary>
        /// 截图访问地址的前缀
        /// </summary>
        public string snapscreenUrlPrefix { get; set; }
        /// <summary>
        /// 插入编辑器的截图的对齐方式
        /// </summary>
        public string snapscreenInsertAlign { get; set; }
        /// <summary>
        /// 图片抓取功能允许抓取的本地域名列表
        /// </summary>
        public List<string> catcherLocalDomain { get; set; }
        /// <summary>
        /// 执行远程图片抓取的服务器端接口名称
        /// </summary>
        public string catcherActionName { get; set; }
        /// <summary>
        /// 提交抓取地址的表单字段名称
        /// </summary>
        public string catcherFieldName { get; set; }
        /// <summary>
        /// 抓取图片的保存路径格式
        /// </summary>
        public string catcherPathFormat { get; set; }
        /// <summary>
        /// 抓取图片访问地址的前缀
        /// </summary>
        public string catcherUrlPrefix { get; set; }
        /// <summary>
        /// 允许抓取的图片最大体积(字节)
        /// </summary>
        public string catcherMaxSize { get; set; }
        /// <summary>
        /// 允许抓取的图片扩展名列表
        /// </summary>
        public List<string> catcherAllowFiles { get; set; }
        /// <summary>
        /// 执行视频上传的服务器端接口名称
        /// </summary>
        public string videoActionName { get; set; }
        /// <summary>
        /// 提交视频上传的表单字段名称
        /// </summary>
        public string videoFieldName { get; set; }
        /// <summary>
        /// 上传视频的保存路径格式
        /// </summary>
        public string videoPathFormat { get; set; }
        /// <summary>
        /// 视频访问地址的前缀
        /// </summary>
        public string videoUrlPrefix { get; set; }
        /// <summary>
        /// 允许上传的视频最大体积(字节)
        /// </summary>
        public string videoMaxSize { get; set; }
        /// <summary>
        /// 允许上传的视频扩展名列表
        /// </summary>
        public List<string> videoAllowFiles { get; set; }
        /// <summary>
        /// 执行附件上传的服务器端接口名称
        /// </summary>
        public string fileActionName { get; set; }
        /// <summary>
        /// 提交附件上传的表单字段名称
        /// </summary>
        public string fileFieldName { get; set; }
        /// <summary>
        /// 上传附件的保存路径格式
        /// </summary>
        public string filePathFormat { get; set; }
        /// <summary>
        /// 附件访问地址的前缀
        /// </summary>
        public string fileUrlPrefix { get; set; }
        /// <summary>
        /// 允许上传的附件最大体积(字节)
        /// </summary>
        public string fileMaxSize { get; set; }
        /// <summary>
        /// 允许上传的附件扩展名列表
        /// </summary>
        public List<string> fileAllowFiles { get; set; }
        /// <summary>
        /// 在线图片管理的服务器端接口名称
        /// </summary>
        public string imageManagerActionName { get; set; }
        /// <summary>
        /// 在线图片管理扫描的目录路径
        /// </summary>
        public string imageManagerListPath { get; set; }
        /// <summary>
        /// 在线图片管理每次列出的文件数量
        /// </summary>
        public string imageManagerListSize { get; set; }
        /// <summary>
        /// 在线图片访问地址的前缀
        /// </summary>
        public string imageManagerUrlPrefix { get; set; }
        /// <summary>
        /// 插入编辑器的在线图片的对齐方式
        /// </summary>
        public string imageManagerInsertAlign { get; set; }
        /// <summary>
        /// 在线图片管理允许的扩展名列表
        /// </summary>
        public List<string> imageManagerAllowFiles { get; set; }
        /// <summary>
        /// 在线文件管理的服务器端接口名称
        /// </summary>
        public string fileManagerActionName { get; set; }
        /// <summary>
        /// 在线文件管理扫描的目录路径
        /// </summary>
        public string fileManagerListPath { get; set; }
        /// <summary>
        /// 在线文件访问地址的前缀
        /// </summary>
        public string fileManagerUrlPrefix { get; set; }
        /// <summary>
        /// 在线文件管理每次列出的文件数量
        /// </summary>
        public string fileManagerListSize { get; set; }
        /// <summary>
        /// 在线文件管理允许的扩展名列表
        /// </summary>
        public List<string> fileManagerAllowFiles { get; set; }
    }
}
