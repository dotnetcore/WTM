using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
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

        public string FieldDisplayName { get; set; }

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
        /// 设置Excel数据验证格式（OpenXML）
        /// </summary>
        /// <param name="dateType">数据类型</param>
        /// <param name="columnIndex">列索引</param>
        /// <param name="worksheetPart">工作表部件</param>
        /// <param name="dataSheetPart">数据参考表部件</param>
        /// <param name="workbookPart">工作簿部件（用于创建命名范围）</param>
        /// <param name="sheetName">主表名称</param>
        /// <param name="dataSheetName">数据表名称</param>
        /// <param name="numberFormatId">输出：数字格式ID</param>
        /// <returns>数据样式索引</returns>
        public void SetColumnFormat(ColumnDataType dateType, int columnIndex,
            WorksheetPart worksheetPart, WorksheetPart dataSheetPart,
            WorkbookPart workbookPart, string sheetName, string dataSheetName,
            out uint? numberFormatId, out bool isLocked)
        {
            isLocked = this.ReadOnly;
            numberFormatId = null;
            Worksheet worksheet = worksheetPart.Worksheet;
            DataValidations dataValidations = worksheet.GetFirstChild<DataValidations>();
            if (dataValidations == null)
            {
                dataValidations = new DataValidations();
                worksheet.InsertAfter(dataValidations, worksheet.GetFirstChild<SheetData>());
            }

            string cellRange = $"{OpenXmlHelper.GetColumnLetter(columnIndex)}2:{OpenXmlHelper.GetColumnLetter(columnIndex)}1048576";
            string errTitle = CoreProgram._localizer?["Sys.Error"]?.ToString() ?? "Error";
            string errInputNum = CoreProgram._localizer?["Sys.PleaseInputNumber"]?.ToString() ?? "Please input integer";
            string promptNumFmt = CoreProgram._localizer?["Sys.PleaseInputNumberFormat"]?.ToString() ?? "Number format";
            string errInputDec = CoreProgram._localizer?["Sys.PleaseInputDecimal"]?.ToString() ?? "Please input decimal";
            string promptDecFmt = CoreProgram._localizer?["Sys.PleaseInputDecimalFormat"]?.ToString() ?? "Decimal format";
            string errExistData = CoreProgram._localizer?["Sys.PleaseInputExistData"]?.ToString() ?? "Please select from list";
            string comboBox = CoreProgram._localizer?["Sys.ComboBox"]?.ToString() ?? "ComboBox";
            string errTextLen = CoreProgram._localizer?["Sys.WrongTextLength"]?.ToString() ?? "Wrong text length";
            string promptInputText = CoreProgram._localizer?["Sys.PleaseInputText"]?.ToString() ?? "Please input text";
            string dataRange = CoreProgram._localizer?["Sys.DataRange"]?.ToString() ?? "Range";

            switch (dateType)
            {
                case ColumnDataType.Date:
                case ColumnDataType.DateTime:
                    numberFormatId = 165; // yyyy-MM-dd HH:mm:ss
                    break;
                case ColumnDataType.Number:
                    this.MinValueOrLength = string.IsNullOrEmpty(this.MinValueOrLength) ? long.MinValue.ToString() : this.MinValueOrLength;
                    this.MaxValuseOrLength = string.IsNullOrEmpty(this.MaxValuseOrLength) ? long.MaxValue.ToString() : this.MaxValuseOrLength;
                    numberFormatId = 1; // 0
                    var numDv = new DataValidation()
                    {
                        Type = DataValidationValues.Whole,
                        AllowBlank = this.IsNullAble,
                        ShowErrorMessage = true,
                        ShowInputMessage = true,
                        SequenceOfReferences = new ListValue<StringValue>() { InnerText = cellRange },
                        ErrorTitle = errTitle,
                        Error = errInputNum,
                        PromptTitle = promptNumFmt,
                        Prompt = $"{dataRange} {MinValueOrLength} - {MaxValuseOrLength}"
                    };
                    numDv.Formula1 = new DocumentFormat.OpenXml.Spreadsheet.Formula1(this.MinValueOrLength);
                    numDv.Formula2 = new DocumentFormat.OpenXml.Spreadsheet.Formula2(this.MaxValuseOrLength);
                    dataValidations.AppendChild(numDv);
                    break;
                case ColumnDataType.Float:
                    this.MinValueOrLength = string.IsNullOrEmpty(this.MinValueOrLength) ? decimal.MinValue.ToString() : this.MinValueOrLength;
                    this.MaxValuseOrLength = string.IsNullOrEmpty(this.MaxValuseOrLength) ? decimal.MaxValue.ToString() : this.MaxValuseOrLength;
                    numberFormatId = 4; // 0.00
                    var decDv = new DataValidation()
                    {
                        Type = DataValidationValues.Decimal,
                        AllowBlank = this.IsNullAble,
                        ShowErrorMessage = true,
                        ShowInputMessage = true,
                        SequenceOfReferences = new ListValue<StringValue>() { InnerText = cellRange },
                        ErrorTitle = errTitle,
                        Error = errInputDec,
                        PromptTitle = promptDecFmt,
                        Prompt = $"{dataRange} {MinValueOrLength} - {MaxValuseOrLength}"
                    };
                    decDv.Formula1 = new DocumentFormat.OpenXml.Spreadsheet.Formula1(this.MinValueOrLength);
                    decDv.Formula2 = new DocumentFormat.OpenXml.Spreadsheet.Formula2(this.MaxValuseOrLength);
                    dataValidations.AppendChild(decDv);
                    break;
                case ColumnDataType.Bool:
                    var boolDv = new DataValidation()
                    {
                        Type = DataValidationValues.List,
                        AllowBlank = this.IsNullAble,
                        ShowErrorMessage = true,
                        ShowInputMessage = true,
                        SequenceOfReferences = new ListValue<StringValue>() { InnerText = cellRange },
                        ErrorTitle = errTitle,
                        Error = errExistData,
                        PromptTitle = comboBox,
                        Prompt = errExistData
                    };
                    boolDv.Formula1 = new DocumentFormat.OpenXml.Spreadsheet.Formula1("Sheet2!$A$1:$B$1");
                    dataValidations.AppendChild(boolDv);
                    numberFormatId = 49; // @
                    break;
                case ColumnDataType.Text:
                    this.MinValueOrLength = string.IsNullOrEmpty(this.MinValueOrLength) ? "0" : this.MinValueOrLength;
                    this.MaxValuseOrLength = string.IsNullOrEmpty(this.MaxValuseOrLength) ? "2000" : this.MaxValuseOrLength;
                    var textDv = new DataValidation()
                    {
                        Type = DataValidationValues.TextLength,
                        AllowBlank = this.IsNullAble,
                        ShowErrorMessage = true,
                        ShowInputMessage = true,
                        SequenceOfReferences = new ListValue<StringValue>() { InnerText = cellRange },
                        ErrorTitle = errTitle,
                        Error = errTextLen,
                        PromptTitle = promptInputText,
                        Prompt = $"{dataRange} {MinValueOrLength} - {MaxValuseOrLength}"
                    };
                    textDv.Formula1 = new DocumentFormat.OpenXml.Spreadsheet.Formula1(this.MinValueOrLength);
                    textDv.Formula2 = new DocumentFormat.OpenXml.Spreadsheet.Formula2(this.MaxValuseOrLength);
                    dataValidations.AppendChild(textDv);
                    numberFormatId = 49; // @
                    break;
                case ColumnDataType.ComboBox:
                case ColumnDataType.Enum:
                    int count = this.ListItems.Count() == 0 ? 1 : this.ListItems.Count();
                    string colLetter = OpenXmlHelper.GetColumnLetter(columnIndex);
                    string rangeName = "dicRange" + columnIndex;

                    // 创建工作簿中的命名范围
                    DefinedNames definedNames = workbookPart.Workbook.GetFirstChild<DefinedNames>();
                    if (definedNames == null)
                    {
                        definedNames = new DefinedNames();
                        workbookPart.Workbook.AppendChild(definedNames);
                    }
                    var definedName = new DefinedName()
                    {
                        Name = rangeName,
                        Text = $"{dataSheetName}!${colLetter}$1:${colLetter}${count}"
                    };
                    definedNames.AppendChild(definedName);

                    // 写入下拉列表项到数据表
                    SheetData dataSheetData = dataSheetPart.Worksheet.GetFirstChild<SheetData>();
                    for (int rowIndex = 0; rowIndex < this.ListItems.Count(); rowIndex++)
                    {
                        Row dataRow = OpenXmlHelper.GetOrCreateRow(dataSheetData, rowIndex + 1);
                        Cell dataCell = OpenXmlHelper.CreateCell($"{colLetter}{rowIndex + 1}",
                            this.ListItems.ToList()[rowIndex].Text,
                            CellValues.String, null);
                        dataRow.AppendChild(dataCell);
                    }

                    var comboDv = new DataValidation()
                    {
                        Type = DataValidationValues.List,
                        AllowBlank = this.IsNullAble,
                        ShowErrorMessage = true,
                        ShowInputMessage = true,
                        SequenceOfReferences = new ListValue<StringValue>() { InnerText = cellRange },
                        ErrorTitle = errTitle,
                        Error = errExistData,
                        PromptTitle = comboBox,
                        Prompt = errExistData
                    };
                    comboDv.Formula1 = new DocumentFormat.OpenXml.Spreadsheet.Formula1(rangeName);
                    dataValidations.AppendChild(comboDv);
                    numberFormatId = 49; // @
                    break;
                default:
                    numberFormatId = 49; // @
                    break;
            }

            if (dataValidations.HasChildren)
            {
                dataValidations.Count = (uint)dataValidations.ChildElements.Count;
            }
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
                ErrorMessage err = null;
                switch (this.DataType)
                {
                    case ColumnDataType.Date:
                    case ColumnDataType.DateTime:
                        DateTime tryDateTimeResult;
                        if (!DateTime.TryParse(value, out tryDateTimeResult))
                        {
                            err = new ErrorMessage { Index = rowIndex, Message = CoreProgram._localizer?["Sys.{0}formaterror", this.ColumnName] };
                        }
                        this.Value = tryDateTimeResult;
                        break;
                    case ColumnDataType.Number:
                        int tryIntResult;
                        if (!int.TryParse(value, out tryIntResult))
                        {
                            err = new ErrorMessage { Index = rowIndex, Message = CoreProgram._localizer?["Sys.{0}formaterror", this.ColumnName] };
                        }
                        this.Value = tryIntResult;
                        break;
                    case ColumnDataType.Float:
                        decimal tryDecimalResult;
                        if (!decimal.TryParse(value, out tryDecimalResult))
                        {
                            err = new ErrorMessage { Index = rowIndex, Message = CoreProgram._localizer?["Sys.{0}formaterror", this.ColumnName] };
                        }
                        this.Value = tryDecimalResult;
                        break;
                    case ColumnDataType.Bool:
                        if (value == CoreProgram._localizer?["Sys.Yes"])
                        {
                            this.Value = true;
                        }
                        else if (value == CoreProgram._localizer?["Sys.No"])
                        {
                            this.Value = false;
                        }
                        else
                        {
                            err = new ErrorMessage { Index = rowIndex, Message = CoreProgram._localizer?["Sys.{0}formaterror", this.ColumnName] };
                        }
                        break;
                    case ColumnDataType.Text:
                        this.Value = value;
                        break;
                    case ColumnDataType.ComboBox:
                    case ColumnDataType.Enum:
                        if (!this.ListItems.Any(x => x.Text == value))
                        {
                            err = new ErrorMessage { Index = rowIndex, Message = CoreProgram._localizer?["Sys.{0}ValueNotExist", this.ColumnName] };
                        }
                        else
                        {
                            this.Value = this.ListItems.Where(x => x.Text == value).FirstOrDefault().Value;
                        }
                        break;
                    default:
                        err = new ErrorMessage { Index = rowIndex, Message = CoreProgram._localizer?["Sys.{0}ValueTypeNotAllowed", this.ColumnName] };
                        break;
                }

                if (err != null && this.SubTableType == null)
                {
                    errorMessage.Add(err);
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
            cp.FieldDisplayName = field.GetPropertyDisplayName();
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
            if (t == typeof(int) || t == typeof(long) || t == typeof(short))
            {
                var sl = t.GetCustomAttributes(typeof(RangeAttribute), false).Cast<RangeAttribute>().FirstOrDefault();
                cp.DataType = ColumnDataType.Number;
                if (sl != null)
                {
                    if (sl.Maximum != null)
                    {
                        cp.MaxValuseOrLength = sl.Maximum.ToString();
                    }
                    if (sl.Minimum != null)
                    {
                        cp.MinValueOrLength = sl.Minimum.ToString();
                    }
                }
            }
            else if (t == typeof(float) || t == typeof(double) || t == typeof(decimal))
            {
                cp.DataType = ColumnDataType.Float;
            }
            else if (t == typeof(bool))
            {
                cp.DataType = ColumnDataType.Bool;
            }
            else if (t.IsEnum)
            {
                cp.DataType = ColumnDataType.Enum;
                cp.EnumType = t;
            }
            else if (t == typeof(DateTime))
            {
                cp.DataType = ColumnDataType.Date;
                if (isDateTime)
                    cp.DataType = ColumnDataType.DateTime;
            }
            else
            {
                var sl = field.GetPropertyInfo().GetCustomAttributes(typeof(StringLengthAttribute), false).Cast<StringLengthAttribute>().FirstOrDefault();
                var req = field.GetPropertyInfo().GetCustomAttributes(typeof(RequiredAttribute), false).Cast<RequiredAttribute>().FirstOrDefault();
                cp.DataType = ColumnDataType.Text;
                if (req == null)
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
