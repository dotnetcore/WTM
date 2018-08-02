using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc
{
    [ReInit(ReInitModes.ALWAYS)]
    public class CodeGenVM : BaseVM
    {
        public CodeGenListVM FieldList { get; set; }

        public List<FieldInfo> FieldInfos { get; set; }

        public string PreviewFile { get; set; }

        public string ModelName
        {
            get
            {
                return SelectedModel?.Split(',').FirstOrDefault()?.Split('.').LastOrDefault()??"";
            }
        }
        [Display(Name = "Model命名空间")]
        [ValidateNever()]
        public string ModelNS => SelectedModel?.Split(',').FirstOrDefault()?.Split('.').SkipLast(1).ToSpratedString(seperator: ".");
        [Display(Name = "模块名称")]
        [Required(ErrorMessage = "{0}是必填项")]
        public string ModuleName { get; set; }
        [RegularExpression("^[A-Za-z_]+", ErrorMessage ="{0}只能以英文字母或下划线开头")]
        public string Area { get; set; }
        [ValidateNever()]
        [BindNever()]
        public List<ComboSelectListItem> AllModels { get; set; }
        [Required(ErrorMessage = "{0}是必填项")]
        [Display(Name = "选择模型")]
        public string SelectedModel { get; set; }
        [ValidateNever()]
        public string EntryDir { get; set; }


        public string _mainDir;
        [ValidateNever()]
        public string MainDir
        {
            get
            {
                if (_mainDir == null)
                {
                    _mainDir = EntryDir?.Replace("\\bin\\Debug\\netcoreapp2.0\\", "", true, null);
                }
                return _mainDir;
            }
            set
            {
                _mainDir = value;
            }
        }

        public string _vmdir;
        [ValidateNever()]
        public string VmDir
        {
            get
            {
                if (_vmdir == null)
                {
                    var up = Directory.GetParent(MainDir);
                    var vmdir = up.GetDirectories().Where(x => x.Name.ToLower().EndsWith(".viewmodel")).FirstOrDefault();
                    if (vmdir == null)
                    {
                        if (string.IsNullOrEmpty(Area))
                        {
                            vmdir = Directory.CreateDirectory(MainDir + $"\\ViewModels\\{ModelName}VMs");
                        }
                        else
                        {
                            vmdir = Directory.CreateDirectory(MainDir + $"\\Areas\\{Area}\\ViewModels\\{ModelName}VMs");
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Area))
                        {
                            vmdir = Directory.CreateDirectory(vmdir.FullName + $"\\{ModelName}VMs");
                        }
                        else
                        {
                            vmdir = Directory.CreateDirectory(vmdir.FullName + $"\\{Area}\\{ModelName}VMs");
                        }

                    }
                    _vmdir = vmdir.FullName;
                }
                return _vmdir;
            }
        }

        public string _controllerdir;
        [ValidateNever()]
        public string ControllerDir
        {
            get
            {
                if (_controllerdir == null)
                {
                    if (string.IsNullOrEmpty(Area))
                    {
                        _controllerdir = Directory.CreateDirectory(MainDir + "\\Controllers").FullName;
                    }
                    else
                    {
                        _controllerdir = Directory.CreateDirectory(MainDir + $"\\Areas\\{Area}\\Controllers").FullName;
                    }
                }
                return _controllerdir;
            }
        }

        public string _viewdir;
        [ValidateNever()]
        public string ViewDir
        {
            get
            {
                if (_viewdir == null)
                {
                    if (string.IsNullOrEmpty(Area))
                    {
                        _viewdir = Directory.CreateDirectory(MainDir + $"\\Views\\{ModelName}").FullName;
                    }
                    else
                    {
                        _viewdir = Directory.CreateDirectory(MainDir + $"\\Areas\\{Area}\\Views\\{ModelName}").FullName;
                    }
                }
                return _viewdir;
            }
        }

        
        private string _controllerNs;
        [Display(Name = "Controller命名空间")]
        [ValidateNever()]
        public string ControllerNs
        {
            get
            {
                if(_controllerNs == null)
                {
                    int index = MainDir.LastIndexOf("\\");
                    if(index > 0)
                    {
                        _controllerNs = MainDir.Substring(index + 1);
                    }
                    else
                    {
                        _controllerNs = MainDir;
                    }
                    _controllerNs += ".Controllers";
                }
                return _controllerNs;
            }
            set
            {
                _controllerNs = value;
            }
        }

        private string _vmNs;
        [Display(Name = "VM命名空间")]
        [ValidateNever()]
        public string VMNs
        {
            get
            {
                if (_vmNs == null)
                {
                    var up = Directory.GetParent(MainDir);
                    var vmdir = up.GetDirectories().Where(x => x.Name.ToLower().EndsWith(".viewmodel")).FirstOrDefault();
                    if (vmdir == null)
                    {
                        if (string.IsNullOrEmpty(Area))
                        {
                            _vmNs = ControllerNs.Replace(".Controllers", $".ViewModels.{ModelName}VMs");
                        }
                        else
                        {
                            _vmNs = ControllerNs.Replace(".Controllers", $".{Area}.ViewModels.{ModelName}VMs");
                        }
                    }
                    else
                    {
                        int index = vmdir.FullName.LastIndexOf("\\");
                        if (index > 0)
                        {
                            _vmNs = vmdir.FullName.Substring(index + 1);
                        }
                        else
                        {
                            _vmNs = vmdir.FullName;
                        }
                        if (string.IsNullOrEmpty(Area))
                        {
                            _vmNs += $".{ModelName}VMs";
                        }
                        else
                        {
                            _vmNs += $".{Area}.{ModelName}VMs";
                        }
                    }
                }
                return _vmNs;
            }
            set
            {
                _vmNs = value;
            }
        }

        protected override void InitVM()
        {
            FieldList = new CodeGenListVM();
            FieldList.CopyContext(this);
        }
        public void DoGen()
        {
            File.WriteAllText($"{ControllerDir}\\{ModelName}Controller.cs",GenerateController(), Encoding.UTF8);

            File.WriteAllText($"{VmDir}\\{ModelName}VM.cs", GenerateVM("CrudVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}\\{ModelName}ListVM.cs", GenerateVM("ListVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}\\{ModelName}BatchVM.cs", GenerateVM("BatchVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}\\{ModelName}ImportVM.cs", GenerateVM("ImportVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}\\{ModelName}Searcher.cs", GenerateVM("Searcher"), Encoding.UTF8);

            File.WriteAllText($"{ViewDir}\\Index.cshtml", GenerateView("ListView"), Encoding.UTF8);
            File.WriteAllText($"{ViewDir}\\Create.cshtml", GenerateView("CreateView"), Encoding.UTF8);
            File.WriteAllText($"{ViewDir}\\Edit.cshtml", GenerateView("EditView"), Encoding.UTF8);
            File.WriteAllText($"{ViewDir}\\Delete.cshtml", GenerateView("DeleteView"), Encoding.UTF8);
            File.WriteAllText($"{ViewDir}\\Details.cshtml", GenerateView("DetailsView"), Encoding.UTF8);
            File.WriteAllText($"{ViewDir}\\Import.cshtml", GenerateView("ImportView"), Encoding.UTF8);
            File.WriteAllText($"{ViewDir}\\BatchEdit.cshtml", GenerateView("BatchEditView"), Encoding.UTF8);
            File.WriteAllText($"{ViewDir}\\BatchDelete.cshtml", GenerateView("BatchDeleteView"), Encoding.UTF8);

        }

        public string GenerateController()
        {
            var rv = GetResource("Controller.txt").Replace("$vmnamespace$",VMNs).Replace("$namespace$", ControllerNs).Replace("$des$", ModuleName).Replace("$modelname$", ModelName);
            if (string.IsNullOrEmpty(Area))
            {
                rv = rv.Replace("$area$", "");
            }
            else
            {
                rv = rv.Replace("$area$", $"[Area(\"{Area}\")]");
            }
            return rv;
        }

        public string GenerateVM(string name)
        {
            var rv = GetResource($"{name}.txt").Replace("$modelnamespace$", ModelNS).Replace("$vmnamespace$", VMNs).Replace("$modelname$", ModelName).Replace("$area$",$"{Area??""}");
            if (name == "Searcher" || name == "BatchVM")
            {
                string prostring = "";
                string initstr = "";
                Type modelType = Type.GetType(SelectedModel);
                List<FieldInfo> pros = null;
                if (name == "Searcher")
                {
                    pros = FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                }
                if(name == "BatchVM")
                {
                    pros = FieldInfos.Where(x => x.IsBatchField == true).ToList();
                }
                foreach (var pro in pros)
                {
                    if (pro.RelatedField != null)
                    {
                        var subtype = Type.GetType(pro.RelatedField);
                        if (typeof(TopBasePoco).IsAssignableFrom(subtype) == false || subtype == typeof(FileAttachment))
                        {
                            continue;
                        }
                        var fname = "All" + pro.FieldName.Substring(0, pro.FieldName.Length - 2) + "s";
                        prostring += $@"
        public List<ComboSelectListItem> {fname} {{ get; set; }}";
                        initstr += $@"
            {fname} = DC.Set<{subtype.Name}>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.{pro.SubField});";
                    }
                    var proType = modelType.GetProperty(pro.FieldName);
                    var display = proType.GetCustomAttribute<DisplayAttribute>();
                    if (display != null)
                    {
                        prostring += $@"
        [Display(Name = ""{display.Name}"")]";
                    }
                    string typename = proType.PropertyType.Name;
                    if (proType.PropertyType.IsNullable())
                    {
                        typename = proType.PropertyType.GetGenericArguments()[0].Name + "?";
                    }
                    prostring += $@"
        public {typename} {proType.Name} {{ get; set; }}";
                }
                rv = rv.Replace("$pros$", prostring).Replace("$init$", initstr);
                rv = GetRelatedNamespace(pros, rv);
            }
            if(name == "ListVM")
            {
                string headerstring = "";
                string selectstring = "";
                string wherestring = "";
                string subprostring = "";
                string formatstring = "";
                var pros = FieldInfos.Where(x => x.IsListField == true).ToList();
                foreach (var pro in pros)
                {
                    if (pro.RelatedField == null)
                    {
                        headerstring += $@"
                this.MakeGridHeader(x => x.{pro.FieldName}),";
                        selectstring += $@"
                    {pro.FieldName} = x.{pro.FieldName},";
                    }
                    else
                    {
                        var subtype = Type.GetType(pro.RelatedField);
                        if (subtype == typeof(FileAttachment))
                        {
                            headerstring += $@"
                this.MakeGridHeader(x => x.{pro.FieldName}).SetFormat({pro.FieldName}Format),";
                            selectstring += $@"
                    {pro.FieldName} = x.{pro.FieldName},";
                            formatstring += GetResource("HeaderFormat.txt").Replace("$modelname$", ModelName).Replace("$field$",pro.FieldName);
                        }
                        else
                        {
                            var subpro = subtype.GetProperty(pro.SubField);
                            string subtypename = subpro.PropertyType.Name;
                            if (subpro.PropertyType.IsNullable())
                            {
                                subtypename = subpro.PropertyType.GetGenericArguments()[0].Name + "?";
                            }

                            var subdisplay = subpro.GetCustomAttribute<DisplayAttribute>();
                            headerstring += $@"
                this.MakeGridHeader(x => x.{pro.SubField + "_view"}),";
                            selectstring += $@"
                    {pro.SubField + "_view"} = x.{pro.FieldName.Substring(0, pro.FieldName.Length - 2)}.{pro.SubField},";
                            if (subdisplay?.Name != null)
                            {
                                subprostring += $@"
        [Display(Name = ""{subdisplay.Name}"")]";
                                subprostring += $@"
        public {subtypename} {pro.SubField + "_view"} {{ get; set; }}";
                            }
                        }
                    }

                }
                var wherepros = FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                Type modelType = Type.GetType(SelectedModel);
                foreach (var pro in wherepros)
                {
                    var proType = modelType.GetProperty(pro.FieldName).PropertyType;
                    if(proType == typeof(string))
                    {
                        wherestring += $@"
                .CheckContain(Searcher.{pro.FieldName}, x=>x.{pro.FieldName})";
                    }
                    else
                    {
                        wherestring += $@"
                .CheckEqual(Searcher.{pro.FieldName}, x=>x.{pro.FieldName})";

                    }
                }
                rv = rv.Replace("$headers$", headerstring).Replace("$where$",wherestring).Replace("$select$", selectstring).Replace("$subpros$", subprostring).Replace("$format$", formatstring);
                rv = GetRelatedNamespace(pros, rv);
            }
            if (name == "CrudVM" )
            {
                string prostr = "";
                string initstr = "";
                string includestr = "";
                var pros = FieldInfos.Where(x => x.IsFormField == true && x.RelatedField != null).ToList();
                foreach (var pro in pros)
                {
                    var subtype = Type.GetType(pro.RelatedField);
                    if (typeof(TopBasePoco).IsAssignableFrom(subtype) == false || subtype == typeof(FileAttachment))
                    {
                        continue;
                    }
                    var fname = "All" + pro.FieldName.Substring(0, pro.FieldName.Length - 2) + "s";
                    prostr += $@"
        public List<ComboSelectListItem> {fname} {{ get; set; }}";
                    initstr += $@"
            {fname} = DC.Set<{subtype.Name}>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.{pro.SubField});";
                    includestr += $@"
            SetInclude(x => x.{pro.FieldName.Substring(0, pro.FieldName.Length - 2)});";

                }
                rv = rv.Replace("$pros$", prostr).Replace("$init$", initstr).Replace("$include$", includestr);
                rv = GetRelatedNamespace(pros, rv);
            }
            if (name == "ImportVM")
            {
                string prostring = "";
                string initstr = "";
                Type modelType = Type.GetType(SelectedModel);
                List<FieldInfo> pros = FieldInfos.Where(x => x.IsImportField == true).ToList();
                foreach (var pro in pros)
                {
                    if (pro.RelatedField != null)
                    {
                        var subtype = Type.GetType(pro.RelatedField);
                        if (typeof(TopBasePoco).IsAssignableFrom(subtype) == false || subtype == typeof(FileAttachment))
                        {
                            continue;
                        }
                        initstr += $@"
            {pro.FieldName + "_Excel"}.DataType = ColumnDataType.ComboBox;
            {pro.FieldName + "_Excel"}.ListItems = DC.Set<{subtype.Name}>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.{pro.SubField});";
                    }
                    var proType = modelType.GetProperty(pro.FieldName);
                    var display = proType.GetCustomAttribute<DisplayAttribute>();
                    if (display != null)
                    {
                        prostring += $@"
        [Display(Name = ""{display.Name}"")]";
                    }
                    prostring += $@"
        public ExcelPropety {pro.FieldName + "_Excel"} = ExcelPropety.CreateProperty<{ModelName}>(x => x.{pro.FieldName});";
                }
                rv = rv.Replace("$pros$", prostring).Replace("$init$", initstr);
                rv = GetRelatedNamespace(pros, rv);

            }
            return rv;
        }

        public string GenerateView(string name)
        {
            var rv = GetResource($"{name}.txt").Replace("$vmnamespace$", VMNs).Replace("$modelname$", ModelName);
            if (name == "CreateView" || name == "EditView" || name == "DeleteView" || name == "DetailsView" || name == "BatchEditView")
            {
                StringBuilder fieldstr = new StringBuilder();
                string pre = "";
                List<FieldInfo> pros = null;
                if (name == "BatchEditView")
                {
                    pros = FieldInfos.Where(x => x.IsBatchField == true).ToList();
                    pre = "LinkedVM";
                }
                else
                {
                    pros = FieldInfos.Where(x => x.IsFormField == true).ToList();
                    pre = "Entity";
                }
                Type modelType = Type.GetType(SelectedModel);
                fieldstr.Append(Environment.NewLine);
                fieldstr.Append(@"<wt:row items-per-row=""ItemsPerRowEnum.Two"">");
                fieldstr.Append(Environment.NewLine);
                foreach (var item in pros)
                {
                    if (name == "DeleteView" || name == "DetailsView")
                    {
                        if (item.RelatedField != null && item.SubField != "`file")
                        {
                            fieldstr.Append($@"<wt:display field=""{pre}.{item.FieldName.Substring(0, item.FieldName.Length - 2)}.{item.SubField}"" />");
                        }
                        else
                        {
                            fieldstr.Append($@"<wt:display field=""{pre}.{item.FieldName}"" />");
                        }
                    }
                    else
                    {
                        if (item.RelatedField != null)
                        {
                            if (item.SubField == "`file")
                            {
                                fieldstr.Append($@"<wt:upload field=""{pre}.{item.FieldName}"" />");
                            }
                            else
                            {
                                var fname = "All" + item.FieldName.Substring(0, item.FieldName.Length - 2) + "s";
                                if (name == "BatchEditView")
                                {
                                    fname = "LinkedVM." + fname;
                                }
                                fieldstr.Append($@"<wt:combobox field=""{pre}.{item.FieldName}"" items=""{fname}""/>");
                            }
                        }
                        else
                        {
                            var proType = modelType.GetProperty(item.FieldName).PropertyType;
                            Type checktype = proType;
                            if (proType.IsNullable())
                            {
                                checktype = proType.GetGenericArguments()[0];
                            }
                            if(checktype == typeof(bool) || checktype.IsEnum())
                            {
                                fieldstr.Append($@"<wt:combobox field=""{pre}.{item.FieldName}"" />");
                            }
                            else if (checktype.IsPrimitive || checktype == typeof(string))
                            {
                                fieldstr.Append($@"<wt:textbox field=""{pre}.{item.FieldName}"" />");
                            }
                            else if (checktype == typeof(DateTime))
                            {
                                fieldstr.Append($@"<wt:datetime field=""{pre}.{item.FieldName}"" />");
                            }
                        }
                    }
                    fieldstr.Append(Environment.NewLine);
                }
                fieldstr.Append("</wt:row>");
                rv = rv.Replace("$fields$", fieldstr.ToString());
            }
            if (name == "ListView")
            {
                StringBuilder fieldstr = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                Type modelType = Type.GetType(SelectedModel);
                fieldstr.Append(Environment.NewLine);
                fieldstr.Append(@"<wt:row items-per-row=""ItemsPerRowEnum.Three"">");
                fieldstr.Append(Environment.NewLine);
                foreach (var item in pros)
                {
                    if (item.RelatedField != null)
                    {
                        var fname = "All" + item.FieldName.Substring(0, item.FieldName.Length - 2) + "s";
                        fieldstr.Append($@"<wt:combobox field=""Searcher.{item.FieldName}"" items=""Searcher.{fname}"" empty-text=""全部"" />");
                    }
                    else
                    {
                        var proType = modelType.GetProperty(item.FieldName).PropertyType;
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if (checktype.IsPrimitive || checktype == typeof(string))
                        {
                            fieldstr.Append($@"<wt:textbox field=""Searcher.{item.FieldName}"" />");
                        }
                        if (checktype == typeof(DateTime))
                        {
                            fieldstr.Append($@"<wt:datetime field=""Searcher.{item.FieldName}"" />");
                        }
                        if (checktype.IsEnum())
                        {
                            fieldstr.Append($@"<wt:combobox field=""Searcher.{item.FieldName}"" empty-text=""全部"" />");
                        }
                    }
                    fieldstr.Append(Environment.NewLine);
                }
                fieldstr.Append($@"</wt:row>");
                rv = rv.Replace("$fields$", fieldstr.ToString());
            }
            return rv;
        }


        private string GetResource(string fileName)
        {
            //获取编译在程序中的Controller原始代码文本
            Assembly assembly = Assembly.GetExecutingAssembly();
            var textStreamReader = new StreamReader(assembly.GetManifestResourceStream($"WalkingTec.Mvvm.Mvc.GeneratorFiles.{fileName}"));
            string content = textStreamReader.ReadToEnd();
            textStreamReader.Close();
            return content;
        }

        private string GetRelatedNamespace(List<FieldInfo> pros, string s )
        {
            string otherns = @"";
            Type modelType = Type.GetType(SelectedModel);
            foreach (var pro in pros)
            {
                Type proType = null;

                if (string.IsNullOrEmpty(pro.RelatedField))
                {
                    proType = modelType.GetProperty(pro.FieldName).PropertyType;
                }
                else
                {
                    proType = Type.GetType(pro.RelatedField);
                }
                string prons = proType.Namespace;
                if (proType.IsNullable())
                {
                    prons = proType.GetGenericArguments()[0].Namespace;
                }
                if (s.Contains($"using {prons}") == false && otherns.Contains($"using {prons}") == false)
                {
                    otherns += $@"using {prons};
";
                }

            }

            return s.Replace("$othernamespace$", otherns);
        }

    }

    public class FieldInfo
    {
        public string FieldName { get; set; }
        public string RelatedField { get; set; }

        public bool IsSearcherField { get; set; }

        public bool IsListField { get; set; }

        public bool IsFormField { get; set; }

        public bool IsImportField { get; set; }
        public bool IsBatchField { get; set; }

        public string SubField { get; set; }

    }
}
