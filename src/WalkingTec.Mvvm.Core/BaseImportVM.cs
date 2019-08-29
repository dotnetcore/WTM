using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
        /// 下载模板显示名称
        /// </summary>
        public string FileDisplayName { get; set; }

        /// <summary>
        /// 错误列表
        /// </summary>
        public TemplateErrorListVM ErrorListVM { get; set; }

        /// <summary>
        /// 是否验证模板类型（当其他系统模板导入到某模块时可设置为False）
        /// </summary>
        public bool ValidityTemplateType { get; set; }

        /// <summary>
        /// 下载模版页面的参数
        /// </summary>
        public Dictionary<string, string> Parms { get; set; }

        protected List<T> TemplateData;


        /// <summary>
        /// 要导入的Model列表
        /// </summary>
        public List<P> EntityList { get; set; }

        /// <summary>
        /// 模版
        /// </summary>
        public T Template { get; set; }

        //Model数据是否已被赋值
        protected bool isEntityListSet = false;

        protected HSSFWorkbook hssfworkbook;
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
        public byte[] GenerateTemplate(out string displayName)
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

        #region 可重写方法

        /// <summary>
        /// 设置数据唯一性验证，子类中如果需要数据唯一性验证，应重写此方法
        /// </summary>
        /// <returns>唯一性属性</returns>
        public virtual DuplicatedInfo<P> SetDuplicatedCheck()
        {
            return null;
        }

        /// <summary>
        /// 获取上传的结果值
        /// </summary>
        public virtual void SetEntityList()
        {
            EntityList = new List<P>();
            //获取Model类的所有属性
            var pros = typeof(P).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var excelPros = typeof(T).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(x => x.FieldType == typeof(ExcelPropety)).ToList();
            if(TemplateData == null)
            {
                DoMapList();
            }
            //循环Excel中的数据
            foreach (var item in TemplateData)
            {
                int rowIndex = 2;
                bool isMainData = false;
                //主表信息
                Dictionary<string, ExcelPropety> mainInfo = new Dictionary<string, ExcelPropety>();
                string mainValString = string.Empty;

                //子表信息
                Dictionary<Type, List<FieldInfo>> subpros = new Dictionary<Type, List<FieldInfo>>();
                Dictionary<Type, string> subValString = new Dictionary<Type, string>();

                //循环TemplateVM中定义的所有的列，区分出主子表
                foreach (var epro in excelPros)
                {
                    //获取本列的ExcelProperty的值
                    if (typeof(T).GetField(epro.Name).GetValue(item) is ExcelPropety ep)
                    {
                        //如果是子表的字段
                        if (ep.SubTableType != null)
                        {
                            //保存子表字段信息稍后处理
                            if (!subpros.ContainsKey(ep.SubTableType))
                            {
                                subpros[ep.SubTableType] = new List<FieldInfo>();
                            }
                            subpros[ep.SubTableType].Add(epro);
                        }
                        else
                        {
                            //PropertyHelper.SetPropertyValue(entity, epro.Name, ep.Value, stringBasedValue: true);
                            //保存子表字段信息稍后处理
                            mainInfo.Add(ep.FieldName, ep);
                            mainValString += ep.Value;
                        }
                    }
                }

                //子表信息是否为空
                foreach (var sub in subpros)
                {
                    string subVal = string.Empty;
                    foreach (var field in sub.Value)
                    {
                        ExcelPropety ep = typeof(T).GetField(field.Name).GetValue(item) as ExcelPropety;
                        subVal += ep.Value;
                    }
                    subValString.Add(sub.Key, subVal);
                }

                P entity = null;
                //说明主表信息为空
                if (string.IsNullOrEmpty(mainValString))
                {
                    entity = EntityList.LastOrDefault();
                }
                else
                {
                    //初始化一个新的Entity
                    entity = new P();
                    isMainData = true;
                    //给主表赋值
                    foreach (var mep in mainInfo)
                    {
                        SetEntityFieldValue(entity, mep.Value, rowIndex, mep.Key, item);
                    }
                }

                //给子表赋值
                foreach (var sub in subpros)
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
                                var subList = entity.GetType().GetProperty(pro.Name).GetValue(entity);

                                //如果子表不为空
                                if (!string.IsNullOrEmpty(subValString.Where(x => x.Key == sub.Key).FirstOrDefault().Value))
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
                                    var obj = gtype.GetConstructor(System.Type.EmptyTypes).Invoke(null);

                                    //给SubTableType中和本ExcelProperty同名的字段赋值
                                    foreach (var field in sub.Value)
                                    {
                                        ExcelPropety ep = typeof(T).GetField(field.Name).GetValue(item) as ExcelPropety;
                                        //PropertyHelper.SetPropertyValue(obj, field.Name, ep.Value, stringBasedValue: true);
                                        SetEntityFieldValue(obj, ep, rowIndex, ep.FieldName, item);
                                    }
                                    //将付好值得SubTableType实例添加到List中
                                    list.Add(obj);
                                    PropertyHelper.SetPropertyValue(entity, pro.Name, list);
                                }
                                break;
                            }
                        }
                    }

                }
                entity.ExcelIndex = item.ExcelIndex;
                var cinfo = this.SetDuplicatedCheck();
                //if (IsUpdateRecordDuplicated(cinfo, entity) == false)
                //{
                    if (isMainData)
                    {
                        EntityList.Add(entity);
                    }
                //}
            }
            isEntityListSet = true;
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
                            ErrorListVM.EntityList.Add(new ErrorMessage { Message = processResult.EntityValues[0].ErrorMsg, ExcelIndex = rowIndex });
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
                                ErrorListVM.EntityList.Add(new ErrorMessage { Message = entityValue.ErrorMsg, ExcelIndex = rowIndex });
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
                    ErrorListVM.EntityList.Add(new ErrorMessage { Message = errorMsg, ExcelIndex = rowIndex });
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
                    MemberExpression idLeft = Expression.Property(para, "Id");
                    ConstantExpression idRight = Expression.Constant(entity.ID);
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
                            conExp = Expression.And(conExp, conditions[i]);
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
                    MemberExpression idLeft = Expression.Property(para, "Id");
                    ConstantExpression idRight = Expression.Constant(entity.ID);
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
                            conExp = Expression.And(conExp, conditions[i]);
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
                            ErrorListVM.EntityList.Add(new ErrorMessage { Message = AllName + "数据重复", Index = entity.ExcelIndex });
                        }
                        //如果多个字段重复，则拼接形成 xx，yy，zz组合字段重复 这种提示
                        else if (props.Count > 1)
                        {
                            ErrorListVM.EntityList.Add(new ErrorMessage { Message = AllName + "组合字段重复", Index = entity.ExcelIndex });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 保存指定表中的数据
        /// </summary>
        /// <returns>成功返回True，失败返回False</returns>
        public virtual bool BatchSaveData()
        {
            //如果没有赋值，则调用赋值函数
            if (isEntityListSet == false)
            {
                SetEntityList();
            }
            //如果在复制过程中已经有错误，则直接输出错误
            if (ErrorListVM.EntityList.Count > 0)
            {
                DoReInit();
                return false;
            }
            //进行Model层面的验证
            //找到对应的BaseCRUDVM，并初始化
            var vms = this.GetType().Assembly.GetExportedTypes().Where(x => x.IsSubclassOf(typeof(BaseCRUDVM<P>))).ToList();

            var vmtype = vms.Where(x => x.Name.ToLower() == typeof(P).Name.ToLower() + "vm").FirstOrDefault();
            if(vmtype == null)
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
            DuplicatedInfo<P> finalInfo = new DuplicatedInfo<P>
            {
                Groups = new List<DuplicatedGroup<P>>()
            };
            if (dinfo == null)
            {
                dinfo = new DuplicatedInfo<P>();
            }
            if (cinfo == null)
            {
                cinfo = new DuplicatedInfo<P>();
            }
            if (dinfo.Groups != null)
            {
                foreach (var di in dinfo.Groups)
                {
                    bool found = false;
                    if (cinfo.Groups != null)
                    {
                        foreach (var ci in cinfo.Groups)
                        {
                            if (di.ToString() == ci.ToString())
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    if (found == false)
                    {
                        finalInfo.Groups.Add(di);
                    }
                }
            }
            //todo: RedoValidation
            //BaseController bc = new BaseController();
            foreach (var entity in EntityList)
            {
                //bool check = bc.RedoValidation(entity);
                //if (check == false)
                //{
                //    ErrorListVM.ErrorList.Add(new ErrorMessage { Message = bc.ModelState.Where(x => x.Value.Errors != null && x.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).FirstOrDefault(), Index = entity.ExcelIndex });
                //}
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
            //如果上述验证已经有错误，则直接输出错误
            if (ErrorListVM.EntityList.Count > 0)
            {
                (vm as BaseVM)?.MSD.Clear();
                DoReInit();
                return false;
            }

            //循环数据列表
            foreach (var item in EntityList)
            {
                //根据唯一性的设定查找数据库中是否有同样的数据
                P exist = IsDuplicateData(item,finalInfo);
                //如果有重复数据，则进行修改
                if (exist != null)
                {
                    //如果是普通字段，则直接赋值
                    var tempPros = typeof(T).GetFields();
                    foreach (var pro in tempPros)
                    {
                        var excelProp = Template.GetType().GetField(pro.Name).GetValue(Template) as ExcelPropety;
                        var proToSet = typeof(P).GetProperty(excelProp.FieldName);
                        if (proToSet != null)
                        {
                            var val = proToSet.GetValue(item);
                            PropertyHelper.SetPropertyValue(exist, excelProp.FieldName, val, stringBasedValue: true);
                        }
                    }
                    //更新修改时间字段
                    if (tempPros.Where(x => x.Name == "UpdateTime").SingleOrDefault() == null)
                    {
                        if (typeof(BasePoco).IsAssignableFrom(exist.GetType()))
                        {
                            (exist as BasePoco).UpdateTime = DateTime.Now;
                        }
                    }
                    //更新修改人字段
                    if (tempPros.Where(x => x.Name == "UpdateBy").SingleOrDefault() == null)
                    {
                        if (typeof(BasePoco).IsAssignableFrom(exist.GetType()))
                        {
                            (exist as BasePoco).UpdateBy = LoginUserInfo.ITCode;
                        }
                    }
                    exist.ExcelIndex = item.ExcelIndex;
                    //更新字段
                    DC.UpdateEntity(exist);
                }
                //如果没有重复数据，则填加
                else
                {
                    if (typeof(BasePoco).IsAssignableFrom(item.GetType()))
                    {
                        (item as BasePoco).CreateTime = DateTime.Now;
                        (item as BasePoco).CreateBy = LoginUserInfo?.ITCode;
                    }
                    DC.Set<P>().Add(item);
                }
            }
            //如果有错误，则返回
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
                }
                catch (Exception e)
                {
                    SetExceptionMessage(e, null);
                    DoReInit();
                    return false;
                }
            }
            return true;
        }


        #endregion

        #region 上传数据并验证
        /// <summary>
        /// 读取模版中的数据
        /// </summary>
        private void DoMapList()
        {
            try
            {
                Template.InitExcelData();
                Template.InitCustomFormat();
                TemplateData = new List<T>();
                hssfworkbook = new HSSFWorkbook();
                if (UploadFileId == null)
                {
                    ErrorListVM.EntityList.Add(new ErrorMessage { Message = "请上传模板文件" });
                    return;
                }
                var fa = DC.Set<FileAttachment>().Where(x => x.ID == UploadFileId).SingleOrDefault();
                hssfworkbook = FileHelper.GetHSSWorkbook(hssfworkbook, (FileAttachment)fa, ConfigInfo);

                if (ValidityTemplateType && hssfworkbook.GetSheetAt(1).GetRow(0).Cells[2].ToString() != typeof(T).Name)
                {
                    ErrorListVM.EntityList.Add(new ErrorMessage { Message = "错误的模板" });
                    return;
                }
                ISheet sheet = hssfworkbook.GetSheetAt(0);
                System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                var propetys = Template.GetType().GetFields().Where(x => x.FieldType == typeof(ExcelPropety)).ToList();

                //所有ExcelPropety属性
                List<ExcelPropety> excelPropetys = new List<ExcelPropety>();

                for (int porpetyIndex = 0; porpetyIndex < propetys.Count(); porpetyIndex++)
                {
                    ExcelPropety ep = (ExcelPropety)propetys[porpetyIndex].GetValue(Template);
                    excelPropetys.Add(ep);
                }

                #region 验证模版正确性 add by dufei

                //取得列数
                int columnCount = excelPropetys.Count;
                //int excelPropetyCount = excelPropetys.Count;
                var dynamicColumn = excelPropetys.Where(x => x.DataType == ColumnDataType.Dynamic).FirstOrDefault();
                if (dynamicColumn != null)
                {
                    columnCount = columnCount + dynamicColumn.DynamicColumns.Count - 1;
                    //excelPropetyCount = excelPropetyCount + dynamicColumn.DynamicColumns.Count - 1;
                }

                int pIndex = 0;
                var cells = sheet.GetRow(0).Cells;
                if (columnCount != cells.Count)
                {
                    ErrorListVM.EntityList.Add(new ErrorMessage { Message = "请下载新模板或上传符合当前功能的模板" });
                    return;
                }
                else
                {
                    bool hassubtable = false;
                    for (int i = 0; i < cells.Count; i++)
                    {
                        if (excelPropetys[pIndex].SubTableType != null)
                        {
                            hassubtable = true;
                        }
                        if (excelPropetys[pIndex].DataType != ColumnDataType.Dynamic)
                        {
                            if (cells[i].ToString().Trim('*') != excelPropetys[pIndex].ColumnName)
                            {
                                ErrorListVM.EntityList.Add(new ErrorMessage { Message = "请下载新模板或上传符合当前功能的模板" });
                                return;
                            }
                            pIndex++;
                        }
                        else
                        {
                            var listDynamicColumns = excelPropetys[i].DynamicColumns;
                            int dcCount = listDynamicColumns.Count;
                            for (int dclIndex = 0; dclIndex < dcCount; dclIndex++)
                            {
                                if (cells[i].ToString().Trim('*') != listDynamicColumns[dclIndex].ColumnName)
                                {
                                    ErrorListVM.EntityList.Add(new ErrorMessage { Message = "请下载新模板或上传符合当前功能的模板" });
                                    break;
                                }
                                i = i + 1;
                            }
                            i = i - 1;
                            pIndex++;
                        }
                    }

                    //如果有子表，则设置主表字段非必填
                    if (hassubtable == true)
                    {
                        for (int i = 0; i < cells.Count; i++)
                        {
                            if (excelPropetys[i].SubTableType == null)
                            {
                                excelPropetys[i].IsNullAble = true;
                            }

                        }
                    }
                }
                #endregion

                int rowIndex = 2;
                rows.MoveNext();
                while (rows.MoveNext())
                {
                    HSSFRow row = (HSSFRow)rows.Current;
                    if (IsEmptyRow(row, columnCount))
                    {
                        return;
                    }
                    T result = new T();
                    int propetyIndex = 0;
                    for (int i = 0; i < columnCount; i++)
                    {
                        ExcelPropety excelPropety = CopyExcelPropety(excelPropetys[propetyIndex]); //excelPropetys[propetyIndex]; 
                        var pts = propetys[propetyIndex];
                        string value = row.GetCell(i, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();

                        if (excelPropety.DataType == ColumnDataType.Dynamic)
                        {
                            int dynamicColCount = excelPropety.DynamicColumns.Count();
                            for (int dynamicColIndex = 0; dynamicColIndex < dynamicColCount; dynamicColIndex++)
                            {
                                //验证数据类型并添加错误信息
                                excelPropety.DynamicColumns[dynamicColIndex].ValueValidity(row.GetCell(i + dynamicColIndex, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString(), ErrorListVM.EntityList, rowIndex);
                            }
                            i = i + dynamicColCount - 1;
                        }
                        else
                        {
                            excelPropety.ValueValidity(value, ErrorListVM.EntityList, rowIndex);
                        }

                        if (ErrorListVM.EntityList.Count == 0)
                        {

                            pts.SetValue(result, excelPropety);
                        }
                        propetyIndex++;
                    }
                    result.ExcelIndex = rowIndex;
                    TemplateData.Add(result);
                    rowIndex++;
                }
                return;
            }
            catch
            {
                ErrorListVM.EntityList.Add(new ErrorMessage { Message = "请下载新模板或上传符合当前功能的模板" });
                //ErrorListVM.ErrorList.Add(new ErrorMessage { Message = ex.Message });
            }
            return;
        }

        #endregion

        #region 验证是否空行
        /// <summary>
        /// 验证Excel中某行是否为空行
        /// </summary>
        /// <param name="row">行数</param>
        /// <param name="colCount">列数</param>
        /// <returns>True代表空行，False代表非空行</returns>
        private bool IsEmptyRow(HSSFRow row, int colCount)
        {
            bool result = true;
            for (int i = 0; i < colCount; i++)
            {
                string value = row.GetCell(i, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
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
                    if(de.Entries.Count == 0)
                    {
                        ErrorListVM.EntityList.Add(new ErrorMessage { Index = 0, Message = e.Message });
                    }
                    //循环此错误相关的数据
                    foreach (var ent in de.Entries)
                    {
                        //获取错误数据Id
                        var errorId = (long)((ent.Entity as TopBasePoco).ExcelIndex);
                        //根据State判断修改或删除操作，输出不同的错误信息
                        if (ent.State == EntityState.Deleted)
                        {
                            ErrorListVM.EntityList.Add(new ErrorMessage { Index = errorId, Message = "数据被使用，无法删除" });
                        }
                        else if (ent.State == EntityState.Modified)
                        {
                            ErrorListVM.EntityList.Add(new ErrorMessage { Index = errorId, Message = "修改失败" });
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
            }
        }
        #endregion

        #region 验证数据重复
        /// <summary>
        /// 判断数据是否在库中存在重复数据
        /// </summary>
        /// <param name="Entity">要验证的数据</param>
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
                    if (conditions.Count > 0)
                    {
                        //循环添加条件并生成Where语句
                        Expression conExp = conditions[0];
                        for (int i = 1; i < conditions.Count; i++)
                        {
                            conExp = Expression.And(conExp, conditions[i]);
                        }

                        MethodCallExpression whereCallExpression = Expression.Call(
                             typeof(Queryable),
                             "Where",
                             new Type[] { modelType },
                             baseExp.Expression,
                             Expression.Lambda<Func<P, bool>>(conExp, new ParameterExpression[] { para }));
                        //Expression subSelect = Expression.Call(
                        //       typeof(Queryable),
                        //       "Select",
                        //       new Type[] { modelType, typeof(long)},
                        //       whereCallExpression,
                        //       Expression.Lambda<Func<P, long>>(Expression.PropertyOrField(para, "Id"), new ParameterExpression[] { para }));

                        var result = baseExp.Provider.CreateQuery(whereCallExpression);
                        foreach (var res in result)
                        {
                            return res as P;
                        }
                    }
                }
            }
            return null;
        }
        #endregion


        /// <summary>
        /// 创建重复数据信息
        /// </summary>
        /// <param name="FieldExps">重复数据信息</param>
        /// <returns>重复数据信息</returns>
        protected DuplicatedInfo<T> CreateFieldsInfo(params DuplicatedField<T>[] FieldExps)
        {
            DuplicatedInfo<T> d = new DuplicatedInfo<T>();
            d.AddGroup(FieldExps);
            return d;
        }

        /// <summary>
        /// 创建一个简单重复数据信息
        /// </summary>
        /// <param name="FieldExp">重复数据的字段</param>
        /// <returns>重复数据信息</returns>
        public static DuplicatedField<T> SimpleField(Expression<Func<T, object>> FieldExp)
        {
            return new DuplicatedField<T>(FieldExp);
        }

        /// <summary>
        /// 创建一个关联到其他表数组中数据的重复信息
        /// </summary>
        /// <typeparam name="V">关联表类</typeparam>
        /// <param name="MiddleExp">指向关联表类数组的Lambda</param>
        /// <param name="FieldExps">指向最终字段的Lambda</param>
        /// <returns>重复数据信息</returns>
        public static DuplicatedField<T> SubField<V>(Expression<Func<T, List<V>>> MiddleExp, params Expression<Func<V, object>>[] FieldExps)
        {
            return new ComplexDuplicatedField<T, V>(MiddleExp, FieldExps);
        }

        public ErrorObj GetErrorJson()
        {
            var mse = new ErrorObj();
            mse.Form = new Dictionary<string, string>();
            var err = ErrorListVM?.EntityList?.Where(x => x.Index == 0).FirstOrDefault()?.Message;
            if (string.IsNullOrEmpty(err))
            {
                var fa = DC.Set<FileAttachment>().Where(x => x.ID == UploadFileId).SingleOrDefault();
                hssfworkbook = FileHelper.GetHSSWorkbook(hssfworkbook, (FileAttachment)fa, ConfigInfo);

                var propetys = Template.GetType().GetFields().Where(x => x.FieldType == typeof(ExcelPropety)).ToList();
                List<ExcelPropety> excelPropetys = new List<ExcelPropety>();
                for (int porpetyIndex = 0; porpetyIndex < propetys.Count(); porpetyIndex++)
                {
                    ExcelPropety ep = (ExcelPropety)propetys[porpetyIndex].GetValue(Template);
                    excelPropetys.Add(ep);
                }
                int columnCount = excelPropetys.Count;
                //int excelPropetyCount = excelPropetys.Count;
                var dynamicColumn = excelPropetys.Where(x => x.DataType == ColumnDataType.Dynamic).FirstOrDefault();
                if (dynamicColumn != null)
                {
                    columnCount = columnCount + dynamicColumn.DynamicColumns.Count - 1;
                }
                ISheet sheet = hssfworkbook.GetSheetAt(0);
                var errorStyle = hssfworkbook.CreateCellStyle();
                IFont f = hssfworkbook.CreateFont();
                f.Color = HSSFColor.Red.Index;
                errorStyle.SetFont(f);
                errorStyle.IsLocked = true;
                foreach (var e in ErrorListVM?.EntityList)
                {
                    if(e.Index > 0)
                    {
                        var c = sheet.GetRow((int)(e.Index - 1)).CreateCell(columnCount);
                        c.CellStyle = errorStyle;
                        c.SetCellValue(e.Message);
                    }
                }
                MemoryStream ms = new MemoryStream();
                hssfworkbook.Write(ms);
                ms.Position = 0;
                FileAttachmentVM vm = new FileAttachmentVM();
                vm.CopyContext(this);
                vm.Entity.FileName = "Error-"+ fa.FileName;
                vm.Entity.Length = ms.Length;
                vm.Entity.UploadTime = DateTime.Now;
                vm.Entity.SaveFileMode = ConfigInfo.FileUploadOptions.SaveFileMode;
                vm = FileHelper.GetFileByteForUpload(vm, ms, ConfigInfo, vm.Entity.FileName, null, null);
                vm.Entity.IsTemprory = true;
                if ((!string.IsNullOrEmpty(vm.Entity.Path) && (vm.Entity.SaveFileMode == SaveFileModeEnum.Local || vm.Entity.SaveFileMode == SaveFileModeEnum.DFS)) || (vm.Entity.FileData != null && vm.Entity.SaveFileMode == SaveFileModeEnum.Database))
                {
                    vm.DoAdd();
                }
                ms.Close();
                ms.Dispose();
                err = "导入时发生错误";
                mse.Form.Add("Entity.Import", err);
                mse.Form.Add("Entity.ErrorFileId", vm.Entity.ID.ToString());
            }
            else
            {
                mse.Form.Add("Entity.Import", err);
            }
            return mse;
        }
    }

    #region 辅助类
    public class ErrorMessage : TopBasePoco
    {
        [Display(Name = "行号")]
        public long Index { get; set; }

        [Display(Name = "列号")]
        public long Cell { get; set; }
        [Display(Name = "错误信息")]
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

    #endregion

}