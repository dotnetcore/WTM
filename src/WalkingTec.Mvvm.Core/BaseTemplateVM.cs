using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;

namespace WalkingTec.Mvvm.Core
{
    public class BaseTemplateVM : BaseVM
    {
        #region 属性
        /// <summary>
        /// 下载模板显示名称
        /// </summary>
        public string FileDisplayName { get; set; }

        /// <summary>
        /// 是否验证模板类型（当其他系统模板导入到某模块时可设置为False）
        /// </summary>
        public bool ValidityTemplateType { get; set; }

        /// <summary>
        /// 需要导出的数据
        /// </summary>
        public DataTable TemplateDataTable { get; set; }

        /// <summary>
        /// 下载模版页面参数
        /// </summary>
        public Dictionary<string, string> Parms { get; set; }

        /// <summary>
        /// Excel索引
        /// </summary>
        public long ExcelIndex { get; set; }
        #endregion

        #region 构造函数
        public BaseTemplateVM()
        {
            ValidityTemplateType = true;
            Parms = new Dictionary<string, string>();
            var propetys = this.GetType().GetFields().Where(x => x.FieldType == typeof(ExcelPropety)).ToList();
            for (int porpetyIndex = 0; porpetyIndex < propetys.Count(); porpetyIndex++)
            {
                ExcelPropety excelPropety = (ExcelPropety)propetys[porpetyIndex].GetValue(this);
                if (propetys[porpetyIndex].GetCustomAttributes(typeof(DisplayAttribute), false).Length == 0)
                {
                    excelPropety.ColumnName = excelPropety.FieldDisplayName;
                }
                else
                {
                    excelPropety.ColumnName = propetys[porpetyIndex].GetPropertyDisplayName();
                }
            }
        }
        #endregion

        #region 初始化Excel属性数据
        /// <summary>
        /// 初始化Excel属性数据  包括动态列,列表中的下拉选项
        /// </summary>
        public virtual void InitExcelData()
        {

        }

        public virtual void InitCustomFormat()
        {

        }

        #endregion

        #region  初始化模版数据
        /// <summary>
        /// 初始化模版数据
        /// </summary>
        public virtual void SetTemplateDataValus()
        {

        }
        #endregion

