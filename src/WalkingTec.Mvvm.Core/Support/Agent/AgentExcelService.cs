using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using WalkingTec.Mvvm.Core.Support.FileHandlers;

namespace WalkingTec.Mvvm.Core.Support.Agent
{
    /// <summary>
    /// Agent生成Excel的核心构建器：把表头/行数据/样式选项生成为xlsx字节数组，
    /// 并保存为框架附件返回下载信息。沿用ListVM导出的OpenXML实现方式。
    /// 数据的来源（模型内联提供或服务端查询）由调用方解析后传入
    /// </summary>
    public static class AgentExcelService
    {
        /// <summary>
        /// 单次导出的最大行数，防止生成超大文件
        /// </summary>
        public const int MaxRows = 500;

        /// <summary>
        /// 生成xlsx并保存为框架附件
        /// </summary>
        /// <param name="wtm">WTM上下文</param>
        /// <param name="baseUrl">本服务基址，用于拼接下载链接</param>
        /// <param name="fileName">文件名（.xlsx结尾）</param>
        /// <param name="title">大标题，空则不加标题行</param>
        /// <param name="headers">列名</param>
        /// <param name="rows">行数据</param>
        /// <param name="style">样式选项，空用默认</param>
        /// <returns>含下载链接的结果对象（回填给模型）</returns>
        public static object GenerateAndSave(WTMContext wtm, string baseUrl, string fileName, string title,
            List<string> headers, List<List<string>> rows, JsonElement? style)
        {
            if (headers == null || headers.Count == 0 || rows == null || rows.Count == 0)
                return new { error = "没有可导出的数据行" };

            string headerBg = Hex(style, "headerBackColor") ?? "4472C4";
            string headerFont = Hex(style, "headerFontColor") ?? "FFFFFF";
            bool zebra = Bool(style, "zebra", true);
            bool border = Bool(style, "border", true);
            bool freeze = Bool(style, "freezeHeader", true);
            bool autoWidth = Bool(style, "autoWidth", true);

            var bytes = BuildXlsx(title, headers, rows, headerBg, headerFont, zebra, border, freeze, autoWidth);

            //保存为框架附件并生成下载链接
            var fp = wtm?.ServiceProvider?.GetService(typeof(WtmFileProvider)) as WtmFileProvider ?? new WtmFileProvider(wtm);
            string fileId;
            using (var ms = new MemoryStream(bytes))
            {
                var file = fp.Upload(fileName, bytes.Length, ms, null, null, null, null, wtm?.CreateDC());
                fileId = file.GetID();
            }
            var url = $"{baseUrl?.TrimEnd('/')}/api/admin/FileApi/getfile/{fileId}";
            return new
            {
                message = $"已生成Excel《{fileName}》：{rows.Count}行×{headers.Count}列",
                fileName,
                rowCount = rows.Count,
                columnCount = headers.Count,
                url,
                tip = "把url作为下载链接告诉用户（登录状态下可直接打开）",
            };
        }

