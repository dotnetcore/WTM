using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 文档解析服务 — 根据文件扩展名选择解析器，将文件转为纯文本
    /// </summary>
    public class DocumentParserService
    {
        private readonly LlmService _llmService;

        /// <summary>
        /// 构造文档解析服务
        /// </summary>
        /// <param name="llmService">用于图片视觉识别的大语言模型服务，可为null</param>
        public DocumentParserService(LlmService llmService = null)
        {
            _llmService = llmService;
        }

        /// <summary>
        /// 解析上传的文件，返回纯文本和文档类型
        /// </summary>
        /// <param name="fileName">上传的文件名，用于根据扩展名选择解析器</param>
        /// <param name="fileBytes">文件的字节内容</param>
        /// <returns>解析得到的纯文本内容和文档类型组成的元组</returns>
        public async Task<(string content, DocTypeEnum docType)> ParseAsync(string fileName, byte[] fileBytes)
        {
            var ext = Path.GetExtension(fileName).ToLower();
            return ext switch
            {
                ".txt" or ".md" or ".csv" => (Encoding.UTF8.GetString(fileBytes), DocTypeEnum.Text),
                ".docx" => (ParseWord(fileBytes), DocTypeEnum.Word),
                ".xlsx" => (ParseExcel(fileBytes), DocTypeEnum.Excel),
                ".pdf" => (await ParsePdfAsync(fileBytes), DocTypeEnum.Pdf),
                ".png" or ".jpg" or ".jpeg" or ".bmp" or ".gif" or ".webp" =>
                    (await ParseImageAsync(fileBytes, fileName), DocTypeEnum.Image),
                _ => throw new NotSupportedException($"不支持的文件类型: {ext}")
            };
        }

        /// <summary>
        /// 解析Word(docx)文档，逐段提取文本
        /// </summary>
        /// <param name="bytes">文件字节内容</param>
        /// <returns>提取的纯文本</returns>
        private static string ParseWord(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            using var doc = WordprocessingDocument.Open(ms, false);
            var body = doc.MainDocumentPart?.Document?.Body;
            if (body == null) return "";
            var sb = new StringBuilder();
            foreach (var para in body.Elements<Paragraph>())
            {
                sb.AppendLine(para.InnerText);
            }
            return sb.ToString().Trim();
        }

        /// <summary>
        /// 解析Excel(xlsx)文档，按行列顺序提取所有单元格文本，列间以制表符分隔
        /// </summary>
        /// <param name="bytes">文件字节内容</param>
        /// <returns>提取的纯文本，解析失败时返回空字符串</returns>
        private static string ParseExcel(byte[] bytes)
        {
            try
            {
                using var ms = new MemoryStream(bytes);
                using var doc = SpreadsheetDocument.Open(ms, false);
                var wbp = doc.WorkbookPart;
                if (wbp == null || wbp.Workbook == null) return "";
                var sheet = wbp.Workbook.Sheets?.GetFirstChild<Sheet>();
                if (sheet == null || sheet.Id == null) return "";
                if (!wbp.TryGetPartById(sheet.Id, out var part) || part is not WorksheetPart wsp)
                    return "";
                var data = wsp.Worksheet?.GetFirstChild<SheetData>();
                if (data == null) return "";

                var sb = new StringBuilder();
                //读取共享字符串表，用于将SharedString类型的单元格值替换为实际文本
                var sharedStrings = wbp.SharedStringTablePart?.SharedStringTable?
                    .Elements<SharedStringItem>().Select(x => x.InnerText ?? "").ToList()
                    ?? new System.Collections.Generic.List<string>();

                foreach (var row in data.Elements<Row>())
            {
                foreach (var cell in row.Elements<Cell>())
                {
                    string val = cell.CellValue?.Text ?? "";
                    if (cell.DataType?.Value == CellValues.SharedString && int.TryParse(val, out var idx) && idx < sharedStrings.Count)
                        val = sharedStrings[idx];
                    sb.Append(val).Append('\t');
                }
                sb.AppendLine();
                }
                return sb.ToString().Trim();
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 解析PDF文档（尚未接入PDF解析组件，当前仅返回提示文本）
        /// </summary>
        /// <param name="bytes">文件字节内容</param>
        /// <returns>提示信息文本</returns>
        private static Task<string> ParsePdfAsync(byte[] bytes)
        {
            // PDF 解析需要 UglyToad.PdfPig NuGet 包，暂返回提示
            return Task.FromResult("[PDF 内容，需要添加 PdfPig 包解析]");
        }

        /// <summary>
        /// 解析图片文件，调用配置的大语言模型视觉能力识别图片内容并转为文字描述
        /// </summary>
        /// <param name="bytes">图片字节内容</param>
        /// <param name="fileName">图片文件名</param>
        /// <returns>图片内容的文字描述，识别失败或未配置模型时返回提示信息</returns>
        private async Task<string> ParseImageAsync(byte[] bytes, string fileName)
        {
            if (_llmService == null)
                return $"[图片: {fileName}，需要配置 LLM 视觉模型进行识别]";

            try
            {
                var base64 = Convert.ToBase64String(bytes);
                var ext = Path.GetExtension(fileName).TrimStart('.').ToLower();
                var mime = ext switch
                {
                    "png" => "image/png",
                    "jpg" or "jpeg" => "image/jpeg",
                    "gif" => "image/gif",
                    "webp" => "image/webp",
                    _ => "image/png"
                };

                var prompt = "请详细描述这张图片中的内容。如果是思维导图、流程图、架构图等，请将其转化为结构化的文字描述，保留原始逻辑层次关系。如果是表格图片，请将表格内容转为文字。如果是普通图片，请描述图片中的场景和文字。";
                var imageUrl = $"data:{mime};base64,{base64}";

                return await _llmService.ChatWithImageAsync(prompt, imageUrl);
            }
            catch
            {
                return $"[图片: {fileName}，视觉识别失败]";
            }
        }
    }
}
