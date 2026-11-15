using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
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
        /// <summary>
        /// 导入模板
        /// </summary>
        T Template { get; }

        /// <summary>
        /// 生成Excel导入模板文件
        /// </summary>
        /// <param name="displayName">输出的模板文件显示名称</param>
        /// <returns>模板文件内容的字节数组</returns>
        byte[] GenerateTemplate(out string displayName);

        /// <summary>
        /// 设置模板参数，供生成模板时使用
        /// </summary>
        /// <param name="parms">参数字典</param>
        void SetParms(Dictionary<string, string> parms);

        /// <summary>
        /// 导入过程中产生的错误列表
        /// </summary>
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

        /// <summary>
        /// 从上传的Excel中解析出的模板数据列表
        /// </summary>
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
        /// <summary>
        /// 初始化导入VM，默认创建错误列表VM和模板实例，并开启模板类型验证
        /// </summary>
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
        /// 唯一性验证条件；返回 null 表示不进行唯一性验证
        /// </returns>
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
            //已解析过模板数据则直接返回，避免重复解析
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

                //初始化模板中的Excel列定义和自定义格式
                Template.InitExcelData();
                Template.InitCustomFormat();

                //以只读方式打开上传的Excel文件
                using (var doc = SpreadsheetDocument.Open(new MemoryStream(fileBytes), false))
                {
                    //获取工作簿，并按SheetId顺序取出所有工作表
                    var workbookPart = doc.WorkbookPart;
                    if (workbookPart?.Workbook == null) return;
                    var sheetsElement = workbookPart.Workbook.GetFirstChild<Sheets>();
                    if (sheetsElement == null) return;
                    var sheets = sheetsElement.Elements<Sheet>()
                        .OrderBy(s => s.SheetId?.Value ?? 0).ToList();

                    // read previous template format
                    //读取第二个工作表（模板标记页）C1单元格中记录的模板类型，验证上传文件是否为本模块的模板
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
                    //获取第一个工作表（数据页）的数据区
                    var dataPart = (WorksheetPart)workbookPart.GetPartById(sheets[0].Id);
                    var dataSheet = dataPart.Worksheet.GetFirstChild<SheetData>();
                    var allRows = dataSheet.Elements<Row>().ToList();
                    if (allRows.Count == 0) return;

                    var headerRow = allRows[0];
                    int headerCellCount = headerRow.Elements<Cell>().Count();

                    ///
                    //通过反射获取模板类中定义的所有Excel列属性
                    List<ExcelPropety> ListTemplateProptetys = new List<ExcelPropety>();
                    var ListPropetys = Template.GetType().GetFields().Where(x => x.FieldType == typeof(ExcelPropety)).ToList();
                    for (int i = 0; i < ListPropetys.Count(); i++)
                    {
                        ExcelPropety ep = (ExcelPropety)ListPropetys[i].GetValue(Template);
                        ListTemplateProptetys.Add(ep);
                    }

                    ///
                    //计算模板总列数（动态列按其子列数展开），与Excel表头列数不符则认为模板错误
                    var dynamicColumn = ListTemplateProptetys.Where(x => x.DataType == ColumnDataType.Dynamic).FirstOrDefault();
                    int columnCount = dynamicColumn == null ? ListTemplateProptetys.Count : (ListTemplateProptetys.Count + dynamicColumn.DynamicColumns.Count - 1);
                    if (columnCount != headerCellCount)
                    {
                        ErrorListVM.EntityList.Add(new ErrorMessage { Message = CoreProgram._localizer?["Sys.WrongTemplate"] });
                        return;
                    }

                    ///
                    //遍历所有列，判断模板中是否定义了子表列
                    int pIndex = 0;
                    HasSubTable = false;
                    for (int i = 0; i < headerCellCount; i++)
                    {
                        HasSubTable = ListTemplateProptetys[pIndex].SubTableType != null ? true : HasSubTable;
                        pIndex++;
                    }

                    ///
                    //存在子表时，主表列一律允许为空，因为子表行可以不重复填写主表字段
                    if (HasSubTable)
                    {
                        for (int i = 0; i < headerCellCount; i++)
                        {
                            ListTemplateProptetys[i].IsNullAble = ListTemplateProptetys[i].SubTableType == null ? true : ListTemplateProptetys[i].IsNullAble;
                        }
                    }

                    ///
                    //从第二行开始逐行读取数据，连续10个空行视为数据已结束
                    int rowIndex = 2;
                    int consecutiveEmptyRows = 0;
                    for (int r = 1; r < allRows.Count; r++)
                    {
                        var row = allRows[r];
                        var cells = row.Elements<Cell>().ToList();

                        if (IsEmptyRowOpenXml(cells, columnCount))
                        {
                            consecutiveEmptyRows++;
                            if (consecutiveEmptyRows >= 10)
                            {
                                return;
                            }
                            continue;
                        }
                        consecutiveEmptyRows = 0;

                        T result = new T();
                        pIndex = 0;
                        for (int i = 0; i < columnCount; i++)
                        {
                            string value = i < cells.Count ? GetCellTextValue(cells[i], workbookPart) : "";
                            ExcelPropety excelPropety = CopyExcelPropety(ListTemplateProptetys[pIndex]);

                            //文本类型列：读取公式的缓存计算结果
                            if (excelPropety.DataType == ColumnDataType.Text)
                            {
                                value = GetCellFormulaValueOpenXml(cells.ElementAtOrDefault(i), value);
                            }

                            //动态列：依次读取并验证该列展开的所有动态子列
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
            catch (Exception e)
            {
                var baseMsg = CoreProgram._localizer?["Sys.WrongTemplate"] ?? "WrongTemplate";
                ErrorListVM.EntityList.Add(new ErrorMessage { Message = $"{baseMsg}({e.GetType().Name}: {e.Message})" });
            }
        }

        ///
        /// <summary>
///
        /// </summary>
        /// <param name="cell">要读取的OpenXML单元格</param>
        /// <param name="workbookPart">单元格所属的工作簿部件，用于解析共享字符串</param>
        /// <returns>单元格的文本内容；无法解析时返回空字符串</returns>
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

        /// <summary>
        /// 获取单元格中公式的缓存计算结果
        /// </summary>
        /// <param name="cell">要读取的OpenXML单元格</param>
        /// <param name="Value">单元格原本读取到的文本值</param>
        /// <returns>值以等号开头且存在公式缓存值时返回缓存值，否则返回原值</returns>
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

        /// <summary>
        /// 判断一行单元格是否全部为空
        /// </summary>
        /// <param name="cells">该行包含的单元格列表</param>
        /// <param name="colCount">需要检查的列数</param>
        /// <param name="workbookPart">工作簿部件，用于解析单元格文本</param>
        /// <returns>所有被检查的单元格均为空时返回 true，否则返回 false</returns>
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
                    //如果主表实体支持多租户，设置租户代码
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

                                    //将付好值得SubTableType实例添加到List中
                                    list.Add(SubTypeEntity);

                                    PropertyHelper.SetPropertyValue(entity, pro.Name, list);

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
            //优先使用导入VM自身定义的唯一性验证，未定义时使用对应CRUD VM的验证
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
            foreach (var entity in EntityList)
            {

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

        /// <summary>
        /// 将Excel列的值经过格式化处理后设置到实体对应字段上
        /// </summary>
        /// <param name="entity">要赋值的实体对象（主表或子表实例）</param>
        /// <param name="ep">Excel列属性定义，包含列值和格式化委托</param>
        /// <param name="rowIndex">当前数据所在Excel行号，用于错误定位</param>
        /// <param name="fieldName">实体上要赋值的字段名</param>
        /// <param name="templateVM">当前行的模板数据实例，传递给格式化委托使用</param>
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

        /// <summary>
        /// 验证修改数据时是否与EntityList中的其他数据重复
        /// </summary>
        /// <param name="checkCondition">唯一性验证条件</param>
        /// <param name="entity">要验证的实体</param>
        /// <returns>存在重复返回 true，否则返回 false</returns>
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

        /// <summary>
        /// 验证当前实体是否与本地EntityList中的其他数据重复，重复时将错误信息加入错误列表
        /// </summary>
        /// <param name="checkCondition">唯一性验证条件</param>
        /// <param name="entity">要验证的实体</param>
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


        /// <summary>
        /// 对模型的所有属性逐一执行验证特性校验
        /// </summary>
        /// <param name="model">要验证的模型对象</param>
        /// <param name="context">验证上下文</param>
        /// <param name="results">验证结果集合，失败信息会被加入其中</param>
        private void TryValidateObject(object model, ValidationContext context, ICollection<ValidationResult> results)
        {
            var modelType = model.GetType();
            foreach (var p in modelType.GetProperties())
            {
                var propertyValue = p.GetValue(model);
                TryValidateProperty(propertyValue, context, results, p);
            }
        }

        /// <summary>
        /// 对单个属性执行所有验证特性校验，并将错误信息本地化后加入结果集合
        /// </summary>
        /// <param name="value">属性值</param>
        /// <param name="context">验证上下文</param>
        /// <param name="results">验证结果集合</param>
        /// <param name="propertyInfo">属性信息，为空时根据上下文成员名获取</param>
        private void TryValidateProperty(object value, ValidationContext context, ICollection<ValidationResult> results, PropertyInfo propertyInfo = null)
        {
            var modelType = context.ObjectType;
            if (propertyInfo == null)
            {
                propertyInfo = modelType.GetProperty(context.MemberName!);
            }

            if (propertyInfo != null)
            {
                //获取属性上定义的所有验证特性
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
                            //范围验证：根据最大/最小值的设置情况拼装本地化参数
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
                            //字符串长度验证：根据最大/最小长度的设置情况拼装本地化参数
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
            //删除提交时标记为删除的临时附件文件
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
            //对每个实体执行数据注解验证，错误信息记入错误列表
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
            //ListAdd用于收集待Bulk插入的数据（当前Bulk插入为保留的空实现，未实际使用）
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

                        //模板中未定义更新时间字段时，自动更新已有数据的更新时间
                        if (tempPros.Where(x => x.Name == "UpdateTime").SingleOrDefault() == null)
                        {
                            if (typeof(IBasePoco).IsAssignableFrom(exist.GetType()))
                            {
                                (exist as IBasePoco).UpdateTime = DateTime.Now;
                                DC.UpdateProperty(exist, "UpdateTime");
                            }
                        }

                        //模板中未定义更新人字段时，自动更新已有数据的更新人
                        if (tempPros.Where(x => x.Name == "UpdateBy").SingleOrDefault() == null)
                        {
                            if (typeof(IBasePoco).IsAssignableFrom(exist.GetType()))
                            {
                                (exist as IBasePoco).UpdateBy = LoginUserInfo.ITCode;
                                DC.UpdateProperty(exist, "UpdateBy");
                            }
                        }
                        exist.ExcelIndex = item.ExcelIndex;

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
                    //预留的Bulk插入分支，当前为空实现
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
            //导入成功后删除上传的临时文件
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
            //保留的空实现，当前未执行任何批量插入操作
        }

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

        /// <summary>
        /// 创建一组字段组成的唯一性验证信息
        /// </summary>
        /// <param name="FieldExps">重复验证字段的Lambda表达式数组</param>
        /// <returns>唯一性验证信息</returns>
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

        /// <summary>
        /// 将导入错误信息写入Excel并生成前端需要的错误JSON对象
        /// </summary>
        /// <returns>包含错误提示及错误文件Id的ErrorObj对象</returns>
        public ErrorObj GetErrorJson()
        {
            var mse = new ErrorObj();
            mse.Form = new Dictionary<string, string>();
            var err = ErrorListVM?.EntityList?.Where(x => x.Index == 0).FirstOrDefault()?.Message;
            if (string.IsNullOrEmpty(err))
            {
                //没有整体错误时，把具体行的错误写回Excel并生成错误文件供下载
                Models.IWtmFile fa = null;
                if(Wtm.ServiceProvider == null) {
                    return mse;
                }
                var fp = Wtm.ServiceProvider.GetRequiredService<WtmFileProvider>();
                fa = fp.GetFile(UploadFileId, true, DC);
                //反射获取模板中定义的所有Excel列，并计算总列数（动态列按子列展开）
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
                //以可写方式打开原始上传的Excel，在每条出错行的行尾追加错误信息单元格
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
                                    //在最后一列之后追加一个单元格，写入该行的错误信息
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
                //将带错误标识的Excel重新上传，返回错误文件Id供前端下载
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

    /// <summary>
    /// 导入错误信息，记录Excel中出错的位置及错误内容
    /// </summary>
    public class ErrorMessage : TopBasePoco
    {
        /// <summary>
        /// 错误所在行号（对应Excel中的行）
        /// </summary>
        [Display(Name = "Sys.RowIndex")]
        public long Index { get; set; }

        /// <summary>
        /// 错误所在单元格列号
        /// </summary>
        [Display(Name = "Sys.CellIndex")]
        public long Cell { get; set; }
        /// <summary>
        /// 错误信息内容
        /// </summary>
        [Display(Name = "Sys.ErrorMsg")]
        public string Message { get; set; }
    }

    /// <summary>
    /// 错误数据列表
    /// </summary>
    public class TemplateErrorListVM : BasePagedListVM<ErrorMessage, BaseSearcher>
    {

        /// <summary>
        /// 初始化错误列表VM，默认不启用分页
        /// </summary>
        public TemplateErrorListVM()
        {
            EntityList = new List<ErrorMessage>();
            NeedPage = false;
        }

        /// <summary>
        /// 初始化错误列表的表格列头，显示行号和错误信息两列
        /// </summary>
        /// <returns>表格列头定义集合</returns>
        protected override IEnumerable<IGridColumn<ErrorMessage>> InitGridHeader()
        {
            return new List<GridColumn<ErrorMessage>>{
                this.MakeGridHeader(x => x.Index, 60),
                this.MakeGridHeader(x => x.Message)
            };
        }

        /// <summary>
        /// 获取错误列表的查询语句，按行号排序
        /// </summary>
        /// <returns>按行号升序排列的错误信息查询</returns>
        public override IOrderedQueryable<ErrorMessage> GetSearchQuery()
        {
            return EntityList.AsQueryable().OrderBy(x => x.Index);
        }
    }

}
