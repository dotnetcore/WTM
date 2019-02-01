using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc
{
    public class CodeGenListVM : BasePagedListVM<CodeGenListView,BaseSearcher>
    {

        public string ModelFullName { get; set; }

        public CodeGenListVM()
        {
            NeedPage = false;
        }

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

        private string getCheckBox(string fieldname, bool val)
        {
            return UIService.MakeCheckBox(val, name: fieldname, value:"true");
        }

        private string withHidden(string fieldname, string val)
        {
            return val + $"<input type=\"hidden\" name=\"{fieldname}\" value=\"{val}\" />";
        }

        private string subField(string fieldname, CodeGenListView entity)
        {
            string rv = $"<input type=\"hidden\" name=\"{fieldname}.RelatedField\" value=\"{entity.LinkedType}\" />";
            if (string.IsNullOrEmpty(entity.LinkedType) == false)
            {
                var linktype = Type.GetType(entity.LinkedType);
                if (linktype != typeof(FileAttachment))
                {
                    var subpros = Type.GetType(entity.LinkedType).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly).Where(x=>x.GetMemberType() == typeof(string)).OrderBy(x => x.Name).ToList().ToListItems(x => x.Name, x => x.Name);
                    var subproswithname = subpros.Where(x => x.Text.ToLower().Contains("name")).ToList();
                    var subproswithoutname = subpros.Where(x => x.Text.ToLower().Contains("name") == false).ToList();
                    subpros = new List<ComboSelectListItem>();
                    subpros.AddRange(subproswithname);
                    subpros.AddRange(subproswithoutname);
                    rv += UIService.MakeCombo(fieldname + ".SubField", subpros);
                }
                else
                {
                    rv += $"<input type=\"hidden\" name=\"{fieldname}.SubField\" value=\"`file\" />";
                }
            }
            return rv;
        }

        public override IOrderedQueryable<CodeGenListView> GetSearchQuery()
        {
            Type modeltype =  Type.GetType(ModelFullName);
            var pros = modeltype.GetProperties();
            List<CodeGenListView> lv = new List<CodeGenListView>();
            int count = 0;
            Type[] basetype = new Type[] { typeof(BasePoco), typeof(TopBasePoco), typeof(PersistPoco) };
            foreach (var pro in pros)
            {
                if (basetype.Contains(pro.DeclaringType) == false)
                {
                    CodeGenListView view = new CodeGenListView()
                    {
                        FieldName = pro.Name,
                        FieldDes = pro.GetPropertyDisplayName(),
                        Index = count
                    };
                    Type checktype = pro.PropertyType;
                    if (pro.PropertyType.IsNullable())
                    {
                        checktype = pro.PropertyType.GetGenericArguments()[0];
                    }
                    bool show = false;
                    view.IsFormField = true;
                    view.IsListField = true;
                    view.IsImportField = true;
                    if (checktype.IsPrimitive || checktype == typeof(string) || checktype == typeof(DateTime) || checktype.IsEnum())
                    {
                        show = true;
                    }
                    if(checktype == typeof(Guid))
                    {
                        if (pro.Name.ToLower().EndsWith("id"))
                        {
                            var linkedtype = pros.Where(x => x.Name.ToLower() + "id" == pro.Name.ToLower()).FirstOrDefault();
                            if (linkedtype?.PropertyType == typeof(FileAttachment))
                            {
                                view.IsImportField = false;
                            }
                            view.LinkedType = linkedtype?.PropertyType.AssemblyQualifiedName;
                        }
                        show = true;
                    }
                    if(show == true)
                    {
                        lv.Add(view);
                        count++;
                    }
                }
            }
            return lv.AsQueryable().OrderBy(x => x.FieldName);
        }
    }

    public class CodeGenListView : BasePoco
    {
        [Display(Name ="字段名称")]
        public string FieldName { get; set; }

        [Display(Name = "字段描述")]
        public string FieldDes { get; set; }


        [Display(Name = "搜索条件")]
        public bool IsSearcherField { get; set; }

        [Display(Name = "列表展示")]
        public bool IsListField { get; set; }

        [Display(Name = "表单字段")]
        public bool IsFormField { get; set; }


        [Display(Name = "关联表显示字段")]
        public string SubField { get; set; }

        [Display(Name = "导入字段")]
        public bool IsImportField { get; set; }

        [Display(Name = "批量更新字段")]
        public bool IsBatchField { get; set; }

        public int Index { get; set; }

        [Display(Name = "关联类型")]
        public string LinkedType { get; set; }
    }
}
