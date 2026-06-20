using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// OpenXML Excel 辅助类
    /// </summary>
    public static class OpenXmlHelper
    {
        /// <summary>
        /// 获取或创建 WorkbookPart
        /// </summary>
        public static WorkbookPart GetOrCreateWorkbookPart(SpreadsheetDocument doc)
        {
            if (doc.WorkbookPart == null)
            {
                doc.AddWorkbookPart();
                doc.WorkbookPart.Workbook = new Workbook();
                Sheets sheets = new Sheets();
                doc.WorkbookPart.Workbook.AppendChild(sheets);
                doc.WorkbookPart.Workbook.Save();
            }
            return doc.WorkbookPart;
        }

        /// <summary>
        /// 获取或创建 WorksheetPart
        /// </summary>
        public static WorksheetPart GetOrCreateWorksheetPart(SpreadsheetDocument doc, uint sheetIndex)
        {
            var workbookPart = GetOrCreateWorkbookPart(doc);
            Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();

            Sheet sheet = sheets.Elements<Sheet>().FirstOrDefault(s => s.SheetId == sheetIndex);
            if (sheet != null)
            {
                return (WorksheetPart)workbookPart.GetPartById(sheet.Id);
            }

            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            uint maxSheetId = sheets.Elements<Sheet>().Any()
                ? sheets.Elements<Sheet>().Max(s => s.SheetId.Value)
                : 0u;

            Sheet newSheet = new Sheet()
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = maxSheetId + 1,
                Name = $"Sheet{maxSheetId + 1}"
            };
            sheets.AppendChild(newSheet);
            workbookPart.Workbook.Save();

            return worksheetPart;
        }

        /// <summary>
        /// 获取 SheetData
        /// </summary>
        public static SheetData GetSheetData(WorksheetPart worksheetPart)
        {
            return worksheetPart.Worksheet.GetFirstChild<SheetData>()
                ?? worksheetPart.Worksheet.AppendChild(new SheetData());
        }

        /// <summary>
        /// 获取或创建行
        /// </summary>
        public static Row GetOrCreateRow(SheetData sheetData, int rowIndex)
        {
            Row row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == (uint)rowIndex);
            if (row == null)
            {
                row = new Row() { RowIndex = (uint)rowIndex };
                // 插入到正确位置
                var rows = sheetData.Elements<Row>().ToList();
                int insertIndex = rows.FindIndex(r => r.RowIndex > (uint)rowIndex);
                if (insertIndex >= 0)
                    sheetData.InsertAt(row, insertIndex);
                else
                    sheetData.AppendChild(row);
            }
            return row;
        }

        /// <summary>
        /// 创建单元格
        /// </summary>
        public static Cell CreateCell(string reference, string value, CellValues dataType, uint? styleIndex = null)
        {
            var cell = new Cell() { CellReference = reference };
            if (styleIndex.HasValue)
                cell.StyleIndex = styleIndex.Value;

            if (dataType == CellValues.String)
            {
                cell.DataType = CellValues.InlineString;
                cell.AppendChild(new InlineString(new Text(value ?? "")));
            }
            else if (dataType == CellValues.Number)
            {
                cell.DataType = CellValues.Number;
                cell.CellValue = new CellValue(value ?? "0");
            }
            else if (dataType == CellValues.Date)
            {
                cell.DataType = CellValues.Date;
                cell.CellValue = new CellValue(value ?? "");
            }
            else
            {
                cell.DataType = CellValues.String;
                cell.CellValue = new CellValue(value ?? "");
            }

            return cell;
        }

        /// <summary>
        /// 获取列字母（0=A, 1=B, ..., 25=Z, 26=AA, ...）
        /// </summary>
        public static string GetColumnLetter(int columnIndex)
        {
            string letter = "";
            while (columnIndex >= 0)
            {
                letter = (char)('A' + columnIndex % 26) + letter;
                columnIndex = columnIndex / 26 - 1;
            }
            return letter;
        }

        /// <summary>
        /// 获取单元格引用字符串
        /// </summary>
        public static string GetCellReference(int columnIndex, int rowIndex)
        {
            return GetColumnLetter(columnIndex) + (rowIndex + 1);
        }

        /// <summary>
        /// 创建标题样式
        /// </summary>
        public static Stylesheet CreateStylesheet(string headerBackColor = "B0C4DE", string headerFontColor = "000000")
        {
            var stylesheet = new Stylesheet();

            // 字体: 0=默认, 1=标题
            var fonts = new Fonts(
                new Font(), // 0 - 默认
                new Font(  // 1 - 标题
                    new FontName() { Val = "Calibri" },
                    new FontSize() { Val = 12 },
                    new Color() { Rgb = headerFontColor }
                )
            );

            // 填充: 0=无, 1=灰色背景
            var fills = new Fills(
                new Fill(new PatternFill() { PatternType = PatternValues.None }),     // 0
                new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }),   // 1
                new Fill(new PatternFill(                                             // 2 - 标题背景色
                    new BackgroundColor() { Rgb = headerBackColor },
                    new ForegroundColor() { Rgb = headerBackColor }
                ) { PatternType = PatternValues.Solid })
            );

            // 边框: 0=无, 1=细边框
            var borders = new Borders(
                new Border(), // 0 - 无边框
                new Border(   // 1 - 四面细边框
                    new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new DiagonalBorder()
                )
            );

            // 单元格格式: 0=默认, 1=标题样式, 2=数据样式(带边框)
            var cellFormats = new CellFormats(
                new CellFormat(),                                                     // 0 - 默认
                new CellFormat(                                                        // 1 - 标题
                    new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }
                ) { FontId = 1, FillId = 2, BorderId = 1, ApplyFont = true, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true },
                new CellFormat(                                                        // 2 - 数据单元格(带边框)
                    new Alignment() { Vertical = VerticalAlignmentValues.Center, WrapText = true }
                ) { FontId = 0, FillId = 0, BorderId = 1, ApplyFont = true, ApplyBorder = true, ApplyAlignment = true }
            );

            stylesheet.AppendChild(fonts);
            stylesheet.AppendChild(fills);
            stylesheet.AppendChild(borders);
            stylesheet.AppendChild(cellFormats);

            return stylesheet;
        }

        /// <summary>
        /// 从内存流创建 SpreadsheetDocument 用于读取
        /// </summary>
        public static SpreadsheetDocument OpenForReading(Stream stream)
        {
            return SpreadsheetDocument.Open(stream, false);
        }

        /// <summary>
        /// 创建新的 SpreadsheetDocument 用于写入
        /// </summary>
        public static SpreadsheetDocument CreateForWriting(MemoryStream ms)
        {
            return SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook);
        }
    }
}
