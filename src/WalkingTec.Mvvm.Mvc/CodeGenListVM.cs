using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Attributes;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// 代码生成器的字段配置列表 VM，以表格形式列出指定模型的所有字段，
    /// 供用户勾选每个字段在查询、列表、表单、导入、批量编辑中的使用方式
    /// </summary>
    public class CodeGenListVM : BasePagedListVM<CodeGenListView,BaseSearcher>
    {

        /// <summary>
        /// 要生成代码的模型类型全名
        /// </summary>
        public string ModelFullName { get; set; }

        /// <summary>
        /// 构造函数，设置不需要分页，字段列表一次性全部显示
        /// </summary>
        public CodeGenListVM()
        {
            NeedPage = false;
        }

        /// <summary>
        /// 初始化表格列头，通过格式化函数为每个字段生成可回传的隐藏域、复选框和下拉框等控件
        /// </summary>
        /// <returns>表格列定义集合</returns>
        protected override IEnumerable<IGridColumn<CodeGenListView>> InitGridHeader()
        {
            return new List<IGridColumn<CodeGenListView>>
            {
                this.MakeGridHeader(x=>x.FieldName,200).SetFormat((entity,val)=>{return withHidden($"FieldInfos[{entity.Index}].FieldName",entity.FieldName); }),
                this.MakeGridHeader(x=>x.FieldDes,200),
                this.MakeGridHeader(x=>x.SubField,200).SetFormat((entity,val)=>{return subField($"FieldInfos[{entity.Index}]",entity); }),
                this.MakeGridHeader(x=>x.IsSearcherField,150).SetFormat((entity,val)=>{return getCheckBox($"FieldInfos[{entity.Index}].IsSearcherField",entity.IsSearcherField); }),
                this.MakeGridHeader(x=>x.IsListField,150).SetFormat((entity,val)=>{return getCheckBox($"FieldInfos[{entity.Index}].IsListField",entity.IsListField); }),
                this.MakeGridHeader(x=>x.IsFormField,150).SetFormat((entity,val)=>{return getCheckBox($"FieldInfos[{entity.Index}].IsFormField",entity.IsFormField); }),
                this.MakeGridHeader(x=>x.IsImportField,150).SetFormat((entity,val)=>{return getCheckBox($"FieldInfos[{entity.Index}].IsImportField",entity.IsImportField); }),
                this.MakeGridHeader(x=>x.IsBatchField,150).SetFormat((entity,val)=>{return getCheckBox($"FieldInfos[{entity.Index}].IsBatchField",entity.IsBatchField); })
          };
        }

        /// <summary>
        /// 生成指定名称的复选框 HTML，用于字段配置项的勾选
        /// </summary>
        /// <param name="fieldname">表单字段名</param>
        /// <param name="val">是否勾选</param>
        /// <returns>复选框 HTML 字符串</returns>
        private string getCheckBox(string fieldname, bool val)
        {
            return UIService.MakeCheckBox(val, name: fieldname, value:"true");
        }

        /// <summary>
        /// 将字段值与一个同名的隐藏域拼接，保证提交表单时字段名能够回传
        /// </summary>
        /// <param name="fieldname">表单字段名</param>
        /// <param name="val">字段值</param>
        /// <returns>拼接后的 HTML 字符串</returns>
        private string withHidden(string fieldname, string val)
        {
            return val + $"<input type='hidden' name='{fieldname}' value='{val}' />";
        }

        /// <summary>
        /// 生成关联字段所需的隐藏域和下拉框；当属性关联了其他模型时，列出关联模型的字符串属性供选择作为下拉显示字段
        /// </summary>
        /// <param name="fieldname">表单字段名（含索引前缀）</param>
        /// <param name="entity">字段配置信息</param>
        /// <returns>生成的 HTML 字符串</returns>
        private string subField(string fieldname, CodeGenListView entity)
        {
            string rv = $"<input type='hidden' name='{fieldname}.RelatedField' value='{entity.LinkedType}' />";
            rv += $"<input type='hidden' name='{fieldname}.SubIdField' value='{entity.SubIdField}' />";
            //配置了关联类型时才需要生成子字段选择
            if (string.IsNullOrEmpty(entity.LinkedType) == false)
            {
                var linktype = Type.GetType(entity.LinkedType);
                if (linktype != typeof(FileAttachment))
                {
                    //获取关联类型的所有字符串属性（排除 BatchError），并转为下拉选项
                    var subpros = Type.GetType(entity.LinkedType).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(x=>x.GetMemberType() == typeof(string) && x.Name != "BatchError").OrderBy(x => x.Name).ToList().ToListItems(x => x.Name, x => x.Name);
                    //名称包含 name 的属性排在前面，便于用户选择下拉显示字段
                    var subproswithname = subpros.Where(x => x.Text.ToLower().Contains("name")).ToList();
                    var subproswithoutname = subpros.Where(x => x.Text.ToLower().Contains("name") == false).ToList();
                    subpros = new List<ComboSelectListItem>();
                    subpros.AddRange(subproswithname);
                    subpros.AddRange(subproswithoutname);
                    //没有任何字符串属性时，使用 Id 作为默认选项
                    if(subpros.Count == 0)
                    {
                        subpros.Add(new ComboSelectListItem { Text = "Id", Value = "Id" });
                    }
                    rv += UIService.MakeCombo(fieldname + ".SubField", subpros);
                }
                else
                {
                    //附件类型使用特殊标记，无需选择子字段
                    rv += $"<input type='hidden' name='{fieldname}.SubField' value='`file' />";
                }
            }
            return rv;
        }

        /// <summary>
        /// 通过反射解析模型的所有属性，生成字段配置列表；
        /// 自动识别外键、一对多、多对多、附件以及 NotMapped 等情况，并设置各字段的默认勾选项
        /// </summary>
        /// <returns>按字段名排序的字段配置查询</returns>
        public override IOrderedQueryable<CodeGenListView> GetSearchQuery()
        {
            Type modeltype =  Type.GetType(ModelFullName);
            var pros = modeltype.GetAllProperties();
            List<CodeGenListView> lv = new List<CodeGenListView>();
            int count = 0;
            //框架基类自带、无需参与代码生成的字段
            List<string> skipFields = new List<string>()
            {
               nameof(TopBasePoco.BatchError),
               nameof(TopBasePoco.Checked),
               nameof(TopBasePoco.ExcelIndex),
            };
            //继承 IBasePoco 的模型，创建人、创建时间等审计字段也跳过
            if (typeof(IBasePoco).IsAssignableFrom(modeltype))
            {
                skipFields.AddRange(
                    new string[]{
               nameof(IBasePoco.CreateBy),
               nameof(IBasePoco.CreateTime),
               nameof(IBasePoco.UpdateBy),
               nameof(IBasePoco.UpdateTime) }
                    );
            }
            //继承 IPersistPoco 的模型，软删除标记 IsValid 也跳过
            if (typeof(IPersistPoco).IsAssignableFrom(modeltype))
            {
                skipFields.Add(nameof(IPersistPoco.IsValid));
            }

            //记录已由关联属性处理过的外键字段名，避免同一外键在列表中生成两行
            List<string> ignoreField = new List<string>();
            foreach (var pro in pros)
            {
                if (skipFields.Contains(pro.Name) == false)
                {
                    //只读属性无法生成录入界面，跳过
                    if(pro.CanWrite == false)
                    {
                        continue;
                    }
                    //非字符串类型的 Id 主键由框架统一处理，跳过
                    if(pro.Name.ToLower() == "id" && pro.PropertyType != typeof(string))
                    {
                        continue;
                    }
                    CodeGenListView view = new CodeGenListView()
                    {
                        FieldName = pro.Name,
                        FieldDes = pro.GetPropertyDisplayName(),
                        SubIdField = "",
                        Index = count
                    };
                    //是否标记了 NotMapped 特性
                    var notmapped = pro.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault();
                    //可空类型取其底层类型后再判断
                    Type checktype = pro.PropertyType;
                    if (pro.PropertyType.IsNullable())
                    {
                        checktype = pro.PropertyType.GetGenericArguments()[0];
                    }
                    //该类型的外键字段已生成过，跳过避免重复
                    if (ignoreField.Contains(checktype.Name))
                    {
                        continue;
                    }
                    //默认勾选为表单、列表、导入字段
                    bool show = false;
                    view.IsFormField = true;
                    view.IsListField = true;
                    view.IsImportField = true;
                    //基元类型、字符串、日期、枚举和 decimal 可直接生成界面
                    if (checktype.IsPrimitive || checktype == typeof(string) || checktype == typeof(DateTime) || checktype.IsEnum() || checktype == typeof(decimal))
                    {
                        show = true;
                    }
                    //属性类型为模型类时，按外键关联（一对多）处理
                    if (typeof(TopBasePoco).IsAssignableFrom(checktype)){
                        //从 EF Core 模型中查找该导航属性对应的外键属性名
                        var fk = DC.GetFKName2(modeltype, pro.Name);
                        if(fk != null)
                        {
                            ignoreField.Add(fk);
                            show = true;
                        }
                        //附件类型标记为附件且不支持导入，其他模型类型按一对多关联处理
                        if (checktype == typeof(FileAttachment))
                        {
                            view.IsImportField = false;
                            view.FieldDes += $"({MvcProgram._localizer["Codegen.Attachment"]})";
                        }
                        else
                        {
                            view.FieldDes += $"({MvcProgram._localizer["Codegen.OneToMany"]})";
                        }
                        view.LinkedType = checktype.AssemblyQualifiedName;
                        if (fk != null)
                        {
                            //模型中不存在该外键属性时，在描述中给出错误提示
                            if (modeltype.GetSingleProperty(fk) == null)
                            {
                                view.FieldDes = $"<font color='#ff0000'>(Error:Can't find {fk.Replace("ID","Id")} in {checktype.Name})</font>";
                            }
                        }
                    }
                    //集合属性按多对多中间表处理
                    if (checktype.IsList())
                    {
                        checktype = pro.PropertyType.GetGenericArguments()[0];
                        if (checktype.IsNullable())
                        {
                            checktype = checktype.GetGenericArguments()[0];
                        }
                        //仅处理标记了 MiddleTable 特性的中间表类型
                        var middletable = checktype.GetCustomAttributes(typeof(MiddleTableAttribute), false).FirstOrDefault();
                        if (middletable != null)
                        {
                            view.FieldDes += $"({MvcProgram._localizer["Codegen.ManyToMany"]})";
                            view.IsImportField = false;
                            var subpros = checktype.GetAllProperties();
                            foreach (var spro in subpros)
                            {
                                if(skipFields.Contains(spro.Name) == false)
                                {
                                    Type subchecktype = spro.PropertyType;
                                    if (spro.PropertyType.IsNullable())
                                    {
                                        subchecktype = spro.PropertyType.GetGenericArguments()[0];
                                    }
                                    //中间表中指向其他模型（排除自身）的属性即为多对多关联
                                    if (typeof(TopBasePoco).IsAssignableFrom(subchecktype) && subchecktype != modeltype)
                                    {
                                        view.LinkedType = subchecktype.AssemblyQualifiedName;
                                        var fk = DC.GetFKName2(checktype, spro.Name);
                                        view.SubIdField = fk;
                                        show = true;
                                        //中间表中找不到外键属性时，在描述中给出错误提示
                                        if(checktype.GetSingleProperty(fk) == null) {
                                            view.FieldDes = $"<font color='#ff0000'>(Error:Can't find {fk.Replace("ID", "Id")} in {checktype.Name})</font>";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //NotMapped 字段不参与查询、表单、导入、批量、列表等任何生成选项
                    if (notmapped != null)
                    {
                        view.FieldDes += "(NotMapped)";
                        view.IsFormField = false;
                        view.IsSearcherField = false;
                        view.IsBatchField = false;
                        view.IsImportField = false;
                        view.IsListField = false;
                    }
                    if (show == true)
                    {
                        lv.Add(view);
                        count++;
                    }
                }
            }

            //移除与已处理外键重名的字段行，并将其后所有字段的序号依次前移
            for (int i = 0; i < lv.Count; i++)
            {
                if (ignoreField.Contains(lv[i].FieldName))
                {
                    for(int j = i; j < lv.Count; j++)
                    {
                        lv[j].Index--;
                    }
                    lv.RemoveAt(i);
                    i--;
                }
            }

            return lv.AsQueryable().OrderBy(x => x.FieldName);
        }
    }

    /// <summary>
    /// 代码生成器字段列表的行数据模型，描述模型中单个字段的各项生成配置
    /// </summary>
    public class CodeGenListView : BasePoco
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        [Display(Name = "_Admin.FieldName")]
        public string FieldName { get; set; }

        /// <summary>
        /// 字段显示名，可能附带附件、一对多、多对多或 NotMapped 等标记
        /// </summary>
        [Display(Name = "Codegen.FieldDes")]
        public string FieldDes { get; set; }


        /// <summary>
        /// 是否作为查询（Searcher）条件字段
        /// </summary>
        [Display(Name = "Codegen.IsSearcherField")]
        public bool IsSearcherField { get; set; }

        /// <summary>
        /// 是否在列表页面显示
        /// </summary>
        [Display(Name = "Codegen.IsListField")]
        public bool IsListField { get; set; }

        /// <summary>
        /// 是否在表单页面显示
        /// </summary>
        [Display(Name = "Codegen.IsFormField")]
        public bool IsFormField { get; set; }


        /// <summary>
        /// 关联模型中用作下拉框显示的子字段名
        /// </summary>
        [Display(Name = "Codegen.SubField")]
        public string SubField { get; set; }

        /// <summary>
        /// 多对多中间表中指向关联模型的外键属性名
        /// </summary>
        public string SubIdField { get; set; }

        /// <summary>
        /// 是否参与 Excel 导入
        /// </summary>
        [Display(Name = "Codegen.IsImportField")]
        public bool IsImportField { get; set; }

        /// <summary>
        /// 是否参与批量编辑
        /// </summary>
        [Display(Name = "Codegen.IsBatchField")]
        public bool IsBatchField { get; set; }

        /// <summary>
        /// 字段在列表中的序号，用于表单回传时定位 FieldInfos 数组中的元素
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 关联模型类型的程序集限定名
        /// </summary>
        [Display(Name = "Codegen.LinkedType")]
        public string LinkedType { get; set; }

    }
}