        #region  生成模板
        /// <summary>
        /// 生成模板
        /// </summary>
        /// <param name="displayName">文件名</param>
        /// <returns>生成的模版文件</returns>
        public byte[] GenerateTemplate(out string displayName)
        {
            //设置导出的文件名称
            string SheetName = !string.IsNullOrEmpty(FileDisplayName) ? FileDisplayName : this.GetType().Name;
            displayName = SheetName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + "_" + DateTime.Now.ToString("hh^mm^ss") + ".xlsx";

            //加载初始化数据和下拉菜单数据，可重载
            InitExcelData();

            //设置TemplateDataTable的各列的类型
            CreateDataTable();

            //设置初始化数据到DataTable中
            SetTemplateDataValus();

            using (MemoryStream ms = new MemoryStream())
            {
                using (SpreadsheetDocument doc = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = doc.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();
                    workbookPart.Workbook.AppendChild(new Sheets());

                    // 创建样式表
                    var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylesPart.Stylesheet = CreateTemplateStylesheet();
                    stylesPart.Stylesheet.Save();

                    // 创建主Sheet
                    WorksheetPart sheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    sheetPart.Worksheet = new Worksheet(new SheetData());
                    SheetData sheetData = sheetPart.Worksheet.GetFirstChild<SheetData>();

                    Sheet sheet = new Sheet()
                    {
                        Id = workbookPart.GetIdOfPart(sheetPart),
                        SheetId = 1,
                        Name = SheetName
                    };
                    workbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(sheet);

                    // 创建第二Sheet（枚举/布尔参考数据）
                    WorksheetPart enumSheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    enumSheetPart.Worksheet = new Worksheet(new SheetData());
                    SheetData enumSheetData = enumSheetPart.Worksheet.GetFirstChild<SheetData>();
                    Row enumRow = new Row() { RowIndex = 1 };
                    // enumSheet: A1=Yes, B1=No, C1=template type marker
                    enumRow.AppendChild(new Cell()
                    {
                        CellReference = "A1",
                        DataType = CellValues.InlineString,
                        InlineString = new InlineString(new Text(CoreProgram._localizer?["Sys.Yes"] ?? "Yes"))
                    });
                    enumRow.AppendChild(new Cell()
                    {
                        CellReference = "B1",
                        DataType = CellValues.InlineString,
                        InlineString = new InlineString(new Text(CoreProgram._localizer?["Sys.No"] ?? "No"))
                    });
                    enumRow.AppendChild(new Cell()
                    {
                        CellReference = "C1",
                        DataType = CellValues.InlineString,
                        InlineString = new InlineString(new Text(this.GetType().Name))
                    });
                    enumSheetData.AppendChild(enumRow);

                    Sheet enumSheet = new Sheet()
                    {
                        Id = workbookPart.GetIdOfPart(enumSheetPart),
                        SheetId = 2,
                        Name = "Sheet2"
                    };
                    workbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(enumSheet);

                    // 创建第三Sheet（数据Sheet - 给下拉框用）
                    WorksheetPart dataSheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    dataSheetPart.Worksheet = new Worksheet(new SheetData());
                    SheetData dataSheetData = dataSheetPart.Worksheet.GetFirstChild<SheetData>();

                    Sheet dataSheet = new Sheet()
                    {
                        Id = workbookPart.GetIdOfPart(dataSheetPart),
                        SheetId = 3,
                        Name = "Sheet3"
                    };
                    workbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(dataSheet);

                    // 设置列头
                    var propetys = this.GetType().GetFields().Where(x => x.FieldType == typeof(ExcelPropety)).ToList();
                    int _currentColunmIndex = 0;
                    bool IsProtect = false;

                    Row headerRow = new Row() { RowIndex = 1, Height = 20 };

                    for (int porpetyIndex = 0; porpetyIndex < propetys.Count(); porpetyIndex++)
                    {
                        ExcelPropety excelPropety = (ExcelPropety)propetys[porpetyIndex].GetValue(this);
                        ColumnDataType dateType = (excelPropety.DataType == ColumnDataType.DateTime || excelPropety.DataType == ColumnDataType.Date) ? ColumnDataType.Text : excelPropety.DataType;

                        if (excelPropety.ReadOnly)
                            IsProtect = true;

                        string colName = excelPropety.IsNullAble ? excelPropety.ColumnName : excelPropety.ColumnName + "*";

                        // 确定表头样式索引: 1=灰, 2=黄, 3=红
                        uint headerStyleIndex = 1;
                        switch (excelPropety.BackgroudColor)
                        {
                            case BackgroudColorEnum.Yellow: headerStyleIndex = 2; break;
                            case BackgroudColorEnum.Red: headerStyleIndex = 3; break;
                        }

                        string cellRef = OpenXmlHelper.GetCellReference(_currentColunmIndex, 0);
                        var headerCell = new Cell() { CellReference = cellRef, StyleIndex = headerStyleIndex };
                        headerCell.DataType = CellValues.InlineString;
                        headerCell.AppendChild(new InlineString(new Text(colName)));
                        headerRow.AppendChild(headerCell);

                        // 设置数据单元格样式与验证
                        uint? numberFormatId = null;
                        bool isLocked = false;
                        excelPropety.SetColumnFormat(dateType, _currentColunmIndex, sheetPart, dataSheetPart,
                            workbookPart, SheetName, "Sheet3", out numberFormatId, out isLocked);
                        if (isLocked) IsProtect = true;

                        // 设置列宽
                        Columns columns = sheetPart.Worksheet.GetFirstChild<Columns>();
                        if (columns == null)
                        {
                            columns = new Columns();
                            sheetPart.Worksheet.InsertBefore(columns, sheetPart.Worksheet.GetFirstChild<SheetData>());
                        }
                        uint colNum = (uint)(_currentColunmIndex + 1);
                        double width = excelPropety.CharCount > 0 ? excelPropety.CharCount : 15;
                        columns.AppendChild(new Column() { Min = colNum, Max = colNum, Width = width, CustomWidth = true });

                        _currentColunmIndex++;
                    }
                    sheetData.AppendChild(headerRow);

                    // 添加模版数据
                    if (TemplateDataTable?.Rows.Count > 0)
                    {
                        for (int i = 0; i < TemplateDataTable.Rows.Count; i++)
                        {
                            DataRow tableRow = TemplateDataTable.Rows[i];
                            Row dataRow = new Row() { RowIndex = (uint)(i + 2) };
                            for (int porpetyIndex = 0; porpetyIndex < propetys.Count(); porpetyIndex++)
                            {
                                string colName = propetys[porpetyIndex].Name;
                                string cellRef2 = OpenXmlHelper.GetCellReference(porpetyIndex, i + 1);
                                var dataCell = OpenXmlHelper.CreateCell(cellRef2, tableRow[colName].ToString(), CellValues.String);
                                dataRow.AppendChild(dataCell);
                            }
                            sheetData.AppendChild(dataRow);
                        }
                    }

                    // 冻结窗格
                    var sheetProperties = new SheetProperties();
                    sheetProperties.AppendChild(new Pane()
                    {
                        VerticalSplit = 1,
                        HorizontalSplit = 0,
                        TopLeftCell = "A2",
                        ActivePane = PaneValues.BottomLeft,
                        State = PaneStateValues.Frozen
                    });
                    sheetPart.Worksheet.AppendChild(sheetProperties);

                    // 保护Sheet
                    if (IsProtect)
                    {
                        sheetPart.Worksheet.AppendChild(new SheetProtection()
                        {
                            Sheet = true,
                            Password = "password"
                        });
                    }

                    // 隐藏第2和第3个Sheet
                    workbookPart.Workbook.AppendChild(new BookViews(new WorkbookView() { ShowHorizontalScroll = true }));
                    enumSheet.State = SheetStateValues.Hidden;
                    dataSheet.State = SheetStateValues.Hidden;

                    workbookPart.Workbook.Save();
                }
                return ms.ToArray();
            }
        }

