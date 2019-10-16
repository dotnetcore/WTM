using System.Collections.Generic;

namespace WalkingTec.Mvvm.Mvc.UEditor
{
    internal class UEditorConfigJson
    {
        public string imageActionName { get; set; }
        public string imageFieldName { get; set; }
        public string imageMaxSize { get; set; }
        public List<string> imageAllowFiles { get; set; }
        public string imageCompressEnable { get; set; }
        public string imageCompressBorder { get; set; }
        public string imageInsertAlign { get; set; }
        public string imageUrlPrefix { get; set; }
        public string imagePathFormat { get; set; }
        public string scrawlActionName { get; set; }
        public string scrawlFieldName { get; set; }
        public string scrawlPathFormat { get; set; }
        public string scrawlMaxSize { get; set; }
        public string scrawlUrlPrefix { get; set; }
        public string scrawlInsertAlign { get; set; }
        public string snapscreenActionName { get; set; }
        public string snapscreenPathFormat { get; set; }
        public string snapscreenUrlPrefix { get; set; }
        public string snapscreenInsertAlign { get; set; }
        public List<string> catcherLocalDomain { get; set; }
        public string catcherActionName { get; set; }
        public string catcherFieldName { get; set; }
        public string catcherPathFormat { get; set; }
        public string catcherUrlPrefix { get; set; }
        public string catcherMaxSize { get; set; }
        public List<string> catcherAllowFiles { get; set; }
        public string videoActionName { get; set; }
        public string videoFieldName { get; set; }
        public string videoPathFormat { get; set; }
        public string videoUrlPrefix { get; set; }
        public string videoMaxSize { get; set; }
        public List<string> videoAllowFiles { get; set; }
        public string fileActionName { get; set; }
        public string fileFieldName { get; set; }
        public string filePathFormat { get; set; }
        public string fileUrlPrefix { get; set; }
        public string fileMaxSize { get; set; }
        public List<string> fileAllowFiles { get; set; }
        public string imageManagerActionName { get; set; }
        public string imageManagerListPath { get; set; }
        public string imageManagerListSize { get; set; }
        public string imageManagerUrlPrefix { get; set; }
        public string imageManagerInsertAlign { get; set; }
        public List<string> imageManagerAllowFiles { get; set; }
        public string fileManagerActionName { get; set; }
        public string fileManagerListPath { get; set; }
        public string fileManagerUrlPrefix { get; set; }
        public string fileManagerListSize { get; set; }
        public List<string> fileManagerAllowFiles { get; set; }
    }
}