        /// <summary>
        /// 用OpenXML生成xlsx：可选大标题行（合并单元格）、表头样式、隔行变色、边框、冻结表头、列宽自适应
        /// </summary>
        private static byte[] BuildXlsx(string title, List<string> headers, List<List<string>> rows,
            string headerBg, string headerFont, bool zebra, bool border, bool freeze, bool autoWidth)
        {
            using var ms = new MemoryStream();
            using (var doc = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                var wbPart = doc.AddWorkbookPart();
                wbPart.Workbook = new Workbook();
                wbPart.Workbook.AppendChild(new Sheets());

                var stylesPart = wbPart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = BuildStylesheet(headerBg, headerFont, zebra, border);
                stylesPart.Stylesheet.Save();

                var wsPart = wbPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                var sheetViews = new SheetViews(new SheetView { WorkbookViewId = 0 });
                wsPart.Worksheet = new Worksheet(sheetViews, sheetData);
                var sheet = new Sheet { Id = wbPart.GetIdOfPart(wsPart), SheetId = 1, Name = "数据" };
                wbPart.Workbook.GetFirstChild<Sheets>().AppendChild(sheet);

                bool hasTitle = !string.IsNullOrWhiteSpace(title);
                uint rowIndex = 1;

                //标题行：整行合并居中
                if (hasTitle)
                {
                    var titleRow = new Row { RowIndex = rowIndex };
                    for (int c = 0; c < headers.Count; c++)
                    {
                        var cell = new Cell { CellReference = $"{ColName(c)}{rowIndex}", StyleIndex = 3 };
                        if (c == 0)
                        {
                            cell.DataType = CellValues.InlineString;
                            cell.AppendChild(new InlineString(new Text(title)));
                        }
                        titleRow.AppendChild(cell);
                    }
                    sheetData.AppendChild(titleRow);
                    rowIndex++;
                }

                //表头行
                var headerRow = new Row { RowIndex = rowIndex };
                for (int c = 0; c < headers.Count; c++)
                {
                    var cell = new Cell { CellReference = $"{ColName(c)}{rowIndex}", DataType = CellValues.InlineString, StyleIndex = 1 };
                    cell.AppendChild(new InlineString(new Text(headers[c] ?? "")));
                    headerRow.AppendChild(cell);
                }
                sheetData.AppendChild(headerRow);
                var headerRowIndex = rowIndex;
                rowIndex++;

                //数据行（隔行变色）
                uint dataStart = rowIndex;
                int dataIndex = 0;
                foreach (var row in rows)
                {
                    var dataRow = new Row { RowIndex = rowIndex };
                    var styleIdx = zebra && dataIndex % 2 == 1 ? 2u : 0u;
                    for (int c = 0; c < headers.Count; c++)
                    {
                        var text = c < row.Count ? row[c] ?? "" : "";
                        var cell = new Cell { CellReference = $"{ColName(c)}{rowIndex}", StyleIndex = styleIdx };
                        if (IsPureNumber(text))
                        {
                            cell.DataType = CellValues.Number;
                            cell.CellValue = new CellValue(double.Parse(text));
                        }
                        else
                        {
                            cell.DataType = CellValues.InlineString;
                            cell.AppendChild(new InlineString(new Text(text)));
                        }
                        dataRow.AppendChild(cell);
                    }
                    sheetData.AppendChild(dataRow);
                    rowIndex++;
                    dataIndex++;
                }

                //标题行合并单元格
                if (hasTitle)
                {
                    var mergeCells = new MergeCells();
                    mergeCells.AppendChild(new MergeCell { Reference = $"A1:{ColName(headers.Count - 1)}1" });
                    wsPart.Worksheet.InsertAfter(mergeCells, sheetData);
                }

                //冻结标题+表头
                if (freeze)
                {
                    var frozenRows = (int)headerRowIndex;
                    var pane = new Pane
                    {
                        VerticalSplit = frozenRows,
                        TopLeftCell = $"A{frozenRows + 1}",
                        ActivePane = PaneValues.BottomLeft,
                        State = PaneStateValues.Frozen,
                    };
                    sheetViews.GetFirstChild<SheetView>().AppendChild(pane);
                }

                //列宽自适应：按表头与内容的最大字符长度估算（中文按2倍宽）
                if (autoWidth)
                {
                    var widths = new double[headers.Count];
                    for (int c = 0; c < headers.Count; c++)
                    {
                        double max = DisplayWidth(headers[c] ?? "");
                        foreach (var row in rows)
                            if (c < row.Count)
                                max = Math.Max(max, DisplayWidth(row[c] ?? ""));
                        widths[c] = Math.Min(60, Math.Max(10, max + 4));
                    }
                    var cols = new Columns();
                    for (int c = 0; c < headers.Count; c++)
                        cols.AppendChild(new Column { Min = (uint)(c + 1), Max = (uint)(c + 1), Width = widths[c], CustomWidth = true });
                    //Columns必须位于SheetData之前、SheetViews之后
                    wsPart.Worksheet.InsertAfter(cols, sheetViews);
                }

                wbPart.Workbook.Save();
            }
            return ms.ToArray();
        }