        private static Stylesheet CreateTemplateStylesheet()
        {
            var stylesheet = new Stylesheet();

            // 字体
            var fonts = new Fonts(
                new Font(), // 0 - 默认
                new Font(new FontName() { Val = "Calibri" }, new FontSize() { Val = 11 }) // 1 - 标题
            );

            // 填充
            var fills = new Fills(
                new Fill(new PatternFill() { PatternType = PatternValues.None }),
                new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }),
                new Fill(new PatternFill(new ForegroundColor() { Rgb = new HexBinaryValue("D9E2F3") }) { PatternType = PatternValues.Solid }),  // 2 - 灰色
                new Fill(new PatternFill(new ForegroundColor() { Rgb = new HexBinaryValue("FFF2CC") }) { PatternType = PatternValues.Solid }),  // 3 - 黄色
                new Fill(new PatternFill(new ForegroundColor() { Rgb = new HexBinaryValue("FCE4EC") }) { PatternType = PatternValues.Solid })   // 4 - 红色
            );

            // 边框
            var borders = new Borders(
                new Border(), // 0
                new Border(   // 1 - 细边框
                    new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new DiagonalBorder()
                )
            );

            // 数字格式: 165+为自定义格式
            var numberingFormats = new NumberingFormats(
                new NumberingFormat() { NumberFormatId = 165, FormatCode = "yyyy-MM-dd HH:mm:ss" }
            );

            // 单元格格式
            var cellFormats = new CellFormats(
                new CellFormat(), // 0 - 默认
                new CellFormat(   // 1 - 灰色表头
                    new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center, WrapText = true }
                ) { FontId = 1, FillId = 2, BorderId = 1, ApplyFont = true, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true },
                new CellFormat(   // 2 - 黄色表头
                    new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center, WrapText = true }
                ) { FontId = 1, FillId = 3, BorderId = 1, ApplyFont = true, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true },
                new CellFormat(   // 3 - 红色表头
                    new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center, WrapText = true }
                ) { FontId = 1, FillId = 4, BorderId = 1, ApplyFont = true, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true },
                new CellFormat(   // 4 - 数据单元格(文本)
                    new Alignment() { WrapText = true }
                ) { FontId = 0, FillId = 0, BorderId = 1, NumberFormatId = 49, ApplyNumberFormat = true, ApplyFont = true, ApplyBorder = true, ApplyAlignment = true },
                new CellFormat(   // 5 - 数据单元格(数字)
                    new Alignment() { WrapText = true }
                ) { FontId = 0, FillId = 0, BorderId = 1, NumberFormatId = 1, ApplyNumberFormat = true, ApplyFont = true, ApplyBorder = true, ApplyAlignment = true },
                new CellFormat(   // 6 - 数据单元格(小数)
                    new Alignment() { WrapText = true }
                ) { FontId = 0, FillId = 0, BorderId = 1, NumberFormatId = 4, ApplyNumberFormat = true, ApplyFont = true, ApplyBorder = true, ApplyAlignment = true },
                new CellFormat(   // 7 - 数据单元格(日期)
                    new Alignment() { WrapText = true }
                ) { FontId = 0, FillId = 0, BorderId = 1, NumberFormatId = 165, ApplyNumberFormat = true, ApplyFont = true, ApplyBorder = true, ApplyAlignment = true }
            );

            stylesheet.AppendChild(numberingFormats);
            stylesheet.AppendChild(fonts);
            stylesheet.AppendChild(fills);
            stylesheet.AppendChild(borders);
            stylesheet.AppendChild(cellFormats);

            return stylesheet;
        }
        #endregion

        #region 初始化DataTable(不含动态列)
        private void CreateDataTable()
        {
            TemplateDataTable = new DataTable();
            var propetys = this.GetType().GetFields().Where(x => x.FieldType == typeof(ExcelPropety)).ToList();
            foreach (var p in propetys)
            {
                ExcelPropety excelPropety = (ExcelPropety)p.GetValue(this);
                ColumnDataType dateType = excelPropety.DataType;
                switch (dateType)
                {
                    case ColumnDataType.Bool:
                        TemplateDataTable.Columns.Add(p.Name, typeof(bool));
                        break;
                    case ColumnDataType.Date:
                        TemplateDataTable.Columns.Add(p.Name, typeof(string));
                        break;
                    case ColumnDataType.Number:
                        TemplateDataTable.Columns.Add(p.Name, typeof(int));
                        break;
                    case ColumnDataType.Text:
                        TemplateDataTable.Columns.Add(p.Name, typeof(string));
                        break;
                    case ColumnDataType.Float:
                        TemplateDataTable.Columns.Add(p.Name, typeof(decimal));
                        break;
                    default:
                        TemplateDataTable.Columns.Add(p.Name, typeof(string));
                        break;
                }
            }
        }
        #endregion

    }
}
