using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core
{
    public class ExcelPropety
    {
        #region 属性
        private string _columnName;
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName
        {
            get
            {
                string col = _columnName;
                return col;
            }
            set
            {
                _columnName = value;
            }
        }

        public string FieldName { get; set; }

        //private string _backgroudColor;
        ///// <summary>
        ///// 背景色
        ///// </summary>
        //public string BackgroudColor
        //{
        //    get { return _backgroudColor; }
        //    set { _backgroudColor = value; }
        //}

        /// <summary>
        /// 背景色
        /// </summary>
        public BackgroudColorEnum BackgroudColor { get; set; }

        private Type _resourceType;
        /// <summary>
        /// 多语言
        /// </summary>
        public Type ResourceType
        {
            get { return _resourceType; }
            set { _resourceType = value; }
        }

        private ColumnDataType _dataType;
        /// <summary>
        /// 数据类型
        /// </summary>
        public ColumnDataType DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        private Type _enumType;
        public Type EnumType
        {
            get { return _enumType; }
            set
            {
                _enumType = value;
                this.ListItems = _enumType.ToListItems();
            }
        }

        private string _minValueOrLength;
        /// <summary>
        /// 最小长度
        /// </summary>
        public string MinValueOrLength
        {
            get { return _minValueOrLength; }
            set { _minValueOrLength = value; }
        }

        private string _maxValuseOrLength;
        /// <summary>
        /// 最大长度
        /// </summary>
        public string MaxValuseOrLength
        {
            get { return _maxValuseOrLength; }
            set { _maxValuseOrLength = value; }
        }

        private bool _isNullAble;
        /// <summary>
        /// 是否可以为空
        /// </summary>
        public bool IsNullAble
        {
            get { return _isNullAble; }
            set { _isNullAble = value; }
        }

        private IEnumerable<ComboSelectListItem> _listItems;
        /// <summary>
        /// 类表中数据
        /// </summary>
        public IEnumerable<ComboSelectListItem> ListItems
        {
            get { return _listItems; }
            set { _listItems = value; }
        }

        private object _value;
        /// <summary>
        /// Value
        /// </summary>
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private List<ExcelPropety> _dynamicColumns;
        /// <summary>
        /// 动态列
        /// </summary>
        public List<ExcelPropety> DynamicColumns
        {
            get { return _dynamicColumns == null ? new List<ExcelPropety>() : _dynamicColumns; }
            set { _dynamicColumns = value; }
        }

        public Type SubTableType { get; set; }

        public bool ReadOnly { get; set; }
        /// <summary>
        /// 字符数量
        /// </summary>
        public int CharCount { get; set; }

        #endregion

        #region 设定Excel数据验证
        /// <summary>
        /// 设置Excel单元格样式（标题），数据格式
        /// </summary>
        /// <param name="dateType">数据类型</param>
        /// <param name="porpetyIndex">单元格索引</param>
        /// <param name="sheet">Sheet页</param>
        /// <param name="dataSheet">数据Sheet页</param>
        /// <param name="dataStyle">样式</param>
        /// <param name="dataFormat">格式</param>
        public void SetColumnFormat(ColumnDataType dateType, int porpetyIndex, HSSFSheet sheet, HSSFSheet dataSheet, ICellStyle dataStyle, IDataFormat dataFormat)
        {
            HSSFDataValidation dataValidation = null;
            switch (dateType)
            {
                case ColumnDataType.Date:
                    this.MinValueOrLength = DateTime.Parse("1950/01/01").ToString("yyyy/MM/dd");
                    this.MaxValuseOrLength = string.IsNullOrEmpty(this.MaxValuseOrLength) ? DateTime.MaxValue.ToString("yyyy/MM/dd") : this.MaxValuseOrLength;
                    dataValidation = new HSSFDataValidation(new CellRangeAddressList(1, 65535, porpetyIndex, porpetyIndex),
                    DVConstraint.CreateDateConstraint(OperatorType.BETWEEN, this.MinValueOrLength, this.MaxValuseOrLength, "yyyy/MM/dd"));
                    dataValidation.CreateErrorBox("错误", "请输入日期");
                    dataValidation.CreatePromptBox("请输入日期格式 yyyy/mm/dd", "在" + MinValueOrLength + " 到 " + MaxValuseOrLength + "之间");
                    //dataStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("yyyy/MM/dd");
                    dataStyle.DataFormat = dataFormat.GetFormat("yyyy/mm/dd");
                    break;
                case ColumnDataType.DateTime:
                    this.MinValueOrLength = DateTime.Parse("1950/01/01").ToString("yyyy/MM/dd HH:mm:ss");
                    this.MaxValuseOrLength = string.IsNullOrEmpty(this.MaxValuseOrLength) ? DateTime.MaxValue.ToString("yyyy/MM/dd HH:mm:ss") : this.MaxValuseOrLength;
                    dataValidation = new HSSFDataValidation(new CellRangeAddressList(1, 65535, porpetyIndex, porpetyIndex),
                        DVConstraint.CreateDateConstraint(OperatorType.BETWEEN, this.MinValueOrLength, this.MaxValuseOrLength, "yyyy/MM/dd HH:mm:ss"));
                    dataValidation.CreateErrorBox("错误", "请输入日期");
                    dataValidation.CreatePromptBox("请输入日期格式 yyyy/mm/dd HH:mm:ss", "在" + MinValueOrLength + " 到 " + MaxValuseOrLength + "之间");
                    //dataStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("yyyy/MM/dd");
                    dataStyle.DataFormat = dataFormat.GetFormat("yyyy/mm/dd HH:mm:ss");
                    break;
                case ColumnDataType.Number:
                    this.MinValueOrLength = string.IsNullOrEmpty(this.MinValueOrLength) ? long.MinValue.ToString() : this.MinValueOrLength;
                    this.MaxValuseOrLength = string.IsNullOrEmpty(this.MaxValuseOrLength) ? long.MaxValue.ToString() : this.MaxValuseOrLength;
                    dataValidation = new HSSFDataValidation(new CellRangeAddressList(1, 65535, porpetyIndex, porpetyIndex),
                         DVConstraint.CreateNumericConstraint(ValidationType.INTEGER, OperatorType.BETWEEN, this.MinValueOrLength, this.MaxValuseOrLength));
                    dataValidation.CreateErrorBox("错误", "请输入数字");
                    dataStyle.DataFormat = dataFormat.GetFormat("0");
                    dataValidation.CreatePromptBox("请输入数字格式", "在" + MinValueOrLength + " 到 " + MaxValuseOrLength + "之间");
                    break;
                case ColumnDataType.Float:
                    this.MinValueOrLength = string.IsNullOrEmpty(this.MinValueOrLength) ? decimal.MinValue.ToString() : this.MinValueOrLength;
                    this.MaxValuseOrLength = string.IsNullOrEmpty(this.MaxValuseOrLength) ? decimal.MaxValue.ToString() : this.MaxValuseOrLength;
                    dataValidation = new HSSFDataValidation(new CellRangeAddressList(1, 65535, porpetyIndex, porpetyIndex),
                         DVConstraint.CreateNumericConstraint(ValidationType.DECIMAL, OperatorType.BETWEEN, this.MinValueOrLength, this.MaxValuseOrLength));
                    dataValidation.CreateErrorBox("错误", "请输入小数");
                    dataStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    dataValidation.CreatePromptBox("请输入小数", "在" + MinValueOrLength + " 到 " + MaxValuseOrLength + "之间");
                    break;
                case ColumnDataType.Bool:
                    dataValidation = new HSSFDataValidation(new CellRangeAddressList(1, 65535, porpetyIndex, porpetyIndex),
                    DVConstraint.CreateFormulaListConstraint("Sheet1!$A$1:$B$1"));
                    dataValidation.CreateErrorBox("错误", "请输入下拉菜单中存在的数据");
                    sheet.AddValidationData(dataValidation);
                    dataValidation.CreatePromptBox("下拉菜单", "请输入下拉菜单中存在的数据");
                    break;
                case ColumnDataType.Text:
                    this.MinValueOrLength = string.IsNullOrEmpty(this.MinValueOrLength) ? "0" : this.MinValueOrLength;
                    this.MaxValuseOrLength = string.IsNullOrEmpty(this.MaxValuseOrLength) ? "2000" : this.MaxValuseOrLength;
                    dataValidation = new HSSFDataValidation(new CellRangeAddressList(1, 65535, porpetyIndex, porpetyIndex),
                      DVConstraint.CreateNumericConstraint(ValidationType.TEXT_LENGTH, OperatorType.BETWEEN, MinValueOrLength, MaxValuseOrLength));
                    dataValidation.CreateErrorBox("错误", "文本长度不符合要求");
                    dataStyle.DataFormat = dataFormat.GetFormat("@");
                    dataValidation.CreatePromptBox("请输入文本", "在" + MinValueOrLength + " 到 " + MaxValuseOrLength + "之间");
                    break;
                case ColumnDataType.ComboBox:
                case ColumnDataType.Enum:
                    int count = this.ListItems.Count() == 0 ? 1 : this.ListItems.Count();
                    string cloIndex = "";
                    if(porpetyIndex > 25)
                    {
                        cloIndex += Convert.ToChar((int)(Math.Floor(porpetyIndex / 26d)) -1 + 65);
                    }
                    cloIndex += Convert.ToChar(65 + porpetyIndex % 26).ToString();
                    //定义名称
                    IName range = sheet.Workbook.CreateName();
                    range.RefersToFormula = "Sheet2!$" + cloIndex + "$1:$" + cloIndex + "$" + count;
                    range.NameName = "dicRange" + porpetyIndex;
                    //dataValidation = new HSSFDataValidation(new CellRangeAddressList(1, 65535, porpetyIndex, porpetyIndex),
                    //    DVConstraint.CreateFormulaListConstraint("Sheet2!$" + cloIndex + "$1:$" + cloIndex + "$" + count));
                    dataValidation = new HSSFDataValidation(new CellRangeAddressList(1, 65535, porpetyIndex, porpetyIndex),
                        DVConstraint.CreateFormulaListConstraint("dicRange" + porpetyIndex));
                    dataValidation.CreateErrorBox("错误", "请输入下拉菜单中存在的数据");

                    var listItemsTemp = this.ListItems.ToList();
                    for (int rowIndex = 0; rowIndex < this.ListItems.Count(); rowIndex++)
                    {
                        //HSSFRow dataSheetRow = (HSSFRow)dataSheet.CreateRow(rowIndex);
                        HSSFRow dataSheetRow = (HSSFRow)dataSheet.GetRow(rowIndex);
                        if (dataSheetRow == null)
                        {
                            dataSheetRow = (HSSFRow)dataSheet.CreateRow(rowIndex);
                        }
                        //dataSheetRow.CreateCell(porpetyIndex).SetCellValue(this.ListItems.ToList()[rowIndex].Text);
                        dataSheetRow.CreateCell(porpetyIndex).SetCellValue(listItemsTemp[rowIndex].Text);
                        dataStyle.DataFormat = dataFormat.GetFormat("@");
                        dataSheetRow.Cells.Where(x => x.ColumnIndex == porpetyIndex).FirstOrDefault().CellStyle = dataStyle;
                    }
                    sheet.AddValidationData(dataValidation);
                    dataValidation.CreatePromptBox("下拉菜单", "请输入下拉菜单中存在的数据");
                    break;
                default:
                    dataValidation = new HSSFDataValidation(new CellRangeAddressList(1, 65535, porpetyIndex, porpetyIndex),
                      DVConstraint.CreateNumericConstraint(ValidationType.TEXT_LENGTH, OperatorType.BETWEEN, this.MinValueOrLength, this.MaxValuseOrLength));
                    dataValidation.CreateErrorBox("错误", "文本长度不符合要求");
                    dataStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                    break;
            }
            if (!this.IsNullAble)
            {
                dataValidation.EmptyCellAllowed = false;
            }
            sheet.SetDefaultColumnStyle(porpetyIndex, dataStyle);
            sheet.AddValidationData(dataValidation);
        }
        #endregion

        #region 验证Excel数据
        /// <summary>
        /// 验证Value 并生成错误信息(edit by dufei 2014-06-12,修改了当列设置为不验证时候，下拉列表获取不到值的问题)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="errorMessage"></param>
        /// <param name="rowIndex"></param>
        public void ValueValidity(string value, List<ErrorMessage> errorMessage, int rowIndex)
        {
            if (this.IsNullAble && string.IsNullOrEmpty(value))
            {
                this.Value = value;
            }
            else
            {
                switch (this.DataType)
                {
                    case ColumnDataType.Date:
                    case ColumnDataType.DateTime:
                        DateTime tryDateTimeResult;
                        if (!DateTime.TryParse(value, out tryDateTimeResult))
                        {
                            errorMessage.Add(new ErrorMessage { Index = rowIndex, Message = string.Format("列:{0},日期格式错误", this.ColumnName) });
                            //errorMessage.Add(new ErrorMessage { ColumnName = this.ColumnName, Index = rowIndex, Message = "日期格式错误" });
                        }
                        this.Value = tryDateTimeResult;
                        break;
                    case ColumnDataType.Number:
                        int tryIntResult;
                        if (!int.TryParse(value, out tryIntResult))
                        {
                            errorMessage.Add(new ErrorMessage { Index = rowIndex, Message = string.Format("列:{0},数字格式错误", this.ColumnName) });
                            //errorMessage.Add(new ErrorMessage { ColumnName = this.ColumnName, Index = rowIndex, Message = "日期格式错误" });
                        }
                        this.Value = tryIntResult;
                        break;
                    case ColumnDataType.Float:
                        decimal tryDecimalResult;
                        if (!decimal.TryParse(value, out tryDecimalResult))
                        {
                            errorMessage.Add(new ErrorMessage { Index = rowIndex, Message = string.Format("列:{0},小数格式错误", this.ColumnName) });
                            //errorMessage.Add(new ErrorMessage { ColumnName = this.ColumnName, Index = rowIndex, Message = "日期格式错误" });
                        }
                        this.Value = tryDecimalResult;
                        break;
                    case ColumnDataType.Bool:
                        if (value == "是")
                        {
                            this.Value = true;
                        }
                        else if (value == "否")
                        {
                            this.Value = false;
                        }
                        else
                        {
                            errorMessage.Add(new ErrorMessage { Index = rowIndex, Message = string.Format("列:{0},应该输入【是】或者【否】", this.ColumnName) });
                        }
                        break;
                    case ColumnDataType.Text:
                        this.Value = value;
                        break;
                    case ColumnDataType.ComboBox:
                    case ColumnDataType.Enum:
                        if (!this.ListItems.Any(x => x.Text == value))
                        {
                            errorMessage.Add(new ErrorMessage { Index = rowIndex, Message = string.Format("列:{0},输入的值在数据库中不存在", this.ColumnName) });
                        }
                        else
                        {
                            this.Value = this.ListItems.Where(x => x.Text == value).FirstOrDefault().Value;
                        }
                        break;
                    default:
                        errorMessage.Add(new ErrorMessage { Index = rowIndex, Message = string.Format("列:{0},输入的值不在允许的数据类型范围内", this.ColumnName) });
                        break;
                }
            }
        }
        #endregion

        #region 自定义委托处理excel数据
        /// <summary>
        /// 处理为多列数据
        /// </summary>
        public CopyData FormatData;
        /// <summary>
        /// 处理为单列数据
        /// </summary>
        public CopySingleData FormatSingleData;

        #endregion

        public static ExcelPropety CreateProperty<T>(Expression<Func<T, object>> field, bool isDateTime = false)
        {
            ExcelPropety cp = new ExcelPropety();
            cp.ColumnName = field.GetPropertyDisplayName();
            var fname = field.GetPropertyName();
            Type t = field.GetPropertyInfo().PropertyType;
            if (fname.Contains('.'))
            {
                int index = fname.LastIndexOf('.');
                cp.FieldName = fname.Substring(index + 1);
                cp.SubTableType = field.GetPropertyInfo().DeclaringType;
            }
            else
            {
                cp.FieldName = fname;
            }
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var req = field.GetPropertyInfo().GetCustomAttributes(typeof(RequiredAttribute), false).Cast<RequiredAttribute>().FirstOrDefault();
                if (req == null)
                {
                    cp.IsNullAble = true;
                }                
                t = t.GenericTypeArguments[0];
            }
            if(t == typeof(int) || t == typeof(long) || t == typeof(short))
            {
                var sl = t.GetCustomAttributes(typeof(RangeAttribute), false).Cast<RangeAttribute>().FirstOrDefault();
                cp.DataType = ColumnDataType.Number;
                if(sl != null)
                {
                    if (sl.Maximum != null)
                    {
                        cp.MaxValuseOrLength = sl.Maximum.ToString();
                    }
                    if(sl.Minimum != null)
                    {
                        cp.MinValueOrLength = sl.Minimum.ToString();
                    }
                }
            }
            else if( t== typeof(float) || t == typeof(double) || t == typeof(decimal))
            {
                cp.DataType = ColumnDataType.Float;
            }
            else if( t == typeof(bool))
            {
                cp.DataType = ColumnDataType.Bool;
            }
            else if (t.IsEnum)
            {
                cp.DataType = ColumnDataType.Enum;
                cp.EnumType = t;
            }
            else if(t == typeof(DateTime))
            {
                cp.DataType = ColumnDataType.Date;
                if (isDateTime)
                    cp.DataType = ColumnDataType.DateTime;
            }
            else
            {
                var sl = field.GetPropertyInfo().GetCustomAttributes(typeof(StringLengthAttribute),false).Cast<StringLengthAttribute>().FirstOrDefault();
                var req = field.GetPropertyInfo().GetCustomAttributes(typeof(RequiredAttribute), false).Cast<RequiredAttribute>().FirstOrDefault();
                cp.DataType = ColumnDataType.Text;
                if(req == null)
                {
                    cp.IsNullAble = true;
                }
                if (sl != null)
                {
                    if (sl.MaximumLength != 0)
                    {
                        cp.MaxValuseOrLength = sl.MaximumLength + "";
                    }
                    if (sl.MinimumLength != 0)
                    {
                        cp.MinValueOrLength = sl.MinimumLength + "";
                    }
                }
            }
            cp.CharCount = 20;
            return cp;
        }
            
    }

    #region 辅助类型

    /// <summary>
    /// 定义处理excel为单个字段的委托
    /// </summary>
    /// <param name="excelValue">excel中的值</param>
    /// <param name="excelTemplate">excel中的值</param>
    /// <param name="entityValue">实体的值</param>
    /// <param name="errorMsg">错误消息，没有错误为空</param>
    public delegate void CopySingleData(object excelValue, BaseTemplateVM excelTemplate, out string entityValue, out string errorMsg);
    /// <summary>
    /// 定义处理excel为多个字段的委托
    /// </summary>
    /// <param name="excelValue">excel中的值</param>
    /// <param name="excelTemplate">excel中的值</param>
    /// <returns>返回的处理结果</returns>
    public delegate ProcessResult CopyData(object excelValue, BaseTemplateVM excelTemplate);

    /// <summary>
    /// 处理结果
    /// </summary>
    public class ProcessResult
    {
        public List<EntityValue> EntityValues { get; set; }
        public ProcessResult()
        {
            EntityValues = new List<EntityValue>();
        }
    }

    /// <summary>
    /// 单字段类
    /// </summary>
    public class EntityValue
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public string FieldValue { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMsg { get; set; }
    }

    public enum ColumnDataType { Text, Number, Date, Float, Bool, ComboBox, Enum, Dynamic, DateTime }

    #endregion
}