        /// <summary>
        /// 生成样式表：0=普通数据 1=表头（底色+白粗体）2=隔行浅色 3=大标题
        /// </summary>
        private static Stylesheet BuildStylesheet(string headerBg, string headerFont, bool zebra, bool border)
        {
            var borders = new Borders();
            borders.AppendChild(new Border()); //0 无边框
            var line = new Border(
                new LeftBorder(new DocumentFormat.OpenXml.Spreadsheet.Color { Auto = true }) { Style = BorderStyleValues.Thin },
                new RightBorder(new DocumentFormat.OpenXml.Spreadsheet.Color { Auto = true }) { Style = BorderStyleValues.Thin },
                new TopBorder(new DocumentFormat.OpenXml.Spreadsheet.Color { Auto = true }) { Style = BorderStyleValues.Thin },
                new BottomBorder(new DocumentFormat.OpenXml.Spreadsheet.Color { Auto = true }) { Style = BorderStyleValues.Thin },
                new DiagonalBorder());
            borders.AppendChild(line); //1 全边框
            uint borderIdx = border ? 1u : 0u;

            var fills = new Fills();
            fills.AppendChild(new Fill(new PatternFill { PatternType = PatternValues.None })); //0
            fills.AppendChild(new Fill(new PatternFill { PatternType = PatternValues.Gray125 })); //1
            fills.AppendChild(new Fill(new PatternFill(new ForegroundColor { Rgb = headerBg }) { PatternType = PatternValues.Solid })); //2 表头底色
            uint zebraFillIdx = 3;
            fills.AppendChild(new Fill(new PatternFill(new ForegroundColor { Rgb = "F2F6FC" }) { PatternType = PatternValues.Solid })); //3 隔行浅蓝
            if (zebra == false)
                zebraFillIdx = 0;

            var fonts = new Fonts();
            fonts.AppendChild(new Font(new FontSize { Val = 11 }, new DocumentFormat.OpenXml.Spreadsheet.Color { Rgb = "000000" }, new FontName { Val = "微软雅黑" })); //0
            fonts.AppendChild(new Font(new Bold(), new FontSize { Val = 11 }, new DocumentFormat.OpenXml.Spreadsheet.Color { Rgb = headerFont }, new FontName { Val = "微软雅黑" })); //1 表头
            fonts.AppendChild(new Font(new Bold(), new FontSize { Val = 14 }, new DocumentFormat.OpenXml.Spreadsheet.Color { Rgb = "000000" }, new FontName { Val = "微软雅黑" })); //2 标题

            var cellFormats = new CellFormats();
            cellFormats.AppendChild(new CellFormat { FontId = 0, FillId = 0, BorderId = borderIdx, Alignment = new Alignment { Vertical = VerticalAlignmentValues.Center } }); //0
            cellFormats.AppendChild(new CellFormat { FontId = 1, FillId = 2, BorderId = borderIdx, Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center } }); //1
            cellFormats.AppendChild(new CellFormat { FontId = 0, FillId = zebraFillIdx, BorderId = borderIdx, Alignment = new Alignment { Vertical = VerticalAlignmentValues.Center } }); //2
            cellFormats.AppendChild(new CellFormat { FontId = 2, FillId = 0, BorderId = 0, Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center } }); //3

            return new Stylesheet(fonts, fills, borders, cellFormats);
        }

        /// <summary>
        /// 判断文本是否为可直接写为数值单元格的数字：
        /// 允许普通数值，排除带前导零的编号（如001）、过长数字串（如身份证/手机号按文本保留更安全时由长度上限控制）
        /// </summary>
        private static bool IsPureNumber(string text)
        {
            if (string.IsNullOrEmpty(text) || text.Length > 15)
                return false;
            var span = text.AsSpan();
            bool hasDigit = false;
            for (int i = 0; i < span.Length; i++)
            {
                char ch = span[i];
                if (ch == '-')
                {
                    if (i > 0)
                        return false;
                }
                else if (ch == '.')
                {
                    if (i == 0 || i == span.Length - 1)
                        return false;
                }
                else if (ch < '0' || ch > '9')
                {
                    return false;
                }
                else
                {
                    hasDigit = true;
                }
            }
            //排除"012"这类前导零编号
            if (hasDigit && text.Length > 1 && text[0] == '0' && text[1] != '.')
                return false;
            return hasDigit;
        }

        /// <summary>
        /// 估算文本显示宽度：中文等全角字符按2、其他按1
        /// </summary>
        private static double DisplayWidth(string s)
        {
            double w = 0;
            foreach (var c in s)
                w += c > 127 ? 2 : 1;
            return w;
        }

        private static string Hex(JsonElement? e, string name)
        {
            if (e == null || e.Value.ValueKind != JsonValueKind.Object)
                return null;
            if (e.Value.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String)
            {
                var s = v.GetString()?.TrimStart('#');
                return string.IsNullOrEmpty(s) ? null : s;
            }
            return null;
        }

        private static bool Bool(JsonElement? e, string name, bool def)
        {
            if (e == null || e.Value.ValueKind != JsonValueKind.Object)
                return def;
            if (!e.Value.TryGetProperty(name, out var v))
                return def;
            if (v.ValueKind == JsonValueKind.True)
                return true;
            if (v.ValueKind == JsonValueKind.False)
                return false;
            return def;
        }

        /// <summary>
        /// 列序号转Excel列名（0→A，25→Z，26→AA）
        /// </summary>
        private static string ColName(int index)
        {
            var sb = new StringBuilder();
            index++;
            while (index > 0)
            {
                index--;
                sb.Insert(0, (char)('A' + index % 26));
                index /= 26;
            }
            return sb.ToString();
        }
    }
}
