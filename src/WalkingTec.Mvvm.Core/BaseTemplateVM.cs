using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;

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
            HSSFWorkbook workbook = new HSSFWorkbook();
            InitExcelData();

            CreateDataTable();    //add by dufei
            SetTemplateDataValus();  //add by dufei

            if (!string.IsNullOrEmpty(FileDisplayName))
            {
                displayName = FileDisplayName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + "_" + DateTime.Now.ToString("hh^mm^ss") + ".xls";
            }
            else
            {
                displayName = this.GetType().Name + "_" + DateTime.Now.ToString("yyyy-MM-dd") + "_" + DateTime.Now.ToString("hh^mm^ss") + ".xls";
            }

            //模板sheet页
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet();
            workbook.SetSheetName(0, string.IsNullOrEmpty(FileDisplayName) ? this.GetType().Name : FileDisplayName);

            HSSFRow row = (HSSFRow)sheet.CreateRow(0);
            row.HeightInPoints = 20;

            HSSFSheet enumSheet = (HSSFSheet)workbook.CreateSheet();
            HSSFRow enumSheetRow1 = (HSSFRow)enumSheet.CreateRow(0);
            enumSheetRow1.CreateCell(0).SetCellValue("是");
            enumSheetRow1.CreateCell(1).SetCellValue("否");
            enumSheetRow1.CreateCell(2).SetCellValue(this.GetType().Name); //为模板添加标记,必要时可添加版本号

            HSSFSheet dataSheet = (HSSFSheet)workbook.CreateSheet();

            #region 设置excel模板列头
            //默认灰色
            var headerStyle = GetCellStyle(workbook);
            headerStyle.IsLocked = true;

            //黄色
            var yellowStyle = GetCellStyle(workbook, BackgroudColorEnum.Yellow);
            yellowStyle.IsLocked = true;

            //红色
            var redStyle = GetCellStyle(workbook, BackgroudColorEnum.Red);
            redStyle.IsLocked = true;

            //取得所有ExcelPropety
            var propetys = this.GetType().GetFields().Where(x => x.FieldType == typeof(ExcelPropety)).ToList();

            int _currentColunmIndex = 0;
            bool IsProtect = false;
            for (int porpetyIndex = 0; porpetyIndex < propetys.Count(); porpetyIndex++)
            {
                ExcelPropety excelPropety = (ExcelPropety)propetys[porpetyIndex].GetValue(this);
                ColumnDataType dateType = excelPropety.DataType;
                if (excelPropety.ReadOnly)
                {
                    IsProtect = true;
                }

                //给必填项加星号
                string colName = excelPropety.IsNullAble ? excelPropety.ColumnName : excelPropety.ColumnName + "*";
                row.CreateCell(_currentColunmIndex).SetCellValue(colName);

                //修改列头样式
                switch (excelPropety.BackgroudColor)
                {
                    case BackgroudColorEnum.Yellow:
                        row.Cells[_currentColunmIndex].CellStyle = yellowStyle;
                        break;
                    case BackgroudColorEnum.Red:
                        row.Cells[_currentColunmIndex].CellStyle = redStyle;
                        break;
                    default:
                        row.Cells[_currentColunmIndex].CellStyle = headerStyle;
                        break;
                }

                var dataStyle = workbook.CreateCellStyle();
                var dataFormat = workbook.CreateDataFormat();

                if (dateType == ColumnDataType.Dynamic)
                {
                    int dynamicColCount = excelPropety.DynamicColumns.Count();
                    for (int dynamicColIndex = 0; dynamicColIndex < dynamicColCount; dynamicColIndex++)
                    {
                        var dynamicCol = excelPropety.DynamicColumns.ToList()[dynamicColIndex];
                        string dynamicColName = excelPropety.IsNullAble ? dynamicCol.ColumnName : dynamicCol.ColumnName + "*";
                        row.CreateCell(_currentColunmIndex).SetCellValue(dynamicColName);
                        row.Cells[_currentColunmIndex].CellStyle = headerStyle;
                        if (dynamicCol.ReadOnly)
                        {
                            IsProtect = true;
                        }
                        //设定列宽
                        if (excelPropety.CharCount > 0)
                        {
                            sheet.SetColumnWidth(_currentColunmIndex, excelPropety.CharCount * 256);
                            dataStyle.WrapText = true;
                        }
                        else
                        {
                            sheet.AutoSizeColumn(_currentColunmIndex);
                        }
                        //设置单元格样式及数据类型
                        dataStyle.IsLocked = excelPropety.ReadOnly;
                        dynamicCol.SetColumnFormat(dynamicCol.DataType, _currentColunmIndex, sheet, dataSheet, dataStyle, dataFormat);
                        _currentColunmIndex++;
                    }
                }
                else
                {
                    //设定列宽
                    if (excelPropety.CharCount > 0)
                    {
                        sheet.SetColumnWidth(_currentColunmIndex, excelPropety.CharCount * 256);
                        dataStyle.WrapText = true;
                    }
                    else
                    {
                        sheet.AutoSizeColumn(_currentColunmIndex);
                    }
                    //设置是否锁定
                    dataStyle.IsLocked = excelPropety.ReadOnly;
                    //设置单元格样式及数据类型
                    excelPropety.SetColumnFormat(dateType, _currentColunmIndex, sheet, dataSheet, dataStyle, dataFormat);
                    _currentColunmIndex++;
                }
            }
            #endregion

            #region 添加模版数据 add by dufei
            if (TemplateDataTable.Rows.Count > 0)
            {
                for (int i = 0; i < TemplateDataTable.Rows.Count; i++)
                {
                    DataRow tableRow = TemplateDataTable.Rows[i];
                    HSSFRow dataRow = (HSSFRow)sheet.CreateRow(1 + i);
                    for (int porpetyIndex = 0; porpetyIndex < propetys.Count(); porpetyIndex++)
                    {
                        string colName = propetys[porpetyIndex].Name;
                        tableRow[colName].ToString();
                        dataRow.CreateCell(porpetyIndex).SetCellValue(tableRow[colName].ToString());
                    }
                }
            }
            #endregion

            //冻结行
            sheet.CreateFreezePane(0, 1, 0, 1);
            //锁定excel
            if (IsProtect)
            {
                sheet.ProtectSheet("password");
            }

            workbook.SetSheetHidden(1, true);
            workbook.SetSheetHidden(2, true);
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            return ms.ToArray();
        }
        #endregion

        #region 取得表头的样式
        private static ICellStyle GetCellStyle(HSSFWorkbook workbook, BackgroudColorEnum backgroudColor = BackgroudColorEnum.Grey)
        {
            var headerStyle = workbook.CreateCellStyle();
            //设定表头样式
            headerStyle.BorderBottom = BorderStyle.Thin;
            headerStyle.BorderLeft = BorderStyle.Thin;
            headerStyle.BorderRight = BorderStyle.Thin;
            headerStyle.BorderTop = BorderStyle.Thin;
            //用灰色填充背景
            short headerbg;

            switch (backgroudColor)
            {
                case BackgroudColorEnum.Grey:
                    headerbg = HSSFColor.LightBlue.Index;
                    break;
                case BackgroudColorEnum.Yellow:
                    headerbg = HSSFColor.LightYellow.Index;
                    break;
                case BackgroudColorEnum.Red:
                    headerbg = HSSFColor.Pink.Index;
                    break;
                default:
                    headerbg = HSSFColor.Pink.Index;
                    break;
            }

            headerStyle.FillForegroundColor = headerbg;
            headerStyle.FillPattern = FillPattern.SolidForeground;
            headerStyle.FillBackgroundColor = headerbg;
            headerStyle.Alignment = HorizontalAlignment.Center;
            return headerStyle;
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