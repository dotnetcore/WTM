using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Support.FileHandlers;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 导入接口
    /// </summary>
    /// <typeparam name="T">导入模版类</typeparam>
    public interface IBaseImport<out T> where T : BaseTemplateVM
    {
        T Template { get; }
        byte[] GenerateTemplate(out string displayName);
        void SetParms(Dictionary<string, string> parms);
        TemplateErrorListVM ErrorListVM { get; set; }
    }

    /// <summary>
    /// 导入基类，Excel导入的类应继承本类
    /// </summary>
    /// <typeparam name="T">导入模版类</typeparam>
    /// <typeparam name="P">导入的Model类</typeparam>
    public class BaseImportVM<T, P> : BaseVM, IBaseImport<T>
        where T : BaseTemplateVM, new()
        where P : TopBasePoco, new()
    {
        #region 字段、属性
        /// <summary>
        /// 上传文件的Id，方便导入等操作中进行绑定，这类操作需要上传文件但不需要记录在数据库中，所以Model层中没有文件Id的字段
        /// </summary>
        [Display(Name = "UploadFile")]
        public string UploadFileId { get; set; }

        /// <summary>
        /// 下载模板显示名称
        /// </summary>
        [JsonIgnore]
        public string FileDisplayName { get; set; }

        /// <summary>
        /// 错误列表
        /// </summary>
        [JsonIgnore]
        public TemplateErrorListVM ErrorListVM { get; set; }

        /// <summary>
        /// 是否验证模板类型（当其他系统模板导入到某模块时可设置为False）
        /// </summary>
        [JsonIgnore]
        public bool ValidityTemplateType { get; set; }

        /// <summary>
        /// 下载模版页面的参数
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, string> Parms { get; set; }

        protected List<T> TemplateData;

        /// <summary>
        /// 要导入的Model列表
        /// </summary>
        [JsonIgnore]
        public List<P> EntityList { get; set; }

        /// <summary>
        /// 模版
        /// </summary>
        [JsonIgnore]
        public T Template { get; set; }

        /// <summary>
        /// Model数据是否已被赋值
        /// </summary>
        protected bool isEntityListSet = false;

        /// <summary>
        /// 声明文件流
        /// </summary>
        protected byte[] excelData;

        /// <summary>
        /// 唯一性验证
        /// </summary>
        protected DuplicatedInfo<P> finalInfo;

        /// <summary>
        /// 是否存在主子表
        /// </summary>
        protected bool HasSubTable { get; set; }

        /// <summary>
        /// 是否在sqlserver时使用bulk导入
        /// </summary>
        public bool UseBulkSave { get; set; }

        /// <summary>
        /// 是否覆盖已有数据
        /// </summary>
        public bool IsOverWriteExistData { get; set; } = true;
        #endregion

        #region 构造函数
        public BaseImportVM()
        {
            ErrorListVM = new TemplateErrorListVM();
            ValidityTemplateType = true;
            Template = new T();
        }
        #endregion

        #region  生成excel
        /// <summary>
        /// 生成模版
        /// </summary>
        /// <param name="displayName">模版文件名</param>
        /// <returns>生成的模版</returns>
        public virtual byte[] GenerateTemplate(out string displayName)
        {
            return Template.GenerateTemplate(out displayName);
        }
        #endregion

        #region 设置参数值
        /// <summary>
        /// 设置模版参数
        /// </summary>
        /// <param name="parms">参数</param>
        public void SetParms(Dictionary<string, string> parms)
        {
            Template.Parms = parms;
        }
        #endregion

        ///

        /// <summary>
        /// 设置数据唯一性验证，子类中如果需要数据唯一性验证，应重写此方法
        /// </summary>
        /// <returns>
        public virtual DuplicatedInfo<P> SetDuplicatedCheck()
        {
            return null;
        }

        /// <summary>
        /// 获取上传的结果值
        /// </summary>
        public virtual void SetEntityList()
        {
            if (!isEntityListSet)
            {
                EntityList = new List<P>();

                //初始化上传的模板数据
                SetTemplateData();

                //如果模板中有错误，直接返回
                if (ErrorListVM.EntityList.Count > 0)
                {
                    return;
                }

                //对EntityList赋值
                SetEntityData();

                //允许子类修改EntityList
                SetCustomEntityData();

                //设置标识为初始化
                isEntityListSet = true;
            }
        }

        /// <summary>
        /// EntityList 填充完毕后调用，子类可重写此方法对 EntityList 进行自定义修改
        /// </summary>
        protected virtual void SetCustomEntityData()
        {
        }

        /// <summary>
        /// 获取上传模板中填写的数据，包含了对模板正确性的验证
        /// </summary>
        public virtual void SetTemplateData()
        {
            if (TemplateData != null && TemplateData.Count > 0)
            {
                return;
            }

            try
            {
                TemplateData = new List<T>();

                ///
                if (UploadFileId == null)
                {
                    ErrorListVM.EntityList.Add(new ErrorMessage { Message = CoreProgram._localizer?["Sys.PleaseUploadTemplate"] });
                    return;
                }
                Models.IWtmFile file = null;
                if (Wtm.ServiceProvider != null)
                {
                    var fp = Wtm.ServiceProvider.GetRequiredService<WtmFileProvider>();
                    file = fp.GetFile(UploadFileId, true, Wtm.CreateDC(false));
                }
                if (file == null)
                {
                    ErrorListVM.EntityList.Add(new ErrorMessage { Message = CoreProgram._localizer?["Sys.WrongTemplate"] });
                    return;
                }

                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    file.DataStream.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
                file.DataStream.Dispose();
                excelData = fileBytes;

                Template.InitExcelData();
                Template.InitCustomFormat();

                using (var doc = SpreadsheetDocument.Open(new MemoryStream(fileBytes), false))
                {
                    var workbookPart = doc.WorkbookPart;
                    if (workbookPart?.Workbook == null) return;
                    var sheetsElement = workbookPart.Workbook.GetFirstChild<Sheets>();
                    if (sheetsElement == null) return;
                    var sheets = sheetsElement.Elements<Sheet>()
                        .OrderBy(s => s.SheetId?.Value ?? 0).ToList();

                    // read previous template format
                    if (sheets.Count >= 2)
                    {
                        var markerPart = (WorksheetPart)workbookPart.GetPartById(sheets[1].Id);
                        var markerSheet = markerPart.Worksheet.GetFirstChild<SheetData>();
                        var markerRow = markerSheet.Elements<Row>().FirstOrDefault();
                        if (markerRow != null)
                        {
                            // 查找 C1 单元格，按引用精确查找
                            Cell markerCell = markerRow.Elements<Cell>()
                                .FirstOrDefault(c => c.CellReference?.Value == "C1");
                            string TemplateHiddenName = GetCellTextValue(markerCell, workbookPart);
                            if (ValidityTemplateType && (string.IsNullOrEmpty(TemplateHiddenName) || !TemplateHiddenName.Equals(typeof(T).Name)))
                            {
                                ErrorListVM.EntityList.Add(new ErrorMessage { Message = CoreProgram._localizer?["Sys.WrongTemplate"] });
                                return;
                            }
                        }
                    }

                    ///
                    if (sheets.Count == 0) return;
                    var dataPart = (WorksheetPart)workbookPart.GetPartById(sheets[0].Id);
                    var dataSheet = dataPart.Worksheet.GetFirstChild<SheetData>();
                    var allRows = dataSheet.Elements<Row>().ToList();
                    if (allRows.Count == 0) return;

                    var headerRow = allRows[0];
                    int headerCellCount = headerRow.Elements<Cell>().Count();

                    ///
                    List<ExcelPropety> ListTemplateProptetys = new List<ExcelPropety>();
                    var ListPropetys = Template.GetType().GetFields().Where(x => x.FieldType == typeof(ExcelPropety)).ToList();
                    for (int i = 0; i < ListPropetys.Count(); i++)
                    {
                        ExcelPropety ep = (ExcelPropety)ListPropetys[i].GetValue(Template);
                        ListTemplateProptetys.Add(ep);
                    }

                    ///
                    var dynamicColumn = ListTemplateProptetys.Where(x => x.DataType == ColumnDataType.Dynamic).FirstOrDefault();
                    int columnCount = dynamicColumn == null ? ListTemplateProptetys.Count : (ListTemplateProptetys.Count + dynamicColumn.DynamicColumns.Count - 1);
                    if (columnCount != headerCellCount)
                    {
                        ErrorListVM.EntityList.Add(new ErrorMessage { Message = CoreProgram._localizer?["Sys.WrongTemplate"] });
                        return;
                    }

                    ///
                    int pIndex = 0;
                    HasSubTable = false;
                    for (int i = 0; i < headerCellCount; i++)
                    {
                        HasSubTable = ListTemplateProptetys[pIndex].SubTableType != null ? true : HasSubTable;
                        pIndex++;
                    }

                    ///
                    if (HasSubTable)
                    {
                        for (int i = 0; i < headerCellCount; i++)
                        {
                            ListTemplateProptetys[i].IsNullAble = ListTemplateProptetys[i].SubTableType == null ? true : ListTemplateProptetys[i].IsNullAble;
                        }
                    }

                    ///
                    int rowIndex = 2;
                    for (int r = 1; r < allRows.Count; r++)
                    {
                        var row = allRows[r];
                        var cells = row.Elements<Cell>().ToList();

                        if (IsEmptyRowOpenXml(cells, columnCount))
                        {
                            return;
                        }

                        T result = new T();
                        pIndex = 0;
                        for (int i = 0; i < columnCount; i++)
                        {
                            string value = i < cells.Count ? GetCellTextValue(cells[i], workbookPart) : "";
                            ExcelPropety excelPropety = CopyExcelPropety(ListTemplateProptetys[pIndex]);

                            if (excelPropety.DataType == ColumnDataType.Text)
                            {
                                value = GetCellFormulaValueOpenXml(cells.ElementAtOrDefault(i), value);
                            }

                            if (excelPropety.DataType == ColumnDataType.Dynamic)
                            {
                                int dynamicColCount = excelPropety.DynamicColumns.Count();
                                for (int dynamicColIndex = 0; dynamicColIndex < dynamicColCount; dynamicColIndex++)
                                {
                                    string dv = (i + dynamicColIndex < cells.Count) ? GetCellTextValue(cells[i + dynamicColIndex], workbookPart) : "";
                                    excelPropety.DynamicColumns[dynamicColIndex].ValueValidity(dv, ErrorListVM.EntityList, rowIndex);
                                }
                                i = i + dynamicColCount - 1;
                            }
                            else
                            {
                                excelPropety.ValueValidity(value, ErrorListVM.EntityList, rowIndex);
                            }

                            //如果没有错误，进行赋值
                            if (ErrorListVM.EntityList.Count == 0)
                            {
                                var pts = ListPropetys[pIndex];
                                pts.SetValue(result, excelPropety);
                            }

                            pIndex++;
                        }
                        result.ExcelIndex = rowIndex;
                        TemplateData.Add(result);
                        rowIndex++;
                    }
                }
                return;
            }
            catch
            {
                ErrorListVM.EntityList.Add(new ErrorMessage { Message = CoreProgram._localizer?["Sys.WrongTemplate"] });
            }
        }

        ///
        /// <summary>
///
        /// </summary>
        public static string GetCellTextValue(Cell cell, WorkbookPart workbookPart = null)
        {
            if (cell == null) return "";

            // Handle inline string
            if (cell.DataType?.Value == CellValues.InlineString && cell.InlineString != null)
            {
                return cell.InlineString.Text?.Text ?? "";
            }

            // Handle shared string — resolve via SharedStringTable
            if (cell.DataType?.Value == CellValues.SharedString && cell.CellValue != null)
            {
                int idx;
                if (int.TryParse(cell.CellValue.Text, out idx) && workbookPart != null)
                {
                    var sst = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                    if (sst != null)
                    {
                        var items = sst.SharedStringTable.Elements<SharedStringItem>().ToList();
                        if (idx >= 0 && idx < items.Count)
                        {
                            return items[idx].InnerText ?? "";
                        }
                    }
                }
                return cell.CellValue.Text;
            }

            // Handle formula cached value
            if (cell.CellFormula != null && cell.CellValue != null)
            {
                return cell.CellValue.Text;
            }

            // Handle plain CellValue
            if (cell.CellValue != null)
            {
                return cell.CellValue.Text;
            }

            // Last resort: try inner text or InlineString without DataType
            if (cell.InlineString != null)
            {
                return cell.InlineString.Text?.Text ?? "";
            }
            if (!string.IsNullOrEmpty(cell.InnerText))
            {
                return cell.InnerText;
            }

            return "";
        }

        public string GetCellFormulaValueOpenXml(Cell cell, string Value)
        {
            if (cell == null) return Value;
            if (!string.IsNullOrEmpty(Value) && Value.IndexOf("=") == 0)
            {
                #region
                if (cell.CellValue != null)
                {
                    return cell.CellValue.Text;
                }
            }
            return Value;
        }

        private bool IsEmptyRowOpenXml(List<Cell> cells, int colCount, WorkbookPart workbookPart = null)
        {
            if (cells == null || cells.Count == 0) return true;
            for (int i = 0; i < Math.Min(colCount, cells.Count); i++)
            {
                string val = GetCellTextValue(cells[i], workbookPart);
                if (!string.IsNullOrEmpty(val?.Trim())) return false;
            }
            return true;
        }
        #endregion

        /// <summary>
        /// 根据模板中的数据，填写导入类的集合中
        /// </summary>
        public virtual void SetEntityData()
        {
            //反射出类中所有属性字段 P是Model层定义的类
            var pros = typeof(P).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            //反射出模板类中的所有属性字段 T是模板类，ExcelProperty 是自定义的Excel属性类
            List<FieldInfo> ListExcelFields = typeof(T).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(x => x.FieldType == typeof(ExcelPropety)).ToList();

            //循环Excel中的数据
            foreach (var item in TemplateData)
            {
                int rowIndex = 2;
                bool isMainData = false;

                //主表信息
                Dictionary<string, ExcelPropety> ParentEntity = new Dictionary<string, ExcelPropety>();
                string ParentEntityValues = string.Empty;

                //子表信息
                Dictionary<Type, List<FieldInfo>> ChildrenEntity = new Dictionary<Type, List<FieldInfo>>();
                Dictionary<Type, string> ChildrenEntityDic = new Dictionary<Type, string>();

                //循环TemplateVM中定义的所有的列，区分出主子表
                foreach (var ExcelField in ListExcelFields)
                {
                    //获取本列的ExcelProperty的值
                    if (typeof(T).GetField(ExcelField.Name).GetValue(item) is ExcelPropety ep)
                    {
                        //如果是子表的字段
                        if (ep.SubTableType != null)
                        {
                            //保存子表字段信息稍后处理
                            if (!ChildrenEntity.ContainsKey(ep.SubTableType))
                            {
                                ChildrenEntity[ep.SubTableType] = new List<FieldInfo>();
                            }
                            ChildrenEntity[ep.SubTableType].Add(ExcelField);
                        }
                        else
                        {
                            //保存子表字段信息稍后处理
                            ParentEntity.Add(ep.FieldName, ep);
                            ParentEntityValues += ep.Value;
                        }
                    }
                }

                //子表信息是否为空
                foreach (var sub in ChildrenEntity)
                {
                    string subVal = string.Empty;
                    foreach (var field in sub.Value)
                    {
                        ExcelPropety ep = typeof(T).GetField(field.Name).GetValue(item) as ExcelPropety;
                        subVal += ep.Value;
                    }
                    ChildrenEntityDic.Add(sub.Key, subVal);
                }

                P entity = null;

                //说明主表信息为空
                if (string.IsNullOrEmpty(ParentEntityValues))
                {
                    entity = EntityList.LastOrDefault();
                }
                else
                {
                    //初始化一个新的Entity
                    entity = new P();
                    isMainData = true;

                    //给主表赋值
                    foreach (var mep in ParentEntity)
                    {
                        SetEntityFieldValue(entity, mep.Value, rowIndex, mep.Key, item);
                    }
                    if (typeof(ITenant).IsAssignableFrom(entity.GetType()))
                    {
                        ITenant ent = entity as ITenant;
                        ent.TenantCode = LoginUserInfo?.CurrentTenant;
                    }

                }

                //给子表赋值
                foreach (var sub in ChildrenEntity)
                {
                    //循环Entity的所有属性，找到List<SubTableType>类型的字段
                    foreach (var pro in pros)
                    {
                        if (pro.PropertyType.IsGenericType)
                        {
                            var gtype = pro.PropertyType.GetGenericArguments()[0];
                            if (gtype == sub.Key)
                            {
                                //子表
                                var subList = entity.GetType().GetSingleProperty(pro.Name).GetValue(entity);
                                string fk = DC.GetFKName<P>(pro.Name);

                                //如果子表不为空
                                if (!string.IsNullOrEmpty(ChildrenEntityDic.Where(x => x.Key == sub.Key).FirstOrDefault().Value))
                                {
                                    IList list = null;
                                    if (subList == null)
                                    {
                                        //初始化List<SubTableType>
                                        list = typeof(List<>).MakeGenericType(gtype).GetConstructor(Type.EmptyTypes).Invoke(null) as IList;
                                    }
                                    else
                                    {
                                        list = subList as IList;
                                    }

                                    //初始化一个SubTableType
                                    var SubTypeEntity = gtype.GetConstructor(System.Type.EmptyTypes).Invoke(null);

                                    //给SubTableType中和本ExcelProperty同名的字段赋值
                                    foreach (var field in sub.Value)
                                    {
                                        ExcelPropety ep = typeof(T).GetField(field.Name).GetValue(item) as ExcelPropety;
                                        SetEntityFieldValue(SubTypeEntity, ep, rowIndex, ep.FieldName, item);
                                    }

                                    if (string.IsNullOrEmpty(fk) == false)
                                    {
                                        PropertyHelper.SetPropertyValue(SubTypeEntity, fk, entity.GetID());
                                    }

                                    if (typeof(IBasePoco).IsAssignableFrom(SubTypeEntity.GetType()))
                                    {
                                        (SubTypeEntity as IBasePoco).CreateTime = DateTime.Now;
                                        (SubTypeEntity as IBasePoco).CreateBy = LoginUserInfo?.ITCode;
                                    }
                                    if (typeof(ITenant).IsAssignableFrom(SubTypeEntity.GetType()))
                                    {
                                        ITenant ent = SubTypeEntity as ITenant;
                                        ent.TenantCode = LoginUserInfo?.CurrentTenant;
                                    }

                                    //var context = new ValidationContext(SubTypeEntity);
                                    //var validationResults = new List<ValidationResult>();
                                    //TryValidateObject(SubTypeEntity, context, validationResults);
                                    //if (validationResults.Count == 0)
                                    //{
                                    //将付好值得SubTableType实例添加到List中
                                    list.Add(SubTypeEntity);

                                    PropertyHelper.SetPropertyValue(entity, pro.Name, list);
                                    //}
                                    //else
                                    //{
                                    //    ErrorListVM.EntityList.Add(new ErrorMessage { Message = validationResults.FirstOrDefault()?.ErrorMessage ?? "Error", ExcelIndex = item.ExcelIndex });
                                    //    break;
                                    //}

                                }
                                break;
                            }
                        }
                    }

                }
                entity.ExcelIndex = item.ExcelIndex;
                if (isMainData)
                {
                    EntityList.Add(entity);
                }
            }
        }

        /// <summary>
        /// 进行上传中的错误验证
        /// </summary>
        public virtual void SetValidateCheck()
        {
            //找到对应的BaseCRUDVM，并初始化
            var vms = this.GetType().Assembly.GetExportedTypes().Where(x => x.IsSubclassOf(typeof(BaseCRUDVM<P>))).ToList();
            var vmtype = vms.Where(x => x.Name.ToLower() == typeof(P).Name.ToLower() + "vm").FirstOrDefault();
            if (vmtype == null)
            {
                vmtype = vms.FirstOrDefault();
            }

            IBaseCRUDVM<P> vm = null;
            DuplicatedInfo<P> dinfo = null;
            if (vmtype != null)
            {
                vm = vmtype.GetConstructor(System.Type.EmptyTypes).Invoke(null) as IBaseCRUDVM<P>;
                vm.CopyContext(this);
                dinfo = (vm as dynamic).SetDuplicatedCheck();
            }
            var cinfo = this.SetDuplicatedCheck();
            finalInfo = new DuplicatedInfo<P>
            {
                Groups = new List<DuplicatedGroup<P>>()
            };
            if (cinfo != null)
            {
                foreach (var item in cinfo?.Groups)
                {
                    finalInfo.Groups.Add(item);
                }
            }
            else if (dinfo != null)
            {
                foreach (var item in dinfo?.Groups)
                {
                    finalInfo.Groups.Add(item);
                }
            }
            //调用controller方法验证model
            //var vmethod = Controller?.GetType().GetMethod("RedoValidation");
            foreach (var entity in EntityList)
            {
                //try
                //{
                //    vmethod.Invoke(Controller, new object[] { entity });
                //}
                //catch { }

                if (vm != null)
                {
                    vm.SetEntity(entity);
                    vm.ByPassBaseValidation = true;
                    vm.Validate();
                    var basevm = vm as BaseVM;
                    if (basevm?.MSD?.Count > 0)
                    {
                        foreach (var key in basevm.MSD.Keys)
                        {
                            foreach (var error in basevm.MSD[key])
                            {
                                ErrorListVM.EntityList.Add(new ErrorMessage { Message = error.ErrorMessage, Index = entity.ExcelIndex });
                            }
                        }
                    }
                }
                (vm as BaseVM)?.MSD.Clear();

                //在本地EntityList中验证是否有重复
                ValidateDuplicateData(finalInfo, entity);
            }
        }

        protected void SetEntityFieldValue(object entity, ExcelPropety ep, int rowIndex, string fieldName, T templateVM)
        {
            if (ep.FormatData != null)
            {
                ProcessResult processResult = ep.FormatData(ep.Value, templateVM);
                if (processResult != null)
                {
                    //未添加任何处理结果
                    if (processResult.EntityValues.Count == 0)
                    {
                        PropertyHelper.SetPropertyValue(entity, fieldName, ep.Value, stringBasedValue: true);
                    }
                    //字段为一对一
                    if (processResult.EntityValues.Count == 1)
                    {
                        ep.Value = processResult.EntityValues[0].FieldValue;
                        if (!string.IsNullOrEmpty(processResult.EntityValues[0].ErrorMsg))
                        {
                            ErrorListVM.EntityList.Add(new ErrorMessage { Message = processResult.EntityValues[0].ErrorMsg, ExcelIndex = rowIndex, Index = rowIndex });
                        }
                        PropertyHelper.SetPropertyValue(entity, fieldName, ep.Value, stringBasedValue: true);
                    }
                    //字段为一对多
                    if (processResult.EntityValues.Count > 1)
                    {
                        foreach (var entityValue in processResult.EntityValues)
                        {
                            if (!string.IsNullOrEmpty(entityValue.ErrorMsg))
                            {
                                ErrorListVM.EntityList.Add(new ErrorMessage { Message = entityValue.ErrorMsg, ExcelIndex = rowIndex, Index = rowIndex });
                            }
                            PropertyHelper.SetPropertyValue(entity, entityValue.FieldName, entityValue.FieldValue, stringBasedValue: true);
                        }
                    }
                }
            }
            else if (ep.FormatSingleData != null)
            {
                ep.FormatSingleData(ep.Value, templateVM, out string singleEntityValue, out string errorMsg);
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    ErrorListVM.EntityList.Add(new ErrorMessage { Message = errorMsg, ExcelIndex = rowIndex, Index = rowIndex });
                }
                PropertyHelper.SetPropertyValue(entity, fieldName, singleEntityValue, stringBasedValue: true);
            }
            else
            {
                PropertyHelper.SetPropertyValue(entity, fieldName, ep.Value, stringBasedValue: true);
            }
        }

        protected bool IsUpdateRecordDuplicated(DuplicatedInfo<P> checkCondition, P entity)
        {
            if (checkCondition != null && checkCondition.Groups.Count > 0)
            {
                //生成基础Query
                var baseExp = EntityList.AsQueryable();
                var modelType = typeof(P);
                ParameterExpression para = Expression.Parameter(modelType, "tm");
                //循环所有重复字段组
                foreach (var group in checkCondition.Groups)
                {
                    List<Expression> conditions = new List<Expression>();
                    //生成一个表达式，类似于 x=>x.Id != id，这是为了当修改数据时验证重复性的时候，排除当前正在修改的数据
                    var idproperty = modelType.GetSingleProperty("ID");
                    MemberExpression idLeft = Expression.Property(para, idproperty);
                    ConstantExpression idRight = Expression.Constant(entity.GetID());
                    BinaryExpression idNotEqual = Expression.NotEqual(idLeft, idRight);
                    conditions.Add(idNotEqual);
                    List<PropertyInfo> props = new List<PropertyInfo>();
                    //在每个组中循环所有字段
                    foreach (var field in group.Fields)
                    {
                        Expression exp = field.GetExpression(entity, para);
                        if (exp != null)
                        {
                            conditions.Add(exp);
                        }
                        //将字段名保存，为后面生成错误信息作准备
                        props.AddRange(field.GetProperties());
                    }
                    int count = 0;
                    if (conditions.Count > 1)
                    {
                        //循环添加条件并生成Where语句
                        Expression conExp = conditions[0];
                        for (int i = 1; i < conditions.Count; i++)
                        {
                            conExp = Expression.AndAlso(conExp, conditions[i]);
                        }

                        MethodCallExpression whereCallExpression = Expression.Call(
                             typeof(Queryable),
                             "Where",
                             new Type[] { modelType },
                             baseExp.Expression,
                             Expression.Lambda<Func<P, bool>>(conExp, new ParameterExpression[] { para }));
                        var result = baseExp.Provider.CreateQuery(whereCallExpression);

                        foreach (var res in result)
                        {
                            count++;
                        }
                    }
                    if (count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected void ValidateDuplicateData(DuplicatedInfo<P> checkCondition, P entity)
        {
            if (checkCondition != null && checkCondition.Groups.Count > 0)
            {
                //生成基础Query
                var baseExp = EntityList.AsQueryable();
                var modelType = typeof(P);
                ParameterExpression para = Expression.Parameter(modelType, "tm");
                //循环所有重复字段组
                foreach (var group in checkCondition.Groups)
                {
                    List<Expression> conditions = new List<Expression>();
                    //生成一个表达式，类似于 x=>x.Id != id，这是为了当修改数据时验证重复性的时候，排除当前正在修改的数据
                    var idproperty = modelType.GetSingleProperty("ExcelIndex");
                    MemberExpression idLeft = Expression.Property(para, idproperty);
                    ConstantExpression idRight = Expression.Constant(entity.ExcelIndex);
                    BinaryExpression idNotEqual = Expression.NotEqual(idLeft, idRight);
                    conditions.Add(idNotEqual);
                    List<PropertyInfo> props = new List<PropertyInfo>();
                    //在每个组中循环所有字段
                    foreach (var field in group.Fields)
                    {
                        Expression exp = field.GetExpression(entity, para);
                        if (exp != null)
                        {
                            conditions.Add(exp);
                        }
                        //将字段名保存，为后面生成错误信息作准备
                        props.AddRange(field.GetProperties());
                    }
                    int count = 0;
                    if (conditions.Count > 1)
                    {
                        //循环添加条件并生成Where语句
                        Expression whereCallExpression = baseExp.Expression;
                        for (int i = 0; i < conditions.Count; i++)
                        {
                            whereCallExpression = Expression.Call(
                                 typeof(Queryable),
                                 "Where",
                                 new Type[] { modelType },
                                 whereCallExpression,
                                 Expression.Lambda<Func<P, bool>>(conditions[i], new ParameterExpression[] { para }));
                        }
                        var result = baseExp.Provider.CreateQuery(whereCallExpression);

                        foreach (var res in result)
                        {
                            count++;
                        }
                    }
                    if (count > 0)
                    {
                        //循环拼接所有字段名
                        string AllName = "";
                        foreach (var prop in props)
                        {
                            string name = PropertyHelper.GetPropertyDisplayName(prop);
                            AllName += name + ",";
                        }
                        if (AllName.EndsWith(","))
                        {
                            AllName = AllName.Remove(AllName.Length - 1);
                        }
                        //如果只有一个字段重复，则拼接形成 xxx字段重复 这种提示
                        if (props.Count == 1)
                        {
                            ErrorListVM.EntityList.Add(new ErrorMessage { Message = CoreProgram._localizer?["Sys.DuplicateError", AllName], Index = entity.ExcelIndex });
                        }
                        //如果多个字段重复，则拼接形成 xx，yy，zz组合字段重复 这种提示
                        else if (props.Count > 1)
                        {
                            ErrorListVM.EntityList.Add(new ErrorMessage { Message = CoreProgram._localizer?["Sys.DuplicateGroupError", AllName], Index = entity.ExcelIndex });
                        }
                    }
                }
            }
        }


        private void TryValidateObject(object model, ValidationContext context, ICollection<ValidationResult> results)
        {
            var modelType = model.GetType();
            foreach (var p in modelType.GetProperties())
            {
                var propertyValue = p.GetValue(model);
                TryValidateProperty(propertyValue, context, results, p);
            }
        }

        private void TryValidateProperty(object value, ValidationContext context, ICollection<ValidationResult> results, PropertyInfo propertyInfo = null)
        {
            var modelType = context.ObjectType;
            if (propertyInfo == null)
            {
                propertyInfo = modelType.GetProperty(context.MemberName!);
            }

            if (propertyInfo != null)
            {
                var rules = propertyInfo.GetCustomAttributes(true).Where(i => i.GetType().BaseType == typeof(ValidationAttribute)).Cast<ValidationAttribute>();
                var displayName = propertyInfo.GetPropertyDisplayName();
                var memberName = propertyInfo.Name;
                foreach (var rule in rules)
                {
                    if (!rule.IsValid(value))
                    {
                        string errorMessage = "Error";
                        if (!string.IsNullOrEmpty(rule.ErrorMessage))
                        {
                            if (rule is RangeAttribute range)
                            {
                                if (range.Minimum != null && range.Maximum != null)
                                {
                                    errorMessage = Wtm.Localizer[rule.ErrorMessage, displayName, range.Minimum, range.Maximum];
                                }
                                else if (range.Minimum != null)
                                {
                                    errorMessage = Wtm.Localizer[rule.ErrorMessage, displayName, range.Minimum];
                                }
                                else if (range.Maximum != null)
                                {
                                    errorMessage = Wtm.Localizer[rule.ErrorMessage, displayName, range.Maximum];
                                }
                            }
                            else if (rule is StringLengthAttribute sl)
                            {
                                if (sl.MaximumLength > 0 && sl.MinimumLength > 0)
                                {
                                    errorMessage = Wtm.Localizer[rule.ErrorMessage, displayName, sl.MinimumLength, sl.MaximumLength];
                                }
                                else if (sl.MinimumLength > 0)
                                {
                                    errorMessage = Wtm.Localizer[rule.ErrorMessage, displayName, sl.MinimumLength];
                                }
                                else if (sl.MaximumLength > 0)
                                {
                                    errorMessage = Wtm.Localizer[rule.ErrorMessage, displayName, sl.MaximumLength];
                                }
                            }
                            else
                            {
                                errorMessage = Wtm.Localizer[rule.ErrorMessage, displayName];
                            }
                        }
                        results.Add(new ValidationResult(errorMessage, new string[] { memberName }));
                    }
                }
            }
        }


        /// <summary>
        /// 保存指定表中的数据
        /// </summary>
        /// <returns></returns>
        public virtual bool BatchSaveData()
        {
            ///
            if (DeletedFileIds != null && DeletedFileIds.Count > 0 && Wtm.ServiceProvider != null)
            {
                var fp = Wtm.ServiceProvider.GetRequiredService<WtmFileProvider>();

                foreach (var item in DeletedFileIds)
                {
                    fp.DeleteFile(item.ToString(), Wtm.CreateDC(false));
                }
            }

            //进行赋值
            SetEntityList();
            foreach (var entity in EntityList)
            {
                var context = new ValidationContext(entity);
                var validationResults = new List<ValidationResult>();
                TryValidateObject(entity, context, validationResults);
                if (validationResults.Count > 0)
                {
                    ErrorListVM.EntityList.Add(new ErrorMessage { Message = validationResults.FirstOrDefault()?.ErrorMessage ?? "Error", ExcelIndex = entity.ExcelIndex, Index = entity.ExcelIndex });
                }
            }
            if (ErrorListVM.EntityList.Count > 0)
            {
                DoReInit();
                return false;
            }

            //执行验证
            SetValidateCheck();
            if (ErrorListVM.EntityList.Count > 0)
            {
                DoReInit();
                return false;
            }
            var ModelType = typeof(P);
            //循环数据列表
            List<P> ListAdd = new List<P>();
            foreach (var item in EntityList)
            {
                //根据唯一性的设定查找数据库中是否有同样的数据
                P exist = IsDuplicateData(item, finalInfo);
                //如果设置了覆盖功能
                if (IsOverWriteExistData)
                {
                    if (exist != null)
                    {
                        //如果有重复数据，则进行修改
                        var tempPros = typeof(T).GetFields();
                        foreach (var pro in tempPros)
                        {
                            var excelProp = Template.GetType().GetField(pro.Name).GetValue(Template) as ExcelPropety;
                            var proToSet = typeof(P).GetSingleProperty(excelProp.FieldName);
                            if (proToSet != null)
                            {
                                var val = proToSet.GetValue(item);
                                PropertyHelper.SetPropertyValue(exist, excelProp.FieldName, val, stringBasedValue: true);
                                try
                                {
                                    DC.UpdateProperty(exist, proToSet.Name);
                                }
                                catch { }
                            }
                        }

                        if (tempPros.Where(x => x.Name == "UpdateTime").SingleOrDefault() == null)
                        {
                            if (typeof(IBasePoco).IsAssignableFrom(exist.GetType()))
                            {
                                (exist as IBasePoco).UpdateTime = DateTime.Now;
                                DC.UpdateProperty(exist, "UpdateTime");
                            }
                        }

                        if (tempPros.Where(x => x.Name == "UpdateBy").SingleOrDefault() == null)
                        {
                            if (typeof(IBasePoco).IsAssignableFrom(exist.GetType()))
                            {
                                (exist as IBasePoco).UpdateBy = LoginUserInfo.ITCode;
                                DC.UpdateProperty(exist, "UpdateBy");
                            }
                        }
                        exist.ExcelIndex = item.ExcelIndex;
                        //DC.UpdateEntity(exist);

                        continue;
                    }
                    else
                    {
                        if (typeof(IPersistPoco).IsAssignableFrom(item.GetType()))
                        {
                            (item as IPersistPoco).IsValid = true;
                        }
                    }
                }
                else
                {
                    if (exist == null)
                    {
                        if (typeof(IPersistPoco).IsAssignableFrom(ModelType))
                        {
                            (item as IPersistPoco).IsValid = true;
                        }
                    }
                }
                //进行添加操作
                if (typeof(IBasePoco).IsAssignableFrom(item.GetType()))
                {
                    (item as IBasePoco).CreateTime = DateTime.Now;
                    (item as IBasePoco).CreateBy = LoginUserInfo?.ITCode;
                }
                if (typeof(ITenant).IsAssignableFrom(ModelType))
                {
                    ITenant ent = item as ITenant;
                    ent.TenantCode = LoginUserInfo?.CurrentTenant;
                }

                //如果是SqlServer数据库，而且没有主子表功能，进行Bulk插入
                if (ConfigInfo.Connections.Where(x => x.Key == (CurrentCS ?? "default")).FirstOrDefault().DbType == DBTypeEnum.SqlServer && !HasSubTable && UseBulkSave == true)
                {
                    //ListAdd.Add(item);
                }
                else
                {
                    DC.Set<P>().Add(item);
                }
            }

            if (ErrorListVM.EntityList.Count > 0)
            {
                DoReInit();
                return false;
            }

            //如果没有错误，更新数据库
            if (EntityList.Count > 0)
            {
                try
                {
                    DC.SaveChanges();

                    if (ListAdd.Count > 0)
                    {
                        BulkInsert<P>(DC, DC.GetTableName<P>(), ListAdd);
                    }
                }
                catch (Exception e)
                {
                    SetExceptionMessage(e, null);
                    DoReInit();
                    return false;
                }
            }
            if (string.IsNullOrEmpty(UploadFileId) == false && Wtm.ServiceProvider != null)
            {
                var fp = Wtm.ServiceProvider.GetRequiredService<WtmFileProvider>();
                fp.DeleteFile(UploadFileId, Wtm.CreateDC(false, "default"));
            }

            return true;
        }

        /// <summary>
        /// 批量插入数据库操作，支持SqlServer
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="dc">data context</param>
        /// <param name="tableName"></param>
        /// <param name="list"></param>
        protected static void BulkInsert<K>(IDataContext dc, string tableName, IList<K> list)
        {
            //using (var bulkCopy = new SqlBulkCopy(dc.CSName))
            //{
            //    bulkCopy.BatchSize = list.Count;
            //    bulkCopy.DestinationTableName = tableName;

            //    var table = new DataTable();
            //    var props = typeof(K).GetAllProperties().Distinct(x => x.Name);

            ///
            //    foreach (var propertyInfo in props)
            //    {
            //        var notmapped = propertyInfo.GetCustomAttribute<NotMappedAttribute>();
            //        var notobject = propertyInfo.PropertyType.Namespace.Equals("System") || propertyInfo.PropertyType.IsEnumOrNullableEnum();
            //        if (notmapped == null && notobject)
            //        {
            //            string Name = dc.GetFieldName<K>(propertyInfo.Name);
            //            bulkCopy.ColumnMappings.Add(Name, Name);
            //            table.Columns.Add(Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
            //        }
            //    }

            ///
            //    var values = new object[table.Columns.Count];
            //    foreach (var item in list)
            //    {
            //        var Index = 0;
            //        foreach (var propertyInfo in props)
            //        {
            //            var notmapped = propertyInfo.GetCustomAttribute<NotMappedAttribute>();
            //            var notobject = propertyInfo.PropertyType.Namespace.Equals("System") || propertyInfo.PropertyType.IsEnumOrNullableEnum();
            //            if (notmapped == null && notobject)
            //            {
            //                values[Index] = propertyInfo.GetValue(item);
            //                Index++;
            //            }
            //        }
            //        table.Rows.Add(values);
            //    }
            #region
            //    string Discriminator = dc.GetFieldName<K>("Discriminator");
            //    if (!string.IsNullOrEmpty(Discriminator))
            //    {
            //        bulkCopy.ColumnMappings.Add("Discriminator", "Discriminator");
            //        table.Columns.Add("Discriminator", typeof(string));
            //        for (int i = 0; i < table.Rows.Count; i++)
            //        {
            //            table.Rows[i]["Discriminator"] = typeof(K).Name;
            //        }
            //    }
            //    bulkCopy.WriteToServer(table);
            //}
        }
        #endregion

        ///
        /// <summary>
#region
        /// </summary>
        /// <param name="row">
        /// <param name="colCount">
        /// <returns>
        #endregion

        #region 复制Excel属性
        /// <summary>
        /// 复制Excel属性
        /// </summary>
        /// <param name="excelPropety">单元格属性</param>
        /// <returns>复制后的单元格</returns>
        private ExcelPropety CopyExcelPropety(ExcelPropety excelPropety)
        {
            ExcelPropety ep = new ExcelPropety
            {
                BackgroudColor = excelPropety.BackgroudColor,
                ColumnName = excelPropety.ColumnName,
                DataType = excelPropety.DataType,
                ResourceType = excelPropety.ResourceType,
                IsNullAble = excelPropety.IsNullAble,
                ListItems = excelPropety.ListItems,
                MaxValuseOrLength = excelPropety.MaxValuseOrLength,
                MinValueOrLength = excelPropety.MinValueOrLength,
                Value = excelPropety.Value,
                SubTableType = excelPropety.SubTableType,
                CharCount = excelPropety.CharCount,
                ReadOnly = excelPropety.ReadOnly,
                FormatData = excelPropety.FormatData,
                FormatSingleData = excelPropety.FormatSingleData,
                FieldName = excelPropety.FieldName
            };
            List<ExcelPropety> li = new List<ExcelPropety>();
            foreach (var item in excelPropety.DynamicColumns)
            {
                li.Add(CopyExcelPropety(item));
            }
            ep.DynamicColumns = li;
            return ep;
        }
        #endregion

        #region 设置异常信息
        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="e">异常</param>
        /// <param name="id">数据Id</param>
        protected void SetExceptionMessage(Exception e, long? id)
        {
            //检查是否为数据库操作错误
            if (e is DbUpdateException)
            {
                var de = e as DbUpdateException;
                if (de.Entries != null)
                {
                    if (de.Entries.Count == 0)
                    {
                        ErrorListVM.EntityList.Add(new ErrorMessage { Index = 0, Message = e.Message + e.InnerException?.Message });
                    }
                    //循环此错误相关的数据
                    foreach (var ent in de.Entries)
                    {
                        //获取错误数据Id
                        var errorId = (long)((ent.Entity as TopBasePoco).ExcelIndex);
                        //根据State判断修改或删除操作，输出不同的错误信息
                        if (ent.State == EntityState.Deleted)
                        {
                            ErrorListVM.EntityList.Add(new ErrorMessage { Index = errorId, Message = CoreProgram._localizer?["Sys.DataCannotDelete"] });
                        }
                        else if (ent.State == EntityState.Modified)
                        {
                            ErrorListVM.EntityList.Add(new ErrorMessage { Index = errorId, Message = CoreProgram._localizer?["Sys.EditFailed"] });
                        }
                        else
                        {
                            ErrorListVM.EntityList.Add(new ErrorMessage { Index = errorId, Message = de.Message });
                        }
                    }
                }
            }
            //对于其他类型的错误，直接添加错误信息
            else
            {
                if (id != null)
                {
                    ErrorListVM.EntityList.Add(new ErrorMessage { Index = id.Value, Message = e.Message });
                }
                else
                {
                    ErrorListVM.EntityList.Add(new ErrorMessage { Index = 0, Message = e.Message });
                }
            }
        }
        #endregion

        #region 验证数据重复

        /// <summary>
        /// 判断数据是否在库中存在重复数据
        /// </summary>
        /// <param name="Entity">要验证的数据</param>
        /// <param name="checkCondition">验证表达式</param>
        /// <returns>null代表没有重复</returns>
        protected P IsDuplicateData(P Entity, DuplicatedInfo<P> checkCondition)
        {
            //获取设定的重复字段信息
            if (checkCondition != null && checkCondition.Groups.Count > 0)
            {
                //生成基础Query
                var baseExp = DC.Set<P>().AsQueryable();
                var modelType = typeof(P);
                ParameterExpression para = Expression.Parameter(modelType, "tm");
                //循环所有重复字段组
                foreach (var group in checkCondition.Groups)
                {
                    List<Expression> conditions = new List<Expression>();
                    //生成一个表达式，类似于 x=>x.Id != id，这是为了当修改数据时验证重复性的时候，排除当前正在修改的数据
                    //在每个组中循环所有字段
                    List<PropertyInfo> props = new List<PropertyInfo>();
                    //在每个组中循环所有字段
                    foreach (var field in group.Fields)
                    {
                        Expression exp = field.GetExpression(Entity, para);
                        if (exp != null)
                        {
                            conditions.Add(exp);
                        }
                        //将字段名保存，为后面生成错误信息作准备
                        props.AddRange(field.GetProperties());
                    }
                    if (typeof(ITenant).IsAssignableFrom(modelType) && props.Any(x => x.Name.ToLower() == "tenantcode") == false && Wtm?.ConfigInfo.EnableTenant == true && group.UseTenant == true)
                    {
                        ITenant ent = Entity as ITenant;
                        ent.TenantCode = LoginUserInfo.CurrentTenant;
                        var f = new DuplicatedField<P>(x => (x as ITenant).TenantCode);
                        Expression exp = f.GetExpression(Entity, para);
                        conditions.Add(exp);
                    }
                    if (conditions.Count > 0)
                    {
                        //循环添加条件并生成Where语句
                        Expression whereCallExpression = baseExp.Expression;
                        for (int i = 0; i < conditions.Count; i++)
                        {
                            whereCallExpression = Expression.Call(
                                 typeof(Queryable),
                                 "Where",
                                 new Type[] { modelType },
                                 whereCallExpression,
                                 Expression.Lambda<Func<P, bool>>(conditions[i], new ParameterExpression[] { para }));
                        }
                        var result = baseExp.Provider.CreateQuery(whereCallExpression);


                        foreach (var res in result)
                        {
                            if (IsOverWriteExistData == false)
                            {
                                //循环拼接所有字段名
                                string AllName = "";
                                foreach (var prop in props)
                                {
                                    string name = PropertyHelper.GetPropertyDisplayName(prop);
                                    AllName += name + ",";
                                }
                                if (AllName.EndsWith(","))
                                {
                                    AllName = AllName.Remove(AllName.Length - 1);
                                }
                                //如果只有一个字段重复，则拼接形成 xxx字段重复 这种提示
                                if (props.Count == 1)
                                {
                                    ErrorListVM.EntityList.Add(new ErrorMessage { Message = CoreProgram._localizer?["Sys.DuplicateError", AllName], Index = Entity.ExcelIndex });
                                }
                                //如果多个字段重复，则拼接形成 xx，yy，zz组合字段重复 这种提示
                                else if (props.Count > 1)
                                {
                                    ErrorListVM.EntityList.Add(new ErrorMessage { Message = CoreProgram._localizer?["Sys.DuplicateGroupError", AllName], Index = Entity.ExcelIndex });
                                }

                            }
                            return res as P;
                        }
                    }
                }
            }
            return null;
        }
        #endregion

        protected DuplicatedInfo<P> CreateFieldsInfo(params DuplicatedField<P>[] FieldExps)
        {
            DuplicatedInfo<P> d = new DuplicatedInfo<P>();
            d.AddGroup(FieldExps);
            return d;
        }

        /// <summary>
        /// 创建一个简单重复数据信息
        /// </summary>
        /// <param name="FieldExp">重复数据的字段</param>
        /// <returns>重复数据信息</returns>
        public static DuplicatedField<P> SimpleField(Expression<Func<P, object>> FieldExp)
        {
            return new DuplicatedField<P>(FieldExp);
        }

        /// <summary>
        /// 创建一个关联到其他表数组中数据的重复信息
        /// </summary>
        /// <typeparam name="V">关联表类</typeparam>
        /// <param name="MiddleExp">指向关联表类数组的Lambda</param>
        /// <param name="FieldExps">指向最终字段的Lambda</param>
        /// <returns>重复数据信息</returns>
        public static DuplicatedField<P> SubField<V>(Expression<Func<P, List<V>>> MiddleExp, params Expression<Func<V, object>>[] FieldExps)
        {
            return new ComplexDuplicatedField<P, V>(MiddleExp, FieldExps);
        }

        public ErrorObj GetErrorJson()
        {
            var mse = new ErrorObj();
            mse.Form = new Dictionary<string, string>();
            var err = ErrorListVM?.EntityList?.Where(x => x.Index == 0).FirstOrDefault()?.Message;
            if (string.IsNullOrEmpty(err))
            {
                Models.IWtmFile fa = null;
                if(Wtm.ServiceProvider == null) {
                    return mse;
                }
                var fp = Wtm.ServiceProvider.GetRequiredService<WtmFileProvider>();
                fa = fp.GetFile(UploadFileId, true, DC);
                var propetys = Template.GetType().GetFields().Where(x => x.FieldType == typeof(ExcelPropety)).ToList();
                List<ExcelPropety> excelPropetys = new List<ExcelPropety>();
                for (int porpetyIndex = 0; porpetyIndex < propetys.Count(); porpetyIndex++)
                {
                    ExcelPropety ep = (ExcelPropety)propetys[porpetyIndex].GetValue(Template);
                    excelPropetys.Add(ep);
                }
                int columnCount = excelPropetys.Count;
                var dynamicColumn = excelPropetys.Where(x => x.DataType == ColumnDataType.Dynamic).FirstOrDefault();
                if (dynamicColumn != null)
                {
                    columnCount = columnCount + dynamicColumn.DynamicColumns.Count - 1;
                }
                byte[] newBytes;
                using (var ms = new MemoryStream(excelData))
                using (var doc = SpreadsheetDocument.Open(ms, true))
                {
                    var workbookPart = doc.WorkbookPart;
                    var sheets = workbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().ToList();
                    if (sheets.Count > 0)
                    {
                        var dataPart = (WorksheetPart)workbookPart.GetPartById(sheets[0].Id);
                        var sheetData = dataPart.Worksheet.GetFirstChild<SheetData>();

                        foreach (var e in ErrorListVM?.EntityList)
                        {
                            if (e.Index > 0)
                            {
                                uint rowIdx = (uint)(e.Index - 1);
                                Row row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIdx);
                                if (row != null)
                                {
                                    string colLetter = OpenXmlHelper.GetColumnLetter(columnCount);
                                    string cellRef = $"{colLetter}{rowIdx}";
                                    Cell errorCell = new Cell() { CellReference = cellRef };
                                    errorCell.DataType = CellValues.InlineString;
                                    errorCell.AppendChild(new InlineString(new Text(e.Message)));
                                    row.AppendChild(errorCell);
                                }
                            }
                        }
                    }
                    workbookPart.Workbook.Save();
                    newBytes = ms.ToArray();
                }
                using (var uploadMs = new MemoryStream(newBytes))
                {
                    uploadMs.Position = 0;
                    var newfile = fp.Upload("Error-" + fa.FileName, uploadMs.Length, uploadMs);
                    err = CoreProgram._localizer?["Sys.ImportError"];
                    mse.Form.Add("Entity.Import", err);
                    mse.Form.Add("Entity.ErrorFileId", newfile.GetID());
                }
            }
            else
            {
                mse.Form.Add("Entity.Import", err);
            }
            return mse;
        }
    }

    public class ErrorMessage : TopBasePoco
    {
        [Display(Name = "Sys.RowIndex")]
        public long Index { get; set; }

        [Display(Name = "Sys.CellIndex")]
        public long Cell { get; set; }
        [Display(Name = "Sys.ErrorMsg")]
        public string Message { get; set; }
    }

    /// <summary>
    /// 错误数据列表
    /// </summary>
    public class TemplateErrorListVM : BasePagedListVM<ErrorMessage, BaseSearcher>
    {

        public TemplateErrorListVM()
        {
            EntityList = new List<ErrorMessage>();
            NeedPage = false;
        }

        protected override IEnumerable<IGridColumn<ErrorMessage>> InitGridHeader()
        {
            return new List<GridColumn<ErrorMessage>>{
                this.MakeGridHeader(x => x.Index, 60),
                this.MakeGridHeader(x => x.Message)
            };
        }

        public override IOrderedQueryable<ErrorMessage> GetSearchQuery()
        {
            return EntityList.AsQueryable().OrderBy(x => x.Index);
        }
    }

}
